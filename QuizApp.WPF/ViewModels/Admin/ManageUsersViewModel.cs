using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class ManageUsersViewModel : BaseViewModel
    {
        private readonly UserService _userService;

        // List of non-admin users
        public ObservableCollection<UserDto> Users { get; } = new ObservableCollection<UserDto>();

        private UserDto? _selectedUser;
        public UserDto? SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (SetProperty(ref _selectedUser, value))
                {
                    LoadAssignedQuizzes(_selectedUser);
                }
            }
        }

        // Assigned quizzes for selected user
        public ObservableCollection<QuizDto> AssignedQuizzes { get; } = new ObservableCollection<QuizDto>();

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ManageUsersViewModel(UserService userService)
        {
            _userService = userService;
            _ = LoadUsers();
        }

        private async Task LoadUsers()
        {
            try
            {
                IsLoading = true;
                var users = await _userService.GetUsersAsync();
                Users.Clear();

                // Only show non-admin users
                foreach (var user in users.Where(u => u.Role.ToLower() != "admin"))
                {
                    Users.Add(user);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to load users: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void LoadAssignedQuizzes(UserDto? user)
        {
            AssignedQuizzes.Clear();
            if (user == null) return;

            try
            {
                // Corrected method name
                var quizzes = await _userService.GetAssignedQuizzesAsync(user.Id);
                foreach (var quiz in quizzes)
                {
                    AssignedQuizzes.Add(quiz);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to load assigned quizzes: {ex.Message}");
            }
        }

    }
}
