using QuizApp.API.Models;
using QuizApp.Shared.DTOs;
using Refit;

namespace QuizApp.WPF.Services.Interfaces
{
    public interface IQuestionApi
    {
        [Get("/api/questions")]
        Task<Shared.DTOs.ApiResponse<List<Question>>> GetAllQuestionsAsync([Header("Authorization")] string authorization);

        [Get("/api/questions/{id}")]
        Task<Shared.DTOs.ApiResponse<Question>> GetQuestionByIdAsync(int id, [Header("Authorization")] string authorization);

        [Post("/api/questions")]
        Task<Shared.DTOs.ApiResponse<Question>> CreateQuestionAsync([Body] Question question, [Header("Authorization")] string authorization);

        [Put("/api/questions/{id}")]
        Task<Shared.DTOs.ApiResponse<object>> UpdateQuestionAsync(int id, [Body] Question question, [Header("Authorization")] string authorization);

        [Delete("/api/questions/{id}")]
        Task<Shared.DTOs.ApiResponse<object>> DeleteQuestionAsync(int id, [Header("Authorization")] string authorization);
    }
}