using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services;
using System;
using System.Collections.Generic;
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

        public bool IsEditMode { get; private set; }

        private QuizDto _quiz = new QuizDto();
        public QuizDto Quiz
        {
            get => _quiz;
            set => SetProperty(ref _quiz, value);
        }

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SetStartTimeTodayCommand { get; }
        public ICommand SetStartTimeYesterdayCommand { get; }
        public ICommand SetEndTimeTodayCommand { get; }
        public ICommand SetEndTimeYesterdayCommand { get; }

        public event Action<bool?>? CloseRequested;

        // Constructor for Add mode
        public QuizDialogViewModel(QuizService quizService, ObservableCollection<CategoryDto> categories)
        {
            _quizService = quizService;
            Categories = categories;

            Quiz = new QuizDto
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1)
            };
            IsEditMode = false;

            SaveCommand = new RelayCommand(async () => await SaveAsync());
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(false));

            SetStartTimeTodayCommand = new RelayCommand(() => Quiz.StartTime = DateTime.Now);
            SetStartTimeYesterdayCommand = new RelayCommand(() => Quiz.StartTime = DateTime.Now.AddDays(-1));
            SetEndTimeTodayCommand = new RelayCommand(() => Quiz.EndTime = DateTime.Now);
            SetEndTimeYesterdayCommand = new RelayCommand(() => Quiz.EndTime = DateTime.Now.AddDays(-1));
        }

        // Constructor for Edit mode
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
                    Options = q.Options
                }).ToList() ?? new List<QuestionDto>()
            };

            SelectedQuestions = new ObservableCollection<QuestionDto>(Quiz.Questions);
            IsEditMode = true;
        }

        private async Task SaveAsync()
        {
            try
            {
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

                // Extract question IDs instead of sending full objects
                var questionIds = SelectedQuestions.Select(q => q.Id).ToList();

                if (IsEditMode)
                {
                    // Send only the IDs for update
                    bool success = await _quizService.UpdateAsync(Quiz, questionIds);
                    if (!success)
                        throw new Exception("Failed to update the quiz.");
                }
                else
                {
                    // For creation, keep same logic
                    var createdQuiz = await _quizService.CreateAsync(Quiz, questionIds);
                    if (createdQuiz == null)
                        throw new Exception("Failed to create the quiz.");
                }

                CloseRequested?.Invoke(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
