using QuizApp.WPF.Services;
using QuizApp.WPF.ViewModels.Admin;
using Refit;
using QuizApp.WPF.Services.Interfaces;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class AdminMainViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private object? _currentView;

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

        public AdminMainViewModel(AuthService authService)
        {
            _authService = authService;

            // ✅ Create service instances with proper dependencies
            const string baseApiUrl = "https://localhost:7016";

            var quizApi = RestService.For<IQuizApi>(baseApiUrl);
            var quizService = new QuizService(quizApi, _authService);

            var userService = new UserService(_authService);

            // ✅ Fix CategoryService - add ICategoryApi dependency
            var categoryApi = RestService.For<ICategoryApi>(baseApiUrl);
            var categoryService = new CategoryService(categoryApi, _authService);

            // ✅ Pass both quizService and categoryService where needed
            NavigateToDashboardCommand = new RelayCommand(() =>
                CurrentView = new AdminDashboardViewModel(_authService));

            NavigateToManageQuizzesCommand = new RelayCommand(() =>
                CurrentView = new ManageQuizzesViewModel(quizService, categoryService));

            NavigateToUsersCommand = new RelayCommand(() =>
                CurrentView = new ManageUsersViewModel(userService));

            NavigateToAnalyticsCommand = new RelayCommand(() =>
                CurrentView = new AnalyticsViewModel(quizService));

            LogoutCommand = new RelayCommand(Logout);

            // ✅ Default page
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