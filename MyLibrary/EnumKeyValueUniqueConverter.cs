using MyLibrary.BL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace MyLibrary
{
    class EnumKeyValueUniqueConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts a value of a KeyValuePair&lt;Enum, Enum&gt; (values[0]) to a unique string, 
        /// while comparing it to provided list (values[2]) and adding key when necessary.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string key = string.Empty;

            if (values[0].GetType().Name == "NamedObject")
                return values[0];

            var item = (KeyValuePair<Enum, Enum>)values[0];
            var list = (values[1] as IEnumerable).OfType<KeyValuePair<Enum, Enum>>().ToList();

            if (item.Value != null && 
                (item.Value.GetType().BaseType != typeof(Enum)/* || targetType != typeof(string)*/))
                return item;

            bool isDuplicate = list.Where(i => i.Value.ToString() == item.Value.ToString()).Count() > 1;
            
            if (isDuplicate)
                key = $" ({item.Key.ToString().SplitCamelCase()})";

            if ((EMPTY_ENUM)item.Value == EMPTY_ENUM._Empty_)
                return $"{GUIUtils.EMPTY_STRING}{key}";
            else
                return $"{item.Value.ToString().SplitCamelCase()}{key}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
