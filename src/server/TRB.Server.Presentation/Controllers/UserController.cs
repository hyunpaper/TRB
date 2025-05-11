using Microsoft.AspNetCore.Mvc;
using TRB.Server.Application.DTOs;
using TRB.Server.Application.Interfaces;
using TRB.Server.Application.Services;
using TRB.Server.Domain.Messages;


namespace TRB.Server.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserSignupPublisher _signupPublisher;

        public UserController(IUserService userService, ILogger<UserController> logger, IJwtTokenService jwtTokenService, IUserSignupPublisher signupPublisher)
        {
            _userService = userService;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
            _signupPublisher = signupPublisher;

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDto dto)
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

                // 패스워드 해싱
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password); 

                // ✅ 메시지 큐로 발행
                var message = new UserSignupMessage
                {
                    Email = dto.Email,
                    Password = hashedPassword,
                    RoleId = dto.RoleId
                };

                await _signupPublisher.PublishAsync(message);
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

                var user = await _userService.GetByEmailAsync(dto.Email);

                var token = _jwtTokenService.GenerateToken(user.Email, user.RoleId);

                _logger.LogInformation("로그인 성공: {Email}", dto.Email);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "로그인 중 오류 발생: {Email}", dto.Email);
                return StatusCode(500, "로그인 처리 중 오류 발생");
            }


        }
    }
}
