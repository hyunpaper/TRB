using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TRB.Server.Application.Interfaces;
using TRB.Server.Domain.Messages;
using TRB.Server.Infrastructure.Messaging;

namespace TRB.Server.Infrastructure.Messaging
{
    public class RabbitMessagePublisher : IRabbitMessagePublisher
    {
        private readonly ILogger<RabbitMessagePublisher> _logger;

        public RabbitMessagePublisher(ILogger<RabbitMessagePublisher> logger)
        {
            _logger = logger;
        }

        public Task PublishAsync<T>(T message) where T : IQueueMessage
        {
            var queueName = QueueNaming.GetQueueNameFor<T>();
            _logger.LogInformation("📤 메시지 발행 - 큐: {Queue}, 메시지: {Message}", queueName, message);

            using var enqueuer = new RabbitEnqueuer<T>(queueName, isAutoDelete: false);

            var result = enqueuer.EnqueueMessage(message);
            if (result != QueueErrorState.None)
            {
                _logger.LogError("❌ 메시지 발행 실패: {Queue} {Message}", queueName, message);
                throw new Exception($"큐 발행 실패: {queueName}");
            }

            _logger.LogInformation("✅ 메시지 발행 성공: {Queue}", queueName);
            return Task.CompletedTask;
        }
    }

}
