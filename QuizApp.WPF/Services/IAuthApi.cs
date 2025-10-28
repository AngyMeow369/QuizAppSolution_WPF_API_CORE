using System.Threading.Tasks;
using QuizApp.Shared.DTOs;
using Refit;

namespace QuizApp.WPF.Services
{
    public interface IAuthApi
    {
        [Post("/api/auth/login")]
        Task<QuizApp.Shared.DTOs.ApiResponse<QuizApp.Shared.DTOs.LoginResponse>> LoginAsync([Body] LoginRequest request);

        [Post("/api/auth/register")]
        Task<QuizApp.Shared.DTOs.ApiResponse<object>> RegisterAsync([Body] RegisterRequest request);
    }
}