using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services.User;
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
        private ObservableCollection<QuizDto> _assignedQuizzes;
        private bool _isLoading;

        public UserQuizzesViewModel(UserQuizService quizService)
        {
            _quizService = quizService;
            _assignedQuizzes = new ObservableCollection<QuizDto>();
            LoadQuizzesCommand = new RelayCommand(async () => await LoadAssignedQuizzesAsync());
            _ = LoadAssignedQuizzesAsync();
        }

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

        public ICommand LoadQuizzesCommand { get; }

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
                MessageBox.Show($"Error loading quizzes: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}