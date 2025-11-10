using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class QuizDetailsViewModel : ObservableObject
    {
        private readonly QuizService _quizService;

        public ObservableCollection<QuizDto> Quizzes { get; set; } = new();
        private QuizDto? _selectedQuiz;

        public QuizDto? SelectedQuiz
        {
            get => _selectedQuiz;
            set
            {
                if (SetProperty(ref _selectedQuiz, value) && _selectedQuiz != null)
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

        private async void LoadQuizzes()
        {
            try
            {
                var allQuizzes = await _quizService.GetAllAsync();
                Quizzes = new ObservableCollection<QuizDto>(allQuizzes);
                OnPropertyChanged(nameof(Quizzes));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to load quizzes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadQuestionsForSelectedQuiz(QuizDto quiz)
        {
            try
            {
                var questions = await _quizService.GetQuestionsByQuizIdAsync(quiz.Id);

                // ✅ Replace the questions in DTO
                quiz.Questions = questions;

                // ✅ Force WPF to refresh nested bindings
                OnPropertyChanged(nameof(SelectedQuiz));
                OnPropertyChanged(nameof(SelectedQuiz.Questions));

                // Optional debug check
                MessageBox.Show($"Loaded {questions.Count} questions for quiz '{quiz.Title}'");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to load questions: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
