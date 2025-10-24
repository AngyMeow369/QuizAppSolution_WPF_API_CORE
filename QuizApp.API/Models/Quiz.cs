namespace QuizApp.API.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty; // initialized
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>(); // initialized
        public ICollection<QuizAssignment> Assignments { get; set; } = new List<QuizAssignment>(); // initialized
        public ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>(); // initialized
    }
}
