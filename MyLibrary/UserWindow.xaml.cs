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
    /// Logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : EntityWindow<User>
    {
        private IReadOnlyList<Employee> _employees;

        public UserWindow(UsersManager usersManager, User user = null)
            : base(usersManager, user)
        {
            _employees = new EntityManager<Employee>().Search();
            InitializeComponent();
            InitUIElements(isFirstTime: true);
        }

        protected override Button SaveButton { get { return btnSave; } }

        protected override void InitUIElements(bool isFirstTime = false)
        {
            base.InitUIElements();

            if(isFirstTime) InitCombos();

            if (_entity != null)
            {
                cboEmployees.IsEnabled = false;
                cboUserTypes.IsEnabled = false;
            }
        }

        private void InitCombos()
        {
            cboEmployees.ItemsSource = _employees.OrderBy(e => e.EmployeeID).ToList();

            GUIServices.SetListControlBinding(
                   cboUserTypes,
                   Enum.GetValues(typeof(UserType)),
                   comparer: new NaturalStringComparer()
            );
        }

        private void cboEmployeeID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var employee = cboEmployees.SelectedItem as Employee;
            txtFirstName.Text = employee?.FirstName;
            txtLastName.Text = employee?.LastName;
            UIElement_Changed(sender, e);
        }

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
                _entity = new User(
                    (cboEmployees.SelectedItem as Employee).EmployeeID, 
                    (UserType)cboUserTypes.SelectedItem);
            }

            _entity.UserName = txtUserName.Text;
            _entity.Password = txtPassword.Text;

            // Persist to storage
            if (isNew)
                _entitiesManager.Add(_entity);
            else
                _entity.Update();

            return true;
        }

        protected override bool IsValid()
        {
            if (cboEmployees.SelectedItem == null ||
                cboUserTypes.SelectedItem == null ||
                string.IsNullOrEmpty(txtUserName.Text) ||
                string.IsNullOrEmpty(txtPassword.Text))
                return false;

            return true;
        }

        protected override void DisplayValidationMessage()
        {
            var sb = new StringBuilder();

            if (cboEmployees.SelectedItem == null)
                Utils.AppendValidationMessage(sb, "Employee");

            if (cboUserTypes.SelectedItem == null)
                Utils.AppendValidationMessage(sb, "User Type");

            if (string.IsNullOrEmpty(txtUserName.Text))
                Utils.AppendValidationMessage(sb, "User Name");

            if (string.IsNullOrEmpty(txtPassword.Text))
                Utils.AppendValidationMessage(sb, "Password");

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
                if ((UserType)cboUserTypes.SelectedItem != _entity.UserType ||
                    txtUserName.Text != (_entity.UserName ?? "") ||
                    txtPassword.Text != (_entity.Password ?? ""))
                        btnSave.IsEnabled = true;
                else
                    btnSave.IsEnabled = false;
            }
        }
    }
}
