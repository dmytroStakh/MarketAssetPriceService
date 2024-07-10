using MarketAssetPriceService.Services.Contracts;
using Newtonsoft.Json.Linq;

namespace MarketAssetPriceService.Services.Impl
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _authUrl;
        private readonly string _username;
        private readonly string _password;
        private string _accessToken;
        private DateTime _tokenExpiration;

        public AuthService(string authUrl, string username, string password)
        {
            _httpClient = new HttpClient();
            _authUrl = authUrl;
            _username = username;
            _password = password;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (string.IsNullOrEmpty(_accessToken) || _tokenExpiration <= DateTime.UtcNow)
            {
                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", "app-cli"),
                new KeyValuePair<string, string>("username", _username),
                new KeyValuePair<string, string>("password", _password)
            });

                var response = await _httpClient.PostAsync(_authUrl, content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseString);
                _accessToken = json["access_token"].ToString();
                _tokenExpiration = DateTime.UtcNow.AddSeconds(json["expires_in"].ToObject<int>() - 60); // Refresh token 1 minute before expiration
            }

            return _accessToken;
        }
    }
}
