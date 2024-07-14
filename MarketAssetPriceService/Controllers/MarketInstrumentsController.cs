using MarketAssetPriceService.Enteties;
using MarketAssetPriceService.Repositories.Impl;
using MarketAssetPriceService.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketAssetPriceService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketInstrumentsController : ControllerBase
    {
        private readonly MarketDataContext _context;
        private readonly IMarketInstrumentService _marketInstrumentService;

        public MarketInstrumentsController(MarketDataContext context, IMarketInstrumentService marketInstrumentService)
        {
            _context = context;
            _marketInstrumentService = marketInstrumentService;
        }

        // GET: api/Instruments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Instrument>>> GetInstruments()
        {
            return await _context.Instruments.ToListAsync();
        }

        // POST: api/Instruments
        [HttpPost]
        public async Task<ActionResult<Instrument>> PostInstrument(Instrument instrument)
        {
            _context.Instruments.Add(instrument);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInstrument), new { id = instrument.Id }, instrument);
        }

        // GET: api/Instruments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Instrument>> GetInstrument(Guid id)
        {
            var instrument = await _context.Instruments.FindAsync(id);

            if (instrument == null)
            {
                return NotFound();
            }

            return instrument;
        }

        // GET: api/MarketInstruments/v1/update-instruments
        [HttpGet("v1/update-instruments")]
        public async Task<IActionResult> UpdateInstruments([FromQuery] string provider = "oanda", [FromQuery] string kind = "forex")
        {
            if (string.IsNullOrEmpty(provider) || string.IsNullOrEmpty(kind))
            {
                return BadRequest("Provider and kind are required.");
            }

            try
            {
                await _marketInstrumentService.RetrieveAndSaveInstrumentsAsync(provider, kind);
                return Ok("Instruments updated successfully.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
    }
}
