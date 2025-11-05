using QuizApp.WPF.Services;
using QuizApp.WPF.ViewModels.Admin;
using QuizApp.WPF.Views.Admin;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels
{
    public class AdminMainViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private object? _currentView; // made nullable

        public object? CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public ICommand NavigateToDashboardCommand { get; }
        public ICommand NavigateToManageQuizzesCommand { get; }
        public ICommand NavigateToUsersCommand { get; }
        public ICommand NavigateToAnalyticsCommand { get; }
        public ICommand LogoutCommand { get; }

        public string CurrentUsername => _authService.Username;
        public string UserRole => _authService.Role;

        public AdminMainViewModel()
        {
            _authService = new AuthService();

            // Create the service instances
            var quizService = new QuizService(_authService);
            var userService = new UserService(_authService);

            NavigateToDashboardCommand = new RelayCommand(() => CurrentView = new AdminDashboardViewModel(_authService));
            NavigateToManageQuizzesCommand = new RelayCommand(() => CurrentView = new ManageQuizzesViewModel(quizService));
            NavigateToUsersCommand = new RelayCommand(() => CurrentView = new ManageUsersViewModel(userService));
            NavigateToAnalyticsCommand = new RelayCommand(() => CurrentView = new AnalyticsViewModel(quizService));
            LogoutCommand = new RelayCommand(Logout);

            // default page
            CurrentView = new AdminDashboardViewModel(_authService);
        }

        private void Logout()
        {
            var login = new QuizApp.WPF.Views.Auth.LoginView();
            login.Show();

            foreach (var win in System.Windows.Application.Current.Windows)
            {
                if (win is System.Windows.Window w && w.DataContext == this)
                    w.Close();
            }
        }
    }
}
