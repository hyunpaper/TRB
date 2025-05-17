using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TRB.Server.Application.Interfaces;
using TRB.Server.Application.DTOs;
using System.Security.Cryptography;

namespace TRB.Server.Application.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;
        private readonly IRedisTokenStore _tokenStore;

        public JwtTokenService(IConfiguration config, IRedisTokenStore tokenStore)
        {
            _config = config;
            _tokenStore = tokenStore;
        }

        public string GenerateToken(string email, int roleId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim("roleId", roleId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_config["Jwt:ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 사용자 정보를 기반으로 Access Token(JWT)을 생성한다.
        /// 유효기간은 일반적으로 15분으로 설정됨
        /// </summary>
        public string GenerateAccessToken(UserLoginResponseDto user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(15);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.RoleId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 64바이트의 랜덤 문자열을 생성하여 Refresh Token으로 사용
        /// </summary>
        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        /// <summary>
        /// Redis에 RefreshToken을 저장 (Key: refresh:{userId})
        /// </summary>
        public async Task StoreRefreshTokenAsync(int userId, string token, TimeSpan expires)
        {
            await _tokenStore.StoreRefreshTokenAsync(userId, token, expires);
        }

        /// <summary>
        /// Redis에 저장된 RefreshToken이 클라이언트에서 보낸 것과 일치하는지 확인
        /// </summary>
        public async Task<bool> ValidateRefreshTokenAsync(int userId, string token)
        {
            var stored = await _tokenStore.GetRefreshTokenAsync(userId);
            return stored == token;
        }

        /// <summary>
        /// 사용된 RefreshToken을 Redis에서 제거하여 재사용을 방지
        /// </summary>
        public async Task RemoveRefreshTokenAsync(int userId)
        {
            await _tokenStore.RemoveRefreshTokenAsync(userId);
        }

        /// <summary>
        /// 전달받은 AccessToken 문자열을 검증하고 ClaimsPrincipal로 반환
        /// 유효하지 않으면 null 반환
        /// </summary>
        public ClaimsPrincipal? ValidateAccessToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var handler = new JwtSecurityTokenHandler();

            try
            {
                return handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidAudience = _config["Jwt:Audience"],
                    IssuerSigningKey = key,
                    ValidateLifetime = true
                }, out _);
            }
            catch
            {
                return null;
            }
        }
    }

}
