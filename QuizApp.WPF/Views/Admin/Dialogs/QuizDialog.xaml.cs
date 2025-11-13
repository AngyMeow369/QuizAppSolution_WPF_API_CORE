using QuizApp.WPF.ViewModels.Admin;
using System.Windows;

namespace QuizApp.WPF.Views.Admin
{
    public partial class QuizDialog : Window
    {
        public QuizDialog()
        {
            InitializeComponent();
        }

        public QuizDialog(QuizDialogViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

           
        }
    }
}
