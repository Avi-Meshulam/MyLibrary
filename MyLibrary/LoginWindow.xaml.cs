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
    /// Logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private UsersManager _usersManager = new UsersManager();

        public LoginWindow()
        {
            InitializeComponent();

            // Just for demonstration
            if (_usersManager.Count == 0)
            {
                txtUserName.Text = "admin";
                passwordBox.Password = "admin";
            }

            txtUserName.Focus();
        }

        public User CurrentUser { get; private set; }
        public bool IsLoginRequest { get; private set; }

        private void txtUserName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtUserName.SelectAll();
        }

        private void passwordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            passwordBox.SelectAll();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            IsLoginRequest = true;

            CurrentUser = _usersManager.Login(txtUserName.Text, passwordBox.Password);

            if (CurrentUser == null)
            {
                GUIUtils.ShowMessage("Incorrect User/Password", MessageBoxType.Warning);
                return;
            }

            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
