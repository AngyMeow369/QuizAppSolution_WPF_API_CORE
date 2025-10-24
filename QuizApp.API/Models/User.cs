namespace QuizApp.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty; // initialized
        public string PasswordHash { get; set; } = string.Empty; // initialized
        public string Role { get; set; } = "User"; // default role
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<QuizAssignment> QuizAssignments { get; set; } = new List<QuizAssignment>(); // initialized
        public ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>(); // initialized
    }
}
