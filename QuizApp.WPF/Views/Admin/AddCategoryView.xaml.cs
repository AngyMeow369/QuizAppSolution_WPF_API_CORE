using System.Windows;

namespace QuizApp.WPF.Views.Admin
{
    public partial class AddCategoryView : Window
    {
        public string CategoryName { get; private set; } = string.Empty;
        //public string Description { get; private set; } = string.Empty;
        public bool IsSaved { get; private set; }

        public AddCategoryView()
        {
            InitializeComponent();
            CategoryNameTextBox.Focus();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CategoryNameTextBox.Text))
            {
                MessageBox.Show("Please enter a category name.", "Validation Error",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CategoryName = CategoryNameTextBox.Text.Trim();
            //Description = DescriptionTextBox.Text.Trim();
            IsSaved = true;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}