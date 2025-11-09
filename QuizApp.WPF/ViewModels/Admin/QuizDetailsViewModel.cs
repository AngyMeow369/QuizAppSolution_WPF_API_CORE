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
            set => SetProperty(ref _selectedQuiz, value);
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
                if (allQuizzes != null)
                {
                    Quizzes = new ObservableCollection<QuizDto>(allQuizzes);
                    OnPropertyChanged(nameof(Quizzes));
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to load quizzes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
