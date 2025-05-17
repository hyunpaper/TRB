using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TRB.Server.Application.DTOs;
using TRB.Server.Application.Interfaces;
using TRB.Server.Application.Services;
using TRB.Server.Domain.Messages;
using TRB.Server.Infrastructure.Interfaces;
using TRB.Server.Application.api.requestbodies;
using System.Security.Claims;
using System.Reflection.Metadata;
using TRB.Server.Presistance.Repositories.Commands;


namespace TRB.Server.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRabbitMessagePublisher _messagePublisher;
        private readonly UpdateUserProfileCommandHandler _handler;

        public UserController(
            IUserService userService,
            ILogger<UserController> logger,
            IJwtTokenService jwtTokenService,
            IRabbitMessagePublisher messagePublisher,
            UpdateUserProfileCommandHandler handler)
        {
            _userService = userService;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
            _messagePublisher = messagePublisher;
            _handler = handler;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] UserSignupDto dto, IFormFile? profileImage)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault();
                return BadRequest(errorMessage);
            }

            try
            {
                var existing = await _userService.GetByEmailAsync(dto.UserId.ToString());
                if (existing != null)
                    return Conflict("이미 등록된 이메일입니다.");

                if (profileImage != null && profileImage.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(profileImage.FileName)}";
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profile");

                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    var filePath = Path.Combine(uploadPath, fileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await profileImage.CopyToAsync(stream);

                    dto.ProfileImage = $"/uploads/profile/{fileName}";
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                var message = new UserSignupMessage
                {
                    Email = dto.Email,
                    Password = hashedPassword,
                    RoleId = dto.RoleId,
                    Name = dto.Name,
                    Phone = dto.Phone,
                    BirthDate = dto.BirthDate,
                    Gender = dto.Gender,
                    Address = dto.Address,
                    Nickname = dto.Nickname,
                    ProfileImage = dto.ProfileImage
                };

                await _messagePublisher.PublishAsync(message);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "회원가입 실패: {Email}", dto.Email);
                return StatusCode(500, "회원가입 처리 중 오류 발생");
            }
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> Get(string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                // 1. 로그인 정보 확인
                var isValid = await _userService.LoginAsync(dto);
                if (!isValid)
                    return Unauthorized("이메일 또는 비밀번호가 잘못되었습니다.");

                // 2. 사용자 정보 조회 (AccessToken 발급용)
                var user = await _userService.GetLoginInfoByEmailAsync(dto.Email);

                // 3. AccessToken + RefreshToken 생성
                var accessToken = _jwtTokenService.GenerateAccessToken(user);
                var refreshToken = _jwtTokenService.GenerateRefreshToken();


                await _jwtTokenService.StoreRefreshTokenAsync(user.UserId, refreshToken, TimeSpan.FromDays(7));

                // 4. 응답 객체 구성
                user.AccessToken = accessToken;
                user.RefreshToken = refreshToken;

                _logger.LogInformation("로그인 성공: {Email}", dto.Email);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "로그인 중 오류 발생: {Email}", dto.Email);
                return StatusCode(500, "로그인 처리 중 오류 발생");
            }
        }

        [Authorize]
        [HttpPatch("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileUpdateRequestDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                return Unauthorized("유효하지 않은 사용자입니다.");

            var command = new UpdateUserProfileCommand(userId, dto);
            var result = await _handler.HandleAsync(command);

            if (!result)
                return BadRequest("프로필 수정에 실패했습니다.");

            return Ok("프로필이 성공적으로 수정되었습니다.");
        }


    }
}
