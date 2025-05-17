using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRB.Server.Domain.Messages
{
    public class UserSignupMessage : IQueueMessage
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public char? Gender { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public string ProfileImage { get; set; } = string.Empty;
    }
}
