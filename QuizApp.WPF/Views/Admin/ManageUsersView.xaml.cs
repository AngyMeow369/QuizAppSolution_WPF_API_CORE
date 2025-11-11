using QuizApp.WPF.Services;
using QuizApp.WPF.Services.Interfaces;
using QuizApp.WPF.ViewModels.Admin;
using System.Windows.Controls;

namespace QuizApp.WPF.Views.Admin
{
    public partial class ManageUsersView : UserControl
    {
        // Parameterless for XAML/designer
        public ManageUsersView()
        {
            InitializeComponent();
        }

        // Constructor with UserService only
        public ManageUsersView(UserService userService)
        {
            InitializeComponent();
            DataContext = new ManageUsersViewModel(userService);
        }

        // Original constructor with full DI
        public ManageUsersView(IUserApi userApi, IQuizApi quizApi, IAuthService authService)
        {
            InitializeComponent();
            var userService = new UserService(authService);
            DataContext = new ManageUsersViewModel(userService);
        }
    }

}
