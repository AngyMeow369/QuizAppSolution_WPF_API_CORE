using QuizApp.WPF.Services.Interfaces;
using QuizApp.WPF.ViewModels.User;
using System.Windows;

namespace QuizApp.WPF
{

    /// Interaction logic for MainWindow.xaml

    public partial class MainWindow : Window
    {
        public MainWindow(string username, string token, IAuthService authService)
        {
            InitializeComponent();

            this.DataContext = new MainWindowViewModel(username, token, authService);
        }

    }
}
