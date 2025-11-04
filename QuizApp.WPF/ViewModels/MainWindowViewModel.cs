using QuizApp.WPF.Services;
using QuizApp.WPF.Services.Interfaces;
using QuizApp.WPF.Services.User;
using QuizApp.WPF.ViewModels.User;
using QuizApp.WPF.Views.Auth;
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
        private readonly Services.NavigationService _navigationService;
        private readonly IAuthService _authService; // Add this field
        private object _currentView;
        private string _currentUsername;
        private string _userRole = "User";

        // Update constructor to accept IAuthService
        public MainWindowViewModel(string username, string token, IAuthService authService)
        {
            _currentUsername = username;
            _authService = authService; // Store the auth service
            _navigationService = new Services.NavigationService();

            // Initialize with a default placeholder
            _currentView = CreatePlaceholderView("Loading Dashboard...");

            // Then navigate to dashboard
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
            var dashboardService = new UserDashboardService(_authService); // Pass authService
            var viewModel = new UserDashboardViewModel(dashboardService);
            CurrentView = new UserDashboardView { DataContext = viewModel };
        }

        private void NavigateToQuizzes()
        {
            var quizService = new UserQuizService(_authService); // Pass authService
            var viewModel = new UserQuizzesViewModel(quizService);
            CurrentView = new UserQuizzesView { DataContext = viewModel };
        }

        private void NavigateToResults()
        {
            var resultsService = new UserResultsService(_authService); // Pass authService
            var viewModel = new UserResultsViewModel(resultsService);
            CurrentView = new UserResultsView { DataContext = viewModel };
        }

        private void NavigateToProfile()
        {
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
                // Find existing login window or create new one
                var loginWindow = Application.Current.Windows.OfType<LoginView>().FirstOrDefault();

                if (loginWindow == null)
                {
                    loginWindow = new LoginView();
                    loginWindow.Show();
                }
                else
                {
                    loginWindow.Show();
                    loginWindow.Focus();
                }

                // Clear any user data from auth service if needed
                // _authService.Logout(); // If you implement this method

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