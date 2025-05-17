using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRB.Server.Domain.Messages;
namespace TRB.Server.Application.Interfaces
{
    public interface IRabbitMessagePublisher
    {
        Task PublishAsync<T>(T message) where T : IQueueMessage;
    }
}
