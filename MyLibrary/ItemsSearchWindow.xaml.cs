using MyLibrary.BL;
using System;
using System.Collections.Generic;
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

namespace MyLibrary
{
    /// <summary>
    /// Logic for ItemSearchWindow.xaml
    /// </summary>
    public partial class ItemsSearchWindow : Window
    {
        private EntityManager<LibraryItem> _itemsManager = new EntityManager<LibraryItem>();

        public ItemsSearchWindow()
        {
            InitializeComponent();
            InitUIElements();
        }

        public bool IsSearchDone { get; private set; }

        private List<LibraryItem> _searchResults;
        public List<LibraryItem> SearchResults { get { return _searchResults; } }

        private void InitUIElements()
        {
            InitFiltersData();
        }

        private void InitFiltersData()
        {
            cboISBNPrefixFilter.ItemsSource = 
                Enum.GetValues(typeof(ISBN_Prefix)).Cast<short>().ToList();

            GUIServices.SetListControlBinding(
                lstCategoriesFilter,
                CategoriesRepository.Categories.Keys.ToList(),
                comparer: new NaturalStringComparer()
            );

            SetSubCategoriesListBinding();

            GUIServices.SetListControlBinding(
                lstPublishersFilter,
                new EntityManager<Publisher>().Search(),
                comparer: new NaturalStringComparer()
            );

            GUIServices.SetListControlBinding(
                lstAuthorsFilter,
                new EntityManager<LibraryItem>().Search<Book>()
                .Select(b => (b as Book).Author).Distinct().ToList(),
                comparer: new NaturalStringComparer()
            );
        }

        private void SetSubCategoriesListBinding()
        {
            IEnumerable<KeyValuePair<Enum, Enum>> items;

            if (lstCategoriesFilter.SelectedItems.Count > 0)
                items = CategoriesRepository.GetSubCategoriesAsKeyValuePairs(lstCategoriesFilter.SelectedItems.Cast<Enum>())
                    .Select(i => i.Value != null ? i : new KeyValuePair<Enum, Enum>(i.Key, EMPTY_ENUM._Empty_));
            else
                items = CategoriesRepository.GetSubCategoriesAsKeyValuePairs()
                    .Select(i => i.Value != null ? i : new KeyValuePair<Enum, Enum>(i.Key, EMPTY_ENUM._Empty_));

            GUIServices.SetListControlBinding(
                lstSubCategoriesFilter,
                items.Distinct().ToList(),
                comparer: new EnumKeyValueComparer()
            );
        }

