namespace QuizApp.API.DTOs
{
    public class CreateQuestionDto
    {
        public string Text { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public List<string> Options { get; set; } = new();
        public string CorrectAnswer { get; set; } = string.Empty;
    }

    public class UpdateQuestionDto
    {
        public string Text { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public List<string> Options { get; set; } = new();
        public string CorrectAnswer { get; set; } = string.Empty;
    }
}
