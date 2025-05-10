using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TRB.Server.Infrastructure.Interfaces;
using MySql.Data.MySqlClient;


namespace TRB.Server.Infrastructure.Services
{
    public class ConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString = string.Empty;

        public ConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }

        public MySqlConnection Conn()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
