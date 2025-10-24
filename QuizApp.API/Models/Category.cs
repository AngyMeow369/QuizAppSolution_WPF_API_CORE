namespace QuizApp.API.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // initialized to avoid null

        public ICollection<Question> Questions { get; set; } = new List<Question>(); // initialized empty
    }
}
