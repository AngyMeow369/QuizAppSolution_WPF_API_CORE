using System.Windows.Controls;

namespace QuizApp.WPF.Views.Admin
{
    // Must be public
    public partial class AssignQuizzesView : UserControl
    {
        // Public parameterless constructor
        public AssignQuizzesView()
        {
            InitializeComponent();
        }

        // Optional constructor for manual ViewModel injection (won't affect XAML)
        public AssignQuizzesView(object viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
