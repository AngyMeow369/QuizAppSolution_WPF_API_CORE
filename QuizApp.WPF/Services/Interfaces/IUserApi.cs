using QuizApp.API.Models;
using QuizApp.Shared.DTOs;
using Refit;

namespace QuizApp.WPF.Services.Interfaces
{
    public interface IUserApi
    {
        [Get("/api/users")]
        Task<Shared.DTOs.ApiResponse<List<UserDto>>> GetAllUsersAsync([Header("Authorization")] string authorization);

        [Get("/api/users/{id}")]
        Task<Shared.DTOs.ApiResponse<UserDto>> GetUserByIdAsync(int id, [Header("Authorization")] string authorization);

        [Post("/api/users")]
        Task<Shared.DTOs.ApiResponse<UserDto>> CreateUserAsync([Body] UserDto user, [Header("Authorization")] string authorization);

        [Put("/api/users/{id}")]
        Task<Shared.DTOs.ApiResponse<object>> UpdateUserAsync(int id, [Body] UserDto user, [Header("Authorization")] string authorization);

        [Delete("/api/users/{id}")]
        Task<Shared.DTOs.ApiResponse<object>> DeleteUserAsync(int id, [Header("Authorization")] string authorization);

        [Get("/api/users/{userId}/assigned-quizzes")]
        Task<Shared.DTOs.ApiResponse<List<QuizDto>>> GetAssignedQuizzesAsync(
             [AliasAs("userId")] int userId,
             [Header("Authorization")] string authorization
         );

        [Post("/api/users/{userId}/assign/{quizId}")]
        Task<Shared.DTOs.ApiResponse<object>> AssignQuizAsync(
            [AliasAs("userId")] int userId,
            [AliasAs("quizId")] int quizId,
            [Header("Authorization")] string authorization
        );


    }
}