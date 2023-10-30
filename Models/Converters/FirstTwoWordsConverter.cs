using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BCSH2SemestralniPraceCermakPetr.Models.Converters
{
    public class FirstTwoWordsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                // Use a regular expression to split by space or line breaks
                string[] words = Regex.Split(text, @" |\r\n|\n");
                if (words.Length >= 2)
                {
                    return $"{words[0]} {words[1]}"; // Combine the first two words
                }
            }
            return value; // Return the original value if it can't be split
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
