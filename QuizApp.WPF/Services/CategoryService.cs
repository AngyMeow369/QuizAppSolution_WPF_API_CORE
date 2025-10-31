using QuizApp.Shared.DTOs;
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

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var token = _authService.GetToken();
            var response = await _categoryApi.GetAllCategoriesAsync($"Bearer {token}");
            return response.Success && response.Data != null
                ? response.Data
                : throw new Exception(response.Message ?? "Failed to load categories");
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var token = _authService.GetToken();
            var response = await _categoryApi.GetCategoryByIdAsync(id, $"Bearer {token}");
            return response.Success && response.Data != null
                ? response.Data
                : throw new Exception(response.Message ?? "Failed to load category");
        }

        public async Task<CategoryDto> CreateAsync(CategoryDto category)
        {
            var token = _authService.GetToken();
            var response = await _categoryApi.CreateCategoryAsync(category, $"Bearer {token}");
            return response.Success && response.Data != null
                ? response.Data
                : throw new Exception(response.Message ?? "Failed to create category");
        }

        public async Task UpdateAsync(CategoryDto category)
        {
            var token = _authService.GetToken();
            var response = await _categoryApi.UpdateCategoryAsync(category.Id, category, $"Bearer {token}");
            if (!response.Success)
                throw new Exception(response.Message ?? "Failed to update category");
        }

        public async Task DeleteAsync(int id)
        {
            var token = _authService.GetToken();
            var response = await _categoryApi.DeleteCategoryAsync(id, $"Bearer {token}");
            if (!response.Success)
                throw new Exception(response.Message ?? "Failed to delete category");
        }
    }
}
