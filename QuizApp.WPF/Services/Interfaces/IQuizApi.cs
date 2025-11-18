using QuizApp.Shared.DTOs;
using Refit;

namespace QuizApp.WPF.Services.Interfaces
{
    public interface IQuizApi
    {
        [Get("/api/quizzes")]
        Task<Shared.DTOs.ApiResponse<List<QuizDto>>> GetAllQuizzesAsync([Header("Authorization")] string authorization);

        [Get("/api/quizzes/{id}")]
        Task<Shared.DTOs.ApiResponse<QuizDto>> GetQuizByIdAsync(int id, [Header("Authorization")] string authorization);

        [Post("/api/quizzes")]
        Task<Shared.DTOs.ApiResponse<QuizDto>> CreateQuizAsync([Body] QuizDto quiz, [Query(CollectionFormat.Multi)] List<int> questionIds, [Header("Authorization")] string authorization);

        [Put("/api/quizzes/{id}")]
        Task<Shared.DTOs.ApiResponse<object>> UpdateQuizAsync(int id, [Body] QuizDto quiz, [Header("Authorization")] string authorization);

        [Delete("/api/quizzes/{id}")]
        Task<Shared.DTOs.ApiResponse<object>> DeleteQuizAsync(int id, [Header("Authorization")] string authorization);

        [Post("/api/quizzes/{quizId}/assign/{userId}")]
        Task<Shared.DTOs.ApiResponse<Object>> AssignQuizAsync(int quizId, int userId, [Header("Authorization")] string authorization);

        [Delete("/api/quizzes/{quizId}/assign/{userId}")]
        Task<Shared.DTOs.ApiResponse<object>> RemoveAssignmentAsync(int quizId, int userId, [Header("Authorization")] string authorization);
    }
}
