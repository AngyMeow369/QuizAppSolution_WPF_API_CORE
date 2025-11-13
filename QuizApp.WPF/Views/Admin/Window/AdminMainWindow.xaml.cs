using System.Windows;
using QuizApp.WPF.ViewModels.Admin;
using QuizApp.WPF.Services;

namespace QuizApp.WPF
{
    public partial class AdminMainWindow : Window
    {
        private readonly AuthService _authService;

        public AdminMainWindow(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;

            // ✅ Set DataContext to AdminMainViewModel with AuthService
            DataContext = new AdminMainViewModel(_authService);
        }
    }
}
