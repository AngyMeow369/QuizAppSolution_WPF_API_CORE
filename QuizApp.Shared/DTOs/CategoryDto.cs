namespace QuizApp.Shared.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<QuestionDto> Questions { get; set; } = new();
    }
}
