using QuizApp.API.Models;
using QuizApp.WPF.Services;
using System.Collections.ObjectModel;
using System.Windows;
using QuizApp.Shared.DTOs;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class ManageUsersViewModel : ObservableObject
    {
        private readonly UserService _userService;

        public ObservableCollection<UserDto> Users { get; set; } = new ObservableCollection<UserDto>();

        private UserDto _selectedUser;
        public UserDto SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        private bool _isLoading;
        private AuthService authService;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand AddUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand DeleteUserCommand { get; }

        public ManageUsersViewModel(UserService userService)
        {
            _userService = userService;

            AddUserCommand = new RelayCommand(OnAddUser);
            EditUserCommand = new RelayCommand(OnEditUser, () => SelectedUser != null);
            DeleteUserCommand = new RelayCommand(async () => await OnDeleteUser(), () => SelectedUser != null);

            _ = LoadUsers();
        }

        public ManageUsersViewModel(AuthService authService)
        {
            this.authService = authService;
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

        private void OnAddUser() => MessageBox.Show("AddUser clicked");
        private void OnEditUser() { if (SelectedUser != null) MessageBox.Show($"Edit User: {SelectedUser.Username}"); }
        private async Task OnDeleteUser()
        {
            if (SelectedUser == null) return;
            var result = MessageBox.Show($"Are you sure you want to delete '{SelectedUser.Username}'?", "Confirm Delete", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                await _userService.DeleteUserAsync(SelectedUser.Id);
                Users.Remove(SelectedUser);
                SelectedUser = null;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to delete user: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
