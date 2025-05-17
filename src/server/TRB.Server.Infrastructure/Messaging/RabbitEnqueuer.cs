using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using TRB.Server.Domain.Messages;
using TRB.Server.Infrastructure.Messaging;

namespace TRB.Server.Infrastructure.Messaging
{
    public enum QueueErrorState { None, Unknown }

    public class RabbitEnqueuer<T> : IDisposable where T : IQueueMessage
    {
        private readonly IModel _channel;
        private readonly IRabbitMessageSerializer<T> _serializer;
        private readonly string _exchangeName;
        private readonly string _queueName;

        public RabbitEnqueuer(string queueName, bool isAutoDelete)
        {
            _exchangeName = typeof(T).FullName!;
            _queueName = queueName;
            _serializer = new JsonRabbitMessageSerializer<T>();

            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            // DLX 제거된 큐 선언
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct, durable: true);
            _channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: isAutoDelete);
            _channel.QueueBind(_queueName, _exchangeName, routingKey: _queueName);
        }

        public QueueErrorState EnqueueMessage(T message)
        {
            try
            {
                var props = _channel.CreateBasicProperties();
                props.Persistent = true;
                var body = _serializer.Serialize(message);
                _channel.BasicPublish(_exchangeName, _queueName, props, body);
                return QueueErrorState.None;
            }
            catch
            {
                return QueueErrorState.Unknown;
            }
        }

        public void Dispose() => _channel?.Dispose();
    }
}