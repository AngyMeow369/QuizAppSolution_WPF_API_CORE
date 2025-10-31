using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services;
using QuizApp.WPF.Views.Admin;
using QuizApp.WPF.Views.Auth;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System;
using System.Threading.Tasks;

namespace QuizApp.WPF.ViewModels
{
    public class AdminDashboardViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly CategoryService _categoryService;
        private readonly QuestionService _questionService;
        private readonly QuizService _quizService;
        private readonly UserService _userService;

        private ObservableCollection<UserDto> _users = new();
        private ObservableCollection<CategoryDto> _categories = new();
        private ObservableCollection<QuestionDto> _questions = new();
        private ObservableCollection<QuizDto> _quizzes = new();
        private bool _isLoading;

        public string Username => _authService.Username;
        public string Role => _authService.Role;

        public ObservableCollection<UserDto> Users
        {
            get => _users;
            private set => SetProperty(ref _users, value);
        }

        public ObservableCollection<CategoryDto> Categories
        {
            get => _categories;
            private set => SetProperty(ref _categories, value);
        }

        public ObservableCollection<QuestionDto> Questions
        {
            get => _questions;
            private set => SetProperty(ref _questions, value);
        }

        public ObservableCollection<QuizDto> Quizzes
        {
            get => _quizzes;
            private set => SetProperty(ref _quizzes, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Commands
        public ICommand AddCategoryCommand { get; }
        public ICommand ManageCategoriesCommand { get; }
        public ICommand ManageQuestionsCommand { get; }
        public ICommand ManageQuizzesCommand { get; }
        public ICommand ManageUsersCommand { get; }
        public ICommand ViewAnalyticsCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand LoadDataCommand { get; }

        public ObservableCollection<FeatureCard> FeatureCards { get; }

        public AdminDashboardViewModel(AuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            _userService = new UserService(_authService);
            _categoryService = new CategoryService(_authService);
            _questionService = new QuestionService(_authService);
            _quizService = new QuizService(_authService);

            AddCategoryCommand = new RelayCommand(async () => await AddCategoryAsync());
            ManageCategoriesCommand = new RelayCommand(async () => await ManageCategoriesAsync());
            ManageQuestionsCommand = new RelayCommand(async () => await ManageQuestionsAsync());
            ManageQuizzesCommand = new RelayCommand(async () => await ManageQuizzesAsync());
            ManageUsersCommand = new RelayCommand(async () => await ManageUsersAsync());
            ViewAnalyticsCommand = new RelayCommand(ViewAnalytics);
            SettingsCommand = new RelayCommand(Settings);
            LogoutCommand = new RelayCommand(Logout);
            LoadDataCommand = new RelayCommand(async () => await LoadAllDataAsync());

            FeatureCards = new ObservableCollection<FeatureCard>
            {
                new FeatureCard { Icon = "📚", Title = "Manage Categories", Description = "Create and organize quiz categories", Command = ManageCategoriesCommand },
                new FeatureCard { Icon = "❓", Title = "Manage Questions", Description = "Add and edit quiz questions", Command = ManageQuestionsCommand },
                new FeatureCard { Icon = "📝", Title = "Manage Quizzes", Description = "Build complete quiz sets", Command = ManageQuizzesCommand },
                new FeatureCard { Icon = "👥", Title = "User Management", Description = "Manage user accounts and roles", Command = ManageUsersCommand },
                new FeatureCard { Icon = "📈", Title = "View Analytics", Description = "Platform insights and metrics", Command = ViewAnalyticsCommand },
                new FeatureCard { Icon = "⚙️", Title = "Settings", Description = "System configuration", Command = SettingsCommand }
            };
        }

        private async Task LoadAllDataAsync()
        {
            IsLoading = true;
            try
            {
                await Task.WhenAll(
                    LoadUsersAsync(),
                    LoadCategoriesAsync(),
                    LoadQuestionsAsync(),
                    LoadQuizzesAsync()
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadUsersAsync()
        {
            var users = await _userService.GetUsersAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await _categoryService.GetAllAsync();
            Categories = new ObservableCollection<CategoryDto>(categories);
        }

        private async Task LoadQuestionsAsync()
        {
            var questions = await _questionService.GetAllAsync();
            Questions = new ObservableCollection<QuestionDto>(questions);
        }


        private async Task LoadQuizzesAsync()
        {
            var quizzes = await _quizService.GetQuizzesAsync();
        }

        private async Task ManageUsersAsync()
        {
            await LoadUsersAsync();
            MessageBox.Show($"Loaded {Users.Count} users from database!", "User Management",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task AddCategoryAsync()
        {
            var addCategoryView = new AddCategoryView();
            var result = addCategoryView.ShowDialog();

            if (result == true && addCategoryView.IsSaved)
            {
                try
                {
                    var newCategory = new CategoryDto { Name = addCategoryView.CategoryName };
                    var createdCategory = await _categoryService.CreateAsync(newCategory);
                    await LoadCategoriesAsync();

                    MessageBox.Show($"Category '{createdCategory.Name}' created successfully!",
                                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating category: {ex.Message}",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task ManageCategoriesAsync()
        {
            await LoadCategoriesAsync();
            var categoriesView = new CategoriesManagementView(_categoryService);
            categoriesView.Owner = Application.Current.MainWindow;
            var result = categoriesView.ShowDialog();

            if (result == true)
                await LoadCategoriesAsync();
        }

        private async Task ManageQuestionsAsync()
        {
            await LoadQuestionsAsync();
            MessageBox.Show($"Loaded {Questions.Count} questions!", "Questions",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task ManageQuizzesAsync()
        {
            await LoadQuizzesAsync();
            MessageBox.Show($"Loaded {Quizzes.Count} quizzes!", "Quizzes",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ViewAnalytics()
        {
            MessageBox.Show(
                $"📊 Analytics:\n" +
                $"👥 Users: {Users.Count}\n" +
                $"📚 Categories: {Categories.Count}\n" +
                $"❓ Questions: {Questions.Count}\n" +
                $"📝 Quizzes: {Quizzes.Count}",
                "Analytics",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void Settings() =>
            MessageBox.Show("Settings coming soon!", "Settings", MessageBoxButton.OK, MessageBoxImage.Information);

        private void Logout()
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Logout",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var loginView = new LoginView();
                loginView.Show();

                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this)
                        window.Close();
                }
            }
        }
    }

    public class FeatureCard
    {
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICommand? Command { get; set; }
    }
}