        private void SetISBNGroupFilterItemsSource(ISBN_Prefix prefix)
        {
            switch (prefix)
            {
                case ISBN_Prefix.ISBN_978:
                    cboISBNGroupFilter.ItemsSource = Enum.GetValues(typeof(ISBN_978_GroupIdentifier));
                    break;
                case ISBN_Prefix.ISBN_979:
                    cboISBNGroupFilter.ItemsSource = Enum.GetValues(typeof(ISBN_979_GroupIdentifier));
                    break;
                default:
                    break;
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            _searchResults = Search();
            IsSearchDone = true;
            Close();
        }

        private List<LibraryItem> Search()
        {
            GUIServices.SetBusyState();

            return _itemsManager.Search(new Predicate<LibraryItem>(i =>
            {
                var item = i as LibraryItem;

                return
                    (cboISBNPrefixFilter.SelectedIndex != -1 ?
                        item.ISBN.Prefix == (ISBN_Prefix)cboISBNPrefixFilter.SelectedValue : true) &&
                    (cboISBNGroupFilter.SelectedIndex != -1 ?
                        item.ISBN.GroupIdentifier == (int)cboISBNGroupFilter.SelectedItem : true) &&
                    (txtISBNPublisherFilter.Text != string.Empty ?
                        item.ISBN.PublisherCode.Contains(txtISBNPublisherFilter.Text) : true) &&
                    (txtISBNCatalogueFilter.Text != string.Empty ?
                        item.ISBN.CatalogueNumber.Contains(txtISBNCatalogueFilter.Text) : true) &&
                    (txtTitle.Text != string.Empty ?
                        item.Title.ToLower().Contains(txtTitle.Text.ToLower()) : true) &&
                    (lstCategoriesFilter.SelectedItems.Count > 0 ?
                        lstCategoriesFilter.SelectedItems.Contains(item.Category) : true) &&
                    (lstSubCategoriesFilter.SelectedItems.Count > 0 ?
                        lstSubCategoriesFilter.SelectedItems.Cast<KeyValuePair<Enum, Enum>>()
                            .Any(kv => item.SubCategory == null ?
                                kv.Value.Equals(EMPTY_ENUM._Empty_) :
                                kv.Key.Equals(item.Category) && kv.Value.Equals(item.SubCategory)) : true) &&
                    (lstPublishersFilter.SelectedItems.Count > 0 ?
                        lstPublishersFilter.SelectedItems.Cast<Publisher>()
                            .Any(p => p.PublisherID == item.PublisherID) : true) &&
                    (txtGraphicDesigner.Text != string.Empty ?
                        item.GraphicDesigner?.ToLower().Contains(txtGraphicDesigner.Text.ToLower()) ?? false : true) &&
                    (optTypeBook.IsChecked.GetValueOrDefault() ?
                        (item.GetType() == typeof(Book) ?
                            (lstAuthorsFilter.SelectedItems.Count > 0 ?
                                lstAuthorsFilter.SelectedItems.Contains((item as Book).Author) : true) &&
                            (txtEdition.Text != string.Empty ?
                                (item as Book).Edition?.Contains(txtEdition.Text) ?? false : true) &&
                            (txtTranslator.Text != string.Empty ?
                                (item as Book).Translator?.Contains(txtTranslator.Text) ?? false : true)
                        : false) : (optTypeJournal.IsChecked.GetValueOrDefault() ?
                            (item.GetType() == typeof(Journal) ?
                                (txtEditor.Text != string.Empty ?
                                    (item as Journal).Editor?.Contains(txtEditor.Text) ?? false : true)
                            : false)
                        : true)) &&
                    (dtPrintDateFromFilter.SelectedDate != null ?
                        item.PrintDate >= dtPrintDateFromFilter.SelectedDate : true) &&
                    (dtPrintDateToFilter.SelectedDate != null ?
                        item.PrintDate <= dtPrintDateToFilter.SelectedDate : true);
            }));
        }

        private void optTypeBook_Checked(object sender, RoutedEventArgs e)
        {
            grdBook.Visibility = Visibility.Visible;
            //grdAuthors.IsEnabled = true;
            grdAuthors.Visibility = Visibility.Visible;
            grdJournal.Visibility = Visibility.Collapsed;
        }

        private void optTypeJournal_Checked(object sender, RoutedEventArgs e)
        {
            grdBook.Visibility = Visibility.Collapsed;
            //grdAuthors.IsEnabled = false;
            grdAuthors.Visibility = Visibility.Collapsed;
            grdJournal.Visibility = Visibility.Visible;
        }

        private void optTypeAll_Checked(object sender, RoutedEventArgs e)
        {
            grdBook.Visibility = Visibility.Collapsed;
            //grdAuthors.IsEnabled = false;
            grdAuthors.Visibility = Visibility.Collapsed;
            grdJournal.Visibility = Visibility.Collapsed;
        }

        private void cboISBNPrefixFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            if (comboBox.SelectedValue != null)
                SetISBNGroupFilterItemsSource((ISBN_Prefix)comboBox.SelectedValue);
            else
                cboISBNGroupFilter.ItemsSource = null;
        }

        private void lstCategoriesFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetSubCategoriesListBinding();
        }

        private void btnISBNFilterClear_Click(object sender, RoutedEventArgs e)
        {
            cboISBNPrefixFilter.SelectedIndex = -1;
            cboISBNGroupFilter.SelectedIndex = -1;
            txtISBNPublisherFilter.Text = string.Empty;
            txtISBNCatalogueFilter.Text = string.Empty;
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

        private void btnCategoriesFilterClear_Click(object sender, RoutedEventArgs e)
        {
            lstCategoriesFilter.SelectedItems.Clear();
        }

        private void btnSubCategoriesFilterClear_Click(object sender, RoutedEventArgs e)
        {
            lstSubCategoriesFilter.SelectedItems.Clear();
        }
    }
}
