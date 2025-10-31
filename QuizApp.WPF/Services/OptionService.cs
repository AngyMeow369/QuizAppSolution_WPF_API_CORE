using QuizApp.Shared.DTOs;
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

        private string GetToken()
        {
            var token = _authService.GetToken();
            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("User is not authenticated");
            return $"Bearer {token}";
        }

        public async Task<List<OptionDto>> GetAllAsync()
        {
            var response = await _optionApi.GetAllOptionsAsync(GetToken());
            if (!response.Success || response.Data == null)
                throw new Exception(response.Message);
            return response.Data;
        }

        public async Task<OptionDto> CreateAsync(OptionDto option)
        {
            var response = await _optionApi.CreateOptionAsync(option, GetToken());
            if (!response.Success || response.Data == null)
                throw new Exception(response.Message);
            return response.Data;
        }

        public async Task UpdateAsync(OptionDto option)
        {
            var response = await _optionApi.UpdateOptionAsync(option.Id, option, GetToken());
            if (!response.Success)
                throw new Exception(response.Message);
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _optionApi.DeleteOptionAsync(id, GetToken());
            if (!response.Success)
                throw new Exception(response.Message);
        }
    }
}
