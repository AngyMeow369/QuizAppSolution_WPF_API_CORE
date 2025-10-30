using QuizApp.API.Models;
using QuizApp.WPF.Services;
using QuizApp.WPF.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels
{
    public class AddCategoryViewModel : ObservableObject
    {
        private string _categoryName = string.Empty;
        private readonly CategoryService _categoryService;

        public AddCategoryViewModel(CategoryService categoryService)
        {
            _categoryService = categoryService;
            SaveCommand = new RelayCommand(async () => await SaveCategoryAsync(), CanSave);
            CancelCommand = new RelayCommand(() => Cancel());
        }

        public string CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public bool IsSaved { get; private set; }
        public Action<bool?>? CloseAction { get; set; } // Made nullable

        private bool CanSave() => !string.IsNullOrWhiteSpace(CategoryName);

        private async Task SaveCategoryAsync()
        {
            try
            {
                var newCategory = new Category
                {
                    Name = CategoryName.Trim()
                };

                var createdCategory = await _categoryService.CreateCategoryAsync(newCategory);
                IsSaved = true;

                MessageBox.Show($"Category '{createdCategory.Name}' created successfully!",
                              "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                CloseAction?.Invoke(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating category: {ex.Message}",
                              "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            CloseAction?.Invoke(false);
        }
    }
}