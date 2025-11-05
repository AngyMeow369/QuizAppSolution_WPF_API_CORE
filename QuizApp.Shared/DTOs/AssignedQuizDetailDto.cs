namespace QuizApp.Shared.DTOs
{
    public class AssignedQuizDetailDto
    {
        public int QuizId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Assignment status
        public bool IsCompleted { get; set; }

        // Result info (only if completed)
        public int? Score { get; set; }
        public int? TotalQuestions { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Computed status: "Completed", "Available", "Upcoming", "Missed"
        public string Status { get; set; } = string.Empty;

        public int? AssignedToUserId { get; set; }
        public string? AssignedToUsername { get; set; }

    }
}