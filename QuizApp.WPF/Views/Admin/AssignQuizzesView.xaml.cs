using QuizApp.WPF.Services;
using QuizApp.WPF.Services.Interfaces;
using QuizApp.WPF.ViewModels.Admin;
using Refit;
using System;
using System.Windows.Controls;

namespace QuizApp.WPF.Views.Admin
{
    public partial class AssignQuizzesView : UserControl
    {
        public AssignQuizzesView()
        {
            InitializeComponent();

            // Create shared dependencies
            var authService = new AuthService(); // must implement IAuthService

            // Base address of your API (adjust if needed)
            var apiBaseUrl = "https://localhost:7180"; // or whatever your WebAPI runs on

            // Create a Refit-based implementation of IQuizApi
            var quizApi = RestService.For<IQuizApi>(apiBaseUrl);

            // Inject services
            var userService = new UserService(authService);
            var quizService = new QuizService(quizApi, authService);

            // Set ViewModel
            DataContext = new AssignQuizzesViewModel(userService, quizService);
        }
    }
}