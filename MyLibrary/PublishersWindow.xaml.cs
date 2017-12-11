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
    /// Logic for PublishersManager.xaml
    /// </summary>
    public partial class PublishersWindow : EntitiesWindow<Publisher>
    {
        public PublishersWindow(EntityManager<Publisher> publishersManager)
        {
            InitializeComponent();
            InitEntities(publishersManager);
            InitUI();
        }

        protected override Button AddButton => btnAdd;
        protected override Button EditButton => btnEdit;
        protected override Button DeleteButton => btnDelete;
        protected override DataGrid DataGrid => dgPublishers;

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddButton_Click(typeof(PublisherWindow));
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            EditButton_Click(typeof(PublisherWindow));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteButton_Click();
        }

        private void dgPublishers_ID_Click(object sender, RoutedEventArgs e)
        {
            DataGrid_ID_Click(sender, typeof(PublisherWindow));
        }

        private void dgPublishers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid_SelectionChanged();
        }
    }
}
