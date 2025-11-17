using QuizApp.WPF.Services;
using QuizApp.WPF.ViewModels.Admin;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels.Auth
{
    public class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        private string _username = string.Empty;
        private string _password = string.Empty;
        private bool _isLoading;
        private string _loadingText = "Signing you in...";

        public LoginViewModel()
        {
            _authService = new AuthService();
            LoginCommand = new RelayCommand(async () => await LoginAsync(), CanLogin);
            RegisterCommand = new RelayCommand(() => { });
        }

        // ================================
        // PROPERTIES
        // ================================
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LoginButtonText));
            }
        }

        public string LoadingText
        {
            get => _loadingText;
            set
            {
                _loadingText = value;
                OnPropertyChanged();
            }
        }

        public string LoginButtonText => IsLoading ? "Please wait..." : "Sign In";

        // ================================
        // COMMANDS
        // ================================
        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        private bool CanLogin()
        {
            return !IsLoading &&
                   !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        // ================================
        // LOGIN LOGIC
        // ================================
        private async Task LoginAsync()
        {
            IsLoading = true;

            try
            {
                var response = await _authService.LoginAsync(Username, Password);

                if (!response.Success || response.Data == null)
                {
                    MessageBox.Show(
                        response.Message ?? "Invalid credentials!",
                        "Login Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                    Password = string.Empty;
                    return;
                }

                var data = response.Data;

                // 🔥 Raise event instead of opening windows here
                OnLoginSuccess?.Invoke(
                    data.Username,
                    data.Token,
                    data.Role,
                    _authService
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Connection error: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ===========================================
        // EVENT — LoginWindow will subscribe to this.
        // ===========================================
        public event Action<string, string, string, AuthService>? OnLoginSuccess;
    }
}
