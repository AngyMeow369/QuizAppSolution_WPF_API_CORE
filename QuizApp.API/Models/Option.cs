namespace QuizApp.API.Models
{
    public class Option
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!; // initialized to avoid null

        public string Text { get; set; } = string.Empty; // initialized to avoid null
        public bool IsCorrect { get; set; }
    }
}
