using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyLibrary
{
    public class ReadOnlyContainer
    {
        private static string[] _properties = 
            { "IsHitTestVisible", "Focusable", "IsReadOnly", "IsEditable" };

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.RegisterAttached(
                "IsReadOnly", typeof(bool), typeof(ReadOnlyContainer),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.Inherits, ReadOnlyPropertyChanged));

        public static bool GetIsReadOnly(DependencyObject o)
        {
            return (bool)o.GetValue(IsReadOnlyProperty);
        }

        public static void SetIsReadOnly(DependencyObject o, bool value)
        {
            o.SetValue(IsReadOnlyProperty, value);
        }

        private static void ReadOnlyPropertyChanged(
            DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            PropertyInfo propInfo;

            bool newValue = (bool)e.NewValue;

            foreach (var propertyName in _properties)
            {
                if ((propInfo = dependencyObject.GetType().GetProperty(propertyName)) != null)
                    propInfo.SetValue(dependencyObject, propertyName == "IsReadOnly" ? newValue : !newValue);
            }

            //if (dependencyObject is TextBox)
            //{
            //    var textBox = dependencyObject as TextBox;
            //    textBox.IsReadOnly = isReadOnly;
            //    textBox.IsHitTestVisible = !isReadOnly;
            //    textBox.Focusable = !isReadOnly;
            //}
            //else if (dependencyObject is TextBlock)
            //{
            //    var textBlock = dependencyObject as TextBlock;
            //    textBlock.IsHitTestVisible = !isReadOnly;
            //    textBlock.Focusable = !isReadOnly;
            //}
            //else if (dependencyObject is ComboBox)
            //{
            //    var comboBox = dependencyObject as ComboBox;
            //    comboBox.IsEditable = !isReadOnly;
            //    comboBox.IsHitTestVisible = !isReadOnly;
            //    comboBox.Focusable = !isReadOnly;
            //}
            //else if (dependencyObject is ListBox)
            //{
            //    var listBox = dependencyObject as ListBox;
            //    listBox.IsHitTestVisible = !isReadOnly;
            //    listBox.Focusable = !isReadOnly;
            //}
            //else if (dependencyObject is CheckBox)
            //{
            //    var checkBox = dependencyObject as CheckBox;
            //    checkBox.IsHitTestVisible = !isReadOnly;
            //    checkBox.Focusable = !isReadOnly;
            //}
            //else if (dependencyObject is RadioButton)
            //{
            //    var radioButton = dependencyObject as RadioButton;
            //    radioButton.IsHitTestVisible = !isReadOnly;
            //    radioButton.Focusable = !isReadOnly;
            //}
        }
    }
}
