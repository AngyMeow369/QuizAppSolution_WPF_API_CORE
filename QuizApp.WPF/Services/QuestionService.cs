using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services.Interfaces;
using Refit;

namespace QuizApp.WPF.Services
{
    public class QuestionService
    {
        private readonly IQuestionApi _questionApi;
        private readonly IOptionApi _optionApi;
        private readonly IAuthService _authService;

        public QuestionService(IAuthService authService)
        {
            _authService = authService;
            _questionApi = RestService.For<IQuestionApi>("https://localhost:7016");
            _optionApi = RestService.For<IOptionApi>("https://localhost:7016");
        }

        private string GetToken()
        {
            var token = _authService.GetToken();
            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("User is not authenticated");
            return $"Bearer {token}";
        }

        public async Task<List<QuestionDto>> GetAllAsync()
        {
            var response = await _questionApi.GetAllQuestionsAsync(GetToken());
            if (!response.Success) throw new Exception(response.Message);
            return response.Data ?? new List<QuestionDto>();
        }

        public async Task<QuestionDto> GetByIdAsync(int id)
        {
            var response = await _questionApi.GetQuestionByIdAsync(id, GetToken());
            if (!response.Success || response.Data == null)
                throw new Exception(response.Message);
            return response.Data;
        }

        public async Task<QuestionDto> CreateAsync(QuestionDto question)
        {
            var response = await _questionApi.CreateQuestionAsync(question, GetToken());
            if (!response.Success || response.Data == null)
                throw new Exception(response.Message);
            return response.Data;
        }

        public async Task UpdateAsync(QuestionDto question)
        {
            var response = await _questionApi.UpdateQuestionAsync(question.Id, question, GetToken());
            if (!response.Success)
                throw new Exception(response.Message);
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _questionApi.DeleteQuestionAsync(id, GetToken());
            if (!response.Success)
                throw new Exception(response.Message);
        }

        public async Task<List<OptionDto>> GetOptionsForQuestionAsync(int questionId)
        {
            var response = await _optionApi.GetAllOptionsAsync(GetToken());
            if (!response.Success || response.Data == null)
                throw new Exception(response.Message);

            return response.Data.Where(o => o.QuestionId == questionId).ToList();
        }
    }
}
