using System.Globalization;
using System.Windows.Data;

public class QuestionProgressConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int currentIndex && parameter is int totalCount)
        {
            return $" (Question {currentIndex + 1} of {totalCount})";
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}