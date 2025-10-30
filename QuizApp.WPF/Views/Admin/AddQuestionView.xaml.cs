using QuizApp.API.Models;
using QuizApp.WPF.Services;
using QuizApp.WPF.ViewModels.Admin;
using System.Collections.ObjectModel;
using System.Windows;

namespace QuizApp.WPF.Views.Admin
{
    public partial class AddQuestionView : Window
    {
        private readonly AddQuestionViewModel _viewModel;

        public AddQuestionView(CategoryService categoryService, Question? existingQuestion = null)
        {
            InitializeComponent();

            // Create ViewModel and set DataContext
            _viewModel = new AddQuestionViewModel(categoryService, existingQuestion);
            DataContext = _viewModel;

            // Handle window closing through ViewModel
            _viewModel.CloseAction = (result) =>
            {
                DialogResult = result;
                Close();
            };
        }

        // Expose ViewModel properties to parent ViewModel
        public bool IsSaved => _viewModel.IsSaved;
        public Question Question => _viewModel.Question;
        public ObservableCollection<Option> Options => _viewModel.Options;
    }
}
