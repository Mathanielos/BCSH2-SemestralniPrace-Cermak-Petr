using Avalonia.Data.Converters;
using BCSH2SemestralniPraceCermakPetr.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace BCSH2SemestralniPraceCermakPetr.Models.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.GetType().IsEnum)
            {
                var fieldInfo = value.GetType().GetField(value.ToString());
                var descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (descriptionAttributes.Length > 0)
                {
                    return ((DescriptionAttribute)descriptionAttributes[0]).Description;
                }
            }

            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string description && targetType.IsEnum)
            {
                var enumType = targetType;
                foreach (var field in enumType.GetFields())
                {
                    var descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

                    if (descriptionAttribute != null && descriptionAttribute.Description == description)
                    {
                        return field.GetValue(null); // Found the matching enum value
                    }
                }
            }

            // If no match is found
            throw new ArgumentException("Invalid description");
        }
    }
}
