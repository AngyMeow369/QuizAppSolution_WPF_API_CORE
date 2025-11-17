using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services.Interfaces;
using Refit;

namespace QuizApp.WPF.Services.User
{
    public class UserQuizService
    {
        private readonly IUserQuizApi _quizApi;
        private readonly IAuthService _authService;

        public UserQuizService(IAuthService authService)
        {
            _authService = authService;

            // ✔ USE SHARED CLIENT + SHARED REFiT SETTINGS
            _quizApi = RestService.For<IUserQuizApi>(
                App.SharedHttpClient,
                App.SharedRefitSettings
            );
        }

        private string GetToken()
        {
            var token = _authService.GetToken();
            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("User is not authenticated");

            return $"Bearer {token}";
        }

        public async Task<List<UserAssignedQuizDto>> GetMyAssignedQuizzesAsync()
        {
            var response = await _quizApi.GetMyAssignedQuizzesAsync(GetToken());
            if (!response.Success)
                throw new Exception(response.Message ?? "Failed to load assigned quizzes");

            return response.Data ?? new List<UserAssignedQuizDto>();
        }


        public async Task<QuizTakeDto> GetQuizForTakingAsync(int quizId)
        {
            var response = await _quizApi.GetQuizForTakingAsync(quizId, GetToken());
            if (!response.Success || response.Data == null)
                throw new Exception(response.Message ?? "Failed to load quiz");
            return response.Data;
        }

        public async Task<QuizResultDto> SubmitQuizAsync(int quizId, QuizSubmissionDto submission)
        {
            var response = await _quizApi.SubmitQuizAsync(quizId, submission, GetToken());
            if (!response.Success || response.Data == null)
                throw new Exception(response.Message ?? "Failed to submit quiz");
            return response.Data;
        }
    }
}
