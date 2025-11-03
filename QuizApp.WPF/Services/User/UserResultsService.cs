using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services.Interfaces;
using Refit;

namespace QuizApp.WPF.Services.User
{
    public interface IUserResultsApi
    {
        [Get("/api/user/results/my-results")]
        Task<Shared.DTOs.ApiResponse<List<QuizResultDto>>> GetMyResultsAsync([Header("Authorization")] string authorization);

        [Get("/api/user/results/{id}")]
        Task<Shared.DTOs.ApiResponse<QuizResultDto>> GetResultByIdAsync(int id, [Header("Authorization")] string authorization);
    }

    public class UserResultsService
    {
        private readonly IUserResultsApi _resultsApi;
        private readonly IAuthService _authService;

        public UserResultsService(IAuthService authService)
        {
            _authService = authService;
            _resultsApi = RestService.For<IUserResultsApi>("https://localhost:7016");
        }

        private string GetToken()
        {
            var token = _authService.GetToken();
            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("User is not authenticated");
            return $"Bearer {token}";
        }

        public async Task<List<QuizResultDto>> GetMyResultsAsync()
        {
            var response = await _resultsApi.GetMyResultsAsync(GetToken());
            if (!response.Success) throw new Exception(response.Message);
            return response.Data ?? new List<QuizResultDto>();
        }

        public async Task<QuizResultDto> GetResultByIdAsync(int resultId)
        {
            var response = await _resultsApi.GetResultByIdAsync(resultId, GetToken());
            if (!response.Success || response.Data == null)
                throw new Exception(response.Message);
            return response.Data;
        }
    }
}