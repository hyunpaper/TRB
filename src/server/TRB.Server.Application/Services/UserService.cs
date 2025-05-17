using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRB.Server.Application.DTOs;
using TRB.Server.Application.Interfaces;
using TRB.Server.Domain.Entities;
using TRB.Server.Domain.Interfaces;
using BCrypt.Net;


namespace TRB.Server.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.FindByEmailAsync(email);
            if (user == null) return null;

            return new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                RoleId = user.RoleId,
                Role_name = user.RoleName
            };
        }

        public async Task<bool> SignupAsync(UserSignupDto dto)
        {
            var user = new User
            {
                Email = dto.Email,
                Password = dto.Password,
                RoleId = dto.RoleId,
                CreatedAt = DateTime.UtcNow,
                Enabled = "Y",
                UserId = dto.UserId,
            };

            var profile = new UserProfile
            {
                Name = dto.Name,
                Phone = dto.Phone,
                BirthDate = dto.BirthDate,
                Gender = dto.Gender,
                Address = dto.Address,
                Nickname = dto.Nickname,
                ProfileImage = dto.ProfileImage
            };

            return await _userRepository.InsertUserAndProfileAsync(user, profile);
        }





        public async Task<bool> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.FindByEmailAsync(dto.Email);
            if (user == null) return false;

            return BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
        }

        public async Task<UserLoginResponseDto?> GetLoginInfoByEmailAsync(string email)
        {
            var result = await _userRepository.GetWithProfileByEmailAsync(email);
            if (result == null) return null;

            var (user, profile, roleName) = result.Value;

            return new UserLoginResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                RoleId = user.RoleId,
                Role_name = roleName,
                Nickname = profile?.Nickname,
                ProfileImage = profile?.ProfileImage
            };
        }

    }
}
