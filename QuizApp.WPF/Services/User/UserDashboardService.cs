using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services.Interfaces;
using Refit;

namespace QuizApp.WPF.Services.User
{
    public interface IUserDashboardApi
    {
        [Get("/api/user/dashboard/summary")]
        Task<Shared.DTOs.ApiResponse<UserDashboardDto>> GetDashboardSummaryAsync([Header("Authorization")] string authorization);
    }

    public class UserDashboardService
    {
        private readonly IUserDashboardApi _dashboardApi;
        private readonly IAuthService _authService;

        public UserDashboardService(IAuthService authService)
        {
            _authService = authService;
            _dashboardApi = RestService.For<IUserDashboardApi>("https://localhost:7016");
        }

        private string GetToken()
        {
            var token = _authService.GetToken();
            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("User is not authenticated");
            return $"Bearer {token}";
        }

        public async Task<UserDashboardDto> GetDashboardSummaryAsync()
        {
            var response = await _dashboardApi.GetDashboardSummaryAsync(GetToken());
            if (!response.Success || response.Data == null)
                throw new Exception(response.Message);
            return response.Data;
        }
    }
}