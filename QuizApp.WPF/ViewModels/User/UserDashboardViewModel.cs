using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services.User;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels.User
{
    public class UserDashboardViewModel : INotifyPropertyChanged
    {
        private readonly UserDashboardService _dashboardService;
        private UserDashboardDto _dashboardData;
        private bool _isLoading;

        public UserDashboardViewModel(UserDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
            LoadDashboardCommand = new RelayCommand(async () => await LoadDashboardAsync());
            LoadDashboardCommand.Execute(null);
        }

        public UserDashboardDto DashboardData
        {
            get => _dashboardData;
            set
            {
                _dashboardData = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadDashboardCommand { get; }

        private async Task LoadDashboardAsync()
        {
            try
            {
                IsLoading = true;
                DashboardData = await _dashboardService.GetDashboardSummaryAsync();
            }
            catch (Exception ex)
            {
                // Handle error (show message to user)
                System.Windows.MessageBox.Show($"Error loading dashboard: {ex.Message}", "Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}