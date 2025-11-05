using QuizApp.API.Models;
using QuizApp.WPF.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class AnalyticsViewModel : ObservableObject
    {
        private readonly QuizService _quizService;

        public ObservableCollection<Quiz> Quizzes { get; set; } = new ObservableCollection<Quiz>();

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public AnalyticsViewModel(QuizService quizService)
        {
            _quizService = quizService;

            _ = LoadAnalytics();
        }

        private async Task LoadAnalytics()
        {
            try
            {
                IsLoading = true;
                var quizzes = await _quizService.GetQuizzesAsync();
                Quizzes.Clear();
                foreach (var quiz in quizzes)
                    Quizzes.Add(quiz);

                // TODO: Load additional analytics, e.g., scores, completion rates
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to load analytics: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
