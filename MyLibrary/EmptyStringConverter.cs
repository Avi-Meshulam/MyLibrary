using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MyLibrary
{
    class EmptyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value?.GetType().BaseType != typeof(string) || targetType != typeof(string))
            //    return value;

            string strValue = value?.ToString();
            return string.IsNullOrEmpty(strValue) ? GUIUtils.EMPTY_STRING : strValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value?.GetType() != typeof(string) || targetType.BaseType != typeof(string))
            //    return value;

            string strValue = value.ToString();
            return strValue == GUIUtils.EMPTY_STRING ? string.Empty : strValue;
        }
    }
}