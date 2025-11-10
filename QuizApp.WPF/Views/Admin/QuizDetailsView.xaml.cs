using QuizApp.WPF.Services;
using QuizApp.WPF.Services.Interfaces;
using QuizApp.WPF.ViewModels.Admin;
using System.Windows.Controls;

namespace QuizApp.WPF.Views.Admin
{
    public partial class QuizDetailsView : UserControl
    {
      
        public QuizDetailsView(IQuizApi quizApi, IAuthService authService)
        {
            InitializeComponent();

            var quizService = new QuizService(quizApi, authService);

            DataContext = new QuizDetailsViewModel(quizService);
        }
    }
}
