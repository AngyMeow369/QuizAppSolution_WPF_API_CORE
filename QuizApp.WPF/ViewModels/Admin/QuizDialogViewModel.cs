using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services;
using QuizApp.WPF.Views.Admin;
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
                    (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
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
            AddOptionCommand = new RelayCommand<QuestionViewModel>(AddOptionToQuestion, q => q != null);
            DeleteOptionCommand = new RelayCommand<OptionDto>(DeleteOption, o => o != null);

            IsEditMode = false;
        }
        // ------------------- Constructor for Edit Mode -------------------
        public QuizDialogViewModel(QuizService quizService, ObservableCollection<CategoryDto> categories, QuizDto existingQuiz)
            : this(quizService, categories)
        {
            if (existingQuiz == null)
                throw new ArgumentNullException(nameof(existingQuiz));

            _ = LoadQuizDetails(existingQuiz); // asynchronous fetch of full quiz details
        }

        private async Task LoadQuizDetails(QuizDto existingQuiz)
        {
            try
            {
                IsEditMode = true;

                // Re-fetch full quiz with nested Questions & Options
                var fullQuiz = await _quizService.GetByIdAsync(existingQuiz.Id);

                Quiz = fullQuiz ?? existingQuiz;

                SelectedQuestions.Clear();
                if (Quiz.Questions != null)
                {
                    foreach (var q in Quiz.Questions)
                        SelectedQuestions.Add(QuestionViewModel.FromDto(q));
                }

                SelectedQuestion = SelectedQuestions.FirstOrDefault();
                OnPropertyChanged(nameof(SelectedQuestions));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load quiz details: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        // ------------------- Question Management -------------------
        private void AddQuestion()
        {
            var newQuestionVm = new QuestionViewModel
            {
                Text = "",
                Options = new ObservableCollection<OptionDto>()
            };

            var dialogVm = new QuestionDialogViewModel(newQuestionVm);
            var dialog = new QuestionDialog
            {
                DataContext = dialogVm,
                Owner = Application.Current.MainWindow
            };

            dialogVm.CloseRequested += result =>
            {
                if (result == true) // bool? check, no HasValue/Value
                {
                    SelectedQuestions.Add(newQuestionVm);
                    SelectedQuestion = newQuestionVm;
                }
                dialog.Close();
            };

            dialog.ShowDialog();
        }

        private void EditQuestion()
        {
            if (SelectedQuestion == null) return;

            var dialogVm = new QuestionDialogViewModel(SelectedQuestion);
            var dialog = new QuestionDialog
            {
                DataContext = dialogVm,
                Owner = Application.Current.MainWindow
            };

            dialogVm.CloseRequested += result =>
            {
                if (result == true) // bool? check, no HasValue/Value
                {
                    // changes already reflected via binding, no further action needed
                }
                dialog.Close();
            };

            dialog.ShowDialog();
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
        private void AddOptionToQuestion(QuestionViewModel? question)
        {
            if (question == null) return;

            question.Options.Add(new OptionDto
            {
                Text = "New Option",
                IsCorrect = false
            });

            OnPropertyChanged(nameof(SelectedQuestions));
        }


        private void DeleteOption(OptionDto? option)
        {
            if (SelectedQuestion == null || option == null) return;

            SelectedQuestion.Options.Remove(option);
            OnPropertyChanged(nameof(SelectedQuestions));
        }

        // ------------------- Save Quiz -------------------
        private async Task SaveAsync()
        {
            try
            {
                IsSaving = true;

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

                Quiz.Questions = SelectedQuestions.Select(q => q.ToDto()).ToList();

                bool success;
                if (IsEditMode)
                {
                    success = await _quizService.UpdateAsync(Quiz);
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
