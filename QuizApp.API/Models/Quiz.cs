namespace QuizApp.API.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // ADD THESE TWO LINES - THIS IS CRITICAL:
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();
        public ICollection<QuizAssignment> Assignments { get; set; } = new List<QuizAssignment>();
        public ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>();
    }
}