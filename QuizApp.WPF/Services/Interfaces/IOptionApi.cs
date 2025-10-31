using QuizApp.Shared.DTOs;
using Refit;

namespace QuizApp.WPF.Services.Interfaces
{
    public interface IOptionApi
    {
        [Get("/api/options")]
        Task<Shared.DTOs.ApiResponse<List<OptionDto>>> GetAllOptionsAsync([Header("Authorization")] string authorization);

        [Get("/api/options/{id}")]
        Task<Shared.DTOs.ApiResponse<OptionDto>> GetOptionByIdAsync(int id, [Header("Authorization")] string authorization);

        [Post("/api/options")]
        Task<Shared.DTOs.ApiResponse<OptionDto>> CreateOptionAsync([Body] OptionDto option, [Header("Authorization")] string authorization);

        [Put("/api/options/{id}")]
        Task<Shared.DTOs.ApiResponse<object>> UpdateOptionAsync(int id, [Body] OptionDto option, [Header("Authorization")] string authorization);

        [Delete("/api/options/{id}")]
        Task<Shared.DTOs.ApiResponse<object>> DeleteOptionAsync(int id, [Header("Authorization")] string authorization);
    }
}
