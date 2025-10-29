using QuizApp.WPF.ViewModels;
using System.Windows;

namespace QuizApp.WPF.Views.Admin
{
    public partial class AdminDashboard : Window
    {
        public AdminDashboard()
        {
            InitializeComponent();
            // Don't create ViewModel here - it will be set from LoginViewModel
        }

        // Add a constructor that accepts ViewModel
        public AdminDashboard(AdminDashboardViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}