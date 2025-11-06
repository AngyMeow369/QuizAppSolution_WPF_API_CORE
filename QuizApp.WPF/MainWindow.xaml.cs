using QuizApp.WPF.Services.Interfaces;
using QuizApp.WPF.ViewModels.Admin;
using System.Windows;

namespace QuizApp.WPF
{

    /// Interaction logic for MainWindow.xaml

    public partial class MainWindow : Window
    {
        public MainWindow(string username, string token, IAuthService authService) // Add authService parameter
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel(username, token, authService); // Pass authService
        }
    }
}
