using System;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace Backlogger
{
    /// <summary>
    /// Interaction logic for CollectionWindow.xaml
    /// </summary>
    public partial class CollectionWindow : Window
    {
        public string windowType;
        CollectionViewSource materialsViewSource;
        public CollectionWindow(string type)
        {
            windowType = type;
            InitializeComponent();
            materialsViewSource = ((CollectionViewSource)(FindResource("materialsViewSource")));
            DataContext = this;
            this.Title = type + " Library";
        }

        private void CollectionWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.MainWindow.Show();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addDialog = new AddDialog(windowType);
            addDialog.ShowDialog();
            RefreshGrid();
            materialsViewSource.View.Refresh();
        }

        private void RefreshGrid()
        {
            using (BackloggerEntities context = new BackloggerEntities())
            {
                context.Hobbies.Load();
                context.Materials.Load();
                int hobbyID = (from o in context.Hobbies.Local where o.HobbyName == windowType select o).FirstOrDefault().HobbyID;

                switch (windowType)
                {
                    case "Books":
                        context.Books.Load();
                        materialsViewSource.Source = context.Books.Local;
                        break;
                    case "Movies":
                        context.Movies.Load();
                        materialsViewSource.Source = context.Movies.Local;
                        break;
                    case "Games":
                        context.Games.Load();
                        materialsViewSource.Source = context.Games.Local;
                        break;
                }
            }
        }

        private void CollectionWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
        }

        private void SubscriptionsButton_Click(object sender, RoutedEventArgs e)
        {
            var subDialog = new SubscriptionsDialog(windowType);
            subDialog.ShowDialog();
            materialsViewSource.View.Refresh();
        }

        private void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {

        }
    }
}
