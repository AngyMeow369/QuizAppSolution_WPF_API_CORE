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

        private ObservableCollection<QuizDto> _assignedQuizzes = new();
        private bool _isLoading;

        public UserQuizzesViewModel(UserQuizService quizService)
        {
            _quizService = quizService;

            LoadQuizzesCommand = new RelayCommand(async () => await LoadAssignedQuizzesAsync());

            // Command that receives QuizDto
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
            StartQuizCommand = new RelayCommand<QuizDto>(execute: StartQuiz);
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

            // Auto-load quizzes
            _ = LoadAssignedQuizzesAsync();
        }

        // ============================================================
        // PROPERTIES
        // ============================================================

        public ObservableCollection<QuizDto> AssignedQuizzes
        {
            get => _assignedQuizzes;
            set => SetProperty(ref _assignedQuizzes, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // ============================================================
        // LOAD QUIZZES
        // ============================================================

        private async Task LoadAssignedQuizzesAsync()
        {
            IsLoading = true;
            try
            {
                var quizzes = await _quizService.GetMyAssignedQuizzesAsync();
                AssignedQuizzes = new ObservableCollection<QuizDto>(quizzes);
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

        // ============================================================
        // START QUIZ (VIEW NAVIGATION)
        // ============================================================

        private void StartQuiz(QuizDto quiz)
        {
            MessageBox.Show("StartQuiz VM instance: " + this.GetHashCode());

            if (quiz == null)
                return;

            // Search ALL open windows to find the one containing MainWindowViewModel
            var realMainWindow = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext is MainWindowViewModel);

            if (realMainWindow?.DataContext is MainWindowViewModel mainVm)
            {
                mainVm.NavigateToQuizAttempt(quiz.Id);
                return;
            }

            MessageBox.Show("Could not locate the main user window.");
        }








    }
}
