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
    /// Logic for Users.xaml
    /// </summary>
    public partial class UsersWindow : EntitiesWindow<User>
    {
        public UsersWindow(UsersManager usersManager)
        {
            InitializeComponent();
            InitEntities(usersManager);
            InitUI();
        }

        protected override Button AddButton => btnAdd;
        protected override Button EditButton => btnEdit;
        protected override Button DeleteButton => btnDelete;
        protected override DataGrid DataGrid => dgUsers;

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddButton_Click(typeof(UserWindow));
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            EditButton_Click(typeof(UserWindow));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteButton_Click();
        }

        private void dgUsers_ID_Click(object sender, RoutedEventArgs e)
        {
            DataGrid_ID_Click(sender, typeof(UserWindow));
        }

        private void dgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid_SelectionChanged();
        }
    }
}
