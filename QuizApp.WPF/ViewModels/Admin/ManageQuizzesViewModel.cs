using QuizApp.API.DTOs;
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
                if (SetProperty(ref _selectedQuiz, value))
                {
                    LoadQuestionsForSelectedQuiz();

                    // Notify commands to re-evaluate CanExecute
                    (EditQuizCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (DeleteQuizCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }
        private CategoryDto? _selectedCategory;
        public CategoryDto? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    (EditCategoryCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (DeleteCategoryCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }


        private QuestionDto? _selectedQuestion;
        public QuestionDto? SelectedQuestion
        {
            get => _selectedQuestion;
            set => SetProperty(ref _selectedQuestion, value);
        }

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

        public ICommand AddQuizCommand { get; }
        public ICommand EditQuizCommand { get; }
        public ICommand DeleteQuizCommand { get; }
        public ICommand ShowAddCategoryCommand { get; }
        public ICommand ShowAddQuestionCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand EditCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }


        public ManageQuizzesViewModel(QuizService quizService, CategoryService categoryService)
        {
            _quizService = quizService;
            _categoryService = categoryService;

            AddQuizCommand = new RelayCommand(async () => await AddQuiz());
            EditQuizCommand = new RelayCommand(async () => await EditQuiz(), () => SelectedQuiz != null);
            DeleteQuizCommand = new RelayCommand(async () => await DeleteQuiz(), () => SelectedQuiz != null);

            ShowAddCategoryCommand = new RelayCommand(() => ShowAddCategory());
            ShowAddQuestionCommand = new RelayCommand(() => ShowAddQuestion(), () => SelectedQuiz != null);

            AddCategoryCommand = new RelayCommand(async () => await AddCategory());
            EditCategoryCommand = new RelayCommand(async () => await EditCategory(), () => SelectedCategory != null);
            DeleteCategoryCommand = new RelayCommand(async () => await DeleteCategory(), () => SelectedCategory != null);


            _ = LoadInitialData();
        }

        private async Task AddCategory()
        {
            var name = Microsoft.VisualBasic.Interaction.InputBox("Enter category name:", "Add Category", "");
            if (string.IsNullOrWhiteSpace(name)) return;

            try
            {
                var newCategory = new CategoryDto { Name = name };
                var createdCategory = await _categoryService.CreateAsync(newCategory);
                if (createdCategory != null)
                {
                    Categories.Add(createdCategory);
                }
                else
                {
                    MessageBox.Show("Failed to create category.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task EditCategory()
        {
            if (SelectedCategory == null) return;

            var name = Microsoft.VisualBasic.Interaction.InputBox("Edit category name:", "Edit Category", SelectedCategory.Name);
            if (string.IsNullOrWhiteSpace(name)) return;

            try
            {
                var updatedCategory = new CategoryDto
                {
                    Id = SelectedCategory.Id,
                    Name = name
                };

                var success = await _categoryService.UpdateAsync(updatedCategory);
                if (success)
                {
                    SelectedCategory.Name = name;
                    MessageBox.Show("Category updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to update category.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async Task DeleteCategory()
        {
            if (SelectedCategory == null) return;

            var confirm = MessageBox.Show($"Delete category '{SelectedCategory.Name}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                var success = await _categoryService.DeleteAsync(SelectedCategory.Id);
                if (success)
                {
                    Categories.Remove(SelectedCategory);
                    SelectedCategory = null;
                }
                else
                {
                    MessageBox.Show("Failed to delete category.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                MessageBox.Show($"Failed to load data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show($"Failed to load categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show($"Failed to load quizzes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Quizzes.Clear();
            }
        }

        private void LoadQuestionsForSelectedQuiz()
        {
            Questions.Clear();
            if (SelectedQuiz?.Questions != null)
            {
                foreach (var q in SelectedQuiz.Questions)
                    Questions.Add(q);
            }
        }

        private async Task AddQuiz()
        {
            var dialogVM = new QuizDialogViewModel(_quizService, Categories);
            var dialog = new QuizDialog(dialogVM) { Owner = Application.Current.MainWindow };

            if (dialog.ShowDialog() == true)
                await LoadQuizzes();
        }

        // In ManageQuizzesViewModel
        private async Task EditQuiz()
        {
            if (SelectedQuiz == null) return;

            // Pass the selected quiz to the dialog VM
            var dialogVM = new QuizDialogViewModel(_quizService, Categories, SelectedQuiz);
            var dialog = new QuizDialog(dialogVM) { Owner = Application.Current.MainWindow };

            if (dialog.ShowDialog() == true)
            {
                // Instead of reloading everything, just update the existing quiz in the collection
                var updatedQuiz = await _quizService.GetByIdAsync(SelectedQuiz.Id);
                if (updatedQuiz != null)
                {
                    var index = Quizzes.IndexOf(SelectedQuiz);
                    if (index >= 0)
                    {
                        Quizzes[index] = updatedQuiz;
                        SelectedQuiz = updatedQuiz;
                        OnPropertyChanged(nameof(Quizzes));
                    }
                }
            }
        }


        private async Task DeleteQuiz()
        {
            if (SelectedQuiz == null) return;

            var confirm = MessageBox.Show($"Delete '{SelectedQuiz.Title}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                if (await _quizService.DeleteAsync(SelectedQuiz.Id))
                {
                    Quizzes.Remove(SelectedQuiz);
                    SelectedQuiz = null;
                }
                else
                {
                    MessageBox.Show("Failed to delete the quiz.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting quiz: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowAddCategory() => MessageBox.Show("Add category overlay placeholder");
        private void ShowAddQuestion() => MessageBox.Show("Add question overlay placeholder");
    }
}
