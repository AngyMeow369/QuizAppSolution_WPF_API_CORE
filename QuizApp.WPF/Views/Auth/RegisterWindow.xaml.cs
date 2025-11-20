using System.Windows;
using QuizApp.WPF.ViewModels.Auth;

namespace QuizApp.WPF.Views.Auth
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            DataContext = new RegisterViewModel(NavigateToLogin);
        }

        public void NavigateToLogin()
        {
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}
