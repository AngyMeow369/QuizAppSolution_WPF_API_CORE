using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class AddQuestionViewModel : ObservableObject
    {
        private readonly CategoryService _categoryService;

        private string _questionText = string.Empty;
        private CategoryDto? _selectedCategory;
        private string _headerTitle = string.Empty;
        private string _validationMessage = string.Empty;
        private bool _isValidationVisible;
        private bool _isEditMode;
        private bool _isLoading;

        public AddQuestionViewModel(CategoryService categoryService, QuestionDto? existingQuestion = null)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            Categories = new ObservableCollection<CategoryDto>();
            Options = new ObservableCollection<OptionDto>();
            Question = existingQuestion ?? new QuestionDto();
            _isEditMode = existingQuestion != null;

            InitializeCommands();
            LoadCategoriesAsync();
            InitializeForm();
        }

        #region Properties

        public ObservableCollection<CategoryDto> Categories { get; }
        public ObservableCollection<OptionDto> Options { get; }
        public QuestionDto Question { get; }

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

        public CategoryDto? SelectedCategory
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
        public RelayCommand<OptionDto> RemoveOptionCommand { get; private set; } = null!;
        public RelayCommand SaveCommand { get; private set; } = null!;
        public RelayCommand CancelCommand { get; private set; } = null!;

        private void InitializeCommands()
        {
            AddOptionCommand = new RelayCommand(AddOption);
            RemoveOptionCommand = new RelayCommand<OptionDto>(RemoveOption);
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
                var categories = await _categoryService.GetAllAsync();
                Categories.Clear();

                // ensure CategoryDto conversion if service still returns API models
                foreach (var category in categories)
                {
                    Categories.Add(new CategoryDto
                    {
                        Id = category.Id,
                        Name = category.Name
                    });
                }

                if (_isEditMode && Question.CategoryId > 0)
                    SelectedCategory = Categories.FirstOrDefault(c => c.Id == Question.CategoryId);
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

            Options.Clear();

            if (_isEditMode && Question.Options != null && Question.Options.Any())
            {
                foreach (var option in Question.Options)
                {
                    Options.Add(new OptionDto
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
                // default: one correct and one incorrect
                Options.Add(new OptionDto { Text = "", IsCorrect = false });
                Options.Add(new OptionDto { Text = "", IsCorrect = true });
            }
        }

        private void AddOption()
        {
            Options.Add(new OptionDto { Text = "", IsCorrect = false });
        }

        private void RemoveOption(OptionDto? option)
        {
            if (option == null) return;

            if (Options.Count > 2)
                Options.Remove(option);
            else
                MessageBox.Show("A question must have at least two options.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            if (!validOptions.Any(o => o.IsCorrect))
            {
                ValidationMessage = "⚠️ Please mark at least one option as correct";
                IsValidationVisible = true;
                MessageBox.Show("Please mark at least one option as correct.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Question.Text = QuestionText.Trim();
            Question.CategoryId = SelectedCategory.Id;
            Question.Options = validOptions;

            IsSaved = true;
            CloseAction?.Invoke(true);
        }

        #endregion

        public Action<bool?>? CloseAction { get; set; }
    }
}
