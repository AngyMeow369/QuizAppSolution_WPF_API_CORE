using QuizApp.WPF.Services;
using QuizApp.WPF.Services.Interfaces;
using Refit;
using System.Windows;

namespace QuizApp.WPF
{
    public partial class App : Application
    {
        public static AuthService AuthService { get; private set; } = null!;
        public static IQuizApi QuizApi { get; private set; } = null!;
        public static QuizService QuizService { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            const string quizApiBaseUrl = "https://localhost:7016";

            AuthService = new AuthService();
            QuizApi = RestService.For<IQuizApi>(quizApiBaseUrl);
            QuizService = new QuizService(QuizApi, AuthService);
        }
    }
}
