using MarketAssetPriceService.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MarketAssetPriceService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarsController : ControllerBase
    {
        private readonly IBarsService _barsService;
        private readonly ILogger<BarsController> _logger;

        public BarsController(IBarsService barsService, ILogger<BarsController> logger)
        {
            _barsService = barsService;
            _logger = logger;
        }

        [HttpGet("v1/bars/count-back")]
        public async Task<IActionResult> GetBarsCountBack([FromQuery] string instrumentId, [FromQuery] string provider, [FromQuery] int interval, [FromQuery] string periodicity, [FromQuery] int barsCount)
        {
            if (string.IsNullOrEmpty(instrumentId) || string.IsNullOrEmpty(provider) || interval <= 0 || string.IsNullOrEmpty(periodicity) || barsCount <= 0)
            {
                return BadRequest("All query parameters are required and must be valid.");
            }

            try
            {
                var result = await _barsService.GetBarsCountBackAsync(instrumentId, provider, interval, periodicity, barsCount);
                var rawData = JObject.Parse(result)["data"];
                var structuredData = rawData.Select(item => new
                {
                    Time = item["t"].ToString(),
                    Open = item["o"].ToObject<decimal>(),
                    High = item["h"].ToObject<decimal>(),
                    Low = item["l"].ToObject<decimal>(),
                    Close = item["c"].ToObject<decimal>(),
                    Volume = item["v"].ToObject<int>()
                }).ToList();

                return Ok(new { data = structuredData });
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "HttpRequestException in GetBarsCountBack");
                return StatusCode((int)e.StatusCode, e.Message);
            }
            catch (JsonReaderException e)
            {
                _logger.LogError(e, "JsonReaderException in GetBarsCountBack");
                return StatusCode(500, "Error parsing JSON response");
            }
        }

        [HttpGet("v1/bars/date-range")]
        public async Task<IActionResult> GetBarsDateRange([FromQuery] string instrumentId, [FromQuery] string provider, [FromQuery] int interval, [FromQuery] string periodicity, [FromQuery] string startDate, [FromQuery] string endDate = null)
        {
            if (string.IsNullOrEmpty(instrumentId) || string.IsNullOrEmpty(provider) || interval <= 0 || string.IsNullOrEmpty(periodicity) || string.IsNullOrEmpty(startDate))
            {
                return BadRequest("All query parameters are required and must be valid.");
            }

            try
            {
                var result = await _barsService.GetBarsDateRangeAsync(instrumentId, provider, interval, periodicity, startDate, endDate);
                var rawData = JObject.Parse(result)["data"];
                var structuredData = rawData.Select(item => new
                {
                    Time = item["t"].ToString(),
                    Open = item["o"].ToObject<decimal>(),
                    High = item["h"].ToObject<decimal>(),
                    Low = item["l"].ToObject<decimal>(),
                    Close = item["c"].ToObject<decimal>(),
                    Volume = item["v"].ToObject<int>()
                }).ToList();

                return Ok(new { data = structuredData });
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "HttpRequestException in GetBarsDateRange");
                return StatusCode((int)e.StatusCode, e.Message);
            }
            catch (JsonReaderException e)
            {
                _logger.LogError(e, "JsonReaderException in GetBarsDateRange");
                return StatusCode(500, "Error parsing JSON response");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected exception in GetBarsDateRange");
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        [HttpGet("v1/bars/time-back")]
        [Produces("text/html")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public async Task<IActionResult> GetBarsTimeBack([FromQuery] string instrumentId, [FromQuery] string provider, [FromQuery] int interval, [FromQuery] string periodicity, [FromQuery] string timeBack)
        {
            if (string.IsNullOrEmpty(instrumentId) || string.IsNullOrEmpty(provider) || interval <= 0 || string.IsNullOrEmpty(periodicity) || string.IsNullOrEmpty(timeBack))
            {
                return BadRequest("All query parameters are required and must be valid.");
            }

            try
            {
                var result = await _barsService.GetBarsTimeBackAsync(instrumentId, provider, interval, periodicity, timeBack);

                // Return HTML content
                return Content(result, "text/html");
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "HttpRequestException in GetBarsTimeBack");
                return StatusCode((int)e.StatusCode, e.Message);
            }
        }
    }
}
