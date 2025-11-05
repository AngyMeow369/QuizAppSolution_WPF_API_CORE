using System;
using System.Threading.Tasks;
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
            RegisterCommand = new RelayCommand(() => { /* Register logic here */ });
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
        public ICommand RegisterCommand { get; }

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
                        Window mainWindow;

                        if (loginData.Role == "Admin")
                        {
                            // Pass the same AuthService instance to AdminMainViewModel
                            var adminVM = new AdminMainViewModel(_authService);
                            var adminWindow = new AdminMainWindow();
                            adminWindow.DataContext = adminVM;
                            mainWindow = adminWindow;
                        }
                        else if (loginData.Role == "User")
                        {
                            // Pass token and username if needed for user main window
                            mainWindow = new MainWindow(loginData.Username, loginData.Token, _authService);
                        }
                        else
                        {
                            MessageBox.Show($"Unknown role: {loginData.Role}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        mainWindow.Show();

                        // Close login window
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window.DataContext == this)
                            {
                                window.Close();
                                break;
                            }
                        }
                    });
                }
                else
                {
                    MessageBox.Show(response.Message ?? "Invalid credentials!", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    Password = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }


}
