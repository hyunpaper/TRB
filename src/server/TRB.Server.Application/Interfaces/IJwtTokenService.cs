using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TRB.Server.Application.DTOs;
using TRB.Server.Domain.Entities;

namespace TRB.Server.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(string email, int roleId);
        string GenerateAccessToken(UserLoginResponseDto user);

        /// <summary>
        /// 랜덤한 Refresh Token 문자열 생성
        /// </summary>
        string GenerateRefreshToken();

        /// <summary>
        /// AccessToken을 복호화하고 ClaimsPrincipal 반환
        /// </summary>
        ClaimsPrincipal? ValidateAccessToken(string token);

        /// <summary>
        /// RefreshToken 저장 (ex: Redis 등 외부 저장소)
        /// </summary>
        Task StoreRefreshTokenAsync(int userId, string token, TimeSpan expires);

        /// <summary>
        /// RefreshToken 유효성 검증
        /// </summary>
        Task<bool> ValidateRefreshTokenAsync(int userId, string token);

        /// <summary>
        /// RefreshToken 삭제
        /// </summary>
        Task RemoveRefreshTokenAsync(int userId);

    }

}
