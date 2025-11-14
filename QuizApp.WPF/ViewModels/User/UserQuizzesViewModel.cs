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
            StartQuizCommand = new RelayCommand<QuizDto>(execute: StartQuiz);

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
            if (quiz == null)
                return;

            // Create view & viewmodel manually
            var vm = new QuizAttemptViewModel(_quizService);
            var view = new QuizAttemptView
            {
                DataContext = vm
            };

            // Load quiz
            _ = vm.LoadQuizAsync(quiz.Id);

            // Replace MainWindow content (or use region navigation)
            Application.Current.MainWindow.Content = view;
        }
    }
}
