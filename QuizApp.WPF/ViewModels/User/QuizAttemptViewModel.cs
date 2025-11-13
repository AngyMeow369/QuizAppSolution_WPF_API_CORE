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

        public QuizAttemptViewModel(UserQuizService quizService)
        {
            _quizService = quizService;
            SelectedOptions = new Dictionary<int, int>();
            SubmitCommand = new RelayCommand(async () => await SubmitQuizAsync());
        }

        // Quiz data
        private QuizTakeDto? _quiz;
        public QuizTakeDto? Quiz
        {
            get => _quiz;
            set => SetProperty(ref _quiz, value);
        }

        // Dictionary<QuestionId, OptionId>
        public Dictionary<int, int> SelectedOptions { get; }

        // Timer
        private int _remainingSeconds;
        public int RemainingSeconds
        {
            get => _remainingSeconds;
            set => SetProperty(ref _remainingSeconds, value);
        }

        private DispatcherTimer? _timer;

        // Commands
        public ICommand SubmitCommand { get; }

        // Load quiz
        public async Task LoadQuizAsync(int quizId)
        {
            try
            {
                Quiz = await _quizService.GetQuizForTakingAsync(quizId);

                // Start countdown
                var duration = (int)(Quiz.EndTime - Quiz.StartTime).TotalSeconds;
                if (duration <= 0) duration = 60 * 5; // fallback 5 min
                RemainingSeconds = duration;

                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromSeconds(1);
                _timer.Tick += Timer_Tick;
                _timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load quiz: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Timer_Tick(object? sender, EventArgs e)
        {
            RemainingSeconds--;
            if (RemainingSeconds <= 0)
            {
                _timer?.Stop();
                await SubmitQuizAsync();
            }
        }

        // Track option selection
        public void SelectOption(int questionId, int optionId)
        {
            SelectedOptions[questionId] = optionId;
        }

        // Submit quiz
        private async Task SubmitQuizAsync()
        {
            if (Quiz == null) return;

            var submission = new QuizSubmissionDto
            {
                Answers = SelectedOptions.Select(kv => new QuestionSubmissionDto
                {
                    QuestionId = kv.Key,
                    SelectedOptionId = kv.Value
                }).ToList()
            };

            try
            {
                var result = await _quizService.SubmitQuizAsync(Quiz.Id, submission);
                MessageBox.Show($"Time's up! Your score: {result.Score}/{result.TotalQuestions}", "Quiz Submitted", MessageBoxButton.OK, MessageBoxImage.Information);
                // Optionally navigate back to quiz list
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to submit quiz: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
