namespace MarketAssetPriceService.Services.Contracts
{
    public interface IBarsService
    {
        Task<string> GetBarsCountBackAsync(string instrumentId, string provider, int interval, string periodicity, int barsCount);
        Task<string> GetBarsTimeBackAsync(string instrumentId, string provider, int interval, string periodicity, string timeBack);
        Task<string> GetBarsDateRangeAsync(string instrumentId, string provider, int interval, string periodicity, string startDate, string endDate);
    }
}