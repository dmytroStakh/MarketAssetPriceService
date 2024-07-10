using MarketAssetPriceService.Services.Contracts;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace MarketAssetPriceService.Services.Impl
{
    public class WebSocketService: IWebSocketService
    {
        private ClientWebSocket _webSocket;
        private readonly string _webSocketUrl;
        private readonly ConcurrentQueue<string> _messageQueue;
        private readonly List<Action<string>> _messageHandlers;
        private readonly ILogger<WebSocketService> _logger;
        private CancellationTokenSource _cancellationTokenSource;
        private static readonly object _lock = new object();

        public WebSocketService(string webSocketUrl, ILogger<WebSocketService> logger)
        {
            _webSocket = new ClientWebSocket();
            _webSocketUrl = webSocketUrl;
            _messageQueue = new ConcurrentQueue<string>();
            _messageHandlers = new List<Action<string>>();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public bool IsConnected => _webSocket.State == WebSocketState.Open;

        public async Task ConnectAsync()
        {
            lock (_lock)
            {
                _cancellationTokenSource = new CancellationTokenSource();
            }
            await _webSocket.ConnectAsync(new Uri(_webSocketUrl), _cancellationTokenSource.Token);
            _logger.LogInformation("Connected to WebSocket");
            _ = ReceiveMessagesAsync(_cancellationTokenSource.Token);
        }

        public async Task DisconnectAsync()
        {
            lock (_lock)
            {
                _cancellationTokenSource.Cancel();
            }
            if (_webSocket.State == WebSocketState.Open || _webSocket.State == WebSocketState.CloseReceived || _webSocket.State == WebSocketState.CloseSent)
            {
                try
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    _logger.LogInformation("Disconnected from WebSocket");
                }
                catch (WebSocketException ex)
                {
                    _logger.LogError(ex, "Error during WebSocket close");
                }
            }
        }

        public async Task SendMessageAsync(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
        }

        public async Task ReceiveMessagesAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[1024 * 4];
            while (_webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
                else
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    _messageQueue.Enqueue(message);
                    foreach (var handler in _messageHandlers)
                    {
                        handler(message);
                    }
                }
            }
        }

        public void RegisterMessageHandler(Action<string> handler)
        {
            _messageHandlers.Add(handler);
        }

        public void UnregisterMessageHandler(Action<string> handler)
        {
            _messageHandlers.Remove(handler);
        }
    }
}
