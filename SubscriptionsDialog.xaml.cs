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
    /// Interaction logic for SubscriptionsDialog.xaml
    /// </summary>
    public partial class SubscriptionsDialog : Window
    {
        CollectionViewSource subscriptionsViewSource;
        BackloggerEntities context;
        string windowType;

        public SubscriptionsDialog(string type, BackloggerEntities context)
        {
            this.context = context;
            this.windowType = type;
            InitializeComponent();
            subscriptionsViewSource = ((CollectionViewSource)(FindResource("subscriptionsViewSource")));
            
            switch(windowType)
            {
                case "Books":
                    context.BooksSubscriptions.Load();
                    subscriptionsViewSource.Source = context.BooksSubscriptions.Local;
                    break;
                case "Movies":
                    context.MoviesSubscriptions.Load();
                    subscriptionsViewSource.Source = context.MoviesSubscriptions.Local;
                    break;
                case "Games":
                    context.GamesSubscriptions.Load();
                    subscriptionsViewSource.Source = context.GamesSubscriptions.Local;
                    break;
            }
        }

        private void AddSubcriptionButton_Click(object sender, RoutedEventArgs e)
        {
            AddSubscriptionDialog addSubscriptionDialog = new AddSubscriptionDialog(windowType, context);
            addSubscriptionDialog.Show();
            subscriptionsViewSource.View.Refresh();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            context.SaveChanges();
            this.Close();
        }
    }
}
