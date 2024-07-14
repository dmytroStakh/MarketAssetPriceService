namespace MarketAssetPriceService.Services.Contracts
{
    public interface IMarketInstrumentService
    {
        Task RetrieveAndSaveInstrumentsAsync(string provider, string kind);
    }
}
