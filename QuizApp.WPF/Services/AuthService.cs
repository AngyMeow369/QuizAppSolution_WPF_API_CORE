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
            _authApi = RestService.For<IAuthApi>("https://localhost:7016");
        }

        public string JwtToken => _jwtToken;
        public string Role => _role;
        public string Username => _username;

        // ADD THESE MISSING METHODS:
        public string GetToken()
        {
            return _jwtToken;
        }

        public bool IsAuthenticated()
        {
            return !string.IsNullOrEmpty(_jwtToken);
        }

        // FIX RETURN TYPES - Remove the namespace prefixes since we have using statement
        public async Task<Shared.DTOs.ApiResponse<LoginResponse>> LoginAsync(string username, string password)
        {
            try
            {
                var request = new LoginRequest
                {
                    Username = username,
                    Password = password
                };

                var response = await _authApi.LoginAsync(request);

                if (response?.Success == true && response.Data is not null)
                {
                    _jwtToken = response.Data.Token;
                    _role = response.Data.Role;
                    _username = response.Data.Username;
                }

                return response ?? Shared.DTOs.ApiResponse<LoginResponse>.CreateFailure("Null response from API.");
            }
            catch (ApiException ex)
            {
                return Shared.DTOs.ApiResponse<LoginResponse>.CreateFailure($"Login failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Shared.DTOs.ApiResponse<LoginResponse>.CreateFailure($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<Shared.DTOs.ApiResponse<object>> RegisterAsync(string username, string password, string role = "User")
        {
            try
            {
                var request = new RegisterRequest
                {
                    Username = username,
                    Password = password,
                    Role = role
                };

                var response = await _authApi.RegisterAsync(request);
                return response;
            }
            catch (ApiException ex)
            {
                return Shared.DTOs.ApiResponse<object>.CreateFailure($"Registration failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Shared.DTOs.ApiResponse<object>.CreateFailure($"Unexpected error: {ex.Message}");
            }
        }

        Task<Shared.DTOs.ApiResponse<LoginResponse>> IAuthService.LoginAsync(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}