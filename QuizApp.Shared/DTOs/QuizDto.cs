public class QuizDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; } // ADD THESE
    public DateTime EndTime { get; set; }   // ADD THESE
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}