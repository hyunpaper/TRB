using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using RabbitMQ.Client;
using TRB.Server.Infrastructure.Interfaces;

namespace TRB.Server.Infrastructure.Services
{
    public class RabbitMQFactory : IRabbitMQFactory
    {
        private readonly string _connectionString;

        public RabbitMQFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("RabbitMq") ?? string.Empty;
        }

        public ConnectionFactory Conn()
        {
            return new ConnectionFactory
            {
                Uri = new Uri(_connectionString)
            };
        }
    }
}
