using QuizApp.API.Models;
using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services;
using QuizApp.WPF.ViewModels.Admin;
using QuizApp.WPF.Views.Admin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class ManageQuizzesViewModel : ObservableObject
    {
        private readonly QuizService _quizService;
        private readonly CategoryService _categoryService;


    public ObservableCollection<Quiz> Quizzes { get; set; } = new();
        public ObservableCollection<CategoryDto> Categories { get; set; } = new();

        private Quiz? _selectedQuiz;
        public Quiz? SelectedQuiz
        {
            get => _selectedQuiz;
            set
            {
                SetProperty(ref _selectedQuiz, value);
                (EditQuizCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteQuizCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (AssignQuizCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private CategoryDto? _selectedCategory;
        public CategoryDto? SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Overlay properties
        private UserControl? _currentOverlay;
        public UserControl? CurrentOverlay
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

        // Overlay ViewModels
        public AddCategoryViewModel AddCategoryVM { get; }
        public AddQuestionViewModel AddQuestionVM { get; }

        // Commands
        public ICommand ShowAddCategoryCommand { get; }
        public ICommand ShowAddQuestionCommand { get; }

        public ICommand AddQuizCommand { get; }
        public ICommand EditQuizCommand { get; }
        public ICommand DeleteQuizCommand { get; }
        public ICommand AssignQuizCommand { get; }

        public ManageQuizzesViewModel(QuizService quizService, CategoryService categoryService)
        {
            _quizService = quizService ?? throw new ArgumentNullException(nameof(quizService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));

            AddCategoryVM = new AddCategoryViewModel(categoryService);
            AddCategoryVM.CloseAction = CloseOverlay;

            AddQuestionVM = new AddQuestionViewModel(categoryService);
            AddQuestionVM.CloseAction = CloseOverlay;

            ShowAddCategoryCommand = new RelayCommand(ShowAddCategory);
            ShowAddQuestionCommand = new RelayCommand(ShowAddQuestion);

            AddQuizCommand = new RelayCommand(async () => await OnAddQuiz());
            EditQuizCommand = new RelayCommand(async () => await OnEditQuiz(), () => SelectedQuiz != null);
            DeleteQuizCommand = new RelayCommand(async () => await OnDeleteQuiz(), () => SelectedQuiz != null);
            AssignQuizCommand = new RelayCommand(OnAssignQuiz, () => SelectedQuiz != null);

            _ = LoadInitialData();
        }

        private async Task LoadInitialData()
        {
            await LoadCategories();
            await LoadQuizzes();
        }

        private async Task LoadCategories()
        {
            try
            {
                var cats = await _categoryService.GetAllAsync();
                Categories = new ObservableCollection<CategoryDto>(cats);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load categories: {ex.Message}");
            }
        }

        private async Task LoadQuizzes()
        {
            try
            {
                IsLoading = true;
                var quizzes = await _quizService.GetQuizzesAsync();
                Quizzes.Clear();
                foreach (var quiz in quizzes)
                    Quizzes.Add(quiz);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load quizzes: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Overlay methods
        private void ShowAddCategory()
        {
            CurrentOverlay = new AddCategoryControl { DataContext = AddCategoryVM };
            IsOverlayVisible = true;
        }

        private void ShowAddQuestion()
        {
            AddQuestionVM.Reset();
            CurrentOverlay = new AddQuestionControl { DataContext = AddQuestionVM };
            IsOverlayVisible = true;
        }

        private void CloseOverlay(bool? result = null)
        {
            IsOverlayVisible = false;
            CurrentOverlay = null;
        }

        // Quiz operations
        private async Task OnAddQuiz()
        {
            if (SelectedCategory == null)
            {
                MessageBox.Show("Please select a category before adding a quiz.", "Missing Category", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newQuiz = new Quiz
                {
                    Title = "New Quiz",
                    CategoryId = SelectedCategory.Id,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1)
                };

                IsLoading = true;
                var createdQuiz = await _quizService.CreateQuizAsync(newQuiz, new List<int>());
                Quizzes.Add(createdQuiz);
                MessageBox.Show("Quiz added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add quiz: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task OnEditQuiz()
        {
            if (SelectedQuiz == null)
                return;

            try
            {
                SelectedQuiz.Title += " (Edited)";
                if (SelectedCategory != null)
                    SelectedQuiz.CategoryId = SelectedCategory.Id;

                IsLoading = true;
                await _quizService.UpdateQuizAsync(SelectedQuiz);
                MessageBox.Show("Quiz updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update quiz: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task OnDeleteQuiz()
        {
            if (SelectedQuiz == null)
                return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete '{SelectedQuiz.Title}'?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                IsLoading = true;
                await _quizService.DeleteQuizAsync(SelectedQuiz.Id);
                Quizzes.Remove(SelectedQuiz);
                SelectedQuiz = null;
                MessageBox.Show("Quiz deleted successfully!", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete quiz: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OnAssignQuiz()
        {
            if (SelectedQuiz == null)
                return;

            MessageBox.Show($"Assign Quiz: {SelectedQuiz.Title}", "Assign Quiz");
        }
    }


}
