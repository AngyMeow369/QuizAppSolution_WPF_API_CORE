using System.Windows;
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
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            bool success = await _authService.LoginAsync(username, password);

            if (success)
            {
                MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // TODO: Ideally decode role from JWT or AuthService
                // Example: redirect to Admin dashboard
                var adminDashboard = new AdminDashboard(); // Ensure AdminDashboard exists in Views/Admin
                adminDashboard.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid credentials!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
