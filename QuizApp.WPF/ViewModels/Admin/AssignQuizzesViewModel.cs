using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class AssignQuizzesViewModel : ObservableObject
    {
        private readonly UserService _userService;
        private readonly QuizService _quizService;

        public ObservableCollection<UserDto> Users { get; } = new();
        public ObservableCollection<QuizDto> Quizzes { get; } = new();
        public ObservableCollection<QuizDto> SelectedQuizzes { get; } = new();

        private UserDto? _selectedUser;
        public UserDto? SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (SetProperty(ref _selectedUser, value))
                    (AssignQuizzesCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand AssignQuizzesCommand { get; }


        public AssignQuizzesViewModel(UserService userService, QuizService quizService)
        {
            _userService = userService;
            _quizService = quizService;

            AssignQuizzesCommand = new RelayCommand(async () => await AssignQuizzesAsync(), CanAssignQuizzes);

            SelectedQuizzes.CollectionChanged += (s, e) =>
                (AssignQuizzesCommand as RelayCommand)?.RaiseCanExecuteChanged();

            _ = LoadDataAsync();
        }


        private bool CanAssignQuizzes()
        {
            return SelectedUser != null && SelectedQuizzes.Any();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var users = await _userService.GetUsersAsync();
                Users.Clear();

                foreach (var u in users)
                {
                    // Skip admin users
                    if (!string.Equals(u.Role, "admin", StringComparison.OrdinalIgnoreCase))
                    {
                        Users.Add(u);
                    }
                }

                var quizzes = await _quizService.GetAllAsync();
                Quizzes.Clear();
                foreach (var q in quizzes) Quizzes.Add(q);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to load data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async Task AssignQuizzesAsync()
        {
            if (SelectedUser == null || !SelectedQuizzes.Any()) return;

            try
            {
                var tasks = SelectedQuizzes.Select(q => _userService.AssignQuizAsync(SelectedUser.Id, q.Id));
                await Task.WhenAll(tasks);

                MessageBox.Show(
                    $"Assigned {SelectedQuizzes.Count} quiz(es) to '{SelectedUser.Username}' successfully.",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                SelectedQuizzes.Clear();
                OnPropertyChanged(nameof(SelectedQuizzes));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to assign quizzes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
