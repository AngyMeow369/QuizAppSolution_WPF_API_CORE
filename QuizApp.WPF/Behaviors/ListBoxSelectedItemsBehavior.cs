using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace QuizApp.WPF.Behaviors
{
    public static class ListBoxSelectedItemsBehavior
    {
        public static readonly DependencyProperty BindableSelectedItemsProperty =
            DependencyProperty.RegisterAttached(
                "BindableSelectedItems",
                typeof(IList),
                typeof(ListBoxSelectedItemsBehavior),
                new PropertyMetadata(null, OnBindableSelectedItemsChanged));

        public static void SetBindableSelectedItems(DependencyObject element, IList value)
        {
            element.SetValue(BindableSelectedItemsProperty, value);
        }

        public static IList GetBindableSelectedItems(DependencyObject element)
        {
            return (IList)element.GetValue(BindableSelectedItemsProperty);
        }

        private static void OnBindableSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ListBox listBox) return;

            listBox.SelectionChanged -= ListBox_SelectionChanged;

            if (e.NewValue is IList newList)
            {
                listBox.SelectedItems.Clear();
                foreach (var item in newList)
                    listBox.SelectedItems.Add(item);

                listBox.SelectionChanged += ListBox_SelectionChanged;
            }
        }

        private static void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ListBox listBox) return;
            var selectedItems = GetBindableSelectedItems(listBox);
            if (selectedItems == null) return;

            selectedItems.Clear();
            foreach (var item in listBox.SelectedItems)
                selectedItems.Add(item);
        }
    }
}
