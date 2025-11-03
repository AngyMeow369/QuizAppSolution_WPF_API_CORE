using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services.User;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels.User
{
    public class QuizTakingViewModel : INotifyPropertyChanged
    {
        private readonly UserQuizService _quizService;
        private readonly int _quizId;
        private QuizTakeDto _quiz;
        private ObservableCollection<QuestionSubmissionDto> _answers;
        private int _currentQuestionIndex;
        private bool _isLoading;
        private bool _isSubmitting;

        public QuizTakingViewModel(UserQuizService quizService, int quizId)
        {
            _quizService = quizService;
            _quizId = quizId;
            _answers = new ObservableCollection<QuestionSubmissionDto>();

            LoadQuizCommand = new RelayCommand(async () => await LoadQuizAsync());
            NextQuestionCommand = new RelayCommand(NextQuestion, () => CurrentQuestionIndex < Quiz.Questions.Count - 1);
            PreviousQuestionCommand = new RelayCommand(PreviousQuestion, () => CurrentQuestionIndex > 0);
            SubmitQuizCommand = new RelayCommand(async () => await SubmitQuizAsync(), () => !IsSubmitting && Answers.Count == Quiz.Questions.Count);

            LoadQuizCommand.Execute(null);
        }

        public QuizTakeDto Quiz
        {
            get => _quiz;
            set
            {
                _quiz = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<QuestionSubmissionDto> Answers
        {
            get => _answers;
            set
            {
                _answers = value;
                OnPropertyChanged();
            }
        }

        public int CurrentQuestionIndex
        {
            get => _currentQuestionIndex;
            set
            {
                _currentQuestionIndex = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentQuestion));
                ((RelayCommand)NextQuestionCommand).RaiseCanExecuteChanged();
                ((RelayCommand)PreviousQuestionCommand).RaiseCanExecuteChanged();
            }
        }

        public QuestionTakeDto CurrentQuestion => Quiz?.Questions.Count > 0 ? Quiz.Questions[CurrentQuestionIndex] : null;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public bool IsSubmitting
        {
            get => _isSubmitting;
            set
            {
                _isSubmitting = value;
                OnPropertyChanged();
                ((RelayCommand)SubmitQuizCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand LoadQuizCommand { get; }
        public ICommand NextQuestionCommand { get; }
        public ICommand PreviousQuestionCommand { get; }
        public ICommand SubmitQuizCommand { get; }

        private async Task LoadQuizAsync()
        {
            try
            {
                IsLoading = true;
                Quiz = await _quizService.GetQuizForTakingAsync(_quizId);

                // Initialize answers collection
                foreach (var question in Quiz.Questions)
                {
                    Answers.Add(new QuestionSubmissionDto { QuestionId = question.Id });
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading quiz: {ex.Message}", "Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void NextQuestion()
        {
            if (CurrentQuestionIndex < Quiz.Questions.Count - 1)
                CurrentQuestionIndex++;
        }

        private void PreviousQuestion()
        {
            if (CurrentQuestionIndex > 0)
                CurrentQuestionIndex--;
        }

        public void SelectAnswer(int optionId)
        {
            var currentAnswer = Answers.FirstOrDefault(a => a.QuestionId == CurrentQuestion.Id);
            if (currentAnswer != null)
            {
                currentAnswer.SelectedOptionId = optionId;
                OnPropertyChanged(nameof(Answers));
                ((RelayCommand)SubmitQuizCommand).RaiseCanExecuteChanged();
            }
        }

        private async Task SubmitQuizAsync()
        {
            try
            {
                IsSubmitting = true;

                var submission = new QuizSubmissionDto { Answers = Answers.ToList() };
                var result = await _quizService.SubmitQuizAsync(_quizId, submission);

                System.Windows.MessageBox.Show($"Quiz submitted! Score: {result.Score}/{result.TotalQuestions}", "Success",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

                // Navigate back to quizzes list
                // Navigation logic here
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error submitting quiz: {ex.Message}", "Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsSubmitting = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}