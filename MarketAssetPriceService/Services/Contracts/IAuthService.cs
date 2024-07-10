namespace MarketAssetPriceService.Services.Contracts
{
    public interface IAuthService
    {
        Task<string> GetAccessTokenAsync();
    }
}
