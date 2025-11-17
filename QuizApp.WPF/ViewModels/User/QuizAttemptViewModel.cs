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

            NextQuestionCommand = new RelayCommand(NextQuestion, () => CurrentQuestionIndex < Questions.Count - 1);
            PreviousQuestionCommand = new RelayCommand(PreviousQuestion, () => CurrentQuestionIndex > 0);
            SubmitCommand = new RelayCommand(async () => await SubmitQuizAsync());
        }

        // ============================================================
        // QUIZ DATA
        // ============================================================

        private QuizTakeDto? _quiz;
        public QuizTakeDto? Quiz
        {
            get => _quiz;
            set => SetProperty(ref _quiz, value);
        }

        public ObservableCollection<QuestionTakeDto> Questions { get; } = new();

        public Dictionary<int, int> SelectedOptions { get; }

        // ============================================================
        // QUESTION NAVIGATION
        // ============================================================

        private int _currentQuestionIndex;
        public int CurrentQuestionIndex
        {
            get => _currentQuestionIndex;
            set
            {
                if (SetProperty(ref _currentQuestionIndex, value))
                {
                    OnPropertyChanged(nameof(CurrentQuestion));

                    (NextQuestionCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (PreviousQuestionCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public QuestionTakeDto? CurrentQuestion =>
            (Questions.Count > 0 &&
             CurrentQuestionIndex >= 0 &&
             CurrentQuestionIndex < Questions.Count)
            ? Questions[CurrentQuestionIndex]
            : null;

        public ICommand NextQuestionCommand { get; }
        public ICommand PreviousQuestionCommand { get; }

        private void NextQuestion()
        {
            if (CurrentQuestionIndex < Questions.Count - 1)
                CurrentQuestionIndex++;
        }

        private void PreviousQuestion()
        {
            if (CurrentQuestionIndex > 0)
                CurrentQuestionIndex--;
        }

        // ============================================================
        // TIMER
        // ============================================================

        private int _remainingSeconds;
        public int RemainingSeconds
        {
            get => _remainingSeconds;
            set
            {
                if (SetProperty(ref _remainingSeconds, value))
                    OnPropertyChanged(nameof(RemainingTimeFormatted));
            }
        }

        public string RemainingTimeFormatted
        {
            get
            {
                var ts = TimeSpan.FromSeconds(RemainingSeconds);

                if (ts < TimeSpan.Zero)
                    return "00:00";

                return ts.ToString(@"mm\:ss");
            }
        }




        private DispatcherTimer? _timer;

        // ============================================================
        // COMMANDS
        // ============================================================

        public ICommand SubmitCommand { get; }

        // ============================================================
        // LOAD QUIZ
        // ============================================================

        public async Task LoadQuizAsync(int quizId)
        {
            try
            {
                Quiz = await _quizService.GetQuizForTakingAsync(quizId);

                Questions.Clear();
                foreach (var q in Quiz.Questions)
                    Questions.Add(q);

                CurrentQuestionIndex = 0;

                var duration = (int)(Quiz.EndTime - Quiz.StartTime).TotalSeconds;
                if (duration <= 0) duration = 300;

                RemainingSeconds = duration;

                _timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                _timer.Tick += TimerTick;
                _timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to load quiz: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private async void TimerTick(object? sender, EventArgs e)
        {
            RemainingSeconds--;

            if (RemainingSeconds <= 0)
            {
                _timer?.Stop();
                await SubmitQuizAsync();
            }
        }

        // ============================================================
        // OPTION SELECTION  (used by Option_Checked event)
        // ============================================================

        public void SelectOption(int questionId, int optionId)
        {
            SelectedOptions[questionId] = optionId;

            // Tell UI to refresh the current question binding
            OnPropertyChanged(nameof(CurrentQuestion));
        }

        // ============================================================
        // SUBMIT QUIZ
        // ============================================================

        private async Task SubmitQuizAsync()
        {
            if (Quiz == null)
                return;

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

                _timer?.Stop();

                MessageBox.Show(
                    $"Your score: {result.Score}/{result.TotalQuestions}",
                    "Quiz Submitted",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to submit quiz: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}
