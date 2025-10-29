using QuizApp.API.Models;
using Refit;

namespace QuizApp.WPF.Services.Interfaces
{
    public interface IUserApi
    {
        [Get("/api/users")]
        Task<List<User>> GetAllUsersAsync([Header("Authorization")] string authorization);

        [Get("/api/users/{id}")]
        Task<User> GetUserByIdAsync(int id, [Header("Authorization")] string authorization);

        [Post("/api/users")]
        Task<User> CreateUserAsync([Body] User user, [Header("Authorization")] string authorization);

        [Put("/api/users/{id}")]
        Task UpdateUserAsync(int id, [Body] User user, [Header("Authorization")] string authorization);

        [Delete("/api/users/{id}")]
        Task DeleteUserAsync(int id, [Header("Authorization")] string authorization);
    }
}