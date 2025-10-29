using QuizApp.API.Models;
using QuizApp.WPF.Services.Interfaces;
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
            return await _optionApi.GetAllOptionsAsync($"Bearer {token}");
        }

        public async Task<Option> GetOptionByIdAsync(int id)
        {
            var token = _authService.GetToken();
            return await _optionApi.GetOptionByIdAsync(id, $"Bearer {token}");
        }

        public async Task<Option> CreateOptionAsync(Option option)
        {
            var token = _authService.GetToken();
            return await _optionApi.CreateOptionAsync(option, $"Bearer {token}");
        }

        public async Task UpdateOptionAsync(Option option)
        {
            var token = _authService.GetToken();
            await _optionApi.UpdateOptionAsync(option.Id, option, $"Bearer {token}");
        }

        public async Task DeleteOptionAsync(int optionId)
        {
            var token = _authService.GetToken();
            await _optionApi.DeleteOptionAsync(optionId, $"Bearer {token}");
        }
    }
}