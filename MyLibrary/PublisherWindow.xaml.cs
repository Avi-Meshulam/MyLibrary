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
    /// Logic for PublisherWindow.xaml
    /// </summary>
    public partial class PublisherWindow : EntityWindow<Publisher>
    {
        public PublisherWindow(EntityManager<Publisher> publishersManager, Publisher publisher = null)
            : base(publishersManager, publisher)
        {
            InitializeComponent();
            InitUIElements(isFirstTime: true);
        }

        protected override Button SaveButton { get { return btnSave; } }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveButton_Click(sender, e);
        }

        protected override bool Save()
        {
            if (!base.Save())
                return false;

            bool isNew = false;
            if (_entity == null)
            {
                isNew = true;
                _entity = new Publisher(txtName.Text);
            }
            else
                _entity.Name = txtName.Text;

            if (IsAddressExist())
                SetAddress(_entity);
            else
                _entity.Address = null;

            _entity.PhoneNumber = txtPhoneNumber.Text;
            _entity.Email = txtEmail.Text;
            _entity.WebSite = txtWebSite.Text;

            // Persist to storage
            if (isNew)
                _entitiesManager.Add(_entity);
            else
                _entity.Update();

            return true;
        }

        protected override bool IsValid()
        {
            if (string.IsNullOrEmpty(txtName.Text))
                return false;

            if (IsAddressExist())
                if (!Address.IsValid(txtStreetName.Text, txtStreetNumber.Text, txtCity.Text))
                    return false;

            return true;
        }

        private void SetAddress(Publisher publisher)
        {
            Address address;
            if (publisher.Address == null)
                address = new Address(txtStreetName.Text, txtStreetNumber.Text, txtCity.Text);
            else
            {
                address = publisher.Address;
                address.StreetName = txtStreetName.Text;
                address.StreetNumber = txtStreetNumber.Text;
                address.City = txtCity.Text;
            }

            address.State = txtState.Text;
            address.ZipCode = txtZipCode.Text;
            address.Country = txtCountry.Text;

            publisher.Address = address;
        }

        private bool IsAddressExist()
        {
            return
                !string.IsNullOrEmpty(txtStreetName.Text) ||
                !string.IsNullOrEmpty(txtStreetNumber.Text) ||
                !string.IsNullOrEmpty(txtCity.Text) ||
                !string.IsNullOrEmpty(txtState.Text) ||
                !string.IsNullOrEmpty(txtZipCode.Text) ||
                !string.IsNullOrEmpty(txtCountry.Text);
        }

        protected override void DisplayValidationMessage()
        {
            var sb = new StringBuilder();

            if (string.IsNullOrEmpty(txtName.Text))
                Utils.AppendValidationMessage(sb, "Name");

            if (IsAddressExist())
            {
                if (string.IsNullOrEmpty(txtStreetName.Text))
                    Utils.AppendValidationMessage(sb, "Street Name");

                if (string.IsNullOrEmpty(txtStreetNumber.Text))
                    Utils.AppendValidationMessage(sb, "Street Number");

                if (string.IsNullOrEmpty(txtCity.Text))
                    Utils.AppendValidationMessage(sb, "City");
            }

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
                if (txtName.Text != (_entity.Name ?? "") ||
                    txtStreetName.Text != (_entity.Address?.StreetName ?? "") ||
                    txtStreetNumber.Text != (_entity.Address?.StreetNumber ?? "") ||
                    txtCity.Text != (_entity.Address?.City ?? "") ||
                    txtState.Text != (_entity.Address?.State ?? "") ||
                    txtZipCode.Text != (_entity.Address?.ZipCode ?? "") ||
                    txtCountry.Text != (_entity.Address?.Country ?? "") ||
                    txtPhoneNumber.Text != (_entity.PhoneNumber ?? "")||
                    txtEmail.Text != (_entity.Email ?? "") ||
                    txtWebSite.Text != (_entity.WebSite ?? ""))
                    btnSave.IsEnabled = true;
                else
                    btnSave.IsEnabled = false;
            }
        }
    }
}
