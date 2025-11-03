using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuizApp.WPF.ViewModels.User;

namespace QuizApp.WPF.Views.User
{
    public partial class QuizTakingView : UserControl
    {
        public QuizTakingView()
        {
            InitializeComponent();
            this.DataContextChanged += QuizTakingView_DataContextChanged;
        }

        private void QuizTakingView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is QuizTakingViewModel viewModel)
            {
                viewModel.PropertyChanged += (s, args) =>
                {
                    if (args.PropertyName == nameof(QuizTakingViewModel.IsLoading) ||
                        args.PropertyName == nameof(QuizTakingViewModel.IsSubmitting))
                    {
                        UpdateVisibility();
                    }
                };
                UpdateVisibility();
            }
        }

        private void UpdateVisibility()
        {
            if (DataContext is QuizTakingViewModel viewModel)
            {
                LoadingProgressBar.Visibility = viewModel.IsLoading ? Visibility.Visible : Visibility.Collapsed;
                QuestionContent.Visibility = !viewModel.IsLoading ? Visibility.Visible : Visibility.Collapsed;
                SubmitButton.IsEnabled = !viewModel.IsSubmitting;
            }
        }
    }
}