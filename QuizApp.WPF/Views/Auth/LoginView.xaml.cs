using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuizApp.WPF.ViewModels;

namespace QuizApp.WPF.Views.Auth
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
            Loaded += (s, e) => UsernameTextBox.Focus();
        }

        // Handle PasswordBox binding
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }

        // Enter key support for Username field
        private void UsernameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PasswordBox.Focus();
            }
        }

        // Enter key support for Password field
        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is LoginViewModel viewModel && viewModel.LoginCommand.CanExecute(null))
                {
                    viewModel.LoginCommand.Execute(null);
                }
            }
        }
    }
}