using QuizApp.API.Models;
using QuizApp.WPF.Services.Interfaces;
using Refit;

namespace QuizApp.WPF.Services
{
    public class QuizService
    {
        private readonly IQuizApi _quizApi;
        private readonly IAuthService _authService;

        public QuizService(IAuthService authService)
        {
            _authService = authService;
            _quizApi = RestService.For<IQuizApi>("https://localhost:7016");
        }

        public async Task<List<Quiz>> GetQuizzesAsync()
        {
            var token = _authService.GetToken();
            return await _quizApi.GetAllQuizzesAsync($"Bearer {token}");
        }

        public async Task<Quiz> GetQuizByIdAsync(int id)
        {
            var token = _authService.GetToken();
            return await _quizApi.GetQuizByIdAsync(id, $"Bearer {token}");
        }

        public async Task<Quiz> CreateQuizAsync(Quiz quiz, List<int> questionIds)
        {
            var token = _authService.GetToken();
            return await _quizApi.CreateQuizAsync(quiz, questionIds, $"Bearer {token}");
        }

        public async Task UpdateQuizAsync(Quiz quiz)
        {
            var token = _authService.GetToken();
            await _quizApi.UpdateQuizAsync(quiz.Id, quiz, $"Bearer {token}");
        }

        public async Task DeleteQuizAsync(int quizId)
        {
            var token = _authService.GetToken();
            await _quizApi.DeleteQuizAsync(quizId, $"Bearer {token}");
        }

        public async Task AssignQuizToUserAsync(int quizId, int userId)
        {
            var token = _authService.GetToken();
            await _quizApi.AssignQuizAsync(quizId, userId, $"Bearer {token}");
        }

        public async Task RemoveQuizAssignmentAsync(int quizId, int userId)
        {
            var token = _authService.GetToken();
            await _quizApi.RemoveAssignmentAsync(quizId, userId, $"Bearer {token}");
        }
    }
}