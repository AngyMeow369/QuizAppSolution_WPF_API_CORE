using QuizApp.API.Models;
using QuizApp.Shared.DTOs;
using Refit;

namespace QuizApp.WPF.Services.Interfaces
{
    public interface IUserApi
    {
        [Get("/api/users")]
        Task<Shared.DTOs.ApiResponse<List<User>>> GetAllUsersAsync([Header("Authorization")] string authorization);

        [Get("/api/users/{id}")]
        Task<Shared.DTOs.ApiResponse<User>> GetUserByIdAsync(int id, [Header("Authorization")] string authorization);

        [Post("/api/users")]
        Task<Shared.DTOs.ApiResponse<User>> CreateUserAsync([Body] User user, [Header("Authorization")] string authorization);

        [Put("/api/users/{id}")]
        Task<Shared.DTOs.ApiResponse<object>> UpdateUserAsync(int id, [Body] User user, [Header("Authorization")] string authorization);

        [Delete("/api/users/{id}")]
        Task<Shared.DTOs.ApiResponse<object>> DeleteUserAsync(int id, [Header("Authorization")] string authorization);
    }
}