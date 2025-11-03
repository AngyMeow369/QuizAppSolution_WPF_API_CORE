namespace QuizApp.Shared.DTOs
{
    public class UserDashboardDto
    {
        public int TotalAssignedQuizzes { get; set; }
        public int TotalCompletedQuizzes { get; set; }
        public double AverageScore { get; set; }
        public List<QuizResultDto> RecentResults { get; set; } = new();
        public List<QuizDto> UpcomingQuizzes { get; set; } = new();
    }
}