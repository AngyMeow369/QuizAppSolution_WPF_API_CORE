using QuizApp.WPF.Services;
using QuizApp.WPF.Services.User;
using QuizApp.WPF.ViewModels.User;
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
        private object _currentView;
        private string _currentUsername;
        private string _userRole = "User";

        public MainWindowViewModel(string username, string token)
        {
            _currentUsername = username;
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
            var dashboardService = new UserDashboardService(/* pass auth service */);
            var viewModel = new UserDashboardViewModel(dashboardService);
            CurrentView = new UserDashboardView { DataContext = viewModel };
        }

        private void NavigateToQuizzes()
        {
            var quizService = new UserQuizService(/* pass auth service */);
            var viewModel = new UserQuizzesViewModel(quizService);
            CurrentView = new UserQuizzesView { DataContext = viewModel };
        }

        private void NavigateToResults()
        {
            var resultsService = new UserResultsService(/* pass auth service */);
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