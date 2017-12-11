using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MyLibrary
{
    /// <summary>
    /// Allows custom sort of entities that implement IComparer
    /// </summary>
    class DataGridSortBehavior
    {
        public static readonly DependencyProperty SorterProperty =
            DependencyProperty.RegisterAttached("Sorter", typeof(IComparer), typeof(DataGridSortBehavior));

        public static readonly DependencyProperty AllowCustomSortProperty =
            DependencyProperty.RegisterAttached("AllowCustomSort", typeof(bool),
            typeof(DataGridSortBehavior), new UIPropertyMetadata(false, OnAllowCustomSortChanged));

        public static IComparer GetSorter(DataGridColumn column)
        {
            return (IComparer)column.GetValue(SorterProperty);
        }

        public static void SetSorter(DataGridColumn column, IComparer value)
        {
            column.SetValue(SorterProperty, value);
        }

        public static bool GetAllowCustomSort(DataGrid grid)
        {
            return (bool)grid.GetValue(AllowCustomSortProperty);
        }

        public static void SetAllowCustomSort(DataGrid grid, bool value)
        {
            grid.SetValue(AllowCustomSortProperty, value);
        }

        private static void OnAllowCustomSortChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var grid = (DataGrid)obj;

            bool oldAllow = (bool)e.OldValue;
            bool newAllow = (bool)e.NewValue;

            if (!oldAllow && newAllow)
            {
                grid.Sorting += HandleCustomSorting;
            }
            else
            {
                grid.Sorting -= HandleCustomSorting;
            }
        }

        public static bool ApplySort(DataGrid grid, DataGridColumn column)
        {
            IComparer sorter = GetSorter(column);
            if (sorter == null)
            {
                return false;
            }

            var listCollectionView = CollectionViewSource.GetDefaultView(grid.ItemsSource) as ListCollectionView;
            if (listCollectionView == null)
            {
                throw new Exception("The ICollectionView associated with the DataGrid must be of type, ListCollectionView");
            }

            listCollectionView.CustomSort = new DataGridSortComparer(
                sorter, column.SortDirection ?? ListSortDirection.Ascending, column.SortMemberPath);
            return true;
        }

        private static void HandleCustomSorting(object sender, DataGridSortingEventArgs e)
        {
            GUIServices.SetBusyState();

            IComparer sorter = GetSorter(e.Column);
            if (sorter == null)
            {
                return;
            }

            var grid = (DataGrid)sender;
            e.Column.SortDirection = e.Column.SortDirection == ListSortDirection.Ascending ?
                ListSortDirection.Descending : ListSortDirection.Ascending;
            if (ApplySort(grid, e.Column))
            {
                e.Handled = true;
            }
        }

        private class DataGridSortComparer : IComparer
        {
            private IComparer _comparer;
            private ListSortDirection _sortDirection;
            private string _propertyName;
            private List<PropertyInfo> _propertiesInfo = new List<PropertyInfo>();

            public DataGridSortComparer(IComparer comparer,
                ListSortDirection sortDirection, string propertyName)
            {
                _comparer = comparer;
                _sortDirection = sortDirection;
                _propertyName = propertyName;
            }

            // If SortMemberPath (propertyName) is comma delimited, 
            // the method splits the values and treats each as a separate string property
            public int Compare(object x, object y)
            {
                if (_propertiesInfo.Count == 0)
                {
                    InitalizePropertiesInfo(x, y);

                    if (_propertiesInfo.Count == 0)
                        return 0;
                }

                int result;

                foreach (var propInfo in _propertiesInfo)
                {
                    bool xHasProperty = x.HasProperty(propInfo.Name);
                    bool yHasProperty = y.HasProperty(propInfo.Name);

                    if (!xHasProperty || !yHasProperty)
                    {
                        if (xHasProperty && !yHasProperty) {
                            result = 1;
                            goto return_result;
                        }

                        if (!xHasProperty && yHasProperty) {
                            result = -1;
                            goto return_result;
                        }

                        return 0;
                    }
                }

                object value1, value2;

                // Multi-Column Sort
                if (_propertiesInfo.Count > 1)
                {
                    value1 = _propertiesInfo.Aggregate(string.Empty, (s, p) => s + p.GetValue(x));
                    value2 = _propertiesInfo.Aggregate(string.Empty, (s, p) => s + p.GetValue(y));
                }
                else // Single Column Sort
                {
                    value1 = _propertiesInfo[0]?.GetValue(x);
                    value2 = _propertiesInfo[0]?.GetValue(y);
                }

                result = _comparer.Compare(value1, value2);

            return_result:
                return _sortDirection == ListSortDirection.Ascending ? result : -result;
            }

            private void InitalizePropertiesInfo(object x, object y)
            {
                _propertiesInfo = new List<PropertyInfo>();
                foreach (var propName in _propertyName.Split(','))
                {
                    PropertyInfo propInfo = x.GetType().GetProperty(propName.Trim());
                    if(propInfo != null)
                        _propertiesInfo.Add(propInfo);
                    else
                    {
                        propInfo = y.GetType().GetProperty(propName.Trim());
                        if (propInfo != null)
                            _propertiesInfo.Add(propInfo);
                    }
                }
            }
        }
    }
}
