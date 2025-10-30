using QuizApp.API.Models;
using QuizApp.WPF.Services;
using QuizApp.WPF.Views.Admin;
using QuizApp.WPF.Views.Auth;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels
{
    public class AdminDashboardViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly CategoryService _categoryService;
        private readonly QuestionService _questionService;
        private readonly QuizService _quizService;

        private ObservableCollection<User> _users;
        private ObservableCollection<Category> _categories;
        private ObservableCollection<Question> _questions;
        private ObservableCollection<Quiz> _quizzes;
        private bool _isLoading;

        public string Username => _authService.Username;
        public string Role => _authService.Role;

        // Data Collections
        public ObservableCollection<User> Users
        {
            get => _users;
            private set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            private set
            {
                _categories = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Question> Questions
        {
            get => _questions;
            private set
            {
                _questions = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Quiz> Quizzes
        {
            get => _quizzes;
            private set
            {
                _quizzes = value;
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

        // Commands
        public ICommand AddCategoryCommand { get; } // ADDED THIS
        public ICommand ManageCategoriesCommand { get; }
        public ICommand ManageQuestionsCommand { get; }
        public ICommand ManageQuizzesCommand { get; }
        public ICommand ManageUsersCommand { get; }
        public ICommand ViewAnalyticsCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand LoadDataCommand { get; }

        public ObservableCollection<FeatureCard> FeatureCards { get; }

        // Updated constructor that accepts AuthService parameter
        public AdminDashboardViewModel(AuthService authService)
        {
            _authService = authService; // Use the instance passed from login (has token)
            _userService = new UserService(_authService);
            _categoryService = new CategoryService(_authService);
            _questionService = new QuestionService(_authService);
            _quizService = new QuizService(_authService);

            _users = new ObservableCollection<User>();
            _categories = new ObservableCollection<Category>();
            _questions = new ObservableCollection<Question>();
            _quizzes = new ObservableCollection<Quiz>();

            // Initialize commands
            AddCategoryCommand = new RelayCommand(async () => await AddCategoryAsync()); // ADDED THIS
            ManageCategoriesCommand = new RelayCommand(async () => await ManageCategoriesAsync());
            ManageQuestionsCommand = new RelayCommand(async () => await ManageQuestionsAsync());
            ManageQuizzesCommand = new RelayCommand(async () => await ManageQuizzesAsync());
            ManageUsersCommand = new RelayCommand(async () => await ManageUsersAsync());
            ViewAnalyticsCommand = new RelayCommand(ViewAnalytics);
            SettingsCommand = new RelayCommand(Settings);
            LogoutCommand = new RelayCommand(Logout);
            LoadDataCommand = new RelayCommand(async () => await LoadAllDataAsync());

            // Initialize feature cards
            FeatureCards = new ObservableCollection<FeatureCard>
            {
                new FeatureCard { Icon = "📚", Title = "Manage Categories", Description = "Create and organize quiz categories", Command = ManageCategoriesCommand },
                new FeatureCard { Icon = "❓", Title = "Manage Questions", Description = "Add and edit quiz questions", Command = ManageQuestionsCommand },
                new FeatureCard { Icon = "📝", Title = "Manage Quizzes", Description = "Build complete quiz sets", Command = ManageQuizzesCommand },
                new FeatureCard { Icon = "👥", Title = "User Management", Description = "Manage user accounts and roles", Command = ManageUsersCommand },
                new FeatureCard { Icon = "📈", Title = "View Analytics", Description = "Platform insights and metrics", Command = ViewAnalyticsCommand },
                new FeatureCard { Icon = "⚙️", Title = "Settings", Description = "System configuration", Command = SettingsCommand }
            };

            // Load initial data
            
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
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadUsersAsync()
        {
            var users = await _userService.GetUsersAsync();
            Users = new ObservableCollection<User>(users);
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await _categoryService.GetCategoriesAsync();
            Categories = new ObservableCollection<Category>(categories);
        }

        private async Task LoadQuestionsAsync()
        {
            var questions = await _questionService.GetQuestionsAsync();
            Questions = new ObservableCollection<Question>(questions);
        }

        private async Task LoadQuizzesAsync()
        {
            var quizzes = await _quizService.GetQuizzesAsync();
            Quizzes = new ObservableCollection<Quiz>(quizzes);
        }

        private async Task ManageUsersAsync()
        {
            await LoadUsersAsync();
            MessageBox.Show($"Loaded {Users.Count} users from database!\n\n" +
                          "Next: We'll build a proper user management UI.",
                          "User Management",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ADDED THIS METHOD: For Add Category button
        private async Task AddCategoryAsync()
        {
            // Open AddCategoryView (simple dialog)
            var addCategoryView = new AddCategoryView();
            var result = addCategoryView.ShowDialog();

            if (result == true && addCategoryView.IsSaved)
            {
                try
                {
                    var newCategory = new Category
                    {
                        Name = addCategoryView.CategoryName
                    };

                    var createdCategory = await _categoryService.CreateCategoryAsync(newCategory);

                    // Refresh categories to update the count
                    await LoadCategoriesAsync();

                    MessageBox.Show($"Category '{createdCategory.Name}' created successfully!",
                                  "Success",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating category: {ex.Message}",
                                  "Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // UPDATED THIS METHOD: For Manage Categories button
        private async Task ManageCategoriesAsync()
        {
            await LoadCategoriesAsync();

            // Open CategoriesManagementView (full CRUD)
            var categoriesView = new CategoriesManagementView(_categoryService);
            categoriesView.Owner = Application.Current.MainWindow;
            var result = categoriesView.ShowDialog();

            // Refresh categories after management window closes
            if (result == true)
            {
                await LoadCategoriesAsync();
            }
        }

        private async Task ManageQuestionsAsync()
        {
            await LoadQuestionsAsync();
            MessageBox.Show($"Loaded {Questions.Count} questions from database!\n\n" +
                          "Next: We'll build a proper question management UI.",
                          "Question Management",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task ManageQuizzesAsync()
        {
            await LoadQuizzesAsync();
            MessageBox.Show($"Loaded {Quizzes.Count} quizzes from database!\n\n" +
                          "Next: We'll build a proper quiz management UI.",
                          "Quiz Management",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ViewAnalytics()
        {
            var userCount = Users.Count;
            var categoryCount = Categories.Count;
            var questionCount = Questions.Count;
            var quizCount = Quizzes.Count;

            MessageBox.Show($"📊 Platform Analytics:\n\n" +
                          $"👥 Users: {userCount}\n" +
                          $"📚 Categories: {categoryCount}\n" +
                          $"❓ Questions: {questionCount}\n" +
                          $"📝 Quizzes: {quizCount}\n\n" +
                          "Real analytics dashboard coming soon!",
                          "Analytics",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Settings()
        {
            MessageBox.Show("Settings panel coming soon!", "Feature",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Logout()
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Logout",
                                       MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var loginView = new LoginView();
                    loginView.Show();

                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.DataContext == this)
                            window.Close();
                    }
                });
            }
        }
    }

    public class FeatureCard
    {
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICommand? Command { get; set; } // Made nullable
    }
}