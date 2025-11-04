using System;
using System.Globalization;
using System.Windows.Data;

namespace QuizApp.WPF.Converters
{
    public class ScoreToPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int score)
            {
                if (parameter is int totalQuestions && totalQuestions > 0)
                {
                    return Math.Round((score * 100.0) / totalQuestions, 1);
                }
                return score;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}