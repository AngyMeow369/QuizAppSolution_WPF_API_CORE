using QuizApp.Shared.DTOs;
using QuizApp.WPF.Services;
using QuizApp.WPF.ViewModels.Admin;
using System.Windows;

namespace QuizApp.WPF.Views.Admin
{
    public partial class AddQuestionView : Window
    {
        public bool IsSaved => (DataContext as AddQuestionViewModel)?.IsSaved ?? false;
        public QuestionDto Question => (DataContext as AddQuestionViewModel)?.Question ?? new();
        public IEnumerable<OptionDto> Options => (DataContext as AddQuestionViewModel)?.Options ?? [];

        public AddQuestionView(CategoryService categoryService, QuestionDto? question = null)
        {
            InitializeComponent();
            var vm = new AddQuestionViewModel(categoryService, question);
            vm.CloseAction = DialogResultSetter;
            DataContext = vm;
        }

        private void DialogResultSetter(bool? result)
        {
            DialogResult = result;
            Close();
        }
    }
}
