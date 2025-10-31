using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using QuizApp.WPF.ViewModels;

namespace QuizApp.WPF.Views
{
    public partial class UserListView : UserControl
    {
        private readonly UserListViewModel _viewModel;

        public UserListView()
        {
            InitializeComponent();

            // Ideally inject or reuse a shared HttpClient.
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7016/")
            };

            _viewModel = new UserListViewModel(httpClient);
            DataContext = _viewModel;

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Trigger the command (handles async internally)
            if (_viewModel.LoadUsersCommand.CanExecute(null))
                _viewModel.LoadUsersCommand.Execute(null);
        }
    }
}
