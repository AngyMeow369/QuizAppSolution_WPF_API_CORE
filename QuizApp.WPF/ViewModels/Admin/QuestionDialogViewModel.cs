using QuizApp.Shared.DTOs;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class QuestionDialogViewModel : BaseViewModel
    {
        public QuestionViewModel Question { get; }

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddOptionCommand { get; }
        public ICommand DeleteOptionCommand { get; }

        // Event to notify parent dialog
        public event Action<bool?>? CloseRequested;

        public QuestionDialogViewModel(QuestionViewModel question)
        {
            Question = question ?? throw new ArgumentNullException(nameof(question));

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            AddOptionCommand = new RelayCommand(AddOption);
            DeleteOptionCommand = new RelayCommand<OptionDto>(DeleteOption, o => o != null);
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(Question.Text))
            {
                MessageBox.Show("Question text cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Question.Options.Count == 0)
            {
                MessageBox.Show("Add at least one option.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CloseRequested?.Invoke(true);
        }

        private void Cancel()
        {
            CloseRequested?.Invoke(false);
        }

        private void AddOption()
        {
            Question.Options.Add(new OptionDto
            {
                Text = "Options",
                IsCorrect = false
            });
            OnPropertyChanged(nameof(Question));
        }

        private void DeleteOption(OptionDto? option)
        {
            if (option == null) return;

            Question.Options.Remove(option);
            OnPropertyChanged(nameof(Question));
        }
    }
}
