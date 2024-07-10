namespace MarketAssetPriceService.Services.Contracts
{
    public interface IMarketDataService
    {
        Task SubscribeToMarketDataAsync(string instrumentId, string[] kinds);
    }
}
