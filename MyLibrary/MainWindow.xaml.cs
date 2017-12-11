using System;
using System.Windows;
using System.Windows.Data;
using MyLibrary.BL;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MyLibrary
{
    enum FilterType
    {
        ISBNPrefix,
        ISBNGroup,
        ISBNPublisher,
        ISBNCatalogue,
        Category,
        SubCategory,
        Publisher,
        Author,
        TitleFilter,
        PrintDateFrom,
        PrintDateTo
    }

    /// <summary>
    /// Logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : EntitiesWindow<LibraryItem>
    {
        private EntityManager<Publisher> _publishersManager;
        private EntityManager<Employee> _employeesManager;
        private UsersManager _usersManager;
        private Dictionary<FilterType, FilterEventHandler> _filters =
            new Dictionary<FilterType, FilterEventHandler>();
        private HashSet<FilterType> _textFilters = new HashSet<FilterType>();

        public MainWindow()
        {
            Application.Current.DispatcherUnhandledException += Application_Current_DispatcherUnhandledException;
            InitializeComponent();
            InitUI();
        }

        internal static User CurrentUser;

        protected override Button AddButton => btnAdd;
        protected override Button EditButton => btnEdit;
        protected override Button DeleteButton => btnDelete;
        protected override DataGrid DataGrid => dgItems;

        private IReadOnlyList<LibraryItem> Items
        {
            get { return Entities; }
            set
            {
                Entities = value;
                InitUIData();
            }
        }

        private void InitUIData()
        {
            DataContext = EntitiesViewSource?.View as ListCollectionView;
            InitFiltersData();
            tbUser.Text = CurrentUser == null ? string.Empty :
                _employeesManager.Search(emp => emp.EmployeeID == CurrentUser.EmployeeID)
                .FirstOrDefault()?.ToString();
        }

        protected override void EntitiesManager_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.EntitiesManager_CollectionChanged(sender, e);
            InitFiltersData();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            UnsubscribeFromManagersEvents();
        }

        protected override void InitUI()
        {
            base.InitUI();

            if (CurrentUser == null)
            {
                btnLogin.IsEnabled = true;
                grdMain.IsEnabled = false;
                grdFilters.IsEnabled = false;
                btnGenerateDemoData.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnLogin.IsEnabled = false;
                grdMain.IsEnabled = true;
                SetGenerateDemoDataButton();
            }

            btnLogout.IsEnabled = !btnLogin.IsEnabled;

            if (CurrentUser == null || !CurrentUser.IsLoggedIn)
            {
                tlbItems.IsEnabled = false;
                tlbEntities.IsEnabled = false;
                return;
            }

            tlbItems.IsEnabled = true;
            tlbEntities.IsEnabled = true;

            btnEmployees.IsEnabled = _employeesManager.IsOperationAuthorized(OperationType.View);
            btnUsers.IsEnabled = _employeesManager.Count == 0 ? false
                : _usersManager.IsOperationAuthorized(OperationType.View);
            btnPublishers.IsEnabled = _publishersManager.IsOperationAuthorized(OperationType.View);
        }

        private void btnGenerateDemoData_Click(object sender, RoutedEventArgs e)
        {
            GUIServices.SetBusyState();
            GenerateDemoData();
            SearchByTitle();    // Fill datagrid with generated data
            btnGenerateDemoData.Visibility = Visibility.Collapsed;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddButton_Click(typeof(ItemWindow));
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            EditButton_Click(typeof(ItemWindow));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteButton_Click();
        }

        private void dgItems_ISBN_Click(object sender, RoutedEventArgs e)
        {
            DataGrid_ID_Click(sender, typeof(ItemWindow));
        }

        private void dgItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid_SelectionChanged();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.ShowDialog();

            if (loginWindow.IsLoginRequest)
            {
                GUIServices.SetBusyState();

                CurrentUser = loginWindow.CurrentUser;

                if (CurrentUser != null)
                {
                    InitEntities(new ItemsManager(CurrentUser));
                    InitEntitiesManagers(CurrentUser);
                    InitUI();
                    InitUIData();
                }
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser = null;
            Items = null;
            UnsubscribeFromManagersEvents();
            InitUI();
        }

        private void btnEmployees_Click(object sender, RoutedEventArgs e)
        {
            GUIServices.SetBusyState();
            var employeesWindow = new EmployeesWindow(_employeesManager);
            employeesWindow.ShowDialog();
        }

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            GUIServices.SetBusyState();
            var usersWindow = new UsersWindow(_usersManager);
            usersWindow.ShowDialog();
        }

        private void btnPublishers_Click(object sender, RoutedEventArgs e)
        {
            GUIServices.SetBusyState();
            var publishersWindow = new PublishersWindow(_publishersManager);
            publishersWindow.ShowDialog();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchByTitle(txtSearch.Text);
        }

        private void btnAdvancedSearch_Click(object sender, RoutedEventArgs e)
        {
            var itemSearchWindow = new ItemsSearchWindow();
            itemSearchWindow.ShowDialog();
            if (itemSearchWindow.IsSearchDone)
                Items = itemSearchWindow.SearchResults;
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SearchByTitle(txtSearch.Text);
        }

        private void SearchByTitle(string title = null)
        {
            GUIServices.SetBusyState();

            if (string.IsNullOrEmpty(title))
                Items = EntitiesManager.Search();
            else
                Items = EntitiesManager.Search(i => i.Title.ToLower().Contains(title.ToLower()));
        }

        private void SetGenerateDemoDataButton()
        {
            if (EntitiesManager.Count == 0 && _publishersManager.Count == 0
                && _employeesManager.Count == 0 && _usersManager.Count == 0)
                btnGenerateDemoData.Visibility = Visibility.Visible;
            else
                btnGenerateDemoData.Visibility = Visibility.Collapsed;
        }

        private void InitEntitiesManagers(User user)
        {
            _publishersManager = new EntityManager<Publisher>(user);
            _employeesManager = new EntityManager<Employee>(user);
            _usersManager = new UsersManager(user);

            SubscribeToManagersEvents();
        }

        private void InitFiltersData()
        {
            ResetFilters();

            if (Entities == null)
                return;

            cboISBNPrefixFilter.ItemsSource = Entities?.Select(i => (short)i.ISBN.Prefix).Distinct();

            GUIServices.SetListControlBinding(
                lstCategoriesFilter,
                Entities?.Select(i => i.Category).Distinct().ToList(),
                comparer: new NaturalStringComparer()
            );

            SetSubCategoriesListBinding();

            GUIServices.SetListControlBinding(
                lstPublishersFilter,
                Entities.Select(i => i.PublisherID != null ?
                    _publishersManager[i.PublisherID] : Publisher.Empty)
                        .Distinct().ToList(),
                comparer: new NaturalStringComparer()
            );

            GUIServices.SetListControlBinding(
                lstAuthorsFilter,
                Entities?.Where(i => i.GetType() == typeof(Book))
                    .Select(i => (i as Book).Author).Distinct().ToList(),
                comparer: new NaturalStringComparer()
            );
        }

        private void ResetFilters()
        {
            cboISBNPrefixFilter.ItemsSource = null;
            lstCategoriesFilter.ItemsSource = null;
            lstSubCategoriesFilter.ItemsSource = null;
            lstPublishersFilter.ItemsSource = null;
            lstAuthorsFilter.ItemsSource = null;

            btnPrintDateFilterClear_Click(null, null);

            grdAuthorsFilter.Visibility =
                ((Entities?.Count ?? 0) == 0) || (Entities?.Any(i => i.GetType() == typeof(Book)) ?? true) ?
                    Visibility.Visible : Visibility.Collapsed;

            grdFilters.IsEnabled = (Entities?.Count ?? 0) == 0 ? false : true;
        }

        private void SetSubCategoriesListBinding()
        {
            IEnumerable<KeyValuePair<Enum, Enum>> items;

            if (lstCategoriesFilter.SelectedItems.Count > 0)
                items = Entities.Where(i => lstCategoriesFilter.SelectedItems.Contains(i.Category))
                    .Select(i =>
                        new KeyValuePair<Enum, Enum>(i.Category, i.SubCategory ?? EMPTY_ENUM._Empty_));
            else
                items = Entities.Select(i =>
                    new KeyValuePair<Enum, Enum>(i.Category, i.SubCategory ?? EMPTY_ENUM._Empty_));

            GUIServices.SetListControlBinding(
                lstSubCategoriesFilter,
                items.Distinct().ToList(),
                comparer: new EnumKeyValueComparer()
            );
        }

        private void SubscribeToManagersEvents()
        {
            _employeesManager.CollectionChanged += DataManager_CollectionChanged;
            _usersManager.CollectionChanged += UsersManager_CollectionChanged;
            _publishersManager.CollectionChanged += DataManager_CollectionChanged;
            _publishersManager.RelatedEntitiesAffected += PublishersManager_RelatedEntitiesAffected;
        }

        private void UnsubscribeFromManagersEvents()
        {
            if(_employeesManager != null)
                _employeesManager.CollectionChanged -= DataManager_CollectionChanged;

            if (_usersManager != null)
                _usersManager.CollectionChanged -= DataManager_CollectionChanged;

            if (_publishersManager != null)
            {
                _publishersManager.CollectionChanged -= DataManager_CollectionChanged;
                _publishersManager.RelatedEntitiesAffected -= PublishersManager_RelatedEntitiesAffected;
            }
        }

        private void DataManager_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InitUI();
        }

        private void UsersManager_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems?.Count == 1 && (e.OldItems[0] as User).Equals(CurrentUser))
                btnLogout_Click(this, null);
            else
                InitUI();
        }

        private void PublishersManager_RelatedEntitiesAffected(object sender, Type e)
        {
            if (e == typeof(LibraryItem))
                DataManager_CollectionChanged(this, null);
        }

        // Catch-all exception handler
        private void Application_Current_DispatcherUnhandledException(
            object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            GUIUtils.ShowMessage(GUIUtils.AggregateExceptionMessage(e.Exception), MessageBoxType.Error);
            e.Handled = true;
        }

        #region Filter Events
        private void txtTitleFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            HandleTextBoxFilterChange(sender as TextBox, FilterByTitle, FilterType.TitleFilter);
        }

        private void cboISBNPrefixFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            HandleComboBoxFilterChange(comboBox, FilterByISBNPrefix, FilterType.ISBNPrefix, e);

            if (comboBox.SelectedValue != null)
                SetISBNGroupFilterItemsSource((ISBN_Prefix)comboBox.SelectedValue);
            else
                cboISBNGroupFilter.ItemsSource = null;
        }

        private void cboISBNGroupFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HandleComboBoxFilterChange(sender as ComboBox, FilterByISBNGroup, FilterType.ISBNGroup, e);
        }

        private void txtISBNPublisherFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            HandleTextBoxFilterChange(sender as TextBox, FilterByISBNPublisher, FilterType.ISBNPublisher);
        }

        private void txtISBNCatalogueFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            HandleTextBoxFilterChange(sender as TextBox, FilterByISBNCatalogue, FilterType.ISBNCatalogue);
        }

        private void lstCategoriesFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HandleListBoxFilterChange(sender as ListBox, FilterByCategory, FilterType.Category, e);
            SetSubCategoriesListBinding();
        }

        private void lstSubCategoriesFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HandleListBoxFilterChange(sender as ListBox, FilterBySubCategory, FilterType.SubCategory, e);
        }

        private void lstAuthorsFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HandleListBoxFilterChange(sender as ListBox, FilterByAuthor, FilterType.Author, e);
        }

        private void lstPublishersFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HandleListBoxFilterChange(sender as ListBox, FilterByPublisher, FilterType.Publisher, e);
        }

        private void dtPrintDateFromFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            HandleDatePickerFilterChange(sender as DatePicker,
                FilterByPrintDateFrom, FilterType.PrintDateFrom, e);
        }

        private void dtPrintDateToFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            HandleDatePickerFilterChange(sender as DatePicker,
                FilterByPrintDateTo, FilterType.PrintDateTo, e);
        }

        private void AddFilter(FilterEventHandler filter, FilterType filterType)
        {
            _filters[filterType] = new FilterEventHandler(filter);
            EntitiesViewSource.Filter += _filters[filterType];
        }

        private void RemoveFilter(FilterType filterType)
        {
            EntitiesViewSource.Filter -= _filters[filterType];
            _filters.Remove(filterType);
        }

        private void btnISBNFilterClear_Click(object sender, RoutedEventArgs e)
        {
            cboISBNPrefixFilter.SelectedIndex = -1;
            cboISBNGroupFilter.SelectedIndex = -1;
            txtISBNPublisherFilter.Text = string.Empty;
            txtISBNCatalogueFilter.Text = string.Empty;
        }

        private void btnCategoriesFilterClear_Click(object sender, RoutedEventArgs e)
        {
            lstCategoriesFilter.SelectedItems.Clear();
        }

        private void btnSubCategoriesFilterClear_Click(object sender, RoutedEventArgs e)
        {
            lstSubCategoriesFilter.SelectedItems.Clear();
        }

        private void btnPublishersFilterClear_Click(object sender, RoutedEventArgs e)
        {
            lstPublishersFilter.SelectedItems.Clear();
        }

        private void btnAuthorsFilterClear_Click(object sender, RoutedEventArgs e)
        {
            lstAuthorsFilter.SelectedItems.Clear();
        }

        private void btnPrintDateFilterClear_Click(object sender, RoutedEventArgs e)
        {
            dtPrintDateFromFilter.ClearValue(DatePicker.SelectedDateProperty);
            dtPrintDateToFilter.ClearValue(DatePicker.SelectedDateProperty);
        }

        #endregion //Filter Events

        #region Filter Event Handlers

        private void HandleTextBoxFilterChange(
            TextBox textBox, FilterEventHandler filter, FilterType filterType)
        {
            GUIServices.SetBusyState();

            if (!_textFilters.Contains(filterType))
            {
                AddFilter(filter, filterType);
                _textFilters.Add(filterType);
                return;
            }

            if (textBox.Text == string.Empty)
            {
                RemoveFilter(filterType);
                _textFilters.Remove(filterType);
            }

            RefreshView();
        }

        private void HandleComboBoxFilterChange(ComboBox comboBox,
            FilterEventHandler filter, FilterType filterType, SelectionChangedEventArgs e)
        {
            GUIServices.SetBusyState();

            if (e.RemovedItems.Count == 0)
            {
                AddFilter(filter, filterType);
                return;
            }

            if (comboBox.SelectedIndex == -1)
                RemoveFilter(filterType);

            RefreshView();
        }

        private void HandleListBoxFilterChange(ListBox listBox,
            FilterEventHandler filter, FilterType filterType, SelectionChangedEventArgs e)
        {
            GUIServices.SetBusyState();

            if (listBox.SelectedItems.Count == 1 && e.AddedItems.Count == 1)
            {
                AddFilter(filter, filterType);
                return;
            }

            if (listBox.SelectedItems.Count == 0)
                RemoveFilter(filterType);

            RefreshView();
        }

        private void HandleDatePickerFilterChange(DatePicker datePicker,
            FilterEventHandler filter, FilterType filterType, SelectionChangedEventArgs e)
        {
            GUIServices.SetBusyState();

            if (e.RemovedItems.Count == 0)
            {
                AddFilter(filter, filterType);
                return;
            }

            if (datePicker.SelectedDate == null)
                RemoveFilter(filterType);

            RefreshView();
        }

        private void FilterByISBNPrefix(object sender, FilterEventArgs e)
        {
            if ((e.Item as LibraryItem)?.ISBN.Prefix != (ISBN_Prefix)cboISBNPrefixFilter.SelectedValue)
                e.Accepted = false;
        }

        private void FilterByISBNGroup(object sender, FilterEventArgs e)
        {
            if ((e.Item as LibraryItem)?.ISBN.GroupIdentifier != (int)cboISBNGroupFilter.SelectedItem)
                e.Accepted = false;
        }

        private void FilterByISBNPublisher(object sender, FilterEventArgs e)
        {
            var item = e.Item as LibraryItem;

            if (item == null || !item.ISBN.PublisherCode.Contains(txtISBNPublisherFilter.Text))
                e.Accepted = false;
        }

        private void FilterByISBNCatalogue(object sender, FilterEventArgs e)
        {
            var item = e.Item as LibraryItem;

            if (item == null || !item.ISBN.CatalogueNumber.Contains(txtISBNCatalogueFilter.Text))
                e.Accepted = false;
        }

        private void FilterByTitle(object sender, FilterEventArgs e)
        {
            var item = e.Item as LibraryItem;

            if (item == null || !item.Title.ToLower().Contains(txtTitleFilter.Text.ToLower()))
                e.Accepted = false;
        }

        private void FilterByCategory(object sender, FilterEventArgs e)
        {
            var item = e.Item as LibraryItem;

            if (item == null || !lstCategoriesFilter.SelectedItems.Contains(item.Category))
                e.Accepted = false;
        }

        private void FilterBySubCategory(object sender, FilterEventArgs e)
        {
            var item = e.Item as LibraryItem;

            if (item == null ||
                !lstSubCategoriesFilter.SelectedItems.Cast<KeyValuePair<Enum, Enum>>()
                    .Any(i => i.Key.Equals(item.Category)
                        && i.Value.Equals(item.SubCategory ?? EMPTY_ENUM._Empty_)))
                e.Accepted = false;
        }

        private void FilterByPublisher(object sender, FilterEventArgs e)
        {
            var item = e.Item as LibraryItem;

            if (item == null ||
                !lstPublishersFilter.SelectedItems.Cast<Publisher>()
                    .Any(p => p.PublisherID == (item.PublisherID ?? Publisher.Empty.PublisherID)))
                e.Accepted = false;
        }

        private void FilterByAuthor(object sender, FilterEventArgs e)
        {
            var item = e.Item as LibraryItem;

            if (item == null ||
                item.GetType() != typeof(Book) ||
                !lstAuthorsFilter.SelectedItems.Contains((item as Book).Author))
                e.Accepted = false;
        }

        private void FilterByPrintDateFrom(object sender, FilterEventArgs e)
        {
            var item = e.Item as LibraryItem;

            if (item == null || item.PrintDate < dtPrintDateFromFilter.SelectedDate)
                e.Accepted = false;
        }

        private void FilterByPrintDateTo(object sender, FilterEventArgs e)
        {
            var item = e.Item as LibraryItem;

            if (item == null || item.PrintDate > dtPrintDateToFilter.SelectedDate)
                e.Accepted = false;
        }

        private void SetISBNGroupFilterItemsSource(ISBN_Prefix prefix)
        {
            switch (prefix)
            {
                case ISBN_Prefix.ISBN_978:
                    cboISBNGroupFilter.ItemsSource = Entities
                        .Where(i => i.ISBN.Prefix == prefix)
                        .Select(i => (ISBN_978_GroupIdentifier)i.ISBN.GroupIdentifier).Distinct();
                    break;
                case ISBN_Prefix.ISBN_979:
                    cboISBNGroupFilter.ItemsSource = Entities
                        .Where(i => i.ISBN.Prefix == prefix)
                        .Select(i => (ISBN_979_GroupIdentifier)i.ISBN.GroupIdentifier).Distinct();
                    break;
                default:
                    break;
            }
        }

        #endregion //Filter Event Handlers

        #region GenerateDemoData
        private void GenerateDemoData()
        {
            UnsubscribeFromManagersEvents();

            GenerateDemoPublishers();
            GenerateDemoEmployeesAndUsers();
            List<Publisher> publishers = _publishersManager.Search();
            CreateItemsFromList(GUIUtils.BooksAndAuthors, publishers, typeof(Book));
            CreateItemsFromList(GUIUtils.JournalsAndEditors, publishers, typeof(Journal));

            SubscribeToManagersEvents();
        }

        private void CreateItemsFromList(
            List<Tuple<string, string>> ItemsAndOwners, List<Publisher> publishers, Type itemType)
        {
            var rand = new Random();

            foreach (var tuple in ItemsAndOwners)
            {
                ISBN isbn = GUIUtils.GenerateISBN();
                var category = GUIUtils.GetRandomCategory();

                LibraryItem item;

                if (itemType == typeof(Book))
                    item = EntitiesManager.Add(new Book(isbn, tuple.Item1, category, tuple.Item2));
                else
                    item = EntitiesManager.Add(new Journal(isbn, tuple.Item1, category, tuple.Item2));

                int numberofCopies = rand.Next(1, 10);
                for (int j = 0; j < numberofCopies; j++)
                {
                    item.AddCopy();
                }

                item.SubCategory = GUIUtils.GetRandomSubCategory(category);
                item.Publisher = publishers[rand.Next(0, publishers.Count)];
                item.PrintDate = GUIUtils.GetRandomDate(5);
                item.GraphicDesigner = $"{GUIUtils.GetRandomFirstName()} {GUIUtils.GetRandomLastName()}";

                // Update storage
                item.Update();
            }
        }

        private void GenerateDemoPublishers(int numberOfPublishers = 10)
        {
            for (int i = 1; i <= numberOfPublishers; i++)
            {
                string publisherName;
                do
                {
                    publisherName = GUIUtils.GetRandomPublisherName();
                } while (_publishersManager.Search(p => p.Name == publisherName).Any());

                _publishersManager.Add(new Publisher(publisherName));
            }
        }

        private void GenerateDemoEmployeesAndUsers(int numberOfEmployees = 20)
        {
            Employee employee;
            for (int i = 1; i <= numberOfEmployees; i++)
            {
                employee = _employeesManager.Add(
                    new Employee(GUIUtils.GetRandomFirstName(), GUIUtils.GetRandomLastName()));

                _usersManager.Add(new User(
                    employee.EmployeeID,
                    i == 1 ? UserType.Manager : i < 5 ? UserType.Supervisor : UserType.Worker)
                {
                    UserName = $"user{i:D2}",
                    Password = $"pass{i:D2}"
                }
                );
            }
        }
        #endregion //GenerateDemoData
    }
}