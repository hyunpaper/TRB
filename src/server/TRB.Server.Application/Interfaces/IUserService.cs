using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRB.Server.Application.api.requestbodies;
using TRB.Server.Application.DTOs;
using TRB.Server.Domain.Entities;



namespace TRB.Server.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetByEmailAsync(string userId);
        Task<bool> LoginAsync(LoginDto dto);
        Task<bool> SignupAsync(UserSignupDto dto);
        Task<UserLoginResponseDto?> GetLoginInfoByEmailAsync(string email);
    }

}
