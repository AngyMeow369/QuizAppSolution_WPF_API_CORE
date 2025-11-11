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

        // Keep only one constructor to avoid confusion
        public QuizService(IQuizApi quizApi, IAuthService authService)
        {
            _quizApi = quizApi;
            _authService = authService;
        }

        public async Task<List<QuestionDto>> GetQuestionsByQuizIdAsync(int quizId)
        {
            var quiz = await GetByIdAsync(quizId);
            return quiz?.Questions ?? new List<QuestionDto>();
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

        public async Task<List<QuizDto>> GetAllAsync()
        {
            try
            {
                var token = GetAuthHeader();
                var response = await _quizApi.GetAllQuizzesAsync(token);

                if (response?.Success == true && response.Data != null)
                    return response.Data;

                throw new Exception(response?.Message ?? "Failed to load quizzes.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading quizzes: {ex.Message}");
            }
        }

        public async Task<QuizDto?> GetByIdAsync(int id)
        {
            try
            {
                var token = GetAuthHeader();
                var response = await _quizApi.GetQuizByIdAsync(id, token);

                if (response?.Success == true)
                    return response.Data;

                throw new Exception(response?.Message ?? $"Failed to load quiz {id}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading quiz {id}: {ex.Message}");
            }
        }

        public async Task<QuizDto?> CreateAsync(QuizDto quiz, List<int> questionIds)
        {
            try
            {
                var token = GetAuthHeader();
                var response = await _quizApi.CreateQuizAsync(quiz, questionIds, token);

                if (response?.Success == true)
                    return response.Data;

                throw new Exception(response?.Message ?? "Failed to create quiz");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating quiz: {ex.Message}");
            }
        }

        public async Task<bool> UpdateAsync(QuizDto quiz)
        {
            try
            {
                var token = GetAuthHeader();

                // Send full QuizDto including Questions and Options
                var response = await _quizApi.UpdateQuizAsync(quiz.Id, quiz, token);
                return response?.Success == true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating quiz: {ex.Message}");
            }
        }




        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var token = GetAuthHeader();
                var response = await _quizApi.DeleteQuizAsync(id, token);
                return response?.Success == true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting quiz: {ex.Message}");
            }
        }
    }
}