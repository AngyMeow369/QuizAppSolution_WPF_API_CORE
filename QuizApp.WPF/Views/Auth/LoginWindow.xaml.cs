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

            // Correctly reference LoginView inside this Window
            if (RootLoginView?.DataContext is LoginViewModel vm)
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

            // Set the global MainWindow
            Application.Current.MainWindow = nextWindow;

           

            nextWindow.Show();

            // Close login window
            this.Close();
        }
    }
}
