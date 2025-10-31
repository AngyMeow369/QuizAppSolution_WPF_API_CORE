using QuizApp.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels
{
    public class UserListViewModel : BaseViewModel
    {
        private readonly HttpClient _httpClient;
        private bool _isLoading;
        private string _errorMessage = string.Empty;

        public ObservableCollection<UserDto> Users { get; } = new();

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoadUsersCommand { get; }

        public UserListViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
            LoadUsersCommand = new RelayCommand(() => _ = LoadUsersAsync());
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                Users.Clear();

                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<UserDto>>>("api/users/summary");

                if (response?.Success == true && response.Data != null)
                {
                    foreach (var user in response.Data)
                        Users.Add(user);
                }
                else
                {
                    ErrorMessage = response?.Message ?? "Failed to load users.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading users: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
