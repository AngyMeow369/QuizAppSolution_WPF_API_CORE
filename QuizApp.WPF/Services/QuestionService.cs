using QuizApp.API.Models;
using QuizApp.WPF.Services.Interfaces;
using QuizApp.Shared.DTOs;
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

        public async Task<List<Question>> GetQuestionsAsync()
        {
            var token = GetToken();
            var response = await _questionApi.GetAllQuestionsAsync($"Bearer {token}");
            return response.Success && response.Data != null ? response.Data : throw new Exception(response.Message ?? "Failed to load questions");
        }

        public async Task<Question> GetQuestionByIdAsync(int id)
        {
            var token = GetToken();
            var response = await _questionApi.GetQuestionByIdAsync(id, $"Bearer {token}");
            return response.Success && response.Data != null ? response.Data : throw new Exception(response.Message ?? "Failed to load question");
        }

        public async Task<Question> CreateQuestionAsync(Question question)
        {
            var token = GetToken();
            var response = await _questionApi.CreateQuestionAsync(question, $"Bearer {token}");
            return response.Success && response.Data != null ? response.Data : throw new Exception(response.Message ?? "Failed to create question");
        }

        public async Task UpdateQuestionAsync(Question question)
        {
            var token = GetToken();
            var response = await _questionApi.UpdateQuestionAsync(question.Id, question, $"Bearer {token}");
            if (!response.Success) throw new Exception(response.Message ?? "Failed to update question");
        }

        public async Task DeleteQuestionAsync(int questionId)
        {
            var token = GetToken();
            var response = await _questionApi.DeleteQuestionAsync(questionId, $"Bearer {token}");
            if (!response.Success) throw new Exception(response.Message ?? "Failed to delete question");
        }

        public async Task<List<Option>> GetOptionsForQuestionAsync(int questionId)
        {
            var token = GetToken();
            var response = await _optionApi.GetAllOptionsAsync($"Bearer {token}");
            var allOptions = response.Success && response.Data != null ? response.Data : throw new Exception(response.Message ?? "Failed to load options");
            return allOptions.Where(o => o.QuestionId == questionId).ToList();
        }

        public async Task<Question> CreateQuestionWithOptionsAsync(Question question, List<Option> options)
        {
            var token = GetToken();

            try
            {
                Console.WriteLine("=== CREATING QUESTION WITH OPTIONS ===");

                // 1. Create the question first
                var createResponse = await _questionApi.CreateQuestionAsync(question, $"Bearer {token}");
                if (!createResponse.Success || createResponse.Data == null)
                    throw new Exception(createResponse.Message ?? "Failed to create question");

                var createdQuestion = createResponse.Data;
                Console.WriteLine($"✅ Question created with ID: {createdQuestion.Id}");

                // 2. Create options for this question
                foreach (var option in options)
                {
                    option.QuestionId = createdQuestion.Id;
                    var optionResponse = await _optionApi.CreateOptionAsync(option, $"Bearer {token}");
                    if (!optionResponse.Success)
                        throw new Exception(optionResponse.Message ?? "Failed to create option");

                    Console.WriteLine($"Created option: '{option.Text}' (Correct: {option.IsCorrect})");
                }

                // 3. Return the complete question
                return await GetQuestionByIdAsync(createdQuestion.Id);
            }
            catch (Refit.ApiException apiEx)
            {
                Console.WriteLine($"❌ API ERROR: {apiEx.StatusCode} - {apiEx.Content}");
                throw new Exception($"Failed to create question: {apiEx.Content}");
            }
        }

        public async Task UpdateQuestionWithOptionsAsync(Question question, List<Option> options)
        {
            var token = GetToken();

            try
            {
                Console.WriteLine("=== UPDATING QUESTION WITH OPTIONS ===");

                // 1. Update the question
                var updateResponse = await _questionApi.UpdateQuestionAsync(question.Id, question, $"Bearer {token}");
                if (!updateResponse.Success) throw new Exception(updateResponse.Message ?? "Failed to update question");
                Console.WriteLine("✅ Question updated successfully");

                // 2. Get existing options to delete
                var optionsResponse = await _optionApi.GetAllOptionsAsync($"Bearer {token}");
                var allOptions = optionsResponse.Success && optionsResponse.Data != null ? optionsResponse.Data : throw new Exception(optionsResponse.Message ?? "Failed to load options");
                var questionOptions = allOptions.Where(o => o.QuestionId == question.Id).ToList();
                Console.WriteLine($"🗑️ Deleting {questionOptions.Count} existing options");

                foreach (var existingOption in questionOptions)
                {
                    var deleteResponse = await _optionApi.DeleteOptionAsync(existingOption.Id, $"Bearer {token}");
                    if (!deleteResponse.Success) throw new Exception(deleteResponse.Message ?? "Failed to delete option");
                }

                // 3. Create new options
                Console.WriteLine($"➕ Creating {options.Count} new options");
                foreach (var option in options)
                {
                    option.QuestionId = question.Id;
                    var createResponse = await _optionApi.CreateOptionAsync(option, $"Bearer {token}");
                    if (!createResponse.Success) throw new Exception(createResponse.Message ?? "Failed to create option");
                    Console.WriteLine($"Created option: '{option.Text}' (Correct: {option.IsCorrect})");
                }

                Console.WriteLine("✅ Update completed successfully");
            }
            catch (Refit.ApiException apiEx)
            {
                Console.WriteLine($"❌ API ERROR: {apiEx.StatusCode}");
                Console.WriteLine($"❌ ERROR DETAILS: {apiEx.Content}");
                throw new Exception($"Failed to update question: {apiEx.Content}");
            }
        }

        private string GetToken()
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