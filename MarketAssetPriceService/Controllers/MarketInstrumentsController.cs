using MarketAssetPriceService.Enteties;
using MarketAssetPriceService.Repositories.Impl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketAssetPriceService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketInstrumentsController : ControllerBase
    {
        private readonly MarketDataContext _context;

        public MarketInstrumentsController(MarketDataContext context)
        {
            _context = context;
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
        public async Task<ActionResult<Instrument>> GetInstrument(int id)
        {
            var instrument = await _context.Instruments.FindAsync(id);

            if (instrument == null)
            {
                return NotFound();
            }

            return instrument;
        }
    }
}
