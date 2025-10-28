using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuizApp.WPF.Services;
using QuizApp.WPF.Views.Admin;

namespace QuizApp.WPF.Views.Auth
{
    public partial class LoginView : Window
    {
        private readonly AuthService _authService;

        public LoginView()
        {
            InitializeComponent();
            _authService = new AuthService();

            // Set focus to username field
            Loaded += (s, e) => UsernameTextBox.Focus();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Show loading
            SetLoadingState(true);

            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            // Basic validation
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password.", "Validation Error",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                SetLoadingState(false);
                return;
            }

            try
            {
                var response = await _authService.LoginAsync(username, password);

                if (response.Success)
                {
                    // Success - navigate to dashboard
                    var adminDashboard = new AdminDashboard();
                    adminDashboard.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show(response.Message ?? "Invalid credentials!", "Login Failed",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    PasswordBox.Clear();
                    PasswordBox.Focus();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Connection error: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private void SetLoadingState(bool isLoading)
        {
            LoginButton.IsEnabled = !isLoading;
            UsernameTextBox.IsEnabled = !isLoading;
            PasswordBox.IsEnabled = !isLoading;
            LoadingText.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            LoginButton.Content = isLoading ? "Please wait..." : "Sign In";
        }

        private void RegisterText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Open registration window
            MessageBox.Show("Registration feature coming soon!", "Info",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Enter key support
        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && LoginButton.IsEnabled)
            {
                LoginButton_Click(sender, e);
            }
        }

        private void UsernameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PasswordBox.Focus();
            }
        }
    }
}