using MyLibrary.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace MyLibrary
{
    public abstract class EntityWindow<T> : Window where T : Entity<T>
    {
        protected EntityManager<T> _entitiesManager;
        protected T _entity;

        public EntityWindow(EntityManager<T> entitiesManager, T entity = null)
        {
            _entitiesManager = entitiesManager;
            _entity = entity;
        }

        protected abstract Button SaveButton { get; }
        protected abstract bool IsValid();
        protected abstract void DisplayValidationMessage();

        public bool IsReadOnly { get { return _entitiesManager.IsReadOnly; } }

        protected virtual void InitUIElements(bool isFirstTime = false)
        {
            SaveButton.IsEnabled = false;

            if (_entity != null)
            {
                DataContext = _entity;
                Title = $"{_entity.Type.Name} Details";
            }
            else
            {
                Title = $"Add {typeof(T).Name.SplitCamelCase()}";
            }
        }

        protected void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Save())
            {
                if(EntitiesWindow<T>.CurrentItem != null)
                    _entity = EntitiesWindow<T>.CurrentItem as T;
                InitUIElements();
                EntitiesWindow<T>.RefreshView();
            }
        }

        protected virtual bool Save()
        {
            GUIServices.SetBusyState();

            if (!IsValid())
            {
                DisplayValidationMessage();
                return false;
            }

            return true;
        }

        protected virtual void UIElement_Changed(object sender, RoutedEventArgs e)
        {
            if (!IsValid() || IsReadOnly)
            {
                SaveButton.IsEnabled = false;
                e.Handled = true;
                return;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (SaveButton.IsEnabled)
            {
                MessageBoxResult result =
                    GUIUtils.ShowMessage("Are you sure you want to leave without saving?",
                    MessageBoxType.Question);

                if (result.In(MessageBoxResult.Cancel, MessageBoxResult.No))
                    e.Cancel = true;
            }
        }
    }
}
