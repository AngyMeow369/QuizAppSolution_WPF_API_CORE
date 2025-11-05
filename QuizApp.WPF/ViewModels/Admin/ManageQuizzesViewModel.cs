using QuizApp.API.Models;
using QuizApp.WPF.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace QuizApp.WPF.ViewModels.Admin
{
    public class ManageQuizzesViewModel : ObservableObject
    {
        private readonly QuizService _quizService;


    public ObservableCollection<Quiz> Quizzes { get; set; } = new ObservableCollection<Quiz>();

        private Quiz? _selectedQuiz;
        public Quiz? SelectedQuiz
        {
            get => _selectedQuiz;
            set => SetProperty(ref _selectedQuiz, value);
        }


        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Commands
        public ICommand AddQuizCommand { get; }
        public ICommand EditQuizCommand { get; }
        public ICommand DeleteQuizCommand { get; }
        public ICommand AssignQuizCommand { get; }

        public ManageQuizzesViewModel(QuizService quizService)
        {
            _quizService = quizService;

            AddQuizCommand = new RelayCommand(OnAddQuiz);
            EditQuizCommand = new RelayCommand(OnEditQuiz, () => SelectedQuiz != null);
            DeleteQuizCommand = new RelayCommand(async () => await OnDeleteQuiz(), () => SelectedQuiz != null);
            AssignQuizCommand = new RelayCommand(OnAssignQuiz, () => SelectedQuiz != null);

            _ = LoadQuizzes();
        }

        private async Task LoadQuizzes()
        {
            try
            {
                IsLoading = true;
                var quizzes = await _quizService.GetQuizzesAsync();
                Quizzes.Clear();
                foreach (var quiz in quizzes)
                    Quizzes.Add(quiz);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to load quizzes: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OnAddQuiz()
        {
            // TODO: Open AddQuiz dialog or navigate
            MessageBox.Show("AddQuiz clicked");
        }

        private void OnEditQuiz()
        {
            if (SelectedQuiz == null) return;
            // TODO: Open EditQuiz dialog
            MessageBox.Show($"Edit Quiz: {SelectedQuiz.Title}");
        }

        private async Task OnDeleteQuiz()
        {
            if (SelectedQuiz == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete '{SelectedQuiz.Title}'?", "Confirm Delete", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                await _quizService.DeleteQuizAsync(SelectedQuiz.Id);
                Quizzes.Remove(SelectedQuiz);
                SelectedQuiz = null;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to delete quiz: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OnAssignQuiz()
        {
            if (SelectedQuiz == null) return;
            // TODO: Open AssignQuiz dialog
            MessageBox.Show($"Assign Quiz: {SelectedQuiz.Title}");
        }
    }


}
