using System.Windows;
using System.Windows.Controls;
using QuizApp.WPF.ViewModels.User;

namespace QuizApp.WPF.Views.User
{
    public partial class UserDashboardView : UserControl
    {
        public UserDashboardView()
        {
            InitializeComponent();
            this.DataContextChanged += UserDashboardView_DataContextChanged;
        }

        private void UserDashboardView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is UserDashboardViewModel viewModel)
            {
                viewModel.PropertyChanged += (s, args) =>
                {
                    if (args.PropertyName == nameof(UserDashboardViewModel.IsLoading) ||
                        args.PropertyName == nameof(UserDashboardViewModel.DashboardData))
                    {
                        UpdateVisibility();
                    }
                };
                UpdateVisibility();
            }
        }

        private void UpdateVisibility()
        {
            if (DataContext is UserDashboardViewModel viewModel)
            {
                // Handle loading state
                LoadingProgressBar.Visibility = viewModel.IsLoading ? Visibility.Visible : Visibility.Collapsed;
                StatsPanel.Visibility = !viewModel.IsLoading ? Visibility.Visible : Visibility.Collapsed;
                ContentGrid.Visibility = !viewModel.IsLoading ? Visibility.Visible : Visibility.Collapsed;

                // Handle empty states
                if (viewModel.DashboardData != null)
                {
                    NoResultsText.Visibility = viewModel.DashboardData.RecentResults.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                    NoQuizzesText.Visibility = viewModel.DashboardData.UpcomingQuizzes.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }
    }
}