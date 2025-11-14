using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services.User;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace QuizApp.WPF.ViewModels.User
{
    public class UserDashboardViewModel : BaseViewModel
    {
        private readonly UserDashboardService _dashboardService;
        private UserDashboardDto? _dashboardData; 
        private bool _isLoading;

        public UserDashboardViewModel(UserDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
            _ = LoadDashboardDataAsync();
        }

        public UserDashboardDto? DashboardData 
        {
            get => _dashboardData;
            set => SetProperty(ref _dashboardData, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private async Task LoadDashboardDataAsync()
        {
            IsLoading = true;
            try
            {
                DashboardData = await _dashboardService.GetDashboardSummaryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ DEBUG: Exception in LoadDashboardDataAsync: {ex.Message}");
                Console.WriteLine($"❌ DEBUG: Stack trace: {ex.StackTrace}");
                MessageBox.Show($"Error loading dashboard: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}