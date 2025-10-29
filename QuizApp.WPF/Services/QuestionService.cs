using QuizApp.API.Models;
using QuizApp.WPF.Services.Interfaces;
using Refit;

namespace QuizApp.WPF.Services
{
    public class QuestionService
    {
        private readonly IQuestionApi _questionApi;
        private readonly IAuthService _authService;

        public QuestionService(IAuthService authService)
        {
            _authService = authService;
            _questionApi = RestService.For<IQuestionApi>("https://localhost:7016");
        }

        public async Task<List<Question>> GetQuestionsAsync()
        {
            var token = GetToken(); // Changed to synchronous
            return await _questionApi.GetAllQuestionsAsync($"Bearer {token}");
        }

        public async Task<Question> GetQuestionByIdAsync(int id)
        {
            var token = GetToken();
            return await _questionApi.GetQuestionByIdAsync(id, $"Bearer {token}");
        }

        public async Task<Question> CreateQuestionAsync(Question question)
        {
            var token = GetToken();
            return await _questionApi.CreateQuestionAsync(question, $"Bearer {token}");
        }

        public async Task UpdateQuestionAsync(Question question)
        {
            var token = GetToken();
            await _questionApi.UpdateQuestionAsync(question.Id, question, $"Bearer {token}");
        }

        public async Task DeleteQuestionAsync(int questionId)
        {
            var token = GetToken();
            await _questionApi.DeleteQuestionAsync(questionId, $"Bearer {token}");
        }

        private string GetToken() // Changed to synchronous
        {
            var token = _authService?.GetToken();
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }
            return token;
        }
    }
}