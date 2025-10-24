namespace QuizApp.API.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty; // initialized

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!; // non-nullable, initialized with null-forgiving

        public ICollection<Option> Options { get; set; } = new List<Option>(); // initialized
        public ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>(); // initialized
    }
}
