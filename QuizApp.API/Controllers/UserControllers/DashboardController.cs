using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.API.Data;
using QuizApp.API.Models;
using QuizApp.Shared.DTOs;
using System.Security.Claims;

namespace QuizApp.API.Controllers.UserControllers
{
    [ApiController]
    [Route("api/user/[controller]")]
    [Authorize(Roles = "User")]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/user/dashboard/summary
        [HttpGet("summary")]
        public async Task<ActionResult<ApiResponse<UserDashboardDto>>> GetDashboardSummary()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var user = await _context.Users
                    .Include(u => u.QuizAssignments)
                    .Include(u => u.QuizResults)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return NotFound(ApiResponse<UserDashboardDto>.CreateFailure("User not found."));

                var totalAssigned = user.QuizAssignments.Count;
                var totalCompleted = user.QuizResults.Count;
                var averageScore = totalCompleted > 0
                    ? user.QuizResults.Average(r => (double)r.Score / r.TotalQuestions * 100)
                    : 0;

                var recentResults = await _context.QuizResults
                    .Where(r => r.UserId == userId)
                    .Include(r => r.Quiz)
                    .OrderByDescending(r => r.TakenAt)
                    .Take(5)
                    .Select(r => new QuizResultDto
                    {
                        Id = r.Id,
                        QuizId = r.QuizId,
                        QuizTitle = r.Quiz.Title,
                        Score = r.Score,
                        TotalQuestions = r.TotalQuestions,
                        TakenAt = r.TakenAt
                    })
                    .ToListAsync();

                var upcomingQuizzes = await _context.QuizAssignments
                    .Where(a => a.UserId == userId && !a.Completed && a.Quiz.StartTime > DateTime.UtcNow)
                    .Include(a => a.Quiz)
                    .OrderBy(a => a.Quiz.StartTime)
                    .Take(5)
                    .Select(a => new QuizDto
                    {
                        Id = a.Quiz.Id,
                        Title = a.Quiz.Title,
                        StartTime = a.Quiz.StartTime,
                        EndTime = a.Quiz.EndTime
                    })
                    .ToListAsync();

                var dashboard = new UserDashboardDto
                {
                    TotalAssignedQuizzes = totalAssigned,
                    TotalCompletedQuizzes = totalCompleted,
                    AverageScore = Math.Round(averageScore, 2),
                    RecentResults = recentResults,
                    UpcomingQuizzes = upcomingQuizzes
                };

                return Ok(ApiResponse<UserDashboardDto>.CreateSuccess(dashboard, "Dashboard data retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<UserDashboardDto>.CreateFailure($"Error retrieving dashboard data: {ex.Message}"));
            }
        }
    }
}