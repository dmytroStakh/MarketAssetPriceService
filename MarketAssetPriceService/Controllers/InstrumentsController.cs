using MarketAssetPriceService.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace MarketAssetPriceService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstrumentsController : ControllerBase
    {
        private readonly IInstrumentService _instrumentService;

        public InstrumentsController(IInstrumentService instrumentService)
        {
            _instrumentService = instrumentService;
        }

        [HttpGet("v1/instruments")]
        public async Task<IActionResult> GetInstruments([FromQuery] string provider = "oanda", [FromQuery] string kind = "forex")
        {
            if (string.IsNullOrEmpty(provider) || string.IsNullOrEmpty(kind))
            {
                return BadRequest("Provider and kind are required.");
            }

            try
            {
                var result = await _instrumentService.GetInstrumentsAsync(provider, kind);
                return Ok(result);
            }
            catch (HttpRequestException e)
            {
                return StatusCode((int)e.StatusCode, e.Message);
            }
        }

        [HttpGet("v1/providers")]
        public async Task<IActionResult> GetProviders()
        {
            try
            {
                var result = await _instrumentService.GetProvidersAsync();
                return Ok(result);
            }
            catch (HttpRequestException e)
            {
                return StatusCode((int)e.StatusCode, e.Message);
            }
        }

        [HttpGet("v1/exchanges")]
        public async Task<IActionResult> GetExchanges()
        {
            try
            {
                var result = await _instrumentService.GetExchangesAsync();
                return Ok(result);
            }
            catch (HttpRequestException e)
            {
                return StatusCode((int)e.StatusCode, e.Message);
            }
        }
    }
}
