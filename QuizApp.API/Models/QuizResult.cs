using QuizApp.API.Models;

public class QuizResult
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int Score { get; set; }
    public int TotalQuestions { get; set; } // ADD THIS
    public DateTime TakenAt { get; set; } = DateTime.UtcNow;
}