using System;
using System.Collections.Generic;

namespace QuizApp.Shared.DTOs
{
    public class QuizDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; } // Start time of the quiz
        public DateTime EndTime { get; set; }   // End time of the quiz
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<QuestionDto> Questions { get; set; } = new(); // List of questions in the quiz
    }
}
