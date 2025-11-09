using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizApp.WPF.Services
{
    public class CategoryService
    {
        private readonly ICategoryApi _categoryApi;
        private readonly IAuthService _authService;

        public CategoryService(ICategoryApi categoryApi, IAuthService authService)
        {
            _categoryApi = categoryApi;
            _authService = authService;
        }

        private string GetAuthHeader()
        {
            try
            {
                var token = _authService.GetToken() ?? string.Empty;
                return token.StartsWith("Bearer") ? token : $"Bearer {token}";
            }
            catch
            {
                return "Bearer";
            }
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var token = GetAuthHeader();
            var response = await _categoryApi.GetAllCategoriesAsync(token);

            if (response?.Success == true && response.Data != null)
                return response.Data;

            throw new Exception(response?.Message ?? "Failed to load categories");
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var token = GetAuthHeader();
            var response = await _categoryApi.GetCategoryByIdAsync(id, token);

            if (response?.Success == true && response.Data != null)
                return response.Data;

            throw new Exception(response?.Message ?? $"Failed to load category {id}");
        }

        public async Task<CategoryDto> CreateAsync(CategoryDto category)
        {
            var token = GetAuthHeader();
            var response = await _categoryApi.CreateCategoryAsync(category, token);

            if (response?.Success == true && response.Data != null)
                return response.Data;

            throw new Exception(response?.Message ?? "Failed to create category");
        }

        public async Task<bool> UpdateAsync(CategoryDto category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var token = GetAuthHeader();
            var response = await _categoryApi.UpdateCategoryAsync(category.Id, category, token);

            if (response?.Success == true)
                return true;

            throw new Exception(response?.Message ?? "Failed to update category");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var token = GetAuthHeader();
            var response = await _categoryApi.DeleteCategoryAsync(id, token);

            if (response?.Success == true)
                return true;

            throw new Exception(response?.Message ?? "Failed to delete category");
        }
    }
}
