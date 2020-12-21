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
        string windowType;

        public SubscriptionsDialog(string type)
        {
            this.windowType = type;
            InitializeComponent();
            subscriptionsViewSource = ((CollectionViewSource)(FindResource("subscriptionsViewSource")));

            using (BackloggerEntities context = new BackloggerEntities())
            {
                switch (windowType)
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
        }

        private void AddSubcriptionButton_Click(object sender, RoutedEventArgs e)
        {
            using (BackloggerEntities context = new BackloggerEntities())
            {
                AddSubscriptionDialog addSubscriptionDialog = new AddSubscriptionDialog(windowType);
                addSubscriptionDialog.ShowDialog();
                switch (windowType)
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
                subscriptionsViewSource.View.Refresh();
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            using (BackloggerEntities context = new BackloggerEntities())
            {
                context.Hobbies.Load();
                context.Subscriptions.Load();

                Subscription s = null;
                int hobbyID = (from o in context.Hobbies.Local where o.HobbyName == windowType select o).FirstOrDefault().HobbyID;

                switch (windowType)
                {
                    case "Books":
                        var bs = (BooksSubscription)(SubscriptionsDataGrid.CurrentItem);
                        s = (from o in context.Subscriptions.Local
                             where o.SubscriptionName == bs.SubscriptionName && o.HobbyID == hobbyID && o.Price == bs.Price && o.IsActive == bs.IsActive
                             select o).FirstOrDefault();
                        break;
                    case "Movies":
                        var ms = (MoviesSubscription)(SubscriptionsDataGrid.CurrentItem);
                        s = (from o in context.Subscriptions.Local
                             where o.SubscriptionName == ms.SubscriptionName && o.HobbyID == hobbyID && o.Price == ms.Price && o.IsActive == ms.IsActive
                             select o).FirstOrDefault();
                        break;
                    case "Games":
                        var gs = (GamesSubscription)(SubscriptionsDataGrid.CurrentItem);
                        s = (from o in context.Subscriptions.Local
                             where o.SubscriptionName == gs.SubscriptionName && o.HobbyID == hobbyID && o.Price == gs.Price && o.IsActive == gs.IsActive
                             select o).FirstOrDefault();
                        break;
                }

                AddSubscriptionDialog addSubscriptionDialog = new AddSubscriptionDialog(windowType, s);
                addSubscriptionDialog.ShowDialog();

                switch (windowType)
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
                subscriptionsViewSource.View.Refresh();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this item?", "Delete confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                using (BackloggerEntities context = new BackloggerEntities())
                {
                    context.Hobbies.Load();
                    context.Subscriptions.Load();

                    Subscription s = null;
                    int hobbyID = (from o in context.Hobbies.Local where o.HobbyName == windowType select o).FirstOrDefault().HobbyID;

                    switch (windowType)
                    {
                        case "Books":
                            var bs = (BooksSubscription)(SubscriptionsDataGrid.CurrentItem);
                            s = (from o in context.Subscriptions.Local
                                 where o.SubscriptionName == bs.SubscriptionName && o.HobbyID == hobbyID && o.Price == bs.Price && o.IsActive == bs.IsActive
                                 select o).FirstOrDefault();
                            break;
                        case "Movies":
                            var ms = (MoviesSubscription)(SubscriptionsDataGrid.CurrentItem);
                            s = (from o in context.Subscriptions.Local
                                 where o.SubscriptionName == ms.SubscriptionName && o.HobbyID == hobbyID && o.Price == ms.Price && o.IsActive == ms.IsActive
                                 select o).FirstOrDefault();
                            break;
                        case "Games":
                            var gs = (GamesSubscription)(SubscriptionsDataGrid.CurrentItem);
                            s = (from o in context.Subscriptions.Local
                                 where o.SubscriptionName == gs.SubscriptionName && o.HobbyID == hobbyID && o.Price == gs.Price && o.IsActive == gs.IsActive
                                 select o).FirstOrDefault();
                            break;
                    }

                    context.Subscriptions.Remove(s);
                    context.SaveChanges();

                    switch (windowType)
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
                    subscriptionsViewSource.View.Refresh();
                }
            }
        }
    }
}
