using QuizApp.WPF.Services.User;
using QuizApp.WPF.ViewModels.User;
using System.Windows.Controls;

namespace QuizApp.WPF.Views.User
{
    public partial class QuizAttemptView : UserControl
    {
        public QuizAttemptView(UserQuizService quizService)
        {
            InitializeComponent();
            DataContext = new QuizAttemptViewModel(quizService);
        }
    }

}
