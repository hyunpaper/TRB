using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TRB.Server.Application.Interfaces;
using TRB.Server.Domain.Messages;
using RabbitMQ.Client;

namespace TRB.Server.Infrastructure.Services
{
    public class RabbitMqUserSignupPublisher : IUserSignupPublisher
    {
        private readonly IConfiguration _config;

        public RabbitMqUserSignupPublisher(IConfiguration config)
        {
            _config = config;
        }

        public Task PublishAsync(UserSignupMessage message)
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                HostName = _config["RabbitMQ:HostName"] ?? "localhost",
                Port = int.Parse(_config["RabbitMQ:Port"] ?? "5672"),
                UserName = _config["RabbitMQ:UserName"] ?? "guest",
                Password = _config["RabbitMQ:Password"] ?? "guest"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var queueName = "trb.user.signup";
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: null,
                body: body
            );

            return Task.CompletedTask;
        }
    }

}
