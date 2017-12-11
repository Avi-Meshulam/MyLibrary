using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyLibrary
{
    /// <summary>
    /// Handles logic of an editable combo box, while allowing data filtering
    /// </summary>
    class EditableComboBox : ComboBox
    {
        private TextBox _editableTextBox;

        public int SelectionStart
        {
            get { return _editableTextBox.SelectionStart; }
            set { _editableTextBox.SelectionStart = value; }
        }

        public int SelectionLength
        {
            get { return _editableTextBox.SelectionLength; }
            set { _editableTextBox.SelectionLength = value; }
        }

        public override void OnApplyTemplate()
        {
            var myTextBox = GetTemplateChild("PART_EditableTextBox") as TextBox;
            if (myTextBox != null)
            {
                _editableTextBox = myTextBox;
            }

            base.OnApplyTemplate();
        }

        public void SetCaret(int position)
        {
            _editableTextBox.SelectionStart = position;
            _editableTextBox.SelectionLength = 0;
        }

        public void PreviewKeyUpHandler(List<string> items, KeyEventArgs e)
        {
            // Exit in case of a control key
            if ((e.Key < Key.D0 && !e.Key.In(Key.Back, Key.Delete)) || e.Key > Key.Divide)
            {
                if (e.Key.In(Key.Up, Key.Down))
                    IsDropDownOpen = true;

                return;
            }

            if (Text == string.Empty)
            {
                ItemsSource = items;
                SelectedIndex = -1;
                IsDropDownOpen = false;
                return;
            }

            string comboBox_Text = Text;
            int selectionStart = SelectionStart;

            ItemsSource = items.Where(s => s.ToLower().Contains(Text.ToLower())).ToList();

            IsDropDownOpen = false;

            if (Items.Count > 0)
                IsDropDownOpen = true;

            Text = comboBox_Text;
            SetCaret(selectionStart);
        }
    }
}
