using QuizApp.Shared.DTOs;

namespace QuizApp.WPF.Services.Interfaces
{
    public interface IAuthService
    {
        string JwtToken { get; }
        string Role { get; }
        string Username { get; }

        Task<ApiResponse<LoginResponse>> LoginAsync(string username, string password);
        Task<ApiResponse<object>> RegisterAsync(string username, string password, string role = "User");
        string GetToken();
        bool IsAuthenticated();
    }
}