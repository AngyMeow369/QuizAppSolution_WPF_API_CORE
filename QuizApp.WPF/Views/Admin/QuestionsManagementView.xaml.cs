using QuizApp.API.Models;
using QuizApp.WPF.Services;
using QuizApp.WPF.ViewModels.Admin;
using System.Windows;

namespace QuizApp.WPF.Views.Admin
{
    public partial class QuestionsManagementView : Window
    {
        public QuestionsManagementView(QuestionService questionService, AuthService authService)
        {
            InitializeComponent();

            // Create CategoryService instance
            var categoryService = new CategoryService(authService);

            // Create ViewModel and set DataContext
            var viewModel = new QuestionsManagementViewModel(questionService, categoryService);
            DataContext = viewModel;

            // Handle window closing through ViewModel
            
        }
    }
}