using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace MyLibrary
{
    class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value?.GetType().BaseType != typeof(Enum) /*|| targetType != typeof(string)*/)
                return value;

            return value.ToString().SplitCamelCase().Replace("_", " @");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (/*value?.GetType() != typeof(string) || */targetType.BaseType != typeof(Enum))
                return value;

            return TypeDescriptor.GetConverter(targetType)
                .ConvertFrom(value.ToString().Replace(" ", string.Empty).Replace('@', '_'));
        }
    }
}