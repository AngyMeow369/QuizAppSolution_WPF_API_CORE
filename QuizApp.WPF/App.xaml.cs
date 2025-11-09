using QuizApp.WPF.Services;
using QuizApp.WPF.Services.Interfaces;
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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            const string quizApiBaseUrl = "https://localhost:7016";

            // Create HttpClientHandler to bypass SSL validation for development
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            // HttpClient for Refit
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(quizApiBaseUrl)
            };

            // Refit settings with System.Text.Json serializer
            var refitSettings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            };

            // Initialize services
            AuthService = new AuthService();
            QuizApi = RestService.For<IQuizApi>(httpClient, refitSettings);
            QuizService = new QuizService(QuizApi, AuthService);
        }
    }
}
