using Microsoft.AspNetCore.Mvc;
using TRB.Server.Application.DTOs;
using TRB.Server.Application.Interfaces;
using TRB.Server.Application.Services;
using TRB.Server.Domain.Messages;
using TRB.Server.Infrastructure.Interfaces;


namespace TRB.Server.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IMessagePublisher _messagePublisher;

        public UserController(IUserService userService, ILogger<UserController> logger, IJwtTokenService jwtTokenService, IUserSignupPublisher signupPublisher, IMessagePublisher messagePublisher)
        {
            _userService = userService;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
            _messagePublisher = messagePublisher;
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
                var existing = await _userService.GetByEmailAsync(dto.Email);
                if (existing != null)
                    return Conflict("이미 등록된 이메일입니다.");

                // 1️⃣ 프로필 이미지 저장 처리
                if (profileImage != null && profileImage.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(profileImage.FileName)}";
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profile");

                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    var filePath = Path.Combine(uploadPath, fileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await profileImage.CopyToAsync(stream);

                    // 저장할 상대 경로 (e.g. /uploads/profile/abc.jpg)
                    dto.ProfileImage = $"/uploads/profile/{fileName}";
                }

                // 2️⃣ 패스워드 해싱
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                // 3️⃣ 메시지 전송
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
                _logger.LogInformation("회원가입 메시지 발행 성공: {Email}", dto.Email);
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
                var isValid = await _userService.LoginAsync(dto);

                if (!isValid)
                    return Unauthorized("이메일 또는 비밀번호가 잘못되었습니다.");

                var user = await _userService.GetLoginInfoByEmailAsync(dto.Email);

                var token = _jwtTokenService.GenerateToken(user.Email, user.RoleId);

                _logger.LogInformation("로그인 성공: {Email}", dto.Email);
                return Ok(new { token, user });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "로그인 중 오류 발생: {Email}", dto.Email);
                return StatusCode(500, "로그인 처리 중 오류 발생");
            }
        }
    }
}
