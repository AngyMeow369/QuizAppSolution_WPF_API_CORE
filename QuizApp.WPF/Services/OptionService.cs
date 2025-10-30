using QuizApp.API.Models;
using QuizApp.WPF.Services.Interfaces;
using QuizApp.Shared.DTOs;
using Refit;

namespace QuizApp.WPF.Services
{
    public class OptionService
    {
        private readonly IOptionApi _optionApi;
        private readonly IAuthService _authService;

        public OptionService(IAuthService authService)
        {
            _authService = authService;
            _optionApi = RestService.For<IOptionApi>("https://localhost:7016");
        }

        public async Task<List<Option>> GetOptionsAsync()
        {
            var token = _authService.GetToken();
            var response = await _optionApi.GetAllOptionsAsync($"Bearer {token}");
            return response.Success && response.Data != null ? response.Data : throw new Exception(response.Message ?? "Failed to load options");
        }

        public async Task<Option> GetOptionByIdAsync(int id)
        {
            var token = _authService.GetToken();
            var response = await _optionApi.GetOptionByIdAsync(id, $"Bearer {token}");
            return response.Success && response.Data != null ? response.Data : throw new Exception(response.Message ?? "Failed to load option");
        }

        public async Task<Option> CreateOptionAsync(Option option)
        {
            var token = _authService.GetToken();
            var response = await _optionApi.CreateOptionAsync(option, $"Bearer {token}");
            return response.Success && response.Data != null ? response.Data : throw new Exception(response.Message ?? "Failed to create option");
        }

        public async Task UpdateOptionAsync(Option option)
        {
            var token = _authService.GetToken();
            var response = await _optionApi.UpdateOptionAsync(option.Id, option, $"Bearer {token}");
            if (!response.Success) throw new Exception(response.Message ?? "Failed to update option");
        }

        public async Task DeleteOptionAsync(int optionId)
        {
            var token = _authService.GetToken();
            var response = await _optionApi.DeleteOptionAsync(optionId, $"Bearer {token}");
            if (!response.Success) throw new Exception(response.Message ?? "Failed to delete option");
        }
    }
}