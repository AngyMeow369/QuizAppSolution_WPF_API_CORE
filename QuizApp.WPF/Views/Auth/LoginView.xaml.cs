using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using QuizApp.WPF.ViewModels.Auth;

namespace QuizApp.WPF.Views.Auth
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
                vm.Password = ((PasswordBox)sender).Password;
        }

        private void UsernameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                PasswordBox.Focus();
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter &&
                DataContext is LoginViewModel vm &&
                vm.LoginCommand.CanExecute(null))
            {
                vm.LoginCommand.Execute(null);
            }
        }

        private void SignUp_Click(object sender, MouseButtonEventArgs e)
        {
            // Open registration window
            var registerWindow = new RegisterWindow();
            registerWindow.Show();

            // Close login window
            Window.GetWindow(this)?.Close();
        }

    }
}
