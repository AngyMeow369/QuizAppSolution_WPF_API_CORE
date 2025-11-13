using System.Windows.Controls;

namespace QuizApp.WPF.Views.Admin
{
    public partial class AssignQuizzesView : UserControl
    {
        public AssignQuizzesView(object viewModel)
        {
            InitializeComponent();
            DataContext = viewModel; // Injected from MainViewModel
        }
    }
}
