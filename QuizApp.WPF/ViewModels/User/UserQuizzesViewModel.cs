using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services.User;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels.User
{
    public class UserQuizzesViewModel : INotifyPropertyChanged
    {
        private readonly UserQuizService _quizService;
        private ObservableCollection<QuizDto> _assignedQuizzes;
        private bool _isLoading;
        private QuizDto _selectedQuiz;

        public UserQuizzesViewModel(UserQuizService quizService)
        {
            _quizService = quizService;
            _assignedQuizzes = new ObservableCollection<QuizDto>();

            LoadQuizzesCommand = new RelayCommand(async () => await LoadAssignedQuizzesAsync());
            TakeQuizCommand = new RelayCommand(async () => await TakeQuizAsync(), () => SelectedQuiz != null);

            LoadQuizzesCommand.Execute(null);
        }

        public ObservableCollection<QuizDto> AssignedQuizzes
        {
            get => _assignedQuizzes;
            set
            {
                _assignedQuizzes = value;
                OnPropertyChanged();
            }
        }

        public QuizDto SelectedQuiz
        {
            get => _selectedQuiz;
            set
            {
                _selectedQuiz = value;
                OnPropertyChanged();
                ((RelayCommand)TakeQuizCommand).RaiseCanExecuteChanged();
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

        public ICommand LoadQuizzesCommand { get; }
        public ICommand TakeQuizCommand { get; }

        private async Task LoadAssignedQuizzesAsync()
        {
            try
            {
                IsLoading = true;
                var quizzes = await _quizService.GetMyAssignedQuizzesAsync();
                AssignedQuizzes = new ObservableCollection<QuizDto>(quizzes);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading quizzes: {ex.Message}", "Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task TakeQuizAsync()
        {
            if (SelectedQuiz == null) return;

            // Navigate to quiz taking view
            var quizTakingVm = new QuizTakingViewModel(_quizService, SelectedQuiz.Id);
            // Navigation logic here (we'll implement this next)
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}