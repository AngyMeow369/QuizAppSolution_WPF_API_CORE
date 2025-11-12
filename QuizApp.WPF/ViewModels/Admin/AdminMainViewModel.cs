using QuizApp.WPF.Services;
using Refit;
using System.Text.Json;
using QuizApp.WPF.Services.Interfaces;
using System.Windows.Input;
using QuizApp.WPF.Views.Admin;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class AdminMainViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly QuizService _quizService;
        private readonly UserService _userService;
        private readonly CategoryService _categoryService;

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
        public ICommand NavigateToQuizDetailsCommand { get; }
        public ICommand NavigateToAssignQuizzesCommand { get; }
        public ICommand LogoutCommand { get; }

        public string CurrentUsername => _authService.Username;
        public string UserRole => _authService.Role;

        public AdminMainViewModel(AuthService authService)
        {
            _authService = authService;

            const string baseApiUrl = "https://localhost:7016";

            var refitSettings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })
            };

            // APIs
            var quizApi = RestService.For<IQuizApi>(baseApiUrl, refitSettings);
            var categoryApi = RestService.For<ICategoryApi>(baseApiUrl, refitSettings);

            // Services
            _quizService = new QuizService(quizApi, _authService);
            _userService = new UserService(_authService);
            _categoryService = new CategoryService(categoryApi, _authService);

            // Navigation commands
            NavigateToDashboardCommand = new RelayCommand(() =>
                CurrentView = new AdminDashboardViewModel(_authService));

            NavigateToManageQuizzesCommand = new RelayCommand(() =>
                CurrentView = new ManageQuizzesViewModel(_quizService, _categoryService));

            NavigateToUsersCommand = new RelayCommand(() =>
                CurrentView = new ManageUsersView(_userService));

            NavigateToAnalyticsCommand = new RelayCommand(() =>
                CurrentView = new AnalyticsViewModel(_quizService));

            NavigateToQuizDetailsCommand = new RelayCommand(() =>
                CurrentView = new QuizDetailsViewModel(_quizService));

            NavigateToAssignQuizzesCommand = new RelayCommand(async () =>
            {
                var assignVM = new AssignQuizzesViewModel(_userService, _quizService);
                CurrentView = assignVM;
                await assignVM.LoadDataAsync(); // Load users and quizzes asynchronously after setting the view
            });


            LogoutCommand = new RelayCommand(Logout);

            // Default page
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
