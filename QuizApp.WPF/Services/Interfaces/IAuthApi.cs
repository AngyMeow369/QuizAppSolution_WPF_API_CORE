using System.Threading.Tasks;
using QuizApp.Shared.DTOs;
using Refit;

namespace QuizApp.WPF.Services.Interfaces
{
    public interface IAuthApi
    {
        [Post("/api/auth/login")]
        Task<Shared.DTOs.ApiResponse<LoginResponse>> LoginAsync([Body] LoginRequest request);

        [Post("/api/auth/register")]
        Task<Shared.DTOs.ApiResponse<object>> RegisterAsync([Body] RegisterRequest request);
    }
}