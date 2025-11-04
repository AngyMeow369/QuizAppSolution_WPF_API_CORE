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
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                Console.WriteLine($"🔍 DEBUG API: Looking up user: {username}");

                if (string.IsNullOrEmpty(username))
                    return Unauthorized(ApiResponse<UserDashboardDto>.CreateFailure("User not authenticated."));

                var user = await _context.Users
                    .Include(u => u.QuizAssignments)
                    .Include(u => u.QuizResults)
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    Console.WriteLine($"❌ DEBUG API: User '{username}' not found in database");
                    return NotFound(ApiResponse<UserDashboardDto>.CreateFailure($"User '{username}' not found in database."));
                }

                var userId = user.Id;
                Console.WriteLine($"✅ DEBUG API: Found user ID: {userId}");

                // DEBUG: Check what's being loaded
                Console.WriteLine($"📊 DEBUG API: User QuizAssignments Count: {user.QuizAssignments?.Count}");
                Console.WriteLine($"📊 DEBUG API: User QuizResults Count: {user.QuizResults?.Count}");

                var totalAssigned = user.QuizAssignments?.Count ?? 0;
                var totalCompleted = user.QuizResults?.Count ?? 0;

                Console.WriteLine($"📊 DEBUG API: Calculated - Assignments: {totalAssigned}, Completed: {totalCompleted}");

                // Fix the null reference issue
                double averageScore = 0;
                if (totalCompleted > 0 && user.QuizResults != null)
                {
                    averageScore = user.QuizResults.Average(r => (double)r.Score / r.TotalQuestions * 100);
                }

                // DEBUG: Check individual results
                if (user.QuizResults != null && user.QuizResults.Any())
                {
                    foreach (var result in user.QuizResults)
                    {
                        Console.WriteLine($"🎯 DEBUG API: Result - Quiz: {result.QuizId}, Score: {result.Score}/{result.TotalQuestions}");
                    }
                }

                var recentResults = await _context.QuizResults
                    .Where(r => r.UserId == userId)
                    .Include(r => r.Quiz)
                        .ThenInclude(q => q.Category) // Add category for recent results
                    .OrderByDescending(r => r.TakenAt)
                    .Take(5)
                    .Select(r => new QuizResultDto
                    {
                        Id = r.Id,
                        QuizId = r.QuizId,
                        QuizTitle = r.Quiz.Title,
                        Score = r.Score,
                        TotalQuestions = r.TotalQuestions,
                        TakenAt = r.TakenAt,
                        CategoryName = r.Quiz.Category.Name // Add category name
                    })
                    .ToListAsync();

                var upcomingQuizzes = await _context.QuizAssignments
                    .Where(a => a.UserId == userId && !a.Completed && a.Quiz.StartTime > DateTime.UtcNow)
                    .Include(a => a.Quiz)
                        .ThenInclude(q => q.Category) // Add category for upcoming quizzes
                    .OrderBy(a => a.Quiz.StartTime)
                    .Take(5)
                    .Select(a => new QuizDto
                    {
                        Id = a.Quiz.Id,
                        Title = a.Quiz.Title,
                        StartTime = a.Quiz.StartTime,
                        EndTime = a.Quiz.EndTime,
                        CategoryId = a.Quiz.CategoryId,
                        CategoryName = a.Quiz.Category.Name // Add category name
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

                Console.WriteLine($"🎯 DEBUG API: Final Dashboard - Assigned: {dashboard.TotalAssignedQuizzes}, Completed: {dashboard.TotalCompletedQuizzes}, Avg: {dashboard.AverageScore}");
                Console.WriteLine($"🎯 DEBUG API: Recent Results Count: {dashboard.RecentResults?.Count}");
                Console.WriteLine($"🎯 DEBUG API: Upcoming Quizzes Count: {dashboard.UpcomingQuizzes?.Count}");

                return Ok(ApiResponse<UserDashboardDto>.CreateSuccess(dashboard, "Dashboard data retrieved successfully."));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 DEBUG API: Exception: {ex.Message}");
                Console.WriteLine($"💥 DEBUG API: Stack: {ex.StackTrace}");
                return StatusCode(500, ApiResponse<UserDashboardDto>.CreateFailure($"Error retrieving dashboard data: {ex.Message}"));
            }
        }
    }
}