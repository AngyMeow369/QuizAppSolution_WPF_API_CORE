using QuizApp.API.Models;
using QuizApp.WPF.Services.Interfaces;
using QuizApp.Shared.DTOs;
using Refit;

namespace QuizApp.WPF.Services
{
    public class CategoryService
    {
        private readonly ICategoryApi _categoryApi;
        private readonly IAuthService _authService;

        public CategoryService(IAuthService authService)
        {
            _authService = authService;
            _categoryApi = RestService.For<ICategoryApi>("https://localhost:7016");
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var token = _authService.GetToken();
            var response = await _categoryApi.GetAllCategoriesAsync($"Bearer {token}");
            return response.Success && response.Data != null ? response.Data : throw new Exception(response.Message ?? "Failed to load categories");
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var token = _authService.GetToken();
            var response = await _categoryApi.GetCategoryByIdAsync(id, $"Bearer {token}");
            return response.Success && response.Data != null ? response.Data : throw new Exception(response.Message ?? "Failed to load category");
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            var token = _authService.GetToken();
            var response = await _categoryApi.CreateCategoryAsync(category, $"Bearer {token}");
            return response.Success && response.Data != null ? response.Data : throw new Exception(response.Message ?? "Failed to create category");
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            var token = _authService.GetToken();
            var response = await _categoryApi.UpdateCategoryAsync(category.Id, category, $"Bearer {token}");
            if (!response.Success) throw new Exception(response.Message ?? "Failed to update category");
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            var token = _authService.GetToken();
            var response = await _categoryApi.DeleteCategoryAsync(categoryId, $"Bearer {token}");
            if (!response.Success) throw new Exception(response.Message ?? "Failed to delete category");
        }
    }
}