using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

                //var result = from mt in context.Materials
                //             from aut in context.ConcatenateAuthors(mt.MaterialID)
                //             from gen in context.ConcatenateGenres(mt.MaterialID)
                //             from f in context.MaterialFormats
                //             from s in context.Subscriptions
                //             where mt.HobbyID == hobbyID && mt.MaterialFormatID == f.MaterialFormatID && s.SubscriptionID == mt.SubscriptionID
                //             select new { mt.MaterialID, mt.Title, mt.MaterialFormat, mt.Price, mt.TimeSpent, mt.Rating, mt.DateReleased, aut.AuthorsList, gen.GenresList, f.FormatType, s.SubscriptionName};

                var result = from mt in context.Materials
                             from aut in context.ConcatenateAuthors(mt.MaterialID)
                             from gen in context.ConcatenateGenres(mt.MaterialID)
                             from up in context.LastStatusUpdate(mt.MaterialID)
                             from f in context.MaterialFormats
                             where mt.HobbyID == hobbyID && mt.MaterialFormatID == f.MaterialFormatID
                             select new { mt.MaterialID, mt.Title, mt.Subscription, f.FormatType, mt.Price, mt.TimeSpent, mt.Rating, mt.DateReleased, aut.AuthorsList, gen.GenresList, up.StatusName, mt.Info };

                var res = from mt in result
                          join s in context.Subscriptions on mt.Subscription equals s into sj
                          from ns in sj.DefaultIfEmpty()
                          select new { mt.MaterialID, mt.Title, mt.FormatType, mt.Price, mt.TimeSpent, mt.Rating, mt.DateReleased, mt.AuthorsList, mt.GenresList, mt.StatusName, mt.Info, SubscriptionName = ns.SubscriptionName ?? String.Empty };

                materialsViewSource.Source = res.ToList();
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
        private void MaterialsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var i = e.AddedItems[0] as Material;
            var dg = sender as DataGrid;
            dynamic i = dg.SelectedItem;

            if (i == null)
            {
                CoverImage.Visibility = Visibility.Hidden;
                TextBoxDetails.Visibility = Visibility.Hidden;

                return;
            }

            int id = i.MaterialID;

            string defaultFile = windowType.Remove(windowType.Length - 1);

            CoverImage.Visibility = Visibility.Visible;

            BitmapImage img = null;
            string pathImages = App.projectPath + @"\Resources\Images\";

            if (File.Exists(pathImages + id.ToString() + ".png"))
                img = new BitmapImage(new Uri(pathImages + id.ToString() + ".png"));
            else if (File.Exists(pathImages + id.ToString() + ".jpg"))
                img = new BitmapImage(new Uri(pathImages + id.ToString() + ".jpg"));
            else if (File.Exists(pathImages + id.ToString() + ".jpeg"))
                img = new BitmapImage(new Uri(pathImages + id.ToString() + ".jpeg"));
            else if (File.Exists(pathImages + id.ToString() + ".gif"))
                img = new BitmapImage(new Uri(pathImages + id.ToString() + ".gif"));
            else if (File.Exists(pathImages + defaultFile + ".png"))
                img = new BitmapImage(new Uri(pathImages + defaultFile + ".png"));
            else
                CoverImage.Visibility = Visibility.Hidden;

            CoverImage.Source = img;

            TextBoxDetails.Visibility = Visibility.Visible;
            TextBoxDetails.Text = i.Info;
        }
    }
}
