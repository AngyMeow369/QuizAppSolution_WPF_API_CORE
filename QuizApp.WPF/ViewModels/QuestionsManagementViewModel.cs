using QuizApp.API.Models;
using QuizApp.WPF.Services;
using QuizApp.WPF.Views.Admin;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class QuestionsManagementViewModel : ObservableObject
    {
        private readonly QuestionService _questionService;
        private readonly CategoryService _categoryService;
        private ObservableCollection<Question> _questions = new();
        private bool _isLoading;

        public QuestionsManagementViewModel(QuestionService questionService, CategoryService categoryService)
        {
            _questionService = questionService ?? throw new ArgumentNullException(nameof(questionService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));

            InitializeCommands();

            // Run async safely on load
            _ = LoadQuestionsAsync();
        }

        #region Properties

        public ObservableCollection<Question> Questions
        {
            get => _questions;
            private set
            {
                _questions = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand LoadQuestionsCommand { get; private set; } = null!;
        public RelayCommand AddQuestionCommand { get; private set; } = null!;
        public RelayCommand<Question?> EditQuestionCommand { get; private set; } = null!;
        public RelayCommand<Question?> DeleteQuestionCommand { get; private set; } = null!;
        public RelayCommand CloseCommand { get; private set; } = null!;

        private void InitializeCommands()
        {
            LoadQuestionsCommand = new RelayCommand(async () => await LoadQuestionsAsync());
            AddQuestionCommand = new RelayCommand(async () => await AddQuestionAsync());
            EditQuestionCommand = new RelayCommand<Question?>(async (question) =>
            {
                if (question != null)
                    await EditQuestionAsync(question);
            });
            DeleteQuestionCommand = new RelayCommand<Question?>(async (question) =>
            {
                if (question != null)
                    await DeleteQuestionAsync(question);
            });
            CloseCommand = new RelayCommand(() => CloseAction?.Invoke(true));
        }

        #endregion

        #region Methods

        private async Task LoadQuestionsAsync()
        {
            IsLoading = true;
            try
            {
                var questions = await _questionService.GetQuestionsAsync();
                Questions.Clear();

                foreach (var question in questions)
                {
                    var options = await _questionService.GetOptionsForQuestionAsync(question.Id);

                    Questions.Add(new Question
                    {
                        Id = question.Id,
                        Text = question.Text,
                        CategoryId = question.CategoryId,
                        Category = question.Category,
                        Options = options
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading questions: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AddQuestionAsync()
        {
            try
            {
                var addQuestionView = new AddQuestionView(_categoryService)
                {
                    Owner = Application.Current.MainWindow
                };

                var result = addQuestionView.ShowDialog();

                if (result == true && addQuestionView.IsSaved)
                {
                    await _questionService.CreateQuestionWithOptionsAsync(
                        addQuestionView.Question,
                        addQuestionView.Options.ToList()
                    );

                    MessageBox.Show("Question created successfully!", "Success",
                                  MessageBoxButton.OK, MessageBoxImage.Information);

                    await LoadQuestionsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating question: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task EditQuestionAsync(Question question)
        {
            try
            {
                var editQuestionView = new AddQuestionView(_categoryService, question)
                {
                    Owner = Application.Current.MainWindow
                };

                var result = editQuestionView.ShowDialog();

                if (result == true && editQuestionView.IsSaved)
                {
                    await _questionService.UpdateQuestionWithOptionsAsync(
                        editQuestionView.Question,
                        editQuestionView.Options.ToList()
                    );

                    MessageBox.Show("Question updated successfully!", "Success",
                                  MessageBoxButton.OK, MessageBoxImage.Information);

                    await LoadQuestionsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating question: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteQuestionAsync(Question question)
        {
            var result = MessageBox.Show(
                $"Are you sure you want to delete this question?\n\n\"{question.Text}\"",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                await _questionService.DeleteQuestionAsync(question.Id);
                Questions.Remove(question);

                MessageBox.Show("Question deleted successfully!",
                              "Success",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting question: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        public Action<bool?>? CloseAction { get; set; }
    }
}
