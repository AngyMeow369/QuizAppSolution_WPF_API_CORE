using QuizApp.WPF.Services;
using QuizApp.WPF.Services.Interfaces;
using QuizApp.WPF.Views.Auth;
using Refit;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace QuizApp.WPF
{
    public partial class App : Application
    {
        public static AuthService AuthService { get; private set; } = null!;
        public static IQuizApi QuizApi { get; private set; } = null!;
        public static QuizService QuizService { get; private set; } = null!;

        // ✅ Shared client for ALL Refit services
        public static HttpClient SharedHttpClient { get; private set; } = null!;
        public static RefitSettings SharedRefitSettings { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            const string apiBaseUrl = "https://localhost:7016";

            // SSL bypass for dev
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            // ✅ Assign global HttpClient
            SharedHttpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(apiBaseUrl)
            };

            // ✅ Assign global Refit settings
            SharedRefitSettings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })
            };

            // Global services
            AuthService = new AuthService();

            QuizApi = RestService.For<IQuizApi>(SharedHttpClient, SharedRefitSettings);
            QuizService = new QuizService(QuizApi, AuthService);

            // Start login window
            var loginWindow = new LoginWindow();
            Application.Current.MainWindow = loginWindow;
            loginWindow.Show();
        }
    }
}
