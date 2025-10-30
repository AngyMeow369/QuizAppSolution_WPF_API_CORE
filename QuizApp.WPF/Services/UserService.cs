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

        public async Task<List<User>> GetUsersAsync()
        {
            var token = _authService.GetToken();
            var response = await _userApi.GetAllUsersAsync($"Bearer {token}");
            return response.Success && response.Data != null ? response.Data : throw new Exception(response.Message ?? "Failed to load users");
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            var token = _authService.GetToken();
            var response = await _userApi.GetUserByIdAsync(id, $"Bearer {token}");
            return response.Success && response.Data != null ? response.Data : throw new Exception(response.Message ?? "Failed to load user");
        }

        public async Task<User> CreateUserAsync(User user)
        {
            var token = _authService.GetToken();
            var response = await _userApi.CreateUserAsync(user, $"Bearer {token}");
            return response.Success && response.Data != null ? response.Data : throw new Exception(response.Message ?? "Failed to create user");
        }

        public async Task UpdateUserAsync(User user)
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