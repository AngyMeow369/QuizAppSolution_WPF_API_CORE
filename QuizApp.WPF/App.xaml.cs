using QuizApp.WPF.Services;
using QuizApp.WPF.Services.Interfaces;
using Refit;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
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

            // ✅ Bypass self-signed SSL validation for localhost (dev only)
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

            const string quizApiBaseUrl = "https://localhost:7016";

            var refitSettings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            };

            AuthService = new AuthService();
            QuizApi = RestService.For<IQuizApi>(quizApiBaseUrl, refitSettings);
            QuizService = new QuizService(QuizApi, AuthService);
        }
    }
}
