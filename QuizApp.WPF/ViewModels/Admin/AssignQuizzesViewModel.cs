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

            // Load users and quizzes asynchronously
            _ = LoadDataAsync();
        }

        private bool CanAssignQuizzes()
        {
            return SelectedUser != null && SelectedQuizzes.Count > 0;
        }

        public async Task LoadDataAsync()
        {
            try
            {
                // Load users
                var users = await _userService.GetUsersAsync();
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Users.Clear();
                    foreach (var u in users)
                        Users.Add(u);
                });

                // Load quizzes
                var quizzes = await _quizService.GetAllAsync();
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Quizzes.Clear();
                    foreach (var q in quizzes)
                        Quizzes.Add(q);
                });
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to load data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task AssignQuizzesAsync()
        {
            if (SelectedUser == null || SelectedQuizzes.Count == 0) return;

            try
            {
                // Assign all selected quizzes in parallel
                var tasks = SelectedQuizzes.Select(q => _userService.AssignQuizAsync(SelectedUser.Id, q.Id));
                await Task.WhenAll(tasks);

                MessageBox.Show(
                    $"Assigned {SelectedQuizzes.Count} quiz(es) to '{SelectedUser.Username}' successfully.",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Clear selected quizzes safely
                await Application.Current.Dispatcher.InvokeAsync(() => SelectedQuizzes.Clear());
                OnPropertyChanged(nameof(SelectedQuizzes));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to assign quizzes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
