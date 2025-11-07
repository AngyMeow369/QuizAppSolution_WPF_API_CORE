using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class AdminDashboardViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly CategoryService _categoryService;
        private readonly QuestionService _questionService;
        private readonly QuizService _quizService;

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ObservableCollection<UserDto> Users { get; } = new();
        public ObservableCollection<CategoryDto> Categories { get; } = new();
        public ObservableCollection<QuestionDto> Questions { get; } = new();
        public ObservableCollection<QuizDto> Quizzes { get; } = new();

        public string CurrentUsername => _authService.Username;
        public string UserRole => _authService.Role;

        public ICommand LoadDataCommand { get; }

        public AdminDashboardViewModel(AuthService authService)
        {
            _authService = authService ?? throw new System.ArgumentNullException(nameof(authService));

            _userService = new UserService(_authService);
            _categoryService = new CategoryService(_authService);
            _questionService = new QuestionService(_authService);

            // Ensure QuizService gets its IQuizApi dependency properly
            const string baseApiUrl = "https://localhost:7075"; // ✅ Change this to your API URL
            var quizApi = Refit.RestService.For<QuizApp.WPF.Services.Interfaces.IQuizApi>(baseApiUrl); _quizService = new QuizService(quizApi, _authService);

            LoadDataCommand = new RelayCommand(async () => await LoadAllDataAsync());

            _ = LoadAllDataAsync();
        }

        private async Task LoadAllDataAsync()
        {
            IsLoading = true;
            try
            {
                await Task.WhenAll(LoadUsersAsync(), LoadCategoriesAsync(), LoadQuestionsAsync(), LoadQuizzesAsync());
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                var usersFromService = await _userService.GetUsersAsync();

                Users.Clear();
                foreach (var u in usersFromService)
                {
                    Users.Add(new UserDto
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Role = u.Role,
                        Email = u.Username + "@example.com",
                        AssignedQuizCount = 0,
                        AttemptedQuizCount = 0
                    });
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var categories = await _categoryService.GetAllAsync();
                var questions = await _questionService.GetAllAsync();

                var questionsByCategory = questions.GroupBy(q => q.CategoryId)
                                                   .ToDictionary(g => g.Key, g => g.ToList());

                Categories.Clear();
                foreach (var cat in categories)
                {
                    cat.Questions = questionsByCategory.TryGetValue(cat.Id, out var catQuestions)
                        ? catQuestions
                        : new System.Collections.Generic.List<QuestionDto>();
                    Categories.Add(cat);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadQuestionsAsync()
        {
            try
            {
                var questions = await _questionService.GetAllAsync();
                Questions.Clear();
                foreach (var q in questions)
                    Questions.Add(q);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error loading questions: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadQuizzesAsync()
        {
            try
            {
                var quizzes = await _quizService.GetAllAsync();
                Quizzes.Clear();
                foreach (var q in quizzes)
                {
                    Quizzes.Add(new QuizDto
                    {
                        Id = q.Id,
                        Title = q.Title,
                        StartTime = q.StartTime,
                        EndTime = q.EndTime,
                        CategoryId = q.CategoryId,
                        CategoryName = q.CategoryName
                    });
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error loading quizzes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
