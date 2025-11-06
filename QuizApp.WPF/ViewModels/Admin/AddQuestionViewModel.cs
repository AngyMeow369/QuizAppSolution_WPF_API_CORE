using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services;
using QuizApp.WPF.ViewModels;
using QuizApp.WPF.ViewModels.Admin;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

public class AddQuestionViewModel : ObservableObject
{
    private readonly CategoryService _categoryService;
    private bool _isSaved;

    public AddQuestionViewModel(CategoryService categoryService)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));

        Options = new ObservableCollection<OptionDto>();
        Categories = new ObservableCollection<CategoryDto>();

        AddOptionCommand = new RelayCommand(AddOption);
        RemoveOptionCommand = new RelayCommand<OptionDto>(RemoveOption);
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);

        _ = LoadCategoriesAsync();
    }

    public string QuestionText { get; set; } = string.Empty;
    public ObservableCollection<OptionDto> Options { get; }
    public ObservableCollection<CategoryDto> Categories { get; }
    public CategoryDto? SelectedCategory { get; set; }
    public QuestionDto? Question { get; private set; }
    public bool IsSaved
    {
        get => _isSaved;
        private set { _isSaved = value; OnPropertyChanged(); }
    }

    public string HeaderTitle => Question == null ? "Add New Question" : "Edit Question";

    public Action<bool?>? CloseAction { get; set; }

    public ICommand AddOptionCommand { get; }
    public ICommand RemoveOptionCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public void Reset()
    {
        Question = null;
        QuestionText = string.Empty;
        SelectedCategory = null;
        Options.Clear();
        Options.Add(new OptionDto { Text = "", IsCorrect = false });
        Options.Add(new OptionDto { Text = "", IsCorrect = true });
        IsSaved = false;

        OnPropertyChanged(nameof(QuestionText));
        OnPropertyChanged(nameof(Options));
        OnPropertyChanged(nameof(SelectedCategory));
    }

    public void LoadQuestion(QuestionDto question)
    {
        Question = question;
        QuestionText = question.Text;
        SelectedCategory = Categories.FirstOrDefault(c => c.Id == question.CategoryId);

        Options.Clear();
        foreach (var opt in question.Options) Options.Add(opt);

        IsSaved = false;

        OnPropertyChanged(nameof(QuestionText));
        OnPropertyChanged(nameof(Options));
        OnPropertyChanged(nameof(SelectedCategory));
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var categories = await _categoryService.GetAllAsync();
            Categories.Clear();
            foreach (var cat in categories) Categories.Add(cat);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void AddOption() => Options.Add(new OptionDto { Text = "", IsCorrect = false });

    private void RemoveOption(OptionDto? option)
    {
        if (option == null) return;
        if (Options.Count > 2) Options.Remove(option);
        else MessageBox.Show("A question must have at least two options.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private void Save()
    {
        if (string.IsNullOrWhiteSpace(QuestionText) || SelectedCategory == null) return;

        Question ??= new QuestionDto();
        Question.Text = QuestionText.Trim();
        Question.CategoryId = SelectedCategory.Id;
        Question.Options = Options.Where(o => !string.IsNullOrWhiteSpace(o.Text)).ToList();

        IsSaved = true;
        CloseAction?.Invoke(true);
    }

    private void Cancel()
    {
        Reset();
        CloseAction?.Invoke(false);
    }
}
