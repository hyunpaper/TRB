using Microsoft.AspNetCore.Mvc;
using TRB.Server.Application.DTOs;
using TRB.Server.Application.Interfaces;


namespace TRB.Server.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDto dto)
        {
            try
            {
                await _userService.CreateAsync(dto);

                _logger.LogInformation("회원가입 성공 {@UserLog}", new
                {
                    Event = "UserRegisterSuccess",
                    Email = dto.Email,
                    Timestamp = DateTime.UtcNow
                });

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
    }
}
