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

            MessageBox.Show("MainWindow constructor HIT. Hash = " + this.GetHashCode());

            this.DataContext = new MainWindowViewModel(username, token, authService);
        }

    }
}
