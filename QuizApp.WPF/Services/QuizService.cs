using QuizApp.API.Models;
using QuizApp.WPF.Services.Interfaces;
using QuizApp.Shared.DTOs;
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
            var response = await _quizApi.GetAllQuizzesAsync($"Bearer {token}");
            return response.Success && response.Data != null ? response.Data : throw new Exception(response.Message ?? "Failed to load quizzes");
        }

        public async Task<Quiz> GetQuizByIdAsync(int id)
        {
            var token = _authService.GetToken();
            var response = await _quizApi.GetQuizByIdAsync(id, $"Bearer {token}");
            return response.Success && response.Data != null ? response.Data : throw new Exception(response.Message ?? "Failed to load quiz");
        }

        public async Task<Quiz> CreateQuizAsync(Quiz quiz, List<int> questionIds)
        {
            var token = _authService.GetToken();
            var response = await _quizApi.CreateQuizAsync(quiz, questionIds, $"Bearer {token}");
            return response.Success && response.Data != null ? response.Data : throw new Exception(response.Message ?? "Failed to create quiz");
        }

        public async Task UpdateQuizAsync(Quiz quiz)
        {
            var token = _authService.GetToken();
            var response = await _quizApi.UpdateQuizAsync(quiz.Id, quiz, $"Bearer {token}");
            if (!response.Success) throw new Exception(response.Message ?? "Failed to update quiz");
        }

        public async Task DeleteQuizAsync(int quizId)
        {
            var token = _authService.GetToken();
            var response = await _quizApi.DeleteQuizAsync(quizId, $"Bearer {token}");
            if (!response.Success) throw new Exception(response.Message ?? "Failed to delete quiz");
        }

        public async Task AssignQuizToUserAsync(int quizId, int userId)
        {
            var token = _authService.GetToken();
            var response = await _quizApi.AssignQuizAsync(quizId, userId, $"Bearer {token}");
            if (!response.Success) throw new Exception(response.Message ?? "Failed to assign quiz");
        }

        public async Task RemoveQuizAssignmentAsync(int quizId, int userId)
        {
            var token = _authService.GetToken();
            var response = await _quizApi.RemoveAssignmentAsync(quizId, userId, $"Bearer {token}");
            if (!response.Success) throw new Exception(response.Message ?? "Failed to remove assignment");
        }
    }
}