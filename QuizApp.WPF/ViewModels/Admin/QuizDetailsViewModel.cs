using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class QuizDetailsViewModel : BaseViewModel
    {
        private readonly QuizService _quizService;

        // All quizzes
        public ObservableCollection<QuizDto> Quizzes { get; set; } = new();

        // Questions for the selected quiz
        public ObservableCollection<QuestionDto> Questions { get; set; } = new();

        private QuizDto? _selectedQuiz;
        public QuizDto? SelectedQuiz
        {
            get => _selectedQuiz;
            set
            {
                if (SetProperty(ref _selectedQuiz, value))
                {
                    LoadQuestionsForSelectedQuiz(_selectedQuiz);
                }
            }
        }

        public QuizDetailsViewModel(QuizService quizService)
        {
            _quizService = quizService;
            LoadQuizzes();
        }

        // Load all quizzes and auto-select first one with questions
        private async void LoadQuizzes()
        {
            try
            {
                var allQuizzes = await _quizService.GetAllAsync();
                Quizzes = new ObservableCollection<QuizDto>(allQuizzes);
                OnPropertyChanged(nameof(Quizzes));

                // Auto-select first quiz that has at least one question
                var firstQuizWithQuestions = Quizzes.FirstOrDefault(q => q.QuestionIds?.Count > 0);
                if (firstQuizWithQuestions != null)
                {
                    SelectedQuiz = firstQuizWithQuestions;
                }
                else if (Quizzes.Count > 0)
                {
                    // fallback: select first quiz even if no questions
                    SelectedQuiz = Quizzes[0];
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to load quizzes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Load questions for the selected quiz from API
        private async void LoadQuestionsForSelectedQuiz(QuizDto? quiz)
        {
            Questions.Clear();
            if (quiz == null) return;

            try
            {
                var fullQuiz = await _quizService.GetByIdAsync(quiz.Id);
                if (fullQuiz == null)
                {
                    MessageBox.Show($"Quiz details not found for '{quiz.Title}'", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (fullQuiz.Questions != null)
                {
                    foreach (var q in fullQuiz.Questions)
                    {
                        Questions.Add(q);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to load questions: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
