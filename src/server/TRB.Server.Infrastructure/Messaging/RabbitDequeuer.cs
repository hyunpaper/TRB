using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TRB.Server.Domain.Messages;
using TRB.Server.Infrastructure.Messaging;

namespace TRB.Server.Infrastructure.Messaging.Dequeuer
{
    public class RabbitDequeuer<T> where T : IQueueMessage
    {
        private readonly List<IModel> _channels = new();
        private readonly IRabbitMessageSerializer<T> _serializer = new JsonRabbitMessageSerializer<T>();
        private readonly string _queueName;
        private readonly Logger _logger = LogManager.GetLogger("RabbitDequeuer<" + typeof(T).Name + ">");
        private const int MaxRetry = 3;

        public event Func<T, Task<bool>>? AsyncMessageReceived;
        public event Action<T>? RetryFailed;

        public RabbitDequeuer(string queueName)
        {
            _queueName = queueName;
        }

        public void Start(bool isAsync = true)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            _channels.Add(channel);

            try
            {
                // DLX 제거된 기본 큐 선언
                channel.ExchangeDeclare(typeof(T).FullName, ExchangeType.Direct, durable: true);
                channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);
                channel.QueueBind(queue: _queueName, exchange: typeof(T).FullName, routingKey: "");

                _logger.Info("[RabbitDequeuer<{0}>] 큐 선언 및 바인딩 완료: {1}", typeof(T).Name, _queueName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[RabbitDequeuer<{0}>] 큐 선언 중 예외 발생: {1}", typeof(T).Name, _queueName);
                throw;
            }

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = _serializer.Deserialize(body);

                _logger.Info("[RabbitDequeuer<{0}>] 메시지 수신: {1}", typeof(T).Name, message);

                try
                {
                    var result = await (AsyncMessageReceived?.Invoke(message) ?? Task.FromResult(false));
                    if (result)
                    {
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    else
                    {
                        HandleRetryOrDelete(channel, ea, message);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "[RabbitDequeuer<{0}>] 메시지 처리 중 예외 발생", typeof(T).Name);
                    HandleRetryOrDelete(channel, ea, message);
                }
            };

            try
            {
                channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
                _logger.Info("[RabbitDequeuer<{0}>] 큐 소비 시작: {1}", typeof(T).Name, _queueName);
                Console.WriteLine($"[RabbitDequeuer<{typeof(T).Name}>] 소비 시작된 큐: {_queueName}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[RabbitDequeuer<{0}>] BasicConsume 실패 - 큐가 존재하지 않음: {1}", typeof(T).Name, _queueName);
            }
        }

        private void HandleRetryOrDelete(IModel channel, BasicDeliverEventArgs ea, T message)
        {
            int retryCount = 0;

            if (ea.BasicProperties.Headers != null &&
                ea.BasicProperties.Headers.TryGetValue("x-death", out var deathHeader) &&
                deathHeader is List<object> xDeathList &&
                xDeathList.FirstOrDefault() is Dictionary<string, object> xDeath &&
                xDeath.TryGetValue("count", out var countObj))
            {
                retryCount = Convert.ToInt32(countObj);
            }

            if (retryCount >= MaxRetry)
            {
                RetryFailed?.Invoke(message);
                _logger.Warn("[RabbitDequeuer<{0}>] 최대 재시도 초과. 메시지 삭제: {1}", typeof(T).Name, message);
                channel.BasicAck(ea.DeliveryTag, false); // 삭제 처리
            }
            else
            {
                _logger.Warn("[RabbitDequeuer<{0}>] 처리 실패. 재시도 {1}회차: {2}", typeof(T).Name, retryCount + 1, message);
                channel.BasicNack(ea.DeliveryTag, false, requeue: true);
            }
        }
    }
}
