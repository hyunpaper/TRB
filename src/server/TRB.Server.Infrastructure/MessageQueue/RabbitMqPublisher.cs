using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using TRB.Server.Infrastructure.Interfaces;

public class RabbitMqPublisher : IMessagePublisher
{
    private readonly IConfiguration _config;
    private static int _counter = 0;
    private readonly string[] _queues = new[] { "user.signup.q1", "user.signup.q2", "user.signup.q3" };

    public RabbitMqPublisher(IConfiguration config)
    {
        _config = config;
    }

    public Task PublishAsync<T>(T message)
    {
        var queueIndex = Interlocked.Increment(ref _counter) % _queues.Length;
        var selectedQueue = _queues[queueIndex];

        var factory = new ConnectionFactory
        {
            Uri = new Uri(_config.GetConnectionString("RabbitMq")!)
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: selectedQueue,
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
            routingKey: selectedQueue,
            basicProperties: props,
            body: body
        );

        return Task.CompletedTask;
    }
}
