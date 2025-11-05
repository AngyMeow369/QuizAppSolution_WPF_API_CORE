using QuizApp.WPF.Services;
using QuizApp.WPF.ViewModels.Admin;
using System.Windows.Controls;

namespace QuizApp.WPF.Views.Admin
{
    public partial class ManageQuizzesView : UserControl
    {
        public ManageQuizzesView()
        {
            InitializeComponent();


        // Assuming you have a singleton or injected QuizService
        var quizService = new QuizService(new AuthService());
            DataContext = new ManageQuizzesViewModel(quizService);
        }
    }


}
