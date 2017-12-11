using MyLibrary.BL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

namespace MyLibrary
{
    /// <summary>
    /// Logic for ItemCopies.xaml
    /// </summary>
    public partial class ItemCopiesWindow : Window
    {
        private class ItemCopyViewModel
        {
            public uint CopyNo { get; set; }
            public bool IsBorrowed { get; set; }
            public DateTime? BorrowingDate { get; set; }
            public double? BorrowingPeriod { get; set; }
        }

        private LibraryItem _item;
        private SortedList<uint, ItemCopyViewModel> _itemCopiesViewModel;
        private User _currentUser;

        public ItemCopiesWindow(LibraryItem item, User currentUser)
        {
            _item = item;
            _currentUser = currentUser;

            InitializeComponent();
            InitViewModel();
            InitUIElements();
        }

        public string ItemTitle { get { return _item.Title; } }
        public string BorrowedVsTotal
        {
            get
            {
                return $"{_item.Copies.Values.Where(c => c.IsBorrowed).Count()} / {_item.Copies.Count()}";
            }
        }

        private void InitViewModel()
        {
            _itemCopiesViewModel = new SortedList<uint, ItemCopyViewModel>();

            foreach (var item in _item.Copies.Values)
            {
                _itemCopiesViewModel.Add(
                    item.CopyNo,
                    new ItemCopyViewModel
                    {
                        CopyNo = item.CopyNo,
                        IsBorrowed = item.IsBorrowed,
                        BorrowingDate = item.BorrowingDate,
                        BorrowingPeriod = item.BorrowingPeriod
                    }
                );
            }
        }

        private void InitUIElements()
        {
            Title = $"{_item.GetType().Name} Copies";
            btnSave.IsEnabled = false;
            grdHeader.DataContext = this;
            dgCopies.ItemsSource = _itemCopiesViewModel.Values.ToList();
            CollectionViewSource.GetDefaultView(dgCopies.Items).CollectionChanged
                += ItemCopiesWindow_CollectionChanged;
            SetToolBarsAvailability();
        }

        private void ItemCopiesWindow_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(!(sender is CheckBox))
                UIElement_Changed(sender, e);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var copyNo = _itemCopiesViewModel.Keys.LastOrDefault() + 1;
            _itemCopiesViewModel.Add(copyNo, new ItemCopyViewModel());
            dgCopies.ItemsSource = _itemCopiesViewModel.Values.ToList();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            _itemCopiesViewModel.Remove((dgCopies.SelectedItem as ItemCopyViewModel).CopyNo);
            dgCopies.ItemsSource = _itemCopiesViewModel.Values.ToList();
        }

        private void SetToolBarsAvailability()
        {
            btnAdd.IsEnabled = 
                (Permissions.GetUserPermissions<LibraryItem>(_currentUser) & OperationType.Add) > 0;
            dgCopies_SelectionChanged(this, null);
        }

        private void dgCopies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCopies.SelectedItem != null)
                btnDelete.IsEnabled =
                    (Permissions.GetUserPermissions<LibraryItem>(_currentUser) & OperationType.Delete) > 0;
            else
                btnDelete.IsEnabled = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            GUIServices.SetBusyState();

            SaveDeletedCopies();
            SaveNewAndUpdatedCopies();

            btnSave.IsEnabled = false;
            lblBorrowedVsTotal.Content = BorrowedVsTotal;

            dgCopies.Items.Refresh();
            MainWindow.RefreshView();
        }

        private void SaveNewAndUpdatedCopies()
        {
            foreach (var item in _itemCopiesViewModel.Values)
            {
                if (item.CopyNo == 0)    // => New Copy
                {
                    var newCopy = _item.AddCopy();
                    item.CopyNo = newCopy.CopyNo;
                }

                if (item.IsBorrowed != _item.Copies[item.CopyNo].IsBorrowed)
                {
                    if (item.IsBorrowed)
                        _item.BorrowCopy(item.CopyNo);
                    else
                        _item.ReturnCopy(item.CopyNo);

                    item.BorrowingDate = _item.Copies[item.CopyNo].BorrowingDate;
                    item.BorrowingPeriod = _item.Copies[item.CopyNo].BorrowingPeriod;
                }
            }
        }

        private void SaveDeletedCopies()
        {
            foreach (var itemCopy in _item.Copies.Values.ToList())
            {
                ItemCopyViewModel itemCopyViewModel;
                if (!_itemCopiesViewModel.TryGetValue(itemCopy.CopyNo, out itemCopyViewModel))   // => Deleted Copy
                {
                    _item.DeleteCopy(itemCopy.CopyNo);
                }
            }
        }

        private void UIElement_Changed(object sender, EventArgs e)
        {
            if(sender is CheckBox)
            {
                foreach (var item in _itemCopiesViewModel.Values)
                {
                    if (item.CopyNo == 0 ||
                        item.IsBorrowed != _item.Copies[item.CopyNo].IsBorrowed)
                    {
                        btnSave.IsEnabled = true;
                        return;
                    }
                }
            }

            if(_itemCopiesViewModel.Values.Any(i => i.CopyNo == 0) ||   // => New Copy(s)
                _itemCopiesViewModel.Count() != _item.Copies.Count())   // => Deleted Copy(s)
                btnSave.IsEnabled = true;
            else
                btnSave.IsEnabled = false;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (btnSave.IsEnabled)
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
