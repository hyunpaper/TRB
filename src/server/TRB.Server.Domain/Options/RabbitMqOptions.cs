using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRB.Server.Domain.Options
{
    public class RabbitMqOptions
    {
        public string HostName { get; set; } = string.Empty;
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }

}
