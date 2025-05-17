using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRB.Server.Application.api.requestbodies
{
    public class UserProfileUpdateRequestDto
    {
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? Nickname { get; set; }
        public string? ProfileImage { get; set; }
    }
}
