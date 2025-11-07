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

        // **Inject the logged-in AuthService instance**  
        public AdminMainViewModel(AuthService authService)
        {
            _authService = authService;

            // ✅ Create service instances
            var quizApi = RestService.For<IQuizApi>("https://localhost:7016");
            var quizService = new QuizService(quizApi, _authService); var userService = new UserService(_authService);
            var categoryService = new CategoryService(_authService); // <-- ADD THIS

            // ✅ Pass both quizService and categoryService where needed
            NavigateToDashboardCommand = new RelayCommand(() =>
                CurrentView = new AdminDashboardViewModel(_authService));

            NavigateToManageQuizzesCommand = new RelayCommand(() =>
                CurrentView = new ManageQuizzesViewModel(quizService, categoryService)); // <-- FIXED HERE

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
