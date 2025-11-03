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

        public string GetToken() => _jwtToken;

        public bool IsAuthenticated() => !string.IsNullOrEmpty(_jwtToken);

        public async Task<Shared.DTOs.ApiResponse<LoginResponse>> LoginAsync(string username, string password)
        {
            try
            {
                Console.WriteLine($"🔐 LOGIN ATTEMPT - Username: {username}");

                var request = new LoginRequest { Username = username, Password = password };
                Console.WriteLine($"🌐 Calling API: https://localhost:7016/api/auth/login");

                var response = await _authApi.LoginAsync(request);

                Console.WriteLine($"✅ API Response - Success: {response.Success}");
                Console.WriteLine($"📝 API Response - Message: {response.Message}");

                if (response.Success && response.Data != null)
                {
                    _jwtToken = response.Data.Token;
                    _role = response.Data.Role;
                    _username = response.Data.Username;
                    Console.WriteLine($"🎉 Login Successful - Role: {_role}, Token length: {_jwtToken.Length}");
                }
                else
                {
                    Console.WriteLine($"❌ Login Failed: {response.Message}");
                }

                return response;
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"🔥 REFIT API EXCEPTION:");
                Console.WriteLine($"   Status Code: {ex.StatusCode}");
                Console.WriteLine($"   Message: {ex.Message}");
                Console.WriteLine($"   Content: {ex.Content}");
                Console.WriteLine($"   Headers: {ex.Headers}");
                return Shared.DTOs.ApiResponse<LoginResponse>.CreateFailure($"Login failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 UNEXPECTED ERROR: {ex}");
                return Shared.DTOs.ApiResponse<LoginResponse>.CreateFailure($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<Shared.DTOs.ApiResponse<object>> RegisterAsync(string username, string password, string role = "User")
        {
            try
            {
                var request = new RegisterRequest { Username = username, Password = password, Role = role };
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

        // Add to AuthService.cs
        public void SetToken(string token)
        {
            _jwtToken = token;
            // You might need to parse the token to get role and username
            // For now, we'll set them from the constructor
        }
    }
}