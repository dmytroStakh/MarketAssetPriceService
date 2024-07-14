using MarketAssetPriceService.Repositories.Impl;
using MarketAssetPriceService.Services.Contracts;
using MarketAssetPriceService.Services.Impl;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

// Register MarketDataContext with the connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new ArgumentNullException(nameof(connectionString), "Connection string 'MarketDataDB' not found.");
}

builder.Services.AddDbContext<MarketDataContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register AuthService with the provided credentials
builder.Services.AddSingleton<IAuthService>(new AuthService(
    "https://platform.fintacharts.com/identity/realms/fintatech/protocol/openid-connect/token",
    "r_test@fintatech.com",
    "kisfiz-vUnvy9-sopnyv"));

// Register WebSocketService with the token and logger
builder.Services.AddScoped<IWebSocketService>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<WebSocketService>>();
    var authService = provider.GetRequiredService<IAuthService>();
    var token = authService.GetAccessTokenAsync().Result;
    return new WebSocketService($"wss://platform.fintacharts.com/api/streaming/ws/v1/realtime?token={token}", logger);
});

// Register other services
builder.Services.AddScoped<IInstrumentService, InstrumentService>();
builder.Services.AddScoped<IBarsService, BarsService>();
builder.Services.AddScoped<IMarketDataService, MarketDataService>();
builder.Services.AddScoped<IMarketInstrumentService, MarketInstrumentService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();
