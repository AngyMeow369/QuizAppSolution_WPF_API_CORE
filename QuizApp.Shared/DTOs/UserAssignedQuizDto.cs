using System;

namespace QuizApp.Shared.DTOs
{
    public class UserAssignedQuizDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
