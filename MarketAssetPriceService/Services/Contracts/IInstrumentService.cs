namespace MarketAssetPriceService.Services.Contracts
{
    public interface IInstrumentService
    {
        Task<string> GetInstrumentsAsync(string provider, string kind);
        Task<string> GetProvidersAsync();
        Task<string> GetExchangesAsync();
    }
}
