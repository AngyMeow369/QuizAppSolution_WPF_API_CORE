using QuizApp.API.Models;
using QuizApp.Shared.DTOs;
using Refit;

namespace QuizApp.WPF.Services.Interfaces
{
    public interface IQuizApi
    {
        [Get("/api/quizzes")]
        Task<Shared.DTOs.ApiResponse<List<Quiz>>> GetAllQuizzesAsync([Header("Authorization")] string authorization);

        [Get("/api/quizzes/{id}")]
        Task<Shared.DTOs.ApiResponse<Quiz>> GetQuizByIdAsync(int id, [Header("Authorization")] string authorization);

        [Post("/api/quizzes")]
        Task<Shared.DTOs.ApiResponse<Quiz>> CreateQuizAsync([Body] Quiz quiz, [Query(CollectionFormat.Multi)] List<int> questionIds, [Header("Authorization")] string authorization);

        [Put("/api/quizzes/{id}")]
        Task<Shared.DTOs.ApiResponse<object>> UpdateQuizAsync(int id, [Body] Quiz quiz, [Header("Authorization")] string authorization);

        [Delete("/api/quizzes/{id}")]
        Task<Shared.DTOs.ApiResponse<object>> DeleteQuizAsync(int id, [Header("Authorization")] string authorization);

        [Post("/api/quizzes/{quizId}/assign/{userId}")]
        Task<Shared.DTOs.ApiResponse<QuizAssignment>> AssignQuizAsync(int quizId, int userId, [Header("Authorization")] string authorization);

        [Delete("/api/quizzes/{quizId}/assign/{userId}")]
        Task<Shared.DTOs.ApiResponse<object>> RemoveAssignmentAsync(int quizId, int userId, [Header("Authorization")] string authorization);
    }
}