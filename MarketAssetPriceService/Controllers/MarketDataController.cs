using MarketAssetPriceService.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace MarketAssetPriceService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketDataController : ControllerBase
    {
        private readonly IWebSocketService _webSocketService;
        private readonly IMarketDataService _marketDataService;
        private readonly ILogger<MarketInstrumentsController> _logger;
        private static readonly Dictionary<string, CancellationTokenSource> _activeConnections = new Dictionary<string, CancellationTokenSource>();
        private static readonly object _lock = new object();

        public MarketDataController(IWebSocketService webSocketService, IMarketDataService marketDataService, ILogger<MarketInstrumentsController> logger)
        {
            _webSocketService = webSocketService;
            _marketDataService = marketDataService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("realtime-price/{instrumentId}")]
        public async Task GetRealTimePrice(string instrumentId)
        {
            _logger.LogInformation("Received request to start subscription for instrument ID: {InstrumentId}", instrumentId);

            var kinds = new[] { "ask", "bid", "last" };
            var cancellationTokenSource = new CancellationTokenSource();
            lock (_lock)
            {
                _activeConnections[instrumentId] = cancellationTokenSource;
            }

            await _marketDataService.SubscribeToMarketDataAsync(instrumentId, kinds);

            // Set response headers for SSE
            Response.ContentType = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["Connection"] = "keep-alive";

            void OnMessageReceived(string message)
            {
                try
                {
                    var data = $"data: {message}\n\n";
                    var bytes = Encoding.UTF8.GetBytes(data);
                    Response.Body.WriteAsync(bytes, 0, bytes.Length).Wait();
                    Response.Body.FlushAsync().Wait();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending message to client");
                    _webSocketService.UnregisterMessageHandler(OnMessageReceived);
                }
            }

            _webSocketService.RegisterMessageHandler(OnMessageReceived);

            // Keep the connection open until a cancellation request is received
            try
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await Task.Delay(100); // Adjust as needed
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Request was cancelled");
            }
            finally
            {
                _webSocketService.UnregisterMessageHandler(OnMessageReceived);
                await _webSocketService.DisconnectAsync();
            }
        }

        [HttpPost("close-connection/{instrumentId}")]
        public IActionResult CloseConnection(string instrumentId)
        {
            _logger.LogInformation("Received request to close connection for instrument ID: {InstrumentId}", instrumentId);
            lock (_lock)
            {
                if (_activeConnections.TryGetValue(instrumentId, out var cancellationTokenSource))
                {
                    cancellationTokenSource.Cancel();
                    _activeConnections.Remove(instrumentId);
                    _logger.LogInformation("Connection closed for instrument ID: {InstrumentId}", instrumentId);
                }
                else
                {
                    _logger.LogWarning("No active connection found for instrument ID: {InstrumentId}", instrumentId);
                }
            }
            return Ok("Connection closed");
        }
    }
}

