using MyLibrary.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Logic for Book.xaml
    /// </summary>
    public partial class ItemWindow : EntityWindow<LibraryItem>
    {
        private List<string> _authorsList;

        public ItemWindow(EntityManager<LibraryItem> itemsManager, LibraryItem item = null)
            : base(itemsManager, item)
        {
            _authorsList = itemsManager.Search<Book>()
                .Select(b => (b as Book).Author)
                .OrderBy(s => s, new NaturalStringComparer())
                .Distinct().ToList();

            InitializeComponent();
            InitUIElements(isFirstTime: true);
        }

        protected override Button SaveButton { get { return btnSave; } }

        protected override void InitUIElements(bool isFirstTime = false)
        {
            base.InitUIElements();

            if (isFirstTime)
            {
                grdItem.SetValue(ReadOnlyContainer.IsReadOnlyProperty, IsReadOnly);
                InitCombos();
            }

            if (_entity != null)
            {
                grdISBN.Visibility = Visibility.Visible;
                grdISBNNew.Visibility = Visibility.Collapsed;
                btnCopies.IsEnabled = true;
                brdType.IsEnabled = false;

                if (_entity.GetType() == typeof(Book))
                    optTypeBook.IsChecked = true;
                else
                    optTypeJournal.IsChecked = true;
            }
            else
            {
                optTypeBook.IsChecked = true;
                grdISBN.Visibility = Visibility.Collapsed;
                btnCopies.IsEnabled = false;
            }
        }

        private void InitCombos()
        {
            cboISBNPrefix.ItemsSource =
                Enum.GetValues(typeof(ISBN_Prefix)).Cast<short>().ToList();

            GUIServices.SetListControlBinding(
                   cboSubCategories,
                   new List<Enum>(),
                   comparer: new NaturalStringComparer()
            );

            GUIServices.SetListControlBinding(
                cboCategories,
                CategoriesRepository.Categories.Keys.ToList(),
                comparer: new NaturalStringComparer()
            );

            GUIServices.SetListControlBinding(
                cboPublishers,
                new EntityManager<Publisher>().Search(),
                comparer: new NaturalStringComparer()
            );

            GUIServices.SetListControlBinding(
                cboAuthors,
                _authorsList,
                comparer: new NaturalStringComparer()
            );

            cboAuthors.Text = (_entity as Book)?.Author ?? "";
            cboPublishers.SelectedValue = _entity?.PublisherID;
        }

        private void cboCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboCategories.SelectedItem != null)
            {
                cboSubCategories.ItemsSource =
                    CategoriesRepository.Categories[(Category)cboCategories.SelectedItem];
                cboSubCategories.IsEnabled = true;
            }
            else
            {
                cboSubCategories.ItemsSource = null;
                cboSubCategories.IsEnabled = false;
            }

            UIElement_Changed(sender, e);
        }

        private void btnCopies_Click(object sender, RoutedEventArgs e)
        {
            GUIServices.SetBusyState();
            var itemWindow = new ItemCopiesWindow(_entity, _entitiesManager.CurrentUser);
            itemWindow.ShowDialog();
        }

        private void optTypeBook_Checked(object sender, RoutedEventArgs e)
        {
            grdBook.Visibility = Visibility.Visible;
            grdJournal.Visibility = Visibility.Collapsed;
            UIElement_Changed(sender, e);
        }

        private void optTypeJournal_Checked(object sender, RoutedEventArgs e)
        {
            grdJournal.Visibility = Visibility.Visible;
            grdBook.Visibility = Visibility.Collapsed;
            UIElement_Changed(sender, e);
        }

        private void cboISBNPrefix_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            if (comboBox.SelectedValue != null)
                SetISBNGroupItemsSource((ISBN_Prefix)comboBox.SelectedValue);
            else
                cboISBNGroup.ItemsSource = null;

            UIElement_Changed(sender, e);
        }

        private void SetISBNGroupItemsSource(ISBN_Prefix prefix)
        {
            switch (prefix)
            {
                case ISBN_Prefix.ISBN_978:
                    cboISBNGroup.ItemsSource = Enum.GetValues(typeof(ISBN_978_GroupIdentifier));
                    break;
                case ISBN_Prefix.ISBN_979:
                    cboISBNGroup.ItemsSource = Enum.GetValues(typeof(ISBN_979_GroupIdentifier));
                    break;
                default:
                    break;
            }
        }

        private void btnClearISBN_Click(object sender, RoutedEventArgs e)
        {
            cboISBNPrefix.SelectedIndex = -1;
            cboISBNGroup.SelectedIndex = -1;
            txtISBNPublisher.Text = string.Empty;
            txtISBNCatalogue.Text = string.Empty;
            txtISBNCheckDigit.Text = string.Empty;
        }

        private void btnGenerateISBN_Click(object sender, RoutedEventArgs e)
        {
            int publisherCode;
            int catalogueNumber;

            if (!int.TryParse(txtISBNPublisher.Text, out publisherCode))
                publisherCode = -1;

            if (!int.TryParse(txtISBNCatalogue.Text, out catalogueNumber))
                catalogueNumber = -1;

            var isbn = GUIUtils.GenerateISBN(
                cboISBNPrefix.SelectedItem == null ? null : (ISBN_Prefix?)(short)cboISBNPrefix.SelectedItem,
                cboISBNGroup.SelectedItem as Enum,
                publisherCode,
                catalogueNumber);

            cboISBNPrefix.SelectedItem = (short)isbn.Prefix;
            switch (isbn.Prefix)
            {
                case ISBN_Prefix.ISBN_978:
                    cboISBNGroup.SelectedItem = (ISBN_978_GroupIdentifier)isbn.GroupIdentifier;
                    break;
                case ISBN_Prefix.ISBN_979:
                    cboISBNGroup.SelectedItem = (ISBN_979_GroupIdentifier)isbn.GroupIdentifier;
                    break;
                default:
                    break;
            }
            txtISBNPublisher.Text = isbn.PublisherCode;
            txtISBNCatalogue.Text = isbn.CatalogueNumber;
            txtISBNCheckDigit.Text = isbn.CheckDigit.ToString();
        }

        private void cboAuthors_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            (sender as EditableComboBox).PreviewKeyUpHandler(_authorsList, e);
            UIElement_Changed(sender, e);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveButton_Click(sender, e);
            _entity = MainWindow.CurrentItem as LibraryItem;
            MainWindow.RefreshView();
        }

        protected override bool Save()
        {
            if (!base.Save())
                return false;

            bool isNew = false;
            if (_entity == null)
            {
                _entity = CreateNewItem();
                isNew = true;
            }

            _entity.Title = txtTitle.Text;
            _entity.Category = (Category)cboCategories.SelectedItem;
            _entity.SubCategory = cboSubCategories.SelectedItem as Enum;
            _entity.Publisher = cboPublishers.SelectedItem as Publisher;
            _entity.GraphicDesigner = txtGraphicDesigner.Text;
            _entity.PrintDate = dtPrintDate.SelectedDate;
            if (_entity.GetType() == typeof(Journal))
                (_entity as Journal).Editor = txtEditor.Text;
            else
            {
                (_entity as Book).Edition = txtEdition.Text;
                (_entity as Book).Translator = txtTranslator.Text;
                (_entity as Book).Author = cboAuthors.Text;
            }

            // Persist to storage
            if (isNew)
                _entitiesManager.Add(_entity);
            else
                _entity.Update();

            return true;
        }

        private LibraryItem CreateNewItem()
        {
            var isbn = GUIUtils.GenerateISBN(
                (ISBN_Prefix)cboISBNPrefix.SelectedItem,
                (Enum)cboISBNGroup.SelectedItem,
                int.Parse(txtISBNPublisher.Text),
                int.Parse(txtISBNCatalogue.Text));

            if (optTypeBook.IsChecked == true)
                return new Book(isbn, txtTitle.Text, (Category)cboCategories.SelectedItem);
            else
                return new Journal(isbn, txtTitle.Text, (Category)cboCategories.SelectedItem);
        }

        protected override bool IsValid()
        {
            if(_entity == null)
            {
                int publisherCode;
                int catalogueNumber;

                if (cboISBNPrefix.SelectedItem == null ||
                    cboISBNGroup.SelectedItem == null ||
                    !int.TryParse(txtISBNPublisher.Text, out publisherCode) ||
                    !int.TryParse(txtISBNCatalogue.Text, out catalogueNumber))
                    return false;
            }

            if (string.IsNullOrEmpty(txtTitle.Text) ||
                cboCategories.SelectedItem == null)
                return false;

            return true;
        }

        protected override void DisplayValidationMessage()
        {
            int publisherCode;
            int catalogueNumber;

            var sb = new StringBuilder();

            if (cboISBNPrefix.SelectedItem == null)
                Utils.AppendValidationMessage(sb, "ISBN Prefix");

            if (cboISBNGroup.SelectedItem == null)
                Utils.AppendValidationMessage(sb, "ISBN Group");

            if (string.IsNullOrEmpty(txtISBNPublisher.Text))
                Utils.AppendValidationMessage(sb, "ISBN Publisher Code");
            else if (!int.TryParse(txtISBNPublisher.Text, out publisherCode))
                Utils.AppendValidationMessage(sb, "ISBN Publisher Code", ValidationType.WrongNumberFormat);

            if (string.IsNullOrEmpty(txtISBNCatalogue.Text))
                Utils.AppendValidationMessage(sb, "ISBN Catalogue Number");
            else if (int.TryParse(txtISBNCatalogue.Text, out catalogueNumber))
                Utils.AppendValidationMessage(sb, "ISBN Catalogue Number", ValidationType.WrongNumberFormat);

            if (string.IsNullOrEmpty(txtTitle.Text))
                Utils.AppendValidationMessage(sb, "Title");

            if (sb.Length > 0)
                GUIUtils.ShowMessage(sb.ToString(), MessageBoxType.Warning);
        }

        protected override void UIElement_Changed(object sender, RoutedEventArgs e)
        {
            base.UIElement_Changed(sender, e);

            if (e.Handled) return;

            if (_entity == null)
                btnSave.IsEnabled = true;
            else
            {
                if (txtTitle.Text != _entity.Title ||
                    (Category)cboCategories.SelectedItem != _entity.Category ||
                    cboSubCategories.SelectedItem != _entity.SubCategory ||
                    (cboPublishers.SelectedValue == null ? 
                        null : (uint?)cboPublishers.SelectedValue) != _entity.PublisherID ||
                    txtGraphicDesigner.Text != (_entity.GraphicDesigner ?? "") ||
                    dtPrintDate.SelectedDate != _entity.PrintDate ||
                    (_entity.GetType() == typeof(Journal) ?
                        txtEditor.Text != ((_entity as Journal).Editor ?? "")
                        : (txtEdition.Text != ((_entity as Book).Edition ?? "") ||
                            txtTranslator.Text != ((_entity as Book).Translator ?? "") ||
                            cboAuthors.Text != ((_entity as Book).Author ?? ""))))
                    btnSave.IsEnabled = true;
                else
                    btnSave.IsEnabled = false;
            }
        }
    }
}
