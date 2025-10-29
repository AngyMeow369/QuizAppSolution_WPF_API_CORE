using QuizApp.API.Models;
using Refit;

namespace QuizApp.WPF.Services.Interfaces
{
    public interface IQuestionApi
    {
        [Get("/api/questions")]
        Task<List<Question>> GetAllQuestionsAsync([Header("Authorization")] string authorization);

        [Get("/api/questions/{id}")]
        Task<Question> GetQuestionByIdAsync(int id, [Header("Authorization")] string authorization);

        [Post("/api/questions")]
        Task<Question> CreateQuestionAsync([Body] Question question, [Header("Authorization")] string authorization);

        [Put("/api/questions/{id}")]
        Task UpdateQuestionAsync(int id, [Body] Question question, [Header("Authorization")] string authorization);

        [Delete("/api/questions/{id}")]
        Task DeleteQuestionAsync(int id, [Header("Authorization")] string authorization);
    }
}