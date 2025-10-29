using System.Windows;
using QuizApp.WPF.ViewModels;

namespace QuizApp.WPF.Views.Admin
{
    public partial class AdminDashboard : Window
    {
        public AdminDashboard()
        {
            InitializeComponent();
            DataContext = new AdminDashboardViewModel();
        }
    }
}