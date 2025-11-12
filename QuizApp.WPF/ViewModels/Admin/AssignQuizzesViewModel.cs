using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class AssignQuizzesViewModel : ObservableObject
    {
        private readonly UserService _userService;
        private readonly QuizService _quizService;

        public ObservableCollection<UserDto> Users { get; set; } = new();
        public ObservableCollection<QuizDto> Quizzes { get; set; } = new();

        private UserDto? _selectedUser;
        public UserDto? SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        private QuizDto? _selectedQuiz;
        public QuizDto? SelectedQuiz
        {
            get => _selectedQuiz;
            set => SetProperty(ref _selectedQuiz, value);
        }

        public ICommand AssignQuizCommand { get; }

        public AssignQuizzesViewModel(UserService userService, QuizService quizService)
        {
            _userService = userService;
            _quizService = quizService;

            AssignQuizCommand = new RelayCommand(async () => await AssignQuiz(), CanAssignQuiz);

            LoadData().ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    MessageBox.Show($"Error in LoadData(): {t.Exception.InnerException?.Message ?? t.Exception.Message}");
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }


        private bool CanAssignQuiz() => SelectedUser != null && SelectedQuiz != null;

        private async Task LoadData()
        {
            try
            {
                var users = await _userService.GetUsersAsync();
                Users = new ObservableCollection<UserDto>(users);
                OnPropertyChanged(nameof(Users));

                var quizzes = await _quizService.GetAllAsync();
                Quizzes = new ObservableCollection<QuizDto>(quizzes);
                OnPropertyChanged(nameof(Quizzes));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to load data: {ex.Message}");
            }
        }

        private async Task AssignQuiz()
        {
            if (SelectedUser == null || SelectedQuiz == null) return;

            try
            {
                await _userService.AssignQuizAsync(SelectedUser.Id, SelectedQuiz.Id);
                MessageBox.Show($"Quiz '{SelectedQuiz.Title}' assigned to '{SelectedUser.Username}' successfully.");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to assign quiz: {ex.Message}");
            }
        }
    }
}
