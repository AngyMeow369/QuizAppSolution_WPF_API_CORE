using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services.User;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels.User
{
    public class UserResultsViewModel : BaseViewModel
    {
        private readonly UserResultsService _resultsService;
        private ObservableCollection<QuizResultDto> _quizResults;
        private bool _isLoading;

        public UserResultsViewModel(UserResultsService resultsService)
        {
            _resultsService = resultsService;
            _quizResults = new ObservableCollection<QuizResultDto>();
            LoadResultsCommand = new RelayCommand(async () => await LoadQuizResultsAsync());
            _ = LoadQuizResultsAsync();
        }

        public ObservableCollection<QuizResultDto> QuizResults
        {
            get => _quizResults;
            set
            {
                SetProperty(ref _quizResults, value);
                // Update computed properties when results change
                OnPropertyChanged(nameof(TotalQuizzesTaken));
                OnPropertyChanged(nameof(AverageScore));
                OnPropertyChanged(nameof(PerfectScores));
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Computed properties for summary
        public int TotalQuizzesTaken => QuizResults.Count;
        public double AverageScore => QuizResults.Count > 0 ?
            QuizResults.Average(r => (double)r.Score / r.TotalQuestions * 100) : 0;
        public int PerfectScores => QuizResults.Count(r => r.Score == r.TotalQuestions);

        public ICommand LoadResultsCommand { get; }

        private async Task LoadQuizResultsAsync()
        {
            IsLoading = true;
            try
            {
                var results = await _resultsService.GetMyResultsAsync();
                QuizResults = new ObservableCollection<QuizResultDto>(results);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading results: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}