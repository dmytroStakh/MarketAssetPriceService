using MarketAssetPriceService.Enteties;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace MarketAssetPriceService.Repositories.Impl
{
    public class MarketDataContext : DbContext
    {
        public MarketDataContext(DbContextOptions<MarketDataContext> options) : base(options)
        {
        }

        public DbSet<Instrument> Instruments { get; set; }
    }
}
