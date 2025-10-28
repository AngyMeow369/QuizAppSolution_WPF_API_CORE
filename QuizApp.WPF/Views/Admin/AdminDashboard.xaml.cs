using QuizApp.WPF.Services;
using QuizApp.WPF.Views.Auth;
using System.Windows;

namespace QuizApp.WPF.Views.Admin
{
    public partial class AdminDashboard : Window
    {
        private readonly AuthService _authService;

        public AdminDashboard()
        {
            InitializeComponent();
            _authService = new AuthService();

            // Set username from auth service
            UsernameText.Text = _authService.Username;
        }

        private void ManageCategories_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Categories management coming soon!", "Feature",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ManageQuestions_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Questions management coming soon!", "Feature",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ManageQuizzes_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Quizzes management coming soon!", "Feature",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ManageUsers_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("User management coming soon!", "Feature",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ViewAnalytics_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Analytics dashboard coming soon!", "Feature",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Settings panel coming soon!", "Feature",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Logout",
                                       MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Navigate back to login
                var loginView = new LoginView();
                loginView.Show();
                this.Close();
            }
        }
    }
}