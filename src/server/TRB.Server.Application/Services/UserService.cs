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
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;

            return new UserDto
            {
                Email = user.Email,
                RoleId = user.RoleId
            };
        }

        public async Task CreateAsync(UserDto dto)
        {
            var user = new User
            {
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleId = dto.RoleId,
                CreatedAt = DateTime.UtcNow,
                Enabled = "Y"
            };

            await _userRepository.CreateAsync(user);
        }

        public async Task<bool> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null) return false;

            return BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
        }


    }
}
