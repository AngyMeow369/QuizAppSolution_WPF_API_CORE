using QuizApp.API.Models;
using QuizApp.WPF.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;

namespace QuizApp.WPF.Views.Admin
{
    public partial class CategoriesManagementView : Window
    {
        private readonly CategoryService _categoryService;
        public ObservableCollection<Category> Categories { get; set; }

        public CategoriesManagementView(CategoryService categoryService)
        {
            _categoryService = categoryService;
            Categories = new ObservableCollection<Category>();

            InitializeComponent();
            LoadCategories();
        }

        private async Task LoadCategories()
        {
            try
            {
                var categories = await _categoryService.GetCategoriesAsync();
                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
                CategoriesItemsControl.ItemsSource = Categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewCategoryTextBox.Text))
            {
                MessageBox.Show("Please enter a category name.", "Validation Error",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newCategory = new Category
                {
                    Name = NewCategoryTextBox.Text.Trim()
                };

                var createdCategory = await _categoryService.CreateCategoryAsync(newCategory);
                Categories.Add(createdCategory);

                NewCategoryTextBox.Clear();
                MessageBox.Show($"Category '{createdCategory.Name}' created successfully!",
                              "Success",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating category: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var category = (Category)button.Tag;

            // Store the original category for reference
            var originalCategory = category;

            // Create a professional edit dialog
            var editDialog = new Window
            {
                Title = "Edit Category",
                Width = 500,
                Height = 550,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                Background = System.Windows.Media.Brushes.White,
                ResizeMode = ResizeMode.NoResize,
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI")
            };

            // Main container with shadow
            var mainBorder = new Border
            {
                Background = System.Windows.Media.Brushes.White,
                CornerRadius = new CornerRadius(12),
                Margin = new Thickness(20),
                Effect = new DropShadowEffect
                {
                    Color = System.Windows.Media.Colors.Black,
                    ShadowDepth = 0,
                    Opacity = 0.1,
                    BlurRadius = 20
                }
            };

            var grid = new Grid { Margin = new Thickness(25) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Header with icon
            var headerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 25)
            };

            var iconBorder = new Border
            {
                Width = 40,
                Height = 40,
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(219, 234, 254)),
                CornerRadius = new CornerRadius(10)
            };
            iconBorder.Child = new TextBlock
            {
                Text = "📚",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var headerTextPanel = new StackPanel { Margin = new Thickness(15, 0, 0, 0) };
            headerTextPanel.Children.Add(new TextBlock
            {
                Text = "Edit Category",
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(31, 41, 55))
            });
            headerTextPanel.Children.Add(new TextBlock
            {
                Text = "Update the category name",
                FontSize = 13,
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(107, 114, 128)),
                Margin = new Thickness(0, 4, 0, 0)
            });

            headerPanel.Children.Add(iconBorder);
            headerPanel.Children.Add(headerTextPanel);
            Grid.SetRow(headerPanel, 0);
            grid.Children.Add(headerPanel);

            // Form section
            var formPanel = new StackPanel();

            formPanel.Children.Add(new TextBlock
            {
                Text = "Category Name",
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(55, 65, 81)),
                Margin = new Thickness(0, 0, 0, 10)
            });

            var textBoxBorder = new Border
            {
                BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(229, 231, 235)),
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(6),
                Background = System.Windows.Media.Brushes.White,
                Height = 50
            };

            var textBox = new TextBox
            {
                Text = category.Name,
                FontSize = 14,
                Background = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(15),
                VerticalContentAlignment = VerticalAlignment.Center,
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(31, 41, 55)),
                CaretBrush = System.Windows.Media.Brushes.Black
            };

            // Add focus events for better UX
            textBox.GotFocus += (s, args) =>
            {
                textBoxBorder.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(37, 99, 235));
            };

            textBox.LostFocus += (s, args) =>
            {
                textBoxBorder.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(229, 231, 235));
            };

            textBoxBorder.Child = textBox;
            formPanel.Children.Add(textBoxBorder);

            Grid.SetRow(formPanel, 1);
            grid.Children.Add(formPanel);

            // Buttons
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 20, 0, 0)
            };

            var cancelButton = new Button
            {
                Content = "Cancel",
                Background = System.Windows.Media.Brushes.Transparent,
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(107, 114, 128)),
                BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(229, 231, 235)),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(15, 10, 15, 10),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Cursor = Cursors.Hand,
                Margin = new Thickness(0, 0, 10, 0)
            };
            cancelButton.Click += (s, args) => editDialog.DialogResult = false;

            var saveButton = new Button
            {
                Content = "Save Changes",
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(37, 99, 235)),
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(15, 10, 15, 10),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Cursor = Cursors.Hand
            };
            saveButton.Click += (s, args) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    MessageBox.Show("Please enter a category name.", "Validation Error",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                editDialog.DialogResult = true;
            };

            buttonPanel.Children.Add(cancelButton);
            buttonPanel.Children.Add(saveButton);
            Grid.SetRow(buttonPanel, 2);
            grid.Children.Add(buttonPanel);

            mainBorder.Child = grid;
            editDialog.Content = mainBorder;

            // Set focus to textbox and select all text
            textBox.Focus();
            textBox.SelectAll();

            var result = editDialog.ShowDialog();
            if (result == true && !string.IsNullOrWhiteSpace(textBox.Text))
            {
                try
                {
                    var originalName = originalCategory.Name;
                    var newName = textBox.Text.Trim();

                    // Create a new category object to ensure proper update
                    var updatedCategory = new Category
                    {
                        Id = originalCategory.Id,
                        Name = newName
                    };

                    await _categoryService.UpdateCategoryAsync(updatedCategory);

                    // Force a complete refresh of the categories list
                    await LoadCategories();

                    MessageBox.Show($"Category '{originalName}' updated to '{newName}' successfully!",
                                  "Success",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating category: {ex.Message}",
                                  "Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var category = (Category)button.Tag;

            var result = MessageBox.Show($"Are you sure you want to delete category '{category.Name}'?",
                                       "Confirm Delete",
                                       MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _categoryService.DeleteCategoryAsync(category.Id);
                    Categories.Remove(category);

                    MessageBox.Show($"Category '{category.Name}' deleted successfully!",
                                  "Success",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting category: {ex.Message}",
                                  "Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}