namespace QuizApp.Shared.DTOs
{
    public class QuizDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
