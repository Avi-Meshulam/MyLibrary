using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MyLibrary
{
    /// <summary>
    /// Converts enum values in a combo box
    /// </summary>
    class EnumComboConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value?.GetType().BaseType != typeof(Enum) || targetType != typeof(string))
                return value;

            return $"{(int)value} ({value.ToString().SplitCamelCase()})";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value?.GetType() != typeof(string) || targetType.BaseType != typeof(Enum))
                return value;

            string str = value as string;
            value = str.Substring(str.IndexOf(" ") + 1).Replace(" ", string.Empty);

            value = TypeDescriptor.GetConverter(targetType).ConvertFrom(value);

            return value;
        }
    }
}
