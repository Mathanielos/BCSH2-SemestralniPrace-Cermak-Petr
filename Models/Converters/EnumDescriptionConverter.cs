using Avalonia.Data.Converters;
using BCSH2SemestralniPraceCermakPetr.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCSH2SemestralniPraceCermakPetr.Models.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CountryName country)
            {
                var fieldInfo = country.GetType().GetField(country.ToString());
                var descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (descriptionAttributes.Length > 0)
                {
                    return ((DescriptionAttribute)descriptionAttributes[0]).Description;
                }
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
