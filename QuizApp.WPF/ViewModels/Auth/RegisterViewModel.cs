using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace QuizApp.WPF.ViewModels.Auth
{
    public class RegisterViewModel : BaseViewModel
    {

        private readonly AuthService _auth;
        private readonly Action _navigateToLogin;

        private string _username = "";
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private string _confirmPassword = "";
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); }
        }

        public string RegisterButtonText { get; set; } = "Register";

        public RelayCommand<object> RegisterCommand { get; }

        public RegisterViewModel(Action navigateToLogin)
        {
            _auth = new AuthService();
            _navigateToLogin = navigateToLogin;

            RegisterCommand = new RelayCommand<object>(async _ =>
            {
                if (string.IsNullOrWhiteSpace(Username) ||
                    string.IsNullOrWhiteSpace(Password) ||
                    string.IsNullOrWhiteSpace(ConfirmPassword))
                {
                    MessageBox.Show("All fields are required.");
                    return;
                }

                if (Password != ConfirmPassword)
                {
                    MessageBox.Show("Passwords do not match.");
                    return;
                }

                var req = new RegisterRequest
                {
                    Username = Username,
                    Password = Password,
                    Role = "User"
                };

                var response = await _auth.RegisterAsync(req);

                if (!response.Success)
                {
                    MessageBox.Show(response.Message);
                    return;
                }

                // SUCCESS → Run this
                MessageBox.Show("Registration successful!");
                _navigateToLogin?.Invoke();
            });
        }

        
    }
}
