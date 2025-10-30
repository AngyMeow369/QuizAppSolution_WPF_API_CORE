// Create a DTOs folder in your API project
namespace QuizApp.API.DTOs
{


    public class UpdateQuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        // No navigation properties - just the data we need to update
    }

    public class CreateQuestionDto
    {
        public string Text { get; set; } = string.Empty;
        public int CategoryId { get; set; }
    }

    public class QuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<OptionDto> Options { get; set; } = new();
    }

    public class OptionDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}