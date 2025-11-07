using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services.Interfaces;
using Refit;
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
            try
            {
                var token = GetAuthHeader();
                var response = await _categoryApi.GetAllCategoriesAsync(token);

                if (response?.Success == true && response.Data != null)
                    return response.Data;

                throw new Exception(response?.Message ?? "Failed to load categories");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading categories: {ex.Message}");
            }
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            try
            {
                var token = GetAuthHeader();
                var response = await _categoryApi.GetCategoryByIdAsync(id, token);

                if (response?.Success == true && response.Data != null)
                    return response.Data;

                throw new Exception(response?.Message ?? $"Failed to load category {id}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading category {id}: {ex.Message}");
            }
        }

        public async Task<CategoryDto> CreateAsync(CategoryDto category)
        {
            try
            {
                var token = GetAuthHeader();
                var response = await _categoryApi.CreateCategoryAsync(category, token);

                if (response?.Success == true && response.Data != null)
                    return response.Data;

                throw new Exception(response?.Message ?? "Failed to create category");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating category: {ex.Message}");
            }
        }

        public async Task UpdateAsync(CategoryDto category)
        {
            try
            {
                var token = GetAuthHeader();
                var response = await _categoryApi.UpdateCategoryAsync(category.Id, category, token);

                if (!response?.Success == true)
                    throw new Exception(response?.Message ?? "Failed to update category");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating category: {ex.Message}");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var token = GetAuthHeader();
                var response = await _categoryApi.DeleteCategoryAsync(id, token);

                if (!response?.Success == true)
                    throw new Exception(response?.Message ?? "Failed to delete category");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting category: {ex.Message}");
            }
        }
    }
}