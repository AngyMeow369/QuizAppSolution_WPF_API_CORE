using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services;
using QuizApp.WPF.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class QuizDialogViewModel : ObservableObject
    {
        private readonly QuizService _quizService;

        public ObservableCollection<CategoryDto> Categories { get; set; }
        public ObservableCollection<QuestionViewModel> SelectedQuestions { get; set; } = new();

        private bool _isSaving;
        public bool IsSaving
        {
            get => _isSaving;
            set
            {
                if (SetProperty(ref _isSaving, value))
                {
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        private QuestionViewModel? _selectedQuestion;
        public QuestionViewModel? SelectedQuestion
        {
            get => _selectedQuestion;
            set => SetProperty(ref _selectedQuestion, value);
        }

        public bool IsEditMode { get; private set; }

        private QuizDto _quiz = new();
        public QuizDto Quiz
        {
            get => _quiz;
            set => SetProperty(ref _quiz, value);
        }

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddQuestionCommand { get; }
        public ICommand EditQuestionCommand { get; }
        public ICommand DeleteQuestionCommand { get; }
        public ICommand AddOptionCommand { get; }
        public ICommand DeleteOptionCommand { get; }

        public event Action<bool?>? CloseRequested;

        // ------------------- Constructor for Add Mode -------------------
        public QuizDialogViewModel(QuizService quizService, ObservableCollection<CategoryDto> categories)
        {
            _quizService = quizService;
            Categories = categories;

            Quiz = new QuizDto
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1)
            };

            SaveCommand = new RelayCommand(async () => await SaveAsync(), () => !IsSaving);
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(false));

            AddQuestionCommand = new RelayCommand(AddQuestion);
            EditQuestionCommand = new RelayCommand(EditQuestion, () => SelectedQuestion != null);
            DeleteQuestionCommand = new RelayCommand(DeleteQuestion, () => SelectedQuestion != null);
            AddOptionCommand = new RelayCommand(AddOption, () => SelectedQuestion != null);
            DeleteOptionCommand = new RelayCommand<OptionDto>(DeleteOption, opt => opt != null);

            IsEditMode = false;
        }

        // ------------------- Constructor for Edit Mode -------------------
        public QuizDialogViewModel(QuizService quizService, ObservableCollection<CategoryDto> categories, QuizDto existingQuiz)
            : this(quizService, categories)
        {
            if (existingQuiz == null) throw new ArgumentNullException(nameof(existingQuiz));

            Quiz = new QuizDto
            {
                Id = existingQuiz.Id,
                Title = existingQuiz.Title,
                CategoryId = existingQuiz.CategoryId,
                CategoryName = existingQuiz.CategoryName,
                StartTime = existingQuiz.StartTime,
                EndTime = existingQuiz.EndTime,
                Questions = existingQuiz.Questions?.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    CategoryId = q.CategoryId,
                    CategoryName = q.CategoryName,
                    Options = q.Options?.ToList() ?? new List<OptionDto>()
                }).ToList() ?? new()
            };

            SelectedQuestions = new ObservableCollection<QuestionViewModel>(
                Quiz.Questions.Select(QuestionViewModel.FromDto)
            );

            IsEditMode = true;
        }

        // ------------------- Question Management -------------------
        private void AddQuestion()
        {
            var newQuestion = new QuestionViewModel
            {
                Text = "New Question",
                Options = new ObservableCollection<OptionDto>()
            };

            SelectedQuestions.Add(newQuestion);
            SelectedQuestion = newQuestion;
        }

        private void EditQuestion()
        {
            if (SelectedQuestion == null) return;

            var newText = Microsoft.VisualBasic.Interaction.InputBox(
                "Edit question text:",
                "Edit Question",
                SelectedQuestion.Text);

            if (!string.IsNullOrWhiteSpace(newText))
                SelectedQuestion.Text = newText;
        }

        private void DeleteQuestion()
        {
            if (SelectedQuestion == null) return;

            var confirm = MessageBox.Show(
                $"Delete question '{SelectedQuestion.Text}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm == MessageBoxResult.Yes)
            {
                SelectedQuestions.Remove(SelectedQuestion);
                SelectedQuestion = null;
            }
        }

        // ------------------- Option Management -------------------
        private void AddOption()
        {
            if (SelectedQuestion == null) return;

            SelectedQuestion.Options.Add(new OptionDto
            {
                Text = "New Option",
                IsCorrect = false
            });

            OnPropertyChanged(nameof(SelectedQuestion));
        }

        private void DeleteOption(OptionDto? option)
        {
            if (SelectedQuestion == null || option == null) return;

            SelectedQuestion.Options.Remove(option);
            OnPropertyChanged(nameof(SelectedQuestion));
        }

        // ------------------- Save Quiz -------------------
        private async Task SaveAsync()
        {
            try
            {
                IsSaving = true;

                // Validation
                if (string.IsNullOrWhiteSpace(Quiz.Title))
                {
                    MessageBox.Show("Quiz title cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (Quiz.CategoryId == 0)
                {
                    MessageBox.Show("Please select a category.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Convert SelectedQuestions to DTOs
                Quiz.Questions = SelectedQuestions.Select(q =>
                {
                    var dto = q.ToDto();

                    // Preserve existing Question Ids
                    if (IsEditMode && q.Id != 0)
                        dto.Id = q.Id;

                    // Preserve Option Ids
                    if (dto.Options != null && q.Options != null)
                    {
                        for (int i = 0; i < dto.Options.Count; i++)
                            dto.Options[i].Id = q.Options[i].Id;
                    }

                    return dto;
                }).ToList();

                // Call Create or Update based on mode
                bool success;
                if (IsEditMode)
                {
                    success = await _quizService.UpdateAsync(Quiz, Quiz.Questions.Select(q => q.Id).ToList());
                }
                else
                {
                    var created = await _quizService.CreateAsync(Quiz, Quiz.Questions.Select(q => q.Id).ToList());
                    success = created != null;
                }

                if (!success)
                    throw new Exception("Failed to save quiz.");

                MessageBox.Show("Quiz saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseRequested?.Invoke(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsSaving = false;
            }
        }
    }
}
