using MarketAssetPriceService.Services.Contracts;
using System.Net.Http.Headers;

namespace MarketAssetPriceService.Services.Impl
{
    public class BarsService : IBarsService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly ILogger<BarsService> _logger;

        public BarsService(HttpClient httpClient, IAuthService authService, ILogger<BarsService> logger)
        {
            _httpClient = httpClient;
            _authService = authService;
            _logger = logger;
        }

        public async Task<string> GetBarsCountBackAsync(string instrumentId, string provider, int interval, string periodicity, int barsCount)
        {
            var token = await _authService.GetAccessTokenAsync();
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://platform.fintacharts.com/api/bars/v1/bars/count-back?instrumentId={instrumentId}&provider={provider}&interval={interval}&periodicity={periodicity}&barsCount={barsCount}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"GetBarsCountBackAsync response: {content}");
            return content;
        }

        public async Task<string> GetBarsTimeBackAsync(string instrumentId, string provider, int interval, string periodicity, string timeBack)
        {
            var token = await _authService.GetAccessTokenAsync();
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://platform.fintacharts.com/api/data-consolidators/bars/v1/bars/time-back?instrumentId={instrumentId}&provider={provider}&interval={interval}&periodicity={periodicity}&timeBack={timeBack}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Response Status Code: {StatusCode}", response.StatusCode);
            _logger.LogInformation("Response Headers: {Headers}", response.Headers);
            _logger.LogInformation("Response Content: {Content}", content);

            return content;
        }

        public async Task<string> GetBarsDateRangeAsync(string instrumentId, string provider, int interval, string periodicity, string startDate, string endDate)
        {
            var token = await _authService.GetAccessTokenAsync();
            var requestUrl = $"https://platform.fintacharts.com/api/bars/v1/bars/date-range?instrumentId={instrumentId}&provider={provider}&interval={interval}&periodicity={periodicity}&startDate={startDate}";

            if (!string.IsNullOrEmpty(endDate))
            {
                requestUrl += $"&endDate={endDate}";
            }

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            _logger.LogInformation($"Request URL: {requestUrl}");

            try
            {
                var response = await _httpClient.SendAsync(request);
                _logger.LogInformation($"Response Status Code: {response.StatusCode}");

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"GetBarsDateRangeAsync response: {content}");

                return content;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"HttpRequestException in GetBarsDateRangeAsync: {e.StatusCode}");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected exception in GetBarsDateRangeAsync");
                throw;
            }
        }
    }
}
