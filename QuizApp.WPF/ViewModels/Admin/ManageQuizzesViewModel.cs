using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services;
using QuizApp.WPF.Views.Admin;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class ManageQuizzesViewModel : ObservableObject
    {
        private readonly QuizService _quizService;
        private readonly CategoryService _categoryService;

        public ObservableCollection<QuizDto> Quizzes { get; set; } = new();
        public ObservableCollection<CategoryDto> Categories { get; set; } = new();
        public ObservableCollection<QuestionDto> Questions { get; set; } = new();

        private QuizDto? _selectedQuiz;
        public QuizDto? SelectedQuiz
        {
            get => _selectedQuiz;
            set
            {
                SetProperty(ref _selectedQuiz, value);
                LoadQuestionsForSelectedQuiz();
            }
        }

        private CategoryDto? _selectedCategory;
        public CategoryDto? SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        private QuestionDto? _selectedQuestion;
        public QuestionDto? SelectedQuestion
        {
            get => _selectedQuestion;
            set => SetProperty(ref _selectedQuestion, value);
        }

        private OptionDto? _selectedOption;
        public OptionDto? SelectedOption
        {
            get => _selectedOption;
            set => SetProperty(ref _selectedOption, value);
        }

        // New properties for overlay and loading state
        private object? _currentOverlay;
        public object? CurrentOverlay
        {
            get => _currentOverlay;
            set => SetProperty(ref _currentOverlay, value);
        }

        private bool _isOverlayVisible;
        public bool IsOverlayVisible
        {
            get => _isOverlayVisible;
            set => SetProperty(ref _isOverlayVisible, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        // Commands - Added the missing ones from XAML
        public ICommand AddQuizCommand { get; }
        public ICommand EditQuizCommand { get; }
        public ICommand DeleteQuizCommand { get; }
        public ICommand ShowAddCategoryCommand { get; }
        public ICommand ShowAddQuestionCommand { get; }

        public ICommand AddCategoryCommand { get; }
        public ICommand EditCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }

        public ICommand AddQuestionCommand { get; }
        public ICommand EditQuestionCommand { get; }
        public ICommand DeleteQuestionCommand { get; }

        public ICommand AddOptionCommand { get; }
        public ICommand EditOptionCommand { get; }
        public ICommand DeleteOptionCommand { get; }

        public ManageQuizzesViewModel(QuizService quizService, CategoryService categoryService)
        {
            _quizService = quizService;
            _categoryService = categoryService;


            AddQuizCommand = new RelayCommand(async () => await AddQuiz());
            EditQuizCommand = new RelayCommand(async () => await EditQuiz(), () => SelectedQuiz != null);
            DeleteQuizCommand = new RelayCommand(async () => await DeleteQuiz(), () => SelectedQuiz != null);

            // Add the missing commands that are referenced in XAML
            ShowAddCategoryCommand = new RelayCommand(() => ShowAddCategory());
            ShowAddQuestionCommand = new RelayCommand(() => ShowAddQuestion(), () => SelectedQuiz != null);

            AddCategoryCommand = new RelayCommand(async () => await AddCategory());
            EditCategoryCommand = new RelayCommand(async () => await EditCategory(), () => SelectedCategory != null);
            DeleteCategoryCommand = new RelayCommand(async () => await DeleteCategory(), () => SelectedCategory != null);

            AddQuestionCommand = new RelayCommand(async () => await AddQuestion(), () => SelectedQuiz != null);
            EditQuestionCommand = new RelayCommand(async () => await EditQuestion(), () => SelectedQuestion != null);
            DeleteQuestionCommand = new RelayCommand(async () => await DeleteQuestion(), () => SelectedQuestion != null);

            AddOptionCommand = new RelayCommand(async () => await AddOption(), () => SelectedQuestion != null);
            EditOptionCommand = new RelayCommand(async () => await EditOption(), () => SelectedOption != null);
            DeleteOptionCommand = new RelayCommand(async () => await DeleteOption(), () => SelectedOption != null);

            _ = LoadInitialData();
        }

        private async Task LoadInitialData()
        {
            try
            {
                IsLoading = true;
                await Task.WhenAll(LoadCategories(), LoadQuizzes());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load initial data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllAsync();
                Categories = new ObservableCollection<CategoryDto>(categories);
                OnPropertyChanged(nameof(Categories));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load categories: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Categories.Clear();
            }
        }

        private async Task LoadQuizzes()
        {
            try
            {
                var quizzes = await _quizService.GetAllAsync();
                Quizzes = new ObservableCollection<QuizDto>(quizzes);
                OnPropertyChanged(nameof(Quizzes));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load quizzes: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Quizzes.Clear();
            }
        }

        private void LoadQuestionsForSelectedQuiz()
        {
            Questions.Clear();
            if (SelectedQuiz?.Questions != null)
            {
                foreach (var question in SelectedQuiz.Questions)
                    Questions.Add(question);
            }
            OnPropertyChanged(nameof(Questions));
        }

        // CRUD Methods with improved error handling

        private async Task AddQuiz()
        {
            try
            {
                var dialogVM = new QuizDialogViewModel(_quizService, new ObservableCollection<CategoryDto>(Categories));
                var dialog = new QuizDialog(dialogVM) { Owner = Application.Current.MainWindow };

                if (dialog.ShowDialog() == true)
                {
                    await LoadQuizzes(); // Reload to get the newly created quiz
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening quiz dialog: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task EditQuiz()
        {
            if (SelectedQuiz == null) return;

            try
            {
                var dialogVM = new QuizDialogViewModel(_quizService, new ObservableCollection<CategoryDto>(Categories), SelectedQuiz);
                var dialog = new QuizDialog(dialogVM) { Owner = Application.Current.MainWindow };

                if (dialog.ShowDialog() == true)
                {
                    await LoadQuizzes(); // Reload to reflect changes
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing quiz: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteQuiz()
        {
            if (SelectedQuiz == null) return;

            var confirm = MessageBox.Show($"Are you sure you want to delete '{SelectedQuiz.Title}'?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                bool success = await _quizService.DeleteAsync(SelectedQuiz.Id);
                if (success)
                {
                    Quizzes.Remove(SelectedQuiz);
                    SelectedQuiz = null;
                }
                else
                {
                    MessageBox.Show("Failed to delete the quiz.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting quiz: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Methods for the new commands
        private void ShowAddCategory()
        {
            MessageBox.Show("Add category overlay - to be implemented");
        }

        private void ShowAddQuestion()
        {
            MessageBox.Show("Add question overlay - to be implemented");
        }

        // Placeholder methods for categories, questions, options
        private Task AddCategory() => MessageBox.Show("Add category placeholder").AsTask();
        private Task EditCategory() => MessageBox.Show("Edit category placeholder").AsTask();
        private Task DeleteCategory() => MessageBox.Show("Delete category placeholder").AsTask();
        private Task AddQuestion() => MessageBox.Show("Add question placeholder").AsTask();
        private Task EditQuestion() => MessageBox.Show("Edit question placeholder").AsTask();
        private Task DeleteQuestion() => MessageBox.Show("Delete question placeholder").AsTask();
        private Task AddOption() => MessageBox.Show("Add option placeholder").AsTask();
        private Task EditOption() => MessageBox.Show("Edit option placeholder").AsTask();
        private Task DeleteOption() => MessageBox.Show("Delete option placeholder").AsTask();
    }

    static class TaskExtensions
    {
        public static Task AsTask(this MessageBoxResult _) => Task.CompletedTask;
        public static Task AsTask(this MessageBox _) => Task.CompletedTask;
    }
}