using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using TRB.Server.Application.Interfaces;
using TRB.Server.Domain.Messages;
using Microsoft.Extensions.Configuration;

namespace TRB.Server.Presentation.Producers
{
    public class UserSignupPublisher : IUserSignupPublisher
    {
        private readonly IConfiguration _config;
        private static int _counter = 0;
        private readonly string[] _queueNames = { "user.signup.q1", "user.signup.q2", "user.signup.q3" };

        public UserSignupPublisher(IConfiguration config)
        {
            _config = config;
        }

        public Task PublishAsync(UserSignupMessage message)
        {
            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMQ:HostName"] ?? "localhost",
                Port = int.Parse(_config["RabbitMQ:Port"] ?? "5672"),
                UserName = _config["RabbitMQ:UserName"] ?? "guest",
                Password = _config["RabbitMQ:Password"] ?? "guest"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var queueName = _queueNames[Interlocked.Increment(ref _counter) % _queueNames.Length];

            channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object>
                {
                    { "x-dead-letter-exchange", "dlx.user.signup" }
                });
            var props = channel.CreateBasicProperties();
            props.Persistent = true;

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: props,
                body: body
            );

            return Task.CompletedTask;
        }
    }
}