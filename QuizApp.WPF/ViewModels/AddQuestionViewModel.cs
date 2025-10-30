using QuizApp.API.Models;
using QuizApp.WPF.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class AddQuestionViewModel : ObservableObject
    {
        private readonly CategoryService _categoryService;

        private string _questionText = string.Empty;
        private Category? _selectedCategory;
        private string _headerTitle = string.Empty;
        private string _validationMessage = string.Empty;
        private bool _isValidationVisible;
        private bool _isEditMode;
        private bool _isLoading;

        public AddQuestionViewModel(CategoryService categoryService, Question? existingQuestion = null)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            Categories = new ObservableCollection<Category>();
            Options = new ObservableCollection<Option>();
            Question = existingQuestion ?? new Question();
            _isEditMode = existingQuestion != null;

            InitializeCommands();
            LoadCategoriesAsync();
            InitializeForm();
        }

        #region Properties

        public ObservableCollection<Category> Categories { get; }
        public ObservableCollection<Option> Options { get; }
        public Question Question { get; }

        public string QuestionText
        {
            get => _questionText;
            set
            {
                _questionText = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(QuestionTextLength));
            }
        }

        public int QuestionTextLength => QuestionText.Length;

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
            }
        }

        public string HeaderTitle
        {
            get => _headerTitle;
            set
            {
                _headerTitle = value;
                OnPropertyChanged();
            }
        }

        public string ValidationMessage
        {
            get => _validationMessage;
            set
            {
                _validationMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsValidationVisible
        {
            get => _isValidationVisible;
            set
            {
                _isValidationVisible = value;
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

        public bool IsSaved { get; private set; }

        #endregion

        #region Commands

        public RelayCommand AddOptionCommand { get; private set; } = null!;
        public RelayCommand<Option> RemoveOptionCommand { get; private set; } = null!;
        public RelayCommand SaveCommand { get; private set; } = null!;
        public RelayCommand CancelCommand { get; private set; } = null!;

        private void InitializeCommands()
        {
            AddOptionCommand = new RelayCommand(AddOption);
            RemoveOptionCommand = new RelayCommand<Option>(RemoveOption);
            SaveCommand = new RelayCommand(SaveQuestion);
            CancelCommand = new RelayCommand(() => CloseAction?.Invoke(false));
        }

        #endregion

        #region Methods

        private async void LoadCategoriesAsync()
        {
            IsLoading = true;
            try
            {
                var categories = await _categoryService.GetCategoriesAsync();
                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }

                if (_isEditMode && Question.CategoryId > 0)
                {
                    SelectedCategory = Categories.FirstOrDefault(c => c.Id == Question.CategoryId);
                }
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

        private void InitializeForm()
        {
            HeaderTitle = _isEditMode ? "Edit Question" : "Add New Question";
            QuestionText = Question.Text ?? string.Empty;

            if (_isEditMode && Question.Options != null && Question.Options.Any())
            {
                foreach (var option in Question.Options)
                {
                    Options.Add(new Option
                    {
                        Id = option.Id,
                        Text = option.Text,
                        IsCorrect = option.IsCorrect,
                        QuestionId = option.QuestionId
                    });
                }
            }
            else
            {
                Options.Add(new Option { Text = "", IsCorrect = false });
                Options.Add(new Option { Text = "", IsCorrect = true });
            }
        }

        private void AddOption()
        {
            Options.Add(new Option { Text = "", IsCorrect = false });
        }

        private void RemoveOption(Option? option)
        {
            if (option == null) return;

            if (Options.Count > 2)
            {
                Options.Remove(option);
            }
            else
            {
                MessageBox.Show("A question must have at least two options.", "Validation Error",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SaveQuestion()
        {
            IsValidationVisible = false;

            if (string.IsNullOrWhiteSpace(QuestionText))
            {
                MessageBox.Show("Please enter question text.", "Validation Error",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedCategory == null)
            {
                MessageBox.Show("Please select a category.", "Validation Error",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var validOptions = Options.Where(o => !string.IsNullOrWhiteSpace(o.Text)).ToList();

            if (validOptions.Count < 2)
            {
                ValidationMessage = "⚠️ Please add at least 2 options with text";
                IsValidationVisible = true;
                MessageBox.Show("Please add at least two options with text.", "Validation Error",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var hasCorrectOption = validOptions.Any(o => o.IsCorrect);
            if (!hasCorrectOption)
            {
                ValidationMessage = "⚠️ Please mark at least one option as correct";
                IsValidationVisible = true;
                MessageBox.Show("Please mark at least one option as correct.", "Validation Error",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Question.Text = QuestionText.Trim();
            Question.CategoryId = SelectedCategory.Id;
            Question.Options = Options.ToList();

            IsSaved = true;
            CloseAction?.Invoke(true);
        }

        #endregion

        public Action<bool?>? CloseAction { get; set; }
    }
}
