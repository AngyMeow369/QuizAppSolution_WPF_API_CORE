using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuizApp.WPF.ViewModels.Auth;

namespace QuizApp.WPF.Views.Auth
{
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
            Loaded += RegisterView_Loaded;
        }

        // Set DataContext after control is loaded so Window.GetWindow(this) works
        private void RegisterView_Loaded(object? sender, RoutedEventArgs e)
        {
            // If another part already set DataContext, don't overwrite it
            if (DataContext != null)
                return;

            // Try to get the parent RegisterWindow and pass its NavigateToLogin action
            if (Window.GetWindow(this) is RegisterWindow parentWin)
            {
                DataContext = new RegisterViewModel(parentWin.NavigateToLogin);
            }
            else
            {
                // Fallback: provide a no-op navigation action so ViewModel still works in designer/tests
                DataContext = new RegisterViewModel(() => { /* no-op navigation */ });
            }
        }

        // Keep the ViewModel in sync with the PasswordBox content
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm && sender is PasswordBox pb)
            {
                vm.Password = pb.Password;
            }
        }

        // Keep the ViewModel in sync with the ConfirmPasswordBox content
        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm && sender is PasswordBox pb)
            {
                vm.ConfirmPassword = pb.Password;
            }
        }

        // When user clicks the Login link, navigate back to login via parent window
        private void LoginLink_Click(object sender, MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this) is RegisterWindow parentWin)
            {
                parentWin.NavigateToLogin();
            }
            // else: nothing to do (no parent window found)
        }
    }
}
