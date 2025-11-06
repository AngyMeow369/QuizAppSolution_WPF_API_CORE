using QuizApp.WPF.Services;
using QuizApp.WPF.ViewModels.Admin;
using System.Windows;

namespace QuizApp.WPF.Views.Admin
{
    public partial class CategoriesManagementView : Window
    {
        public CategoriesManagementView(CategoryService categoryService)
        {
            InitializeComponent();

            // Create ViewModel and set DataContext
            var viewModel = new CategoriesManagementViewModel(categoryService);
            DataContext = viewModel;

            // Handle window closing through ViewModel
            viewModel.CloseAction = (result) =>
            {
                DialogResult = result;
                Close();
            };
        }
    }
}