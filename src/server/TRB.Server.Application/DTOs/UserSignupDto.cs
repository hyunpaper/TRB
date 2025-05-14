using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRB.Server.Application.DTOs
{
    public class UserSignupDto
    {
        // users 테이블 관련
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; } = 1;

        // user_profiles 테이블 관련
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public char? Gender { get; set; }
        public string? Address { get; set; }
        public string? Nickname { get; set; }
        public string? ProfileImage { get; set; }
    }

}
