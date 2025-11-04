using QuizApp.Shared.DTOs;
using Refit;

namespace QuizApp.WPF.Services.Interfaces
{
    public interface IUserDashboardApi
    {
        [Get("/api/user/dashboard/summary")]
        Task<Shared.DTOs.ApiResponse<UserDashboardDto>> GetDashboardSummaryAsync([Header("Authorization")] string authorization);
    }

    public interface IUserQuizApi
    {
        [Get("/api/user/quizzes/my-assigned")]
        Task<Shared.DTOs.ApiResponse<List<QuizDto>>> GetMyAssignedQuizzesAsync([Header("Authorization")] string authorization);

        [Get("/api/user/quizzes/{id}/take")]
        Task<Shared.DTOs.ApiResponse<QuizTakeDto>> GetQuizForTakingAsync(int id, [Header("Authorization")] string authorization);

        [Post("/api/user/quizzes/{id}/submit")]
        Task<Shared.DTOs.ApiResponse<QuizResultDto>> SubmitQuizAsync(int id, [Body] QuizSubmissionDto submission, [Header("Authorization")] string authorization);
    }

    public interface IUserResultsApi
    {
        [Get("/api/user/results/my-results")]
        Task<Shared.DTOs.ApiResponse<List<QuizResultDto>>> GetMyResultsAsync([Header("Authorization")] string authorization);

        [Get("/api/user/results/{id}")]
        Task<Shared.DTOs.ApiResponse<QuizResultDto>> GetResultByIdAsync(int id, [Header("Authorization")] string authorization);
    }
}