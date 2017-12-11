using MyLibrary.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections;

namespace MyLibrary
{
    public abstract class EntitiesWindow<T> : Window where T : Entity<T>
    {
        protected static CollectionViewSource EntitiesViewSource;
        protected EntityManager<T> EntitiesManager;

        private IReadOnlyList<T> _entities;
        protected IReadOnlyList<T> Entities
        {
            get { return _entities; }
            set
            {
                _entities = value;
                if (value == null)
                {
                    EntitiesViewSource = null;
                    DataGrid.ItemsSource = null;
                    return;
                }
                EntitiesViewSource = new CollectionViewSource();
                IEnumerable<T> query = typeof(T) == typeof(LibraryItem) ?
                    _entities.OrderBy(e => (e as LibraryItem).Title, new NaturalStringComparer())
                    : _entities.OrderBy(e => e.EntityId());
                EntitiesViewSource.Source = query.ToList();
                DataGrid.ItemsSource = EntitiesViewSource?.View;
            }
        }

        protected void InitEntities(EntityManager<T> entitiesManager)
        {
            EntitiesManager = entitiesManager;
            EntitiesManager.CollectionChanged += EntitiesManager_CollectionChanged;
            Entities = EntitiesManager.Search();
        }

        protected virtual void EntitiesManager_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Entities = EntitiesManager.Search();
            if (e.NewItems?.Count == 1)
                EntitiesViewSource.View.MoveCurrentTo(e.NewItems[0]);
        }

        protected abstract Button AddButton { get; }
        protected abstract Button EditButton { get; }
        protected abstract Button DeleteButton { get; }
        protected abstract DataGrid DataGrid { get; }

        public static object CurrentItem => EntitiesViewSource?.View?.CurrentItem;

        public static void RefreshView()
        {
            EntitiesViewSource?.View?.Refresh();
        }

        protected virtual void AddButton_Click(Type entityWindowType)
        {
            var entityWindow = (EntityWindow<T>)Activator.CreateInstance(entityWindowType, EntitiesManager, null);
            entityWindow.ShowDialog();
        }

        protected virtual void EditButton_Click(Type entityWindowType)
        {
            if (DataGrid.SelectedItem == null)
                return;

            var entityWindow = (EntityWindow<T>)Activator.CreateInstance(
                entityWindowType, EntitiesManager, DataGrid.SelectedItem as T);

            entityWindow.ShowDialog();
        }

        protected virtual void DeleteButton_Click()
        {
            if (DataGrid.SelectedItem == null) return;

            string message = null;
            bool closeOnSuccess = false;

            if (typeof(T) == typeof(Publisher))
                message = $"Are you sure you want to delete publisher '{DataGrid.SelectedItem as Publisher}'?\n\n"
                    + "WARNING! Deleting a publisher will delete all its publications!";
            else if (typeof(T) == typeof(Employee))
                message = $"Are you sure you want to delete employee {DataGrid.SelectedItem as Employee}?\n\n"
                    + "WARNING! Deleting an employee will delete all his user accounts!";
            else if (typeof(T) == typeof(User))
            {
                var user = DataGrid.SelectedItem as User;
                if (user.Equals(EntitiesManager.CurrentUser))
                {
                    closeOnSuccess = true;
                    message = $"Are you sure you want to delete the current logged-in user? (implies logout)";
                }
                else
                    message = $"Are you sure you want to delete user '{DataGrid.SelectedItem as User}'?";
            }
            else if (typeof(T) == typeof(LibraryItem))
                message = $"Are you sure you want to delete {DataGrid.SelectedItem as LibraryItem}?";
            else
                message = $"Are you sure you want to delete selected {typeof(T).Name} (Id = {(DataGrid.SelectedItem as Entity<T>).EntityId()})?";

            MessageBoxResult result = GUIUtils.ShowMessage(message, MessageBoxType.WarningQuestion);
            if (result.In(MessageBoxResult.Cancel, MessageBoxResult.No))
                return;

            if(EntitiesManager.Delete(DataGrid.SelectedItem as T) && closeOnSuccess)
                Close();
        }

        protected virtual void DataGrid_ID_Click(object sender, Type entityWindowType)
        {
            var entity = (sender as FrameworkElement).DataContext as T;
            var entityWindow = (EntityWindow<T>)Activator.CreateInstance(entityWindowType, EntitiesManager, entity);
            entityWindow.ShowDialog();
        }

        protected virtual void DataGrid_SelectionChanged()
        {
            if (DataGrid.SelectedItem != null)
            {
                EditButton.IsEnabled = (Permissions.GetUserPermissions<T>(EntitiesManager.CurrentUser) & OperationType.Edit) > 0;
                DeleteButton.IsEnabled = (Permissions.GetUserPermissions<T>(EntitiesManager.CurrentUser) & OperationType.Delete) > 0;
            }
            else
            {
                EditButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
            }
        }

        protected virtual void InitUI()
        {
            AddButton.IsEnabled = EntitiesManager?.CurrentUser == null ? false
                : (Permissions.GetUserPermissions<T>(EntitiesManager.CurrentUser) & OperationType.Add) > 0;
            // The purpose of the following statement is to set buttons availability
            DataGrid_SelectionChanged();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (EntitiesManager != null)
                EntitiesManager.CollectionChanged -= EntitiesManager_CollectionChanged;
        }
    }
}
