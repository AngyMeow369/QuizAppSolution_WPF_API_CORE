using QuizApp.API.Models;
using QuizApp.WPF.Services.Interfaces;
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
            return await _userApi.GetAllUsersAsync($"Bearer {token}");
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            var token = _authService.GetToken();
            return await _userApi.GetUserByIdAsync(id, $"Bearer {token}");
        }

        public async Task<User> CreateUserAsync(User user)
        {
            var token = _authService.GetToken();
            return await _userApi.CreateUserAsync(user, $"Bearer {token}");
        }

        public async Task UpdateUserAsync(User user)
        {
            var token = _authService.GetToken();
            await _userApi.UpdateUserAsync(user.Id, user, $"Bearer {token}");
        }

        public async Task DeleteUserAsync(int userId)
        {
            var token = _authService.GetToken();
            await _userApi.DeleteUserAsync(userId, $"Bearer {token}");
        }
    }
}