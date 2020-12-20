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
        public BackloggerEntities context = new BackloggerEntities();
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
            var addDialog = new AddDialog(windowType, context);
            addDialog.ShowDialog();
            materialsViewSource.View.Refresh();
        }

        private void CollectionWindow_Loaded(object sender, RoutedEventArgs e)
        {
            context.Materials.Load();
            context.Hobbies.Load();
            context.Authors.Load();
            context.Genres.Load();
            context.Hobbies.Load();
            context.MaterialFormats.Load();
            context.Materials.Load();
            context.Statuses.Load();
            context.StatusUpdates.Load();
            context.Subscriptions.Load();

            switch(windowType)
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

        private void SubscriptionsButton_Click(object sender, RoutedEventArgs e)
        {
            var subDialog = new SubscriptionsDialog(windowType, context);
            subDialog.ShowDialog();
            materialsViewSource.View.Refresh();
        }
    }
}
