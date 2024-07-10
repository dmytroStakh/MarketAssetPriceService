using MarketAssetPriceService.Services.Contracts;
using System.Net.Http.Headers;

namespace MarketAssetPriceService.Services.Impl
{
    public class InstrumentService : IInstrumentService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public InstrumentService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<string> GetInstrumentsAsync(string provider, string kind)
        {
            var token = await _authService.GetAccessTokenAsync();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://platform.fintacharts.com/api/instruments/v1/instruments?provider={provider}&kind={kind}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetProvidersAsync()
        {
            var token = await _authService.GetAccessTokenAsync();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://platform.fintacharts.com/api/instruments/v1/providers");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetExchangesAsync()
        {
            var token = await _authService.GetAccessTokenAsync();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://platform.fintacharts.com/api/instruments/v1/exchanges");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
