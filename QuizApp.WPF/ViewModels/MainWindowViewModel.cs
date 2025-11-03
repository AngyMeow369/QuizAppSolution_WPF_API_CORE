using QuizApp.WPF.Services;
using QuizApp.WPF.Services.User;
using QuizApp.WPF.Views.User;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private readonly UserDashboardService _dashboardService;
        private readonly UserQuizService _quizService;
        private readonly UserResultsService _resultsService;

        private object _currentView;
        private string _currentUsername;
        private string _userRole = "User";

        public MainWindowViewModel(string username, string token)
        {
            _currentUsername = username;

            // Initialize services
            _authService = new AuthService();
            _authService.SetToken(token); // You'll need to add this method to AuthService

            _dashboardService = new UserDashboardService(_authService);
            _quizService = new UserQuizService(_authService);
            _resultsService = new UserResultsService(_authService);

            // Set initial view to dashboard
            NavigateToDashboard();
        }

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public string CurrentUsername
        {
            get => _currentUsername;
            set
            {
                _currentUsername = value;
                OnPropertyChanged();
            }
        }

        public string UserRole
        {
            get => _userRole;
            set
            {
                _userRole = value;
                OnPropertyChanged();
            }
        }

        // Navigation Commands
        public ICommand NavigateToDashboardCommand => new RelayCommand(NavigateToDashboard);
        public ICommand NavigateToQuizzesCommand => new RelayCommand(NavigateToQuizzes);
        public ICommand NavigateToResultsCommand => new RelayCommand(NavigateToResults);
        public ICommand NavigateToProfileCommand => new RelayCommand(NavigateToProfile);
        public ICommand LogoutCommand => new RelayCommand(Logout);

        private void NavigateToDashboard()
        {
            var viewModel = new User.UserDashboardViewModel(_dashboardService);
            CurrentView = new UserDashboardView { DataContext = viewModel };
        }

        private void NavigateToQuizzes()
        {
            var viewModel = new User.UserQuizzesViewModel(_quizService);
            CurrentView = new UserQuizzesView { DataContext = viewModel };
        }

        private void NavigateToResults()
        {
            var viewModel = new User.UserResultsViewModel(_resultsService);
            CurrentView = new UserResultsView { DataContext = viewModel };
        }

        private void NavigateToProfile()
        {
            // Placeholder for profile view
            CurrentView = CreatePlaceholderView("Profile - Coming Soon");
        }

        private TextBlock CreatePlaceholderView(string text)
        {
            return new TextBlock
            {
                Text = text,
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };
        }

        private void Logout()
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Logout",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Find and show login window
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.Title.Contains("Login"))
                    {
                        window.Show();
                        break;
                    }
                }

                // Close current window
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is MainWindow)
                    {
                        window.Close();
                        break;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}