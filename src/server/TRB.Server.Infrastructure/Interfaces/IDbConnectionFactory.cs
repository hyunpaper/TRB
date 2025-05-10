using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRB.Server.Infrastructure.Services;
using MySql.Data.MySqlClient;

namespace TRB.Server.Infrastructure.Interfaces
{
    public interface IDbConnectionFactory
    {
        MySqlConnection Conn();
    }
}
