using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class ManageUsersViewModel : ObservableObject
    {
        private readonly UserService _userService;

        public ObservableCollection<UserDto> Users { get; } = new ObservableCollection<UserDto>();

        private UserDto? _selectedUser;
        public UserDto? SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

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
                foreach (var user in users)
                    Users.Add(user);
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
    }
}
