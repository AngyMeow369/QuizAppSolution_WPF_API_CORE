using QuizApp.API.Models;
using QuizApp.WPF.Services.Interfaces;
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
            return await _categoryApi.GetAllCategoriesAsync($"Bearer {token}");
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var token = _authService.GetToken();
            return await _categoryApi.GetCategoryByIdAsync(id, $"Bearer {token}");
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            var token = _authService.GetToken();
            return await _categoryApi.CreateCategoryAsync(category, $"Bearer {token}");
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            var token = _authService.GetToken();
            await _categoryApi.UpdateCategoryAsync(category.Id, category, $"Bearer {token}");
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            var token = _authService.GetToken();
            await _categoryApi.DeleteCategoryAsync(categoryId, $"Bearer {token}");
        }
    }
}