namespace QuizApp.API.Models
{
    public class QuizResult
    {
        public int Id { get; set; }

        public int QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!; // initialized with null-forgiving

        public int UserId { get; set; }
        public User User { get; set; } = null!; // initialized with null-forgiving

        public int Score { get; set; }
        public DateTime TakenAt { get; set; } = DateTime.UtcNow;
    }
}
