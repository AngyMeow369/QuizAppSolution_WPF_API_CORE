using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services;
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
        private bool _isAddQuestionVisible;

        public QuestionsManagementViewModel(QuestionService questionService, CategoryService categoryService)
        {
            _questionService = questionService ?? throw new ArgumentNullException(nameof(questionService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));

            AddQuestionVM = new AddQuestionViewModel(_categoryService);
            AddQuestionVM.CloseAction = (result) =>
            {
                IsAddQuestionVisible = false; // hide overlay
                if (result == true) _ = LoadQuestionsAsync(); // reload if saved
            };

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

        public bool IsAddQuestionVisible
        {
            get => _isAddQuestionVisible;
            set { _isAddQuestionVisible = value; OnPropertyChanged(); }
        }

        public AddQuestionViewModel AddQuestionVM { get; }
        #endregion

        #region Commands
        public RelayCommand LoadQuestionsCommand { get; private set; } = default!;
        public RelayCommand ShowAddQuestionCommand { get; private set; } = default!;
        public RelayCommand<QuestionDto?> EditQuestionCommand { get; private set; } = default!;
        public RelayCommand<QuestionDto?> DeleteQuestionCommand { get; private set; } = default!;


        private void InitializeCommands()
        {
            LoadQuestionsCommand = new RelayCommand(async () => await LoadQuestionsAsync());

            ShowAddQuestionCommand = new RelayCommand(() =>
            {
                AddQuestionVM.Reset();
                IsAddQuestionVisible = true;
            });

            EditQuestionCommand = new RelayCommand<QuestionDto?>(q =>
            {
                if (q != null)
                {
                    AddQuestionVM.LoadQuestion(q);
                    IsAddQuestionVisible = true;
                }
            });

            DeleteQuestionCommand = new RelayCommand<QuestionDto?>(async q =>
            {
                if (q != null) await DeleteQuestionAsync(q);
            });
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
    }
}
