using QuizApp.Shared.DTOs;

public class QuestionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public List<OptionDto> Options { get; set; } = new();
}
