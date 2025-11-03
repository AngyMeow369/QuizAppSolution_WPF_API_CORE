using QuizApp.API.Models;
using QuizApp.WPF.Services.Interfaces;
using QuizApp.Shared.DTOs;  
using Refit;

namespace QuizApp.WPF.Services
{
    public class UserService
    {
        private readonly IUserApi _userApi;
        private readonly IAuthService _authService;

        public UserService(IAuthService authService)
        {
            _authService = authService;
            _userApi = RestService.For<IUserApi>("https://localhost:7016");
        }

        public async Task<List<UserDto>> GetUsersAsync()
        {
            try
            {
                Console.WriteLine("=== USER SERVICE DEBUG ===");

                var token = _authService.GetToken();
                Console.WriteLine($"🔑 Token exists: {!string.IsNullOrEmpty(token)}");
                Console.WriteLine($"🔑 Token: {token}");

                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("❌ NO TOKEN - Cannot call API");
                    throw new UnauthorizedAccessException("User is not authenticated");
                }

                Console.WriteLine($"🌐 Calling API: /api/users");

                var response = await _userApi.GetAllUsersAsync($"Bearer {token}");

                Console.WriteLine($"📡 API Response - Success: {response.Success}");
                Console.WriteLine($"📡 API Response - Message: {response.Message}");
                Console.WriteLine($"📡 API Response - Data Count: {response.Data?.Count}");

                if (!response.Success)
                {
                    Console.WriteLine($"❌ API Error: {response.Message}");
                    throw new Exception(response.Message ?? "Failed to load users");
                }

                Console.WriteLine($"✅ Successfully loaded {response.Data?.Count} users");
                return response.Data ?? new List<UserDto>();
            }
            catch (ApiException ex)
            {
                Console.WriteLine($"❌ REFIT API Exception:");
                Console.WriteLine($"   Status: {ex.StatusCode}");
                Console.WriteLine($"   Message: {ex.Message}");
                Console.WriteLine($"   Content: {ex.Content}");
                throw new Exception($"API Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ UserService Exception: {ex}");
                throw;
            }
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var token = _authService.GetToken();
            var response = await _userApi.GetUserByIdAsync(id, $"Bearer {token}");
            return response.Success && response.Data != null ? response.Data : throw new Exception(response.Message ?? "Failed to load user");
        }

        public async Task<UserDto> CreateUserAsync(UserDto user)
        {
            var token = _authService.GetToken();
            var response = await _userApi.CreateUserAsync(user, $"Bearer {token}");
            return response.Success && response.Data != null ? response.Data : throw new Exception(response.Message ?? "Failed to create user");
        }

        public async Task UpdateUserAsync(UserDto user)
        {
            var token = _authService.GetToken();
            var response = await _userApi.UpdateUserAsync(user.Id, user, $"Bearer {token}");
            if (!response.Success) throw new Exception(response.Message ?? "Failed to update user");
        }

        public async Task DeleteUserAsync(int userId)
        {
            var token = _authService.GetToken();
            var response = await _userApi.DeleteUserAsync(userId, $"Bearer {token}");
            if (!response.Success) throw new Exception(response.Message ?? "Failed to delete user");
        }
    }
}