using Microsoft.AspNetCore.Mvc;
using TRB.Server.Application;
using TRB.Server.Application.DTOs;
using TRB.Server.Application.Interfaces;
using TRB.Server.Infrastructure;


namespace TRB.Server.Presentation.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserService _userService;

        public AuthController(IJwtTokenService jwtTokenService, IUserService userService)
        {
            _jwtTokenService = jwtTokenService;
            _userService = userService;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRefreshRequestDto dto)
        {
            // 1. 사용자 존재 확인
            var user = await _userService.GetByEmailAsync(dto.UserId.ToString());
            if (user == null)
                return Unauthorized("사용자를 찾을 수 없습니다.");

            // 2. RefreshToken 유효성 확인
            var isValid = await _jwtTokenService.ValidateRefreshTokenAsync(dto.UserId, dto.RefreshToken);
            if (!isValid)
                return Unauthorized("RefreshToken이 유효하지 않습니다.");

            // 3. 기존 토큰 삭제 후 새 토큰 발급
            await _jwtTokenService.RemoveRefreshTokenAsync(dto.UserId);

            var loginDto = new UserLoginResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                RoleId = user.RoleId,
                Role_name = user.Role_name,
            };

            var newAccessToken = _jwtTokenService.GenerateAccessToken(loginDto);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
            await _jwtTokenService.StoreRefreshTokenAsync(user.UserId, newRefreshToken, TimeSpan.FromDays(7));

            return Ok(new TokenRefreshResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
    }
}
