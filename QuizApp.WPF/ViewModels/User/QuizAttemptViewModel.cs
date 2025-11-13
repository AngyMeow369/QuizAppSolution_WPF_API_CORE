using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace QuizApp.WPF.ViewModels.User
{
    public class QuizAttemptViewModel : BaseViewModel
    {
        private readonly UserQuizService _quizService;
        private QuizTakeDto? _quiz;
        private bool _isLoading;
        private bool _isSubmitting;
        private DispatcherTimer? _timer;
        private TimeSpan _timeRemaining;
        private Dictionary<int, int> _selectedAnswers = new(); // QuestionId -> OptionId

        public QuizAttemptViewModel(UserQuizService quizService)
        {
            _quizService = quizService;
            SubmitCommand = new RelayCommand(async () => await SubmitQuizAsync(), () => !_isSubmitting && _quiz != null);
        }

        public QuizTakeDto? Quiz
        {
            get => _quiz;
            set => SetProperty(ref _quiz, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool IsSubmitting
        {
            get => _isSubmitting;
            set
            {
                SetProperty(ref _isSubmitting, value);
                (SubmitCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public TimeSpan TimeRemaining
        {
            get => _timeRemaining;
            set => SetProperty(ref _timeRemaining, value);
        }

        public Dictionary<int, int> SelectedAnswers
        {
            get => _selectedAnswers;
            set => SetProperty(ref _selectedAnswers, value);
        }

        public ICommand SubmitCommand { get; }

        public async Task LoadQuizAsync(int quizId)
        {
            IsLoading = true;
            try
            {
                Quiz = await _quizService.GetQuizForTakingAsync(quizId);

                // Initialize selected answers
                SelectedAnswers = Quiz.Questions.ToDictionary(q => q.Id, q => 0);

                // Initialize timer
                TimeRemaining = Quiz.EndTime - DateTime.UtcNow;
                _timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                _timer.Tick += Timer_Tick;
                _timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading quiz: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            TimeRemaining = TimeRemaining - TimeSpan.FromSeconds(1);
            if (TimeRemaining <= TimeSpan.Zero)
            {
                _timer?.Stop();
                _ = SubmitQuizAsync(autoSubmit: true);
            }
        }

        public async Task SubmitQuizAsync(bool autoSubmit = false)
        {
            if (Quiz == null) return;
            if (!_selectedAnswers.Any()) return;

            IsSubmitting = true;

            try
            {
                var submission = new QuizSubmissionDto
                {
                    Answers = SelectedAnswers
                        .Where(kv => kv.Value != 0)
                        .Select(kv => new QuestionSubmissionDto
                        {
                            QuestionId = kv.Key,
                            SelectedOptionId = kv.Value
                        })
                        .ToList()
                };

                var result = await _quizService.SubmitQuizAsync(Quiz.Id, submission);

                _timer?.Stop();

                string message = $"You scored {result.Score}/{result.TotalQuestions}!";
                if (autoSubmit) message = "Time's up! " + message;

                MessageBox.Show(message, "Quiz Result", MessageBoxButton.OK, MessageBoxImage.Information);

                // Optionally, navigate back to dashboard or results page here
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error submitting quiz: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsSubmitting = false;
            }
        }

        public void SelectOption(int questionId, int optionId)
        {
            if (SelectedAnswers.ContainsKey(questionId))
            {
                SelectedAnswers[questionId] = optionId;
                OnPropertyChanged(nameof(SelectedAnswers));
            }
        }
    }
}
