using QuizApp.API.Models;
using QuizApp.Shared.DTOs;
using Refit;

namespace QuizApp.WPF.Services.Interfaces
{
    public interface ICategoryApi
    {
        [Get("/api/categories")]
        Task<Shared.DTOs.ApiResponse<List<Category>>> GetAllCategoriesAsync([Header("Authorization")] string authorization);

        [Get("/api/categories/{id}")]
        Task<Shared.DTOs.ApiResponse<Category>> GetCategoryByIdAsync(int id, [Header("Authorization")] string authorization);

        [Post("/api/categories")]
        Task<Shared.DTOs.ApiResponse<Category>> CreateCategoryAsync([Body] Category category, [Header("Authorization")] string authorization);

        [Put("/api/categories/{id}")]
        Task<Shared.DTOs.ApiResponse<object>> UpdateCategoryAsync(int id, [Body] Category category, [Header("Authorization")] string authorization);

        [Delete("/api/categories/{id}")]
        Task<Shared.DTOs.ApiResponse<object>> DeleteCategoryAsync(int id, [Header("Authorization")] string authorization);
    }
}