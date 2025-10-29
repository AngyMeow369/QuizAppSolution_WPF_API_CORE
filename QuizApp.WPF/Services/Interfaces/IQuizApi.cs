using QuizApp.API.Models;
using Refit;

namespace QuizApp.WPF.Services.Interfaces
{
    public interface IQuizApi
    {
        [Get("/api/quizzes")]
        Task<List<Quiz>> GetAllQuizzesAsync([Header("Authorization")] string authorization);

        [Get("/api/quizzes/{id}")]
        Task<Quiz> GetQuizByIdAsync(int id, [Header("Authorization")] string authorization);

        [Post("/api/quizzes")]
        Task<Quiz> CreateQuizAsync([Body] Quiz quiz, [Query(CollectionFormat.Multi)] List<int> questionIds, [Header("Authorization")] string authorization);

        [Put("/api/quizzes/{id}")]
        Task UpdateQuizAsync(int id, [Body] Quiz quiz, [Header("Authorization")] string authorization);

        [Delete("/api/quizzes/{id}")]
        Task DeleteQuizAsync(int id, [Header("Authorization")] string authorization);

        [Post("/api/quizzes/{quizId}/assign/{userId}")]
        Task AssignQuizAsync(int quizId, int userId, [Header("Authorization")] string authorization);

        [Delete("/api/quizzes/{quizId}/assign/{userId}")]
        Task RemoveAssignmentAsync(int quizId, int userId, [Header("Authorization")] string authorization);
    }
}