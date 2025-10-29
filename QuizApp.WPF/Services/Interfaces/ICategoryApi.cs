using QuizApp.API.Models;
using Refit;

namespace QuizApp.WPF.Services.Interfaces
{
    public interface ICategoryApi
    {
        [Get("/api/categories")]
        Task<List<Category>> GetAllCategoriesAsync([Header("Authorization")] string authorization);

        [Get("/api/categories/{id}")]
        Task<Category> GetCategoryByIdAsync(int id, [Header("Authorization")] string authorization);

        [Post("/api/categories")]
        Task<Category> CreateCategoryAsync([Body] Category category, [Header("Authorization")] string authorization);

        [Put("/api/categories/{id}")]
        Task UpdateCategoryAsync(int id, [Body] Category category, [Header("Authorization")] string authorization);

        [Delete("/api/categories/{id}")]
        Task DeleteCategoryAsync(int id, [Header("Authorization")] string authorization);
    }
}