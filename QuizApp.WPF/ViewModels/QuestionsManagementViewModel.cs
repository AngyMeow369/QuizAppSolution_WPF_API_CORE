using QuizApp.Shared.DTOs;
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
        private ObservableCollection<QuestionDto> _questions = new();
        private bool _isLoading;

        public QuestionsManagementViewModel(QuestionService questionService, CategoryService categoryService)
        {
            _questionService = questionService ?? throw new ArgumentNullException(nameof(questionService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));

            InitializeCommands();
            _ = LoadQuestionsAsync();
        }

        #region Properties
        public ObservableCollection<QuestionDto> Questions
        {
            get => _questions;
            private set { _questions = value; OnPropertyChanged(); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        public RelayCommand LoadQuestionsCommand { get; private set; } = null!;
        public RelayCommand AddQuestionCommand { get; private set; } = null!;
        public RelayCommand<QuestionDto?> EditQuestionCommand { get; private set; } = null!;
        public RelayCommand<QuestionDto?> DeleteQuestionCommand { get; private set; } = null!;
        public RelayCommand CloseCommand { get; private set; } = null!;

        private void InitializeCommands()
        {
            LoadQuestionsCommand = new RelayCommand(async () => await LoadQuestionsAsync());
            AddQuestionCommand = new RelayCommand(async () => await AddQuestionAsync());
            EditQuestionCommand = new RelayCommand<QuestionDto?>(async (q) => { if (q != null) await EditQuestionAsync(q); });
            DeleteQuestionCommand = new RelayCommand<QuestionDto?>(async (q) => { if (q != null) await DeleteQuestionAsync(q); });
            CloseCommand = new RelayCommand(() => CloseAction?.Invoke(true));
        }
        #endregion

        #region Methods
        private async Task LoadQuestionsAsync()
        {
            IsLoading = true;
            try
            {
                var questions = await _questionService.GetAllAsync();
                Questions = new ObservableCollection<QuestionDto>(questions);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading questions: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                var view = new AddQuestionView(_categoryService)
                {
                    Owner = Application.Current.MainWindow
                };

                var result = view.ShowDialog();
                if (result != true || !view.IsSaved) return;

                var newQuestion = view.Question as QuestionDto;
                var newOptions = view.Options.Cast<OptionDto>().ToList();

                await _questionService.CreateAsync(newQuestion);
                // optional: handle options separately if needed

                MessageBox.Show("Question created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadQuestionsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating question: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task EditQuestionAsync(QuestionDto question)
        {
            try
            {
                var view = new AddQuestionView(_categoryService, question)
                {
                    Owner = Application.Current.MainWindow
                };

                var result = view.ShowDialog();
                if (result != true || !view.IsSaved) return;

                var updatedQuestion = view.Question as QuestionDto;
                await _questionService.UpdateAsync(updatedQuestion);

                MessageBox.Show("Question updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadQuestionsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating question: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteQuestionAsync(QuestionDto question)
        {
            if (MessageBox.Show($"Delete this question?\n\n\"{question.Text}\"",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            try
            {
                await _questionService.DeleteAsync(question.Id);
                Questions.Remove(question);
                MessageBox.Show("Question deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting question: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        public Action<bool?>? CloseAction { get; set; }
    }
}
