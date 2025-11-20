using System;
using System.Threading.Tasks;
using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services.Interfaces;
using Refit;

namespace QuizApp.WPF.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthApi _authApi;
        private string _jwtToken = string.Empty;
        private string _role = string.Empty;
        private string _username = string.Empty;

        public AuthService()
        {
            // update base url if your API host/port differ
            _authApi = RestService.For<IAuthApi>("https://localhost:7016");
        }

        public string JwtToken => _jwtToken;
        public string Role => _role;
        public string Username => _username;

        public string GetToken() => _jwtToken;

        public bool IsAuthenticated() => !string.IsNullOrEmpty(_jwtToken);

        // LOGIN: returns ApiResponse<LoginResponse>
        public async Task<Shared.DTOs.ApiResponse<LoginResponse>> LoginAsync(string username, string password)
        {
            try
            {
                var request = new LoginRequest { Username = username, Password = password };
                var response = await _authApi.LoginAsync(request);

                if (response != null && response.Success && response.Data != null)
                {
                    _jwtToken = response.Data.Token ?? string.Empty;
                    _role = response.Data.Role ?? string.Empty;
                    _username = response.Data.Username ?? string.Empty;
                }

                return response ?? Shared.DTOs.ApiResponse<LoginResponse>.CreateFailure("Null response from API.");
            }
            catch (ApiException apiEx)
            {
                return Shared.DTOs.ApiResponse<LoginResponse>.CreateFailure($"Login failed: {apiEx.Message}");
            }
            catch (Exception ex)
            {
                return Shared.DTOs.ApiResponse<LoginResponse>.CreateFailure($"Unexpected error: {ex.Message}");
            }
        }

        // REGISTER - primary API method (consume RegisterRequest)
        public async Task<Shared.DTOs.ApiResponse<object>> RegisterAsync(RegisterRequest req)
        {
            if (req is null)
                return Shared.DTOs.ApiResponse<object>.CreateFailure("RegisterRequest is null.");

            try
            {
                var response = await _authApi.RegisterAsync(req);
                return response ?? Shared.DTOs.ApiResponse<object>.CreateFailure("Null response from API.");
            }
            catch (ApiException apiEx)
            {
                return Shared.DTOs.ApiResponse<object>.CreateFailure($"Registration failed: {apiEx.Message}");
            }
            catch (Exception ex)
            {
                return Shared.DTOs.ApiResponse<object>.CreateFailure($"Unexpected error: {ex.Message}");
            }
        }

        // Optional convenience overload
        public async Task<Shared.DTOs.ApiResponse<object>> RegisterAsync(string username, string password, string role = "User")
        {
            var req = new RegisterRequest
            {
                Username = username,
                Password = password,
                Role = role
            };

            return await RegisterAsync(req);
        }

        Task<Shared.DTOs.ApiResponse<object>> IAuthService.RegisterAsync(string username, string password, string role)
        {
            throw new NotImplementedException();
        }
    }
}
