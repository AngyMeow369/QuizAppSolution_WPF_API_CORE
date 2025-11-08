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

        public ObservableCollection<CategoryDto> Categories { get; set; } = new();
        public ObservableCollection<QuestionDto> SelectedQuestions { get; set; } = new();
        public List<int> SelectedQuestionIds { get; set; } = new();

        public bool IsEditMode { get; private set; }

        public QuizDto Quiz { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // Commands for Today/Yesterday buttons with private setter
        public ICommand SetStartTimeTodayCommand { get; private set; }
        public ICommand SetStartTimeYesterdayCommand { get; private set; }
        public ICommand SetEndTimeTodayCommand { get; private set; }
        public ICommand SetEndTimeYesterdayCommand { get; private set; }

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

            SaveCommand = new RelayCommand(async () => await Save());
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(false));

            // Assign date commands
            SetStartTimeTodayCommand = new RelayCommand(() => Quiz.StartTime = DateTime.Now);
            SetStartTimeYesterdayCommand = new RelayCommand(() => Quiz.StartTime = DateTime.Now.AddDays(-1));
            SetEndTimeTodayCommand = new RelayCommand(() => Quiz.EndTime = DateTime.Now);
            SetEndTimeYesterdayCommand = new RelayCommand(() => Quiz.EndTime = DateTime.Now.AddDays(-1));
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

            // Assign date commands
            SetStartTimeTodayCommand = new RelayCommand(() => Quiz.StartTime = DateTime.Now);
            SetStartTimeYesterdayCommand = new RelayCommand(() => Quiz.StartTime = DateTime.Now.AddDays(-1));
            SetEndTimeTodayCommand = new RelayCommand(() => Quiz.EndTime = DateTime.Now);
            SetEndTimeYesterdayCommand = new RelayCommand(() => Quiz.EndTime = DateTime.Now.AddDays(-1));
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

                CloseRequested?.Invoke(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
