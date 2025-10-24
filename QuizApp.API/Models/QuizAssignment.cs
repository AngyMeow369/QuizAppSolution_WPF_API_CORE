namespace QuizApp.API.Models
{
    public class QuizAssignment
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!; // initialized with null-forgiving

        public int UserId { get; set; }
        public User User { get; set; } = null!; // initialized with null-forgiving

        public bool Completed { get; set; }
    }
}
