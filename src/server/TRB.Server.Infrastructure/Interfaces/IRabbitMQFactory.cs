using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using TRB.Server.Infrastructure.Services;

namespace TRB.Server.Infrastructure.Interfaces
{
    public interface IRabbitMQFactory
    {
        ConnectionFactory Conn();
    }
}
