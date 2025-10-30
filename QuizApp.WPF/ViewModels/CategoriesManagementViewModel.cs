using QuizApp.API.Models;
using QuizApp.WPF.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels
{
    public class CategoriesManagementViewModel : ObservableObject
    {
        private readonly CategoryService _categoryService;
        private ObservableCollection<Category> _categories = [];
        private string _newCategoryName = string.Empty;
        private bool _isLoading;

        public CategoriesManagementViewModel(CategoryService categoryService)
        {
            _categoryService = categoryService;

            LoadCategoriesCommand = new RelayCommand(async () => await LoadCategoriesAsync());
            AddCategoryCommand = new RelayCommand(async () => await AddCategoryAsync(), CanAddCategory);
            EditCategoryCommand = new RelayCommand<Category>(async c => await EditCategoryAsync(c));
            DeleteCategoryCommand = new RelayCommand<Category>(async c => await DeleteCategoryAsync(c));
            CloseCommand = new RelayCommand(() => CloseAction?.Invoke(true));

            LoadCategoriesCommand.Execute(null);
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            private set => SetProperty(ref _categories, value);
        }

        public string NewCategoryName
        {
            get => _newCategoryName;
            set
            {
                if (SetProperty(ref _newCategoryName, value))
                    (AddCategoryCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand LoadCategoriesCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand EditCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand CloseCommand { get; }

        public Action<bool?>? CloseAction { get; set; }

        private bool CanAddCategory() => !string.IsNullOrWhiteSpace(NewCategoryName);

        private async Task LoadCategoriesAsync()
        {
            IsLoading = true;
            try
            {
                var categories = await _categoryService.GetCategoriesAsync();
                Categories = new ObservableCollection<Category>(categories);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AddCategoryAsync()
        {
            if (string.IsNullOrWhiteSpace(NewCategoryName))
                return;

            try
            {
                var newCategory = new Category { Name = NewCategoryName.Trim() };
                var created = await _categoryService.CreateCategoryAsync(newCategory);
                Categories.Add(created);
                NewCategoryName = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating category: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task EditCategoryAsync(Category? category)
        {
            if (category == null) return;

            var newName = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter new category name:", "Edit Category", category.Name);

            if (string.IsNullOrWhiteSpace(newName)) return;

            try
            {
                var updated = new Category { Id = category.Id, Name = newName.Trim() };
                await _categoryService.UpdateCategoryAsync(updated);
                await LoadCategoriesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating category: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteCategoryAsync(Category? category)
        {
            if (category == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete category '{category.Name}'?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                await _categoryService.DeleteCategoryAsync(category.Id);
                Categories.Remove(category);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting category: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
