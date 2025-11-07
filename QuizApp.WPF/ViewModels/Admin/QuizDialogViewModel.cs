using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services;
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
        public ObservableCollection<QuestionDto> SelectedQuestions { get; set; } = new();
        public List<int> SelectedQuestionIds { get; set; } = new();

        public bool IsEditMode { get; private set; }

        public QuizDto Quiz { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<bool?>? CloseRequested; // Event to signal dialog close

        // Constructor for Add mode
        public QuizDialogViewModel(QuizService quizService, ObservableCollection<CategoryDto> categories)
        {
            _quizService = quizService;
            Categories = categories;
            Quiz = new QuizDto();
            IsEditMode = false;

            SaveCommand = new RelayCommand(async () => await Save());
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(false));
        }

        // Constructor for Edit mode
        public QuizDialogViewModel(QuizService quizService, ObservableCollection<CategoryDto> categories, QuizDto existingQuiz)
        {
            _quizService = quizService;
            Categories = categories;
            Quiz = new QuizDto
            {
                Id = existingQuiz.Id,
                Title = existingQuiz.Title,
                CategoryId = existingQuiz.CategoryId,
                StartTime = existingQuiz.StartTime,
                EndTime = existingQuiz.EndTime,
                Questions = new List<QuestionDto>(existingQuiz.Questions)
            };
            SelectedQuestions = new ObservableCollection<QuestionDto>(Quiz.Questions);
            IsEditMode = true;

            SaveCommand = new RelayCommand(async () => await Save());
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(false));
        }

        private async Task Save()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Quiz.Title))
                {
                    MessageBox.Show("Quiz title cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Quiz.Questions = SelectedQuestions.ToList();

                if (IsEditMode)
                {
                    var success = await _quizService.UpdateAsync(Quiz);
                    if (!success)
                        throw new Exception("Failed to update quiz.");
                }
                else
                {
                    var createdQuiz = await _quizService.CreateAsync(Quiz, Quiz.Questions.Select(q => q.Id).ToList());
                    if (createdQuiz == null)
                        throw new Exception("Failed to create quiz.");
                }

                CloseRequested?.Invoke(true); // Close dialog with "true" result
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
