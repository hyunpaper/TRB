using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRB.Server.Domain.Messages;

namespace TRB.Server.Infrastructure.Messaging
{
    public static class QueueNaming
    {
        private static readonly ConcurrentDictionary<string, int> _counters = new();

        private static readonly Dictionary<Type, string[]> _queueMap = new()
        {
            { typeof(UserSignupMessage), new[] { nameof(UserSignupMessage) + ".1", nameof(UserSignupMessage) + ".2", nameof(UserSignupMessage) + ".3" } },
            // 다른 메시지 타입 큐도 이곳에 추가
        };

        public static string GetQueueNameFor<T>() where T : IQueueMessage
        {
            var type = typeof(T);
            if (!_queueMap.TryGetValue(type, out var queues))
                throw new InvalidOperationException($"큐 이름이 정의되지 않았습니다: {type.Name}");

            var index = _counters.AddOrUpdate(type.FullName!, 0, (_, i) => (i + 1) % queues.Length);
            return queues[index];
        }
        public static string[] GetAllQueueNamesFor<T>() where T : IQueueMessage
        {
            var type = typeof(T);
            if (_queueMap.TryGetValue(type, out var queues))
                return queues;
            return Array.Empty<string>();
        }
    }
}
