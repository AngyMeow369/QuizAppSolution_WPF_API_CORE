using QuizApp.Shared.DTOs;
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

        public async Task<List<UserDto>> GetUsersAsync()
        {
            var token = _authService.GetToken();
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("User is not authenticated");

            var response = await _userApi.GetAllUsersAsync($"Bearer {token}");
            if (!response.Success)
                throw new Exception(response.Message ?? "Failed to load users");

            return response.Data ?? new List<UserDto>();
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var token = _authService.GetToken();
            var response = await _userApi.GetUserByIdAsync(id, $"Bearer {token}");
            return response.Success && response.Data != null
                ? response.Data
                : throw new Exception(response.Message ?? "Failed to load user");
        }

        public async Task<UserDto> CreateUserAsync(UserDto user)
        {
            var token = _authService.GetToken();
            var response = await _userApi.CreateUserAsync(user, $"Bearer {token}");
            return response.Success && response.Data != null
                ? response.Data
                : throw new Exception(response.Message ?? "Failed to create user");
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

        // ✅ This is the key method for ManageUsersView
        public async Task<List<QuizDto>> GetAssignedQuizzesAsync(int userId)
        {
            var token = _authService.GetToken();
            var response = await _userApi.GetAssignedQuizzesAsync(userId, $"Bearer {token}");
            if (!response.Success)
                throw new Exception(response.Message ?? "Failed to load assigned quizzes.");
            return response.Data ?? new List<QuizDto>();
        }

        public async Task AssignQuizAsync(int userId, int quizId)
        {
            var token = _authService.GetToken();
            var response = await _userApi.AssignQuizAsync(userId, quizId, $"Bearer {token}");
            if (!response.Success)
                throw new Exception(response.Message ?? "Failed to assign quiz.");
        }
    }
}
