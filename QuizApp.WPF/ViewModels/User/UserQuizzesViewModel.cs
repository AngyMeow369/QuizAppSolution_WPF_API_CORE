using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services.User;
using QuizApp.WPF.Views.User;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels.User
{
    public class UserQuizzesViewModel : BaseViewModel
    {
        private readonly UserQuizService _quizService;

        public ICommand StartQuizCommand { get; }
        public ICommand LoadQuizzesCommand { get; }

        private ObservableCollection<UserAssignedQuizDto> _assignedQuizzes = new();
        private bool _isLoading;

        public UserQuizzesViewModel(UserQuizService quizService)
        {
            _quizService = quizService;

            LoadQuizzesCommand = new RelayCommand(async () => await LoadAssignedQuizzesAsync());

            StartQuizCommand = new RelayCommand<UserAssignedQuizDto>(execute: StartQuiz);

            _ = LoadAssignedQuizzesAsync();
        }

        public ObservableCollection<UserAssignedQuizDto> AssignedQuizzes
        {
            get => _assignedQuizzes;
            set => SetProperty(ref _assignedQuizzes, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private async Task LoadAssignedQuizzesAsync()
        {
            IsLoading = true;
            try
            {
                var quizzes = await _quizService.GetMyAssignedQuizzesAsync();
                AssignedQuizzes = new ObservableCollection<UserAssignedQuizDto>(quizzes);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading quizzes: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void StartQuiz(UserAssignedQuizDto quiz)
        {
            if (quiz == null)
                return;

            var mw = Application.Current.MainWindow;

            if (mw?.DataContext is QuizApp.WPF.ViewModels.User.MainWindowViewModel mainVm)
            {
                mainVm.NavigateToQuizAttempt(quiz.Id);
                return;
            }

            MessageBox.Show("Could not locate the main user window.");
        }
    }
}
