using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services.User;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels.User
{
    public class UserResultsViewModel : INotifyPropertyChanged
    {
        private readonly UserResultsService _resultsService;
        private ObservableCollection<QuizResultDto> _quizResults;
        private bool _isLoading;

        public UserResultsViewModel(UserResultsService resultsService)
        {
            _resultsService = resultsService;
            _quizResults = new ObservableCollection<QuizResultDto>();

            LoadResultsCommand = new RelayCommand(async () => await LoadResultsAsync());
            LoadResultsCommand.Execute(null);
        }

        public ObservableCollection<QuizResultDto> QuizResults
        {
            get => _quizResults;
            set
            {
                _quizResults = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadResultsCommand { get; }

        private async Task LoadResultsAsync()
        {
            try
            {
                IsLoading = true;
                var results = await _resultsService.GetMyResultsAsync();
                QuizResults = new ObservableCollection<QuizResultDto>(results);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading results: {ex.Message}", "Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}