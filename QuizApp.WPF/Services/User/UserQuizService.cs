using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services.Interfaces;
using Refit;

namespace QuizApp.WPF.Services.User
{
    public interface IUserQuizApi
    {
        [Get("/api/user/quizzes/my-assigned")]
        Task<Shared.DTOs.ApiResponse<List<QuizDto>>> GetMyAssignedQuizzesAsync([Header("Authorization")] string authorization);

        [Get("/api/user/quizzes/{id}/take")]
        Task<Shared.DTOs.ApiResponse<QuizTakeDto>> GetQuizForTakingAsync(int id, [Header("Authorization")] string authorization);

        [Post("/api/user/quizzes/{id}/submit")]
        Task<Shared.DTOs.ApiResponse<QuizResultDto>> SubmitQuizAsync(int id, [Body] QuizSubmissionDto submission, [Header("Authorization")] string authorization);
    }

    public class UserQuizService
    {
        private readonly IUserQuizApi _quizApi;
        private readonly IAuthService _authService;

        public UserQuizService(IAuthService authService)
        {
            _authService = authService;
            _quizApi = RestService.For<IUserQuizApi>("https://localhost:7016");
        }

        private string GetToken()
        {
            var token = _authService.GetToken();
            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("User is not authenticated");
            return $"Bearer {token}";
        }

        public async Task<List<QuizDto>> GetMyAssignedQuizzesAsync()
        {
            var response = await _quizApi.GetMyAssignedQuizzesAsync(GetToken());
            if (!response.Success) throw new Exception(response.Message);
            return response.Data ?? new List<QuizDto>();
        }

        public async Task<QuizTakeDto> GetQuizForTakingAsync(int quizId)
        {
            var response = await _quizApi.GetQuizForTakingAsync(quizId, GetToken());
            if (!response.Success || response.Data == null)
                throw new Exception(response.Message);
            return response.Data;
        }

        public async Task<QuizResultDto> SubmitQuizAsync(int quizId, QuizSubmissionDto submission)
        {
            var response = await _quizApi.SubmitQuizAsync(quizId, submission, GetToken());
            if (!response.Success || response.Data == null)
                throw new Exception(response.Message);
            return response.Data;
        }
    }
}