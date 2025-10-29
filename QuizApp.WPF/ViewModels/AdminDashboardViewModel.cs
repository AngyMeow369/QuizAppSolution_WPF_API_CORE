using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using QuizApp.WPF.Services;
using QuizApp.WPF.Views.Auth;

namespace QuizApp.WPF.ViewModels
{
    public class AdminDashboardViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        public string Username => _authService.Username;
        public string Role => _authService.Role;

        public ObservableCollection<FeatureCard> FeatureCards { get; }

        public ICommand ManageCategoriesCommand { get; }
        public ICommand ManageQuestionsCommand { get; }
        public ICommand ManageQuizzesCommand { get; }
        public ICommand ManageUsersCommand { get; }
        public ICommand ViewAnalyticsCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand LogoutCommand { get; }

        public AdminDashboardViewModel()
        {
            _authService = new AuthService();

            // Initialize commands
            ManageCategoriesCommand = new RelayCommand(ManageCategories);
            ManageQuestionsCommand = new RelayCommand(ManageQuestions);
            ManageQuizzesCommand = new RelayCommand(ManageQuizzes);
            ManageUsersCommand = new RelayCommand(ManageUsers);
            ViewAnalyticsCommand = new RelayCommand(ViewAnalytics);
            SettingsCommand = new RelayCommand(Settings);
            LogoutCommand = new RelayCommand(Logout);

            // Initialize feature cards
            FeatureCards = new ObservableCollection<FeatureCard>
            {
                new FeatureCard { Icon = "📚", Title = "Manage Categories", Description = "Create and organize quiz categories", Command = ManageCategoriesCommand },
                new FeatureCard { Icon = "❓", Title = "Manage Questions", Description = "Add and edit quiz questions", Command = ManageQuestionsCommand },
                new FeatureCard { Icon = "📝", Title = "Manage Quizzes", Description = "Build complete quiz sets", Command = ManageQuizzesCommand },
                new FeatureCard { Icon = "👥", Title = "User Management", Description = "Manage user accounts and roles", Command = ManageUsersCommand },
                new FeatureCard { Icon = "📈", Title = "View Analytics", Description = "Platform insights and metrics", Command = ViewAnalyticsCommand },
                new FeatureCard { Icon = "⚙️", Title = "Settings", Description = "System configuration", Command = SettingsCommand }
            };
        }

        private void ManageCategories()
        {
            MessageBox.Show("Categories management feature coming soon!", "Feature",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ManageQuestions()
        {
            MessageBox.Show("Questions management feature coming soon!", "Feature",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ManageQuizzes()
        {
            MessageBox.Show("Quizzes management feature coming soon!", "Feature",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ManageUsers()
        {
            MessageBox.Show("User management feature coming soon!", "Feature",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ViewAnalytics()
        {
            MessageBox.Show("Analytics dashboard coming soon!", "Feature",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Settings()
        {
            MessageBox.Show("Settings panel coming soon!", "Feature",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Logout()
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Logout",
                                       MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var loginView = new LoginView();
                    loginView.Show();

                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.DataContext == this)
                            window.Close();
                    }
                });
            }
        }
    }

    public class FeatureCard
    {
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICommand Command { get; set; }
    }
}