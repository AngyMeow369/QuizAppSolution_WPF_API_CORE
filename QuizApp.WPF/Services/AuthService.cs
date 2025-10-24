using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using QuizApp.Shared.DTOs; // Move LoginResponse, RegisterRequest, etc., here

namespace QuizApp.WPF.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private string _jwtToken = string.Empty;

        public AuthService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7016/api/")
            };
        }

        public string JwtToken => _jwtToken;

        public async Task<bool> LoginAsync(string username, string password)
        {
            var payload = new LoginRequest
            {
                Username = username,
                Password = password
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("auth/login", content);

            if (!response.IsSuccessStatusCode)
                return false;

            var json = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _jwtToken = loginResponse?.Token ?? string.Empty;
            return !string.IsNullOrEmpty(_jwtToken);
        }

        public async Task<bool> RegisterAsync(string username, string password, string role = "User")
        {
            var payload = new RegisterRequest
            {
                Username = username,
                Password = password,
                Role = role
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("auth/register", content);

            return response.IsSuccessStatusCode;
        }

        public HttpClient GetAuthenticatedClient()
        {
            if (string.IsNullOrEmpty(_jwtToken))
                throw new InvalidOperationException("User is not logged in.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
            return _httpClient;
        }
    }
}
