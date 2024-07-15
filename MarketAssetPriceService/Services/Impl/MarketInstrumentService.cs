using MarketAssetPriceService.Enteties;
using MarketAssetPriceService.Repositories.Impl;
using MarketAssetPriceService.Services.Contracts;
using Newtonsoft.Json;
using System.Net.Http.Headers;

public class MarketInstrumentService : IMarketInstrumentService
{
    private readonly HttpClient _httpClient;
    private readonly MarketDataContext _context;
    private readonly IAuthService _authService;
    private readonly ILogger<MarketInstrumentService> _logger;

    public MarketInstrumentService(HttpClient httpClient, MarketDataContext context, IAuthService authService, ILogger<MarketInstrumentService> logger)
    {
        _httpClient = httpClient;
        _context = context;
        _authService = authService;
        _logger = logger;
    }

    public async Task RetrieveAndSaveInstrumentsAsync(string provider, string kind)
    {
        var token = await _authService.GetAccessTokenAsync();
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://platform.fintacharts.com/api/instruments/v1/instruments?provider={provider}&kind={kind}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var instrumentsResponse = JsonConvert.DeserializeObject<InstrumentResponse>(responseBody);

        foreach (var instrument in instrumentsResponse.Data)
        {
            if (!_context.Instruments.Any(i => i.Id == instrument.Id))
            {
                var newInstrument = new Instrument
                {
                    Id = instrument.Id,
                    Symbol = instrument.Symbol,
                    Kind = instrument.Kind,
                    Description = instrument.Description,
                    TickSize = instrument.TickSize,
                    Currency = instrument.Currency,
                    BaseCurrency = instrument.BaseCurrency,
                    Mappings = instrument.Mappings.Select(m => new MappingsDetails
                    {
                        Id = Guid.NewGuid(),
                        Provider = m.Key,
                        Symbol = m.Value.Symbol,
                        Exchange = m.Value.Exchange,
                        DefaultOrderSize = m.Value.DefaultOrderSize,
                        InstrumentId = instrument.Id
                    }).ToList()
                };

                _context.Instruments.Add(newInstrument);
            }
        }

        await _context.SaveChangesAsync();
    }
}
