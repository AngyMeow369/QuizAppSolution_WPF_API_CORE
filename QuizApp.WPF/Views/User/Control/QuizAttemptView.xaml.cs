using System.Windows;
using System.Windows.Controls;
using QuizApp.Shared.DTOs;
using QuizApp.WPF.ViewModels.User;

namespace QuizApp.WPF.Views.User
{
    public partial class QuizAttemptView : UserControl
    {
        public QuizAttemptView()
        {
            InitializeComponent();
        }

        private void Option_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb &&
                rb.DataContext is OptionDto option &&
                DataContext is QuizAttemptViewModel vm &&
                vm.CurrentQuestion != null)
            {
                vm.SelectOption(vm.CurrentQuestion.Id, option.Id);
            }
        }
    }
}
