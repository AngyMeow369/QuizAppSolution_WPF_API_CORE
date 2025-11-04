using System.Windows;
using System.Windows.Input;
using QuizApp.WPF.Services;
using QuizApp.WPF.Views.Admin;

namespace QuizApp.WPF.ViewModels
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
            RegisterCommand = new RelayCommand(() => { /* Register logic here */ }); // Initialize RegisterCommand
        }

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

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; } // Now properly initialized

        private bool CanLogin()
        {
            return !IsLoading &&
                   !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private async Task LoginAsync()
        {
            IsLoading = true;

            try
            {
                var response = await _authService.LoginAsync(Username, Password);

                if (response.Success && response.Data != null)
                {
                    var loginData = response.Data;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Check user role and redirect accordingly
                        if (loginData.Role == "Admin")
                        {
                            // Redirect to Admin Dashboard
                            var adminVM = new AdminDashboardViewModel(_authService);
                            var adminDashboard = new AdminDashboard();
                            adminDashboard.DataContext = adminVM;
                            adminDashboard.Show();
                        }
                        else if (loginData.Role == "User")
                        {
                            // Redirect to User Dashboard (MainWindow) - FIXED: Pass _authService
                            var userDashboard = new MainWindow(loginData.Username, loginData.Token, _authService);
                            userDashboard.Show();
                        }
                        else
                        {
                            MessageBox.Show($"Unknown role: {loginData.Role}", "Error",
                                          MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        // Close login window
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window.DataContext == this)
                                window.Close();
                        }
                    });
                }
                else
                {
                    MessageBox.Show(response.Message ?? "Invalid credentials!", "Login Failed",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    Password = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection error: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}