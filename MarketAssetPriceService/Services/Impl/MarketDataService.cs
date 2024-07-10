using MarketAssetPriceService.Services.Contracts;
using Newtonsoft.Json;

namespace MarketAssetPriceService.Services.Impl
{
    public class MarketDataService : IMarketDataService
    {
        private readonly IWebSocketService _webSocketService;
        private readonly ILogger<MarketDataService> _logger;

        public MarketDataService(IWebSocketService webSocketService, ILogger<MarketDataService> logger)
        {
            _webSocketService = webSocketService;
            _logger = logger;
        }

        public async Task SubscribeToMarketDataAsync(string instrumentId, string[] kinds)
        {
            if (!_webSocketService.IsConnected)
            {
                await _webSocketService.ConnectAsync();
            }

            var subscriptionMessage = new
            {
                type = "l1-subscription",
                id = "1",
                instrumentId = instrumentId,
                provider = "simulation",
                subscribe = true,
                kinds = kinds
            };

            string message = JsonConvert.SerializeObject(subscriptionMessage);
            await _webSocketService.SendMessageAsync(message);
        }
    }
}
