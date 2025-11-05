using System;
using System.Globalization;
using System.Windows.Data;

namespace QuizApp.WPF.Converters
{
    public class ScoreToPercentageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 &&
                values[0] is int score &&
                values[1] is int totalQuestions &&
                totalQuestions > 0)
            {
                return Math.Round((score * 100.0) / totalQuestions, 1);
            }

            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
