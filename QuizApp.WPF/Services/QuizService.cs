using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services.Interfaces;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizApp.WPF.Services
{
    public class QuizService
    {
        private readonly IQuizApi _quizApi;
        private readonly IAuthService _authService;

        public QuizService(IQuizApi quizApi, IAuthService authService)
        {
            _quizApi = quizApi;
            _authService = authService;
        }

        public QuizService(IAuthService authService)
        {
            _authService = authService;
            const string apiBase = "https://localhost:7016";
            _quizApi = RestService.For<IQuizApi>(apiBase);
        }

        private string GetAuthHeader()
        {
            var token = _authService.GetToken() ?? string.Empty;
            return token.StartsWith("Bearer") ? token : $"Bearer {token}";
        }

        public async Task<List<QuizDto>> GetAllAsync()
        {
            var token = GetAuthHeader();
            var response = await _quizApi.GetAllQuizzesAsync(token);
            if (response?.Success != true) throw new Exception(response?.Message ?? "Failed to load quizzes.");
            return response.Data ?? new List<QuizDto>();
        }

        public async Task<QuizDto?> GetByIdAsync(int id)
        {
            var token = GetAuthHeader();
            var response = await _quizApi.GetQuizByIdAsync(id, token);
            if (response?.Success != true) throw new Exception(response?.Message ?? $"Failed to load quiz {id}");
            return response.Data;
        }

        public async Task<QuizDto?> CreateAsync(QuizDto quiz, List<int> questionIds)
        {
            var response = await _quizApi.CreateQuizAsync(quiz, questionIds, GetAuthHeader());
            return response?.Data;
        }

        public async Task<bool> UpdateAsync(QuizDto quiz)
        {
            var response = await _quizApi.UpdateQuizAsync(quiz.Id, quiz, GetAuthHeader());
            return response?.Success ?? false;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _quizApi.DeleteQuizAsync(id, GetAuthHeader());
            return response?.Success ?? false;
        }

        public async Task<bool> AssignQuizAsync(int quizId, int userId)
        {
            var response = await _quizApi.AssignQuizAsync(quizId, userId, GetAuthHeader());
            return response?.Success ?? false;
        }

        public async Task<bool> RemoveAssignmentAsync(int quizId, int userId)
        {
            var response = await _quizApi.RemoveAssignmentAsync(quizId, userId, GetAuthHeader());
            return response?.Success ?? false;
        }
    }

}