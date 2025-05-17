using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace TRB.Server.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpbitController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UpbitController> _logger;

        public UpbitController(IHttpClientFactory factory, ILogger<UpbitController> logger)
        {
            _httpClient = factory.CreateClient();
            _logger = logger;
        }

        [HttpGet("candles")]
        public async Task<IActionResult> GetCandles(
            [FromQuery] string market,
            [FromQuery] int count = 30,
            [FromQuery] int interval = 1) // ✅ 추가
        {
            try
            {
                var url = $"https://api.upbit.com/v1/candles/minutes/{interval}?market={market}&count={count}"; 
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("업비트 API 호출 실패: {StatusCode}", response.StatusCode);
                    return StatusCode((int)response.StatusCode, "업비트 API 호출 실패");
                }

                var json = await response.Content.ReadAsStringAsync();
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "업비트 API 프록시 중 오류 발생");
                return StatusCode(500, "서버 내부 오류");
            }
        }

    }
}
