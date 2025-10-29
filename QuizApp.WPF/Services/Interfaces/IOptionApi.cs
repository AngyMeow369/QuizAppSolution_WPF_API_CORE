using QuizApp.API.Models;
using Refit;

namespace QuizApp.WPF.Services.Interfaces
{
    public interface IOptionApi
    {
        [Get("/api/options")]
        Task<List<Option>> GetAllOptionsAsync([Header("Authorization")] string authorization);

        [Get("/api/options/{id}")]
        Task<Option> GetOptionByIdAsync(int id, [Header("Authorization")] string authorization);

        [Post("/api/options")]
        Task<Option> CreateOptionAsync([Body] Option option, [Header("Authorization")] string authorization);

        [Put("/api/options/{id}")]
        Task UpdateOptionAsync(int id, [Body] Option option, [Header("Authorization")] string authorization);

        [Delete("/api/options/{id}")]
        Task DeleteOptionAsync(int id, [Header("Authorization")] string authorization);
    }
}