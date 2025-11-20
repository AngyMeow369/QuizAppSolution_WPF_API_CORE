namespace QuizApp.Shared.DTOs
{
    public class QuizTakeDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<QuestionTakeDto> Questions { get; set; } = new();
    }

    public class QuestionTakeDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public List<OptionTakeDto> Options { get; set; } = new();
    }

    public class OptionTakeDto
    {
        public int Id { get; set; }
        public bool IsSelected { get; set; }

        public string Text { get; set; } = string.Empty;
    }

    public class QuizSubmissionDto
    {
        public List<QuestionSubmissionDto> Answers { get; set; } = new();
    }

    public class QuestionSubmissionDto
    {
        public int QuestionId { get; set; }
        public int SelectedOptionId { get; set; }
    }

    public class QuizResultDto
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string QuizTitle { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty; // ADD THIS LINE
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime TakenAt { get; set; }
    }
}