using QuizApp.WPF.Services;
using QuizApp.WPF.ViewModels.Auth;
using System.Windows;

namespace QuizApp.WPF.Views.Auth
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            // Access LoginView inside the Window
            if (Content is LoginView loginView &&
                loginView.DataContext is LoginViewModel vm)
            {
                vm.OnLoginSuccess += HandleLoginSuccess;
            }
        }

        private void HandleLoginSuccess(string username, string token, string role, AuthService auth)
        {
            Window nextWindow;

            if (role == "Admin")
            {
                nextWindow = new AdminMainWindow(auth);
            }
            else
            {
                nextWindow = new MainWindow(username, token, auth);
            }

            Application.Current.MainWindow = nextWindow;
            nextWindow.Show();

            this.Close();
        }
    }
}
