using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRB.Server.Application.DTOs
{
    public class UserLoginResponseDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string Role_name { get; set; } = string.Empty;
        public string? Nickname { get; set; }
        public string? ProfileImage { get; set; }

        //토큰 응답
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
