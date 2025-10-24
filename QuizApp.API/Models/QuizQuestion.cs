namespace QuizApp.API.Models
{
    public class QuizQuestion
    {
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!; // initialized with null-forgiving

        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!; // initialized with null-forgiving
    }
}
