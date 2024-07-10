namespace MarketAssetPriceService.Services.Contracts
{
    public interface IWebSocketService
    {
        bool IsConnected { get; }
        Task ConnectAsync();
        Task DisconnectAsync();
        Task SendMessageAsync(string message);
        Task ReceiveMessagesAsync(CancellationToken cancellationToken);
        void RegisterMessageHandler(Action<string> handler);
        void UnregisterMessageHandler(Action<string> handler);
    }
}
