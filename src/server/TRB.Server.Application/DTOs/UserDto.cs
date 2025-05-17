using System.ComponentModel.DataAnnotations;

namespace TRB.Server.Application.DTOs
{
    public class UserDto
    {
        [Required(ErrorMessage = "이메일은 필수입니다.")]
        [EmailAddress(ErrorMessage = "이메일 형식이 올바르지 않습니다.")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "비밀번호는 필수입니다.")]

        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; } = 1;
        public string Role_name { get; set; } = "User";
        public int UserId { get; set; }
    }

}
