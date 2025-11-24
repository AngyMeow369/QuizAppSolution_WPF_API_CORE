using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class AnalyticsViewModel : BaseViewModel
    {
        private readonly QuizService _quizService;

        public ObservableCollection<QuizDto> Quizzes { get; set; } = new();

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public AnalyticsViewModel(QuizService quizService)
        {
            _quizService = quizService ?? throw new ArgumentNullException(nameof(quizService));
            _ = LoadAnalytics();
        }

        private async Task LoadAnalytics()
        {
            try
            {
                IsLoading = true;
                var quizzes = await _quizService.GetAllAsync();
                Quizzes.Clear();
                foreach (var quiz in quizzes)
                    Quizzes.Add(quiz);

                // TODO: Load additional analytics (scores, completion rates, etc.)
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load analytics: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
