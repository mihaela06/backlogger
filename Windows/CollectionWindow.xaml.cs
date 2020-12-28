using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Backlogger.Model;

namespace Backlogger.Windows
{
    /// <summary>
    /// Interaction logic for CollectionWindow.xaml
    /// </summary>
    public partial class CollectionWindow : Window
    {
        public string windowType;
        CollectionViewSource materialsViewSource;
        CollectionViewSource updatesViewSource;

        private ListView titleFilterList = new ListView();
        private ListView authorFilterList = new ListView();
        private ListView genreFilterList = new ListView();
        private ListView priceFilterList = new ListView();
        private ListView subscriptionFilterList = new ListView();
        private ListView formatFilterList = new ListView();
        private ListView dateReleasedFilterList = new ListView();
        private ListView statusFilterList = new ListView();

        public CollectionWindow(string type)
        {
            windowType = type;
            InitializeComponent();
            materialsViewSource = ((CollectionViewSource)(FindResource("materialsViewSource")));
            updatesViewSource = ((CollectionViewSource)(FindResource("updatesViewSource")));
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
            object o = MaterialsDataGrid.SelectedItem;
            addDialog.ShowDialog();
            RefreshGrid();
            materialsViewSource.View.Refresh();
            MaterialsDataGrid.SelectedItem = o;
        }

        private void RefreshGrid()
        {
            using (BackloggerEntities context = new BackloggerEntities())
            {
                context.Hobbies.Load();
                context.Materials.Load();
                context.Subscriptions.Load();
                context.Statuses.Load();

                int hobbyID = (from o in context.Hobbies.Local where o.HobbyName == windowType select o).FirstOrDefault().HobbyID;

                List<char> titleChars = new List<char>();
                foreach (var i in titleFilterList.Items)
                {
                    var c = i as CheckBox;
                    if ((bool)c.IsChecked)
                    {
                        string s = c.Content.ToString();
                        for (int a = s[0]; a <= s[s.Length - 1]; a++)
                            titleChars.Add((char)a);
                    }
                }

                var result = from mt in context.Materials
                             from aut in context.ConcatenateAuthors(mt.MaterialID)
                             from gen in context.ConcatenateGenres(mt.MaterialID)
                             from up in context.LastStatusUpdate(mt.MaterialID)
                             from f in context.MaterialFormats
                             where mt.HobbyID == hobbyID && mt.MaterialFormatID == f.MaterialFormatID
                             select new
                             {
                                 mt.MaterialID,
                                 mt.Title,
                                 mt.Subscription,
                                 f.FormatType,
                                 mt.Price,
                                 mt.TimeSpent,
                                 mt.Rating,
                                 mt.DateReleased,
                                 aut.AuthorsList,
                                 gen.GenresList,
                                 up.StatusName,
                                 mt.Info
                             };

                var res = from mt in result
                          join s in context.Subscriptions on mt.Subscription equals s into sj
                          from ns in sj.DefaultIfEmpty()
                          select new
                          {
                              mt.MaterialID,
                              mt.Title,
                              mt.FormatType,
                              mt.Price,
                              mt.TimeSpent,
                              mt.Rating,
                              mt.DateReleased,
                              mt.AuthorsList,
                              mt.GenresList,
                              mt.StatusName,
                              mt.Info,
                              ns.SubscriptionName
                          };

                var filtered = res.ToList().Where(o => true);

                if (titleChars.Count > 0)
                    filtered = res.ToList().Where(o => titleChars.Contains(Char.ToUpper(o.Title[0])));

                if (NumberCheckboxesChecked(authorFilterList) > 0)
                    filtered = filtered.Where(IsByFilteredAuthor);

                if (NumberCheckboxesChecked(genreFilterList) > 0)
                    filtered = filtered.Where(HasFilteredGenre);

                if (NumberCheckboxesChecked(priceFilterList) > 0)
                    filtered = filtered.Where(IsInPriceRange);

                if (NumberCheckboxesChecked(subscriptionFilterList) > 0)
                    filtered = filtered.Where(FromFilteredSubscription);

                if (NumberCheckboxesChecked(formatFilterList) > 0)
                    filtered = filtered.Where(HasFilteredFormat);

                if (NumberCheckboxesChecked(dateReleasedFilterList) > 0)
                    filtered = filtered.Where(IsInDateReleasedRange);

                if (NumberCheckboxesChecked(statusFilterList) > 0)
                    filtered = filtered.Where(HasFilteredStatus);

                if (SearchTextBox.Text.Length > 0)
                    filtered = filtered.Where(ContainsSearchString);

                materialsViewSource.Source = filtered;
            }
        }

        private int NumberCheckboxesChecked(ListView lv)
        {
            int no = 0;
            foreach (var i in lv.Items)
            {
                var c = i as CheckBox;
                if ((bool)c.IsChecked)
                {
                    no++;
                }
            }

            return no;
        }
        private bool ContainsSearchString(dynamic m)
        {
            string s = SearchTextBox.Text;

            if (m.Title.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            if (m.AuthorsList.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            if (m.GenresList.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            if (m.SubscriptionName != null)
                if (m.SubscriptionName.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;

            if (m.FormatType.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            if (m.StatusName.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            return false;
        }
        private bool HasFilteredStatus(dynamic m)
        {
            string st = m.StatusName;

            foreach (var i in statusFilterList.Items)
            {
                var c = i as CheckBox;
                if ((bool)c.IsChecked)
                {
                    string s = c.Content.ToString();
                    if (s == st)
                        return true;
                }
            }
            return false;
        }
        private bool IsInDateReleasedRange(dynamic m)
        {
            DateTime? d = m.DateReleased;

            foreach (var i in dateReleasedFilterList.Items)
            {
                var c = i as CheckBox;
                if ((bool)c.IsChecked)
                {
                    string s = c.Content.ToString();
                    if (d == null && s == "Unknown")
                        return true;
                    if (d != null)
                        if (((DateTime)d).Year.ToString() == s)
                            return true;
                }
            }
            return false;
        }
        private bool HasFilteredFormat(dynamic m)
        {
            string f = m.FormatType;

            foreach (var i in formatFilterList.Items)
            {
                var c = i as CheckBox;
                if ((bool)c.IsChecked)
                {
                    string s = c.Content.ToString();
                    if (f == s)
                        return true;
                }
            }
            return false;
        }
        private bool FromFilteredSubscription(dynamic m)
        {
            string sn = m.SubscriptionName;

            foreach (var i in subscriptionFilterList.Items)
            {
                var c = i as CheckBox;
                if ((bool)c.IsChecked)
                {
                    string s = c.Content.ToString();
                    if (sn == null && s == "No subscription")
                        return true;
                    if (sn != null)
                        if (sn == s)
                            return true;
                }
            }
            return false;
        }
        private bool IsInPriceRange(dynamic m)
        {
            decimal? price = m.Price;

            foreach (var i in priceFilterList.Items)
            {
                var c = i as CheckBox;
                if ((bool)c.IsChecked)
                {
                    string s = c.Content.ToString();
                    if (!Char.IsDigit(s[0]))
                    {
                        if (s == "Free" && price == null)
                            return true;
                    }
                    else
                    {
                        decimal l = Convert.ToDecimal(s.Split('-')[0].Trim());
                        decimal u = Convert.ToDecimal(s.Split('-')[1].Trim());
                        if (l <= price && price <= u)
                            return true;
                    }
                }
            }
            return false;
        }

        private bool HasFilteredGenre(dynamic m)
        {
            var genres = m.GenresList.Split(',');

            foreach (var i in genreFilterList.Items)
            {
                var c = i as CheckBox;
                if ((bool)c.IsChecked)
                {
                    foreach (string s in genres)
                    {
                        if (s.Trim() == c.Content.ToString())
                            return true;
                    }
                }
            }
            return false;
        }

        private bool IsByFilteredAuthor(dynamic m)
        {
            var authors = m.AuthorsList.Split(',');

            foreach (var i in authorFilterList.Items)
            {
                var c = i as CheckBox;
                if ((bool)c.IsChecked)
                {
                    foreach (string s in authors)
                    {
                        if (s.Trim() == c.Content.ToString())
                            return true;
                    }
                }
            }
            return false;
        }

        private void CollectionWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
            InitializeFilters();
        }

        private void InitializeFilters()
        {
            foreach (dynamic i in MaterialsDataGrid.Items)
            {
                string t = i.Title;
                if (Char.IsDigit(t[0]))
                    AddCheckBoxToList(titleFilterList, "0 - 9");
                else if ('V' <= Char.ToUpper(t[0]) && Char.ToUpper(t[0]) <= 'Z')
                    AddCheckBoxToList(titleFilterList, "V - Z");
                else if (Char.IsLetter(t[0]))
                {
                    int c = (int)Char.ToUpper(t[0]);
                    if (c % 3 == 2)
                        AddCheckBoxToList(titleFilterList, ((char)c).ToString() + " - " + ((char)(c + 2)).ToString());
                    else if (c % 3 == 0)
                        AddCheckBoxToList(titleFilterList, ((char)(c - 1)).ToString() + " - " + ((char)(c + 1)).ToString());
                    else
                        AddCheckBoxToList(titleFilterList, ((char)(c - 2)).ToString() + " - " + ((char)(c)).ToString());
                }
                else
                    AddCheckBoxToList(titleFilterList, t[0].ToString());

                decimal? p = i.Price;

                if (p == null)
                    AddCheckBoxToList(priceFilterList, "Free");
                else
                {
                    int u = (int)Decimal.Floor((decimal)p);

                    int l = (u / 25) * 25;
                    AddCheckBoxToList(priceFilterList, l.ToString() + " - " + (l + 24.99).ToString());
                }

                string s = i.AuthorsList;
                foreach (string a in s.Split(','))
                    AddCheckBoxToList(authorFilterList, a);

                string g = i.GenresList;
                foreach (string a in g.Split(','))
                    AddCheckBoxToList(genreFilterList, a);

                string f = i.FormatType;
                AddCheckBoxToList(formatFilterList, f);

                DateTime? r = i.DateReleased;
                if (r != null)
                    AddCheckBoxToList(dateReleasedFilterList, ((DateTime)r).Year.ToString());
                else
                    AddCheckBoxToList(dateReleasedFilterList, "Unknown");

                string ls = i.StatusName;
                AddCheckBoxToList(statusFilterList, ls);

                string sn = i.SubscriptionName;
                if (sn != null)
                    AddCheckBoxToList(subscriptionFilterList, sn);
                else
                    AddCheckBoxToList(subscriptionFilterList, "No subscription");
            }
        }

        private void DeleteObsoleteFilters()
        {
            List<CheckBox> lc = new List<CheckBox>();
            foreach (var ch in authorFilterList.Items)
            {
                var c = ch as CheckBox;
                bool relevant = false;
                foreach (dynamic i in MaterialsDataGrid.Items)
                {
                    string s = i.AuthorsList;
                    foreach (string a in s.Split(','))
                        if (a.Trim() == c.Content.ToString())
                        {
                            relevant = true;
                            break;
                        }
                }
                if (!relevant)
                    lc.Add(c);
            }
            foreach (var c in lc)
            {
                if ((bool)c.IsChecked)
                    c.IsChecked = false;
                authorFilterList.Items.Remove(c);
            }

            lc.Clear();

            foreach (var ch in genreFilterList.Items)
            {
                var c = ch as CheckBox;
                bool relevant = false;
                foreach (dynamic i in MaterialsDataGrid.Items)
                {
                    string s = i.GenresList;
                    foreach (string a in s.Split(','))
                        if (a.Trim() == c.Content.ToString())
                        {
                            relevant = true;
                            break;
                        }
                }
                if (!relevant)
                    lc.Add(c);
            }
            foreach (var c in lc)
            {
                if ((bool)c.IsChecked)
                    c.IsChecked = false;
                genreFilterList.Items.Remove(c);
            }

            lc.Clear();

            foreach (var ch in titleFilterList.Items)
            {
                var c = ch as CheckBox;
                bool relevant = false;
                foreach (dynamic i in MaterialsDataGrid.Items)
                {
                    string s = i.Title;
                    char l = c.Content.ToString()[0];
                    char u = c.Content.ToString()[c.Content.ToString().Length - 1];
                    if (l <= Char.ToUpper(s[0]) && Char.ToUpper(s[0]) <= u)
                    {
                        relevant = true;
                        break;
                    }
                }
                if (!relevant)
                    lc.Add(c);
            }
            foreach (var c in lc)
            {
                if ((bool)c.IsChecked)
                    c.IsChecked = false;
                titleFilterList.Items.Remove(c);
            }

            lc.Clear();

            foreach (var ch in priceFilterList.Items)
            {
                var c = ch as CheckBox;
                bool relevant = false;
                foreach (dynamic i in MaterialsDataGrid.Items)
                {
                    decimal? s = i.Price;
                    if (s == null && c.Content.ToString() == "Free")
                    {
                        relevant = true;
                        break;
                    }
                    else if (s != null)
                    {
                        decimal l = Convert.ToDecimal(c.Content.ToString().Split('-')[0].Trim());
                        decimal u = Convert.ToDecimal(c.Content.ToString().Split('-')[1].Trim());
                        if (l <= s && s <= u)
                        {
                            relevant = true;
                            break;
                        }
                    }
                }
                if (!relevant)
                    lc.Add(c);
            }
            foreach (var c in lc)
            {
                if ((bool)c.IsChecked)
                    c.IsChecked = false;
                priceFilterList.Items.Remove(c);
            }

            lc.Clear();

            foreach (var ch in subscriptionFilterList.Items)
            {
                var c = ch as CheckBox;
                bool relevant = false;
                foreach (dynamic i in MaterialsDataGrid.Items)
                {
                    string s = i.SubscriptionName;
                    if (s == null && c.Content.ToString() == "No subscription")
                    {
                        relevant = true;
                        break;
                    }
                    else if (s != null)
                    {
                        if (s == c.Content.ToString())
                        {
                            relevant = true;
                            break;
                        }
                    }
                }
                if (!relevant)
                    lc.Add(c);
            }
            foreach (var c in lc)
            {
                if ((bool)c.IsChecked)
                    c.IsChecked = false;
                subscriptionFilterList.Items.Remove(c);
            }

            lc.Clear();

            foreach (var ch in formatFilterList.Items)
            {
                var c = ch as CheckBox;
                bool relevant = false;
                foreach (dynamic i in MaterialsDataGrid.Items)
                {
                    string s = i.FormatType;
                    if (s == c.Content.ToString())
                    {
                        relevant = true;
                        break;
                    }
                }
                if (!relevant)
                    lc.Add(c);
            }
            foreach (var c in lc)
            {
                if ((bool)c.IsChecked)
                    c.IsChecked = false;
                formatFilterList.Items.Remove(c);
            }


            lc.Clear();

            foreach (var ch in statusFilterList.Items)
            {
                var c = ch as CheckBox;
                bool relevant = false;
                foreach (dynamic i in MaterialsDataGrid.Items)
                {
                    string s = i.StatusName;
                    if (s == c.Content.ToString())
                    {
                        relevant = true;
                        break;
                    }
                }
                if (!relevant)
                    lc.Add(c);
            }
            foreach (var c in lc)
            {
                if ((bool)c.IsChecked)
                    c.IsChecked = false;
                statusFilterList.Items.Remove(c);
            }


            lc.Clear();

            foreach (var ch in dateReleasedFilterList.Items)
            {
                var c = ch as CheckBox;
                bool relevant = false;
                foreach (dynamic i in MaterialsDataGrid.Items)
                {
                    DateTime? s = i.DateReleased;
                    if (s == null && c.Content.ToString() == "Unknown")
                    {
                        relevant = true;
                        break;
                    }
                    if (s != null)
                    {
                        if (((DateTime)s).Year.ToString() == c.Content.ToString())
                        {
                            relevant = true;
                            break;
                        }
                    }
                }
                if (!relevant)
                    lc.Add(c);
            }
            foreach (var c in lc)
            {
                if ((bool)c.IsChecked)
                    c.IsChecked = false;
                dateReleasedFilterList.Items.Remove(c);
            }

        }

        private void AddCheckBoxToList(ListView lv, string s)
        {
            s = s.Trim();
            foreach (CheckBox e in lv.Items)
                if (e.Content.ToString() == s)
                    return;

            var c = new CheckBox();
            c.Checked += new RoutedEventHandler(FilterChanged);
            c.Unchecked += new RoutedEventHandler(FilterChanged);
            c.Content = s;
            int index = 0;
            for (index = 0; index < lv.Items.Count; index++)
            {
                string sc = (lv.Items.GetItemAt(index) as CheckBox).Content.ToString();

                if (lv == priceFilterList)
                {
                    if (s == "Free")
                    {
                        index = 0;
                        break;
                    }
                    if (sc == "Free")
                        continue;
                    int l = Convert.ToInt32(sc.Split('-')[0].Trim());
                    int p = Convert.ToInt32(s.Split('-')[0].Trim());
                    if (l > p)
                        break;
                }
                else if (String.Compare(sc, s, StringComparison.InvariantCulture) > 0)
                    break;
            }
            lv.Items.Insert(index, c);
        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
            var c = sender as CheckBox;
            var lv = c.Parent as ListView;
            var b = lv.Parent as Border;
            var p = b.Parent as Popup;
            var dp = p.Parent as Grid;

            var button = dp.Children[1] as Button;

            int noChecked = 0;
            foreach (var i in lv.Items)
            {
                var ch = i as CheckBox;
                if ((bool)ch.IsChecked)
                    noChecked++;
            }
            if (noChecked == 0)
                button.Background = Brushes.Transparent;
            else
                button.Background = Brushes.LightSalmon;
        }

        private void SubscriptionsButton_Click(object sender, RoutedEventArgs e)
        {
            var subDialog = new SubscriptionsDialog(windowType);
            object o = MaterialsDataGrid.SelectedItem;
            subDialog.ShowDialog();
            materialsViewSource.View.Refresh();
            MaterialsDataGrid.SelectedItem = o;
        }
        private void MaterialsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic i = MaterialsDataGrid.SelectedItem;

            if (i == null)
            {
                CoverImage.Visibility = Visibility.Hidden;
                TextBoxDetails.Visibility = Visibility.Hidden;
                ImageRating.Visibility = Visibility.Hidden;
                ButtonStars1.IsEnabled = false;
                ButtonStars2.IsEnabled = false;
                ButtonStars3.IsEnabled = false;
                ButtonStars4.IsEnabled = false;
                ButtonStars5.IsEnabled = false;
                ButtonStars6.IsEnabled = false;
                ButtonStars7.IsEnabled = false;
                ButtonStars8.IsEnabled = false;
                ButtonStars9.IsEnabled = false;
                ButtonStars10.IsEnabled = false;
                UpdatesDataGrid.Visibility = Visibility.Hidden;

                return;
            }

            int id = i.MaterialID;

            string defaultFile = windowType.Remove(windowType.Length - 1);

            CoverImage.Visibility = Visibility.Visible;
            ImageRating.Visibility = Visibility.Visible;
            UpdatesDataGrid.Visibility = Visibility.Visible;

            ButtonStars1.IsEnabled = true;
            ButtonStars2.IsEnabled = true;
            ButtonStars3.IsEnabled = true;
            ButtonStars4.IsEnabled = true;
            ButtonStars5.IsEnabled = true;
            ButtonStars6.IsEnabled = true;
            ButtonStars7.IsEnabled = true;
            ButtonStars8.IsEnabled = true;
            ButtonStars9.IsEnabled = true;
            ButtonStars10.IsEnabled = true;

            using (var context = new BackloggerEntities())
            {
                var updates = from s in context.Statuses
                              from su in context.StatusUpdates
                              where id == su.MaterialID && s.StatusID == su.StatusID
                              orderby su.DateModified ascending
                              select new
                              {
                                  s.StatusName,
                                  su.DateModified
                              };

                updatesViewSource.Source = updates.ToList();
            }


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
            else if (File.Exists(App.projectPath + @"\Resources\Icons\" + defaultFile + ".png"))
                img = new BitmapImage(new Uri(App.projectPath + @"\Resources\Icons\" + defaultFile + ".png"));
            else
                CoverImage.Visibility = Visibility.Hidden;

            CoverImage.Source = img;

            TextBoxDetails.Visibility = Visibility.Visible;
            TextBoxDetails.Text = i.Info;
            TextBoxDetails.ScrollToHome();
            ReloadStars(i.Rating);
        }

        private void ReloadStars(short? rating)
        {
            switch (rating)
            {
                case null:
                    ImageRating.Source = new BitmapImage(new Uri(App.projectPath + @"\Resources\Icons\0stars.png"));
                    break;
                case 1:
                    ImageRating.Source = new BitmapImage(new Uri(App.projectPath + @"\Resources\Icons\1stars.png"));
                    break;
                case 2:
                    ImageRating.Source = new BitmapImage(new Uri(App.projectPath + @"\Resources\Icons\2stars.png"));
                    break;
                case 3:
                    ImageRating.Source = new BitmapImage(new Uri(App.projectPath + @"\Resources\Icons\3stars.png"));
                    break;
                case 4:
                    ImageRating.Source = new BitmapImage(new Uri(App.projectPath + @"\Resources\Icons\4stars.png"));
                    break;
                case 5:
                    ImageRating.Source = new BitmapImage(new Uri(App.projectPath + @"\Resources\Icons\5stars.png"));
                    break;
                case 6:
                    ImageRating.Source = new BitmapImage(new Uri(App.projectPath + @"\Resources\Icons\6stars.png"));
                    break;
                case 7:
                    ImageRating.Source = new BitmapImage(new Uri(App.projectPath + @"\Resources\Icons\7stars.png"));
                    break;
                case 8:
                    ImageRating.Source = new BitmapImage(new Uri(App.projectPath + @"\Resources\Icons\8stars.png"));
                    break;
                case 9:
                    ImageRating.Source = new BitmapImage(new Uri(App.projectPath + @"\Resources\Icons\9stars.png"));
                    break;
                case 10:
                    ImageRating.Source = new BitmapImage(new Uri(App.projectPath + @"\Resources\Icons\10stars.png"));
                    break;
            }
        }

        private void ButtonStars_MouseEnter(object sender, MouseEventArgs e)
        {
            var b = e.Source as Button;
            string name = b.Name;

            int no = 10;

            if (name[name.Length - 2] == 's')
                no = name[name.Length - 1] - '0';

            ImageRating.Source = new BitmapImage(new Uri(App.projectPath + @"\Resources\Icons\" + no.ToString() + "stars.png"));
        }

        private void ButtonStars_MouseLeave(object sender, MouseEventArgs e)
        {
            dynamic i = MaterialsDataGrid.SelectedItem;
            int? no;
            if (i != null)
                no = i.Rating;
            else
                no = null;

            if (no != null)
                ImageRating.Source = new BitmapImage(new Uri(App.projectPath + @"\Resources\Icons\" + no.ToString() + "stars.png"));
            else
                ImageRating.Source = new BitmapImage(new Uri(App.projectPath + @"\Resources\Icons\0stars.png"));
        }
        private void ButtonStars_Click(object sender, RoutedEventArgs e)
        {
            var b = e.Source as Button;
            string name = b.Name;

            int no = 10;

            if (name[name.Length - 2] == 's')
                no = name[name.Length - 1] - '0';

            dynamic i = MaterialsDataGrid.SelectedItem;
            if (i == null)
                return;

            int oldIndex = MaterialsDataGrid.SelectedIndex;

            using (BackloggerEntities context = new BackloggerEntities())
            {
                context.Hobbies.Load();
                context.Materials.Load();
                int hobbyID = (from o in context.Hobbies.Local where o.HobbyName == windowType select o).FirstOrDefault().HobbyID;

                Material m = (from o in context.Materials.Local where o.MaterialID == i.MaterialID select o).FirstOrDefault();

                m.Rating = (short?)no;
                context.SaveChanges();
            }
            RefreshGrid();
            MaterialsDataGrid.SelectedIndex = oldIndex;
            ReloadStars((short?)no);
        }

        private void TextBoxDetails_LostFocus(object sender, RoutedEventArgs e)
        {
            dynamic i = MaterialsDataGrid.SelectedItem;
            string info = i.Info;

            if (info != TextBoxDetails.Text)
            {
                using (BackloggerEntities context = new BackloggerEntities())
                {
                    context.Materials.Load();
                    Material m = (from o in context.Materials.Local where o.MaterialID == i.MaterialID select o).FirstOrDefault();

                    m.Info = TextBoxDetails.Text;
                    context.SaveChanges();
                }
                RefreshGrid();
            }
        }

        private void EditMaterial_Click(object sender, RoutedEventArgs e)
        {
            dynamic i = MaterialsDataGrid.SelectedItem;

            DateTime? dateAdded;

            using (BackloggerEntities context = new BackloggerEntities())
            {
                context.Materials.Load();

                Material m = (from o in context.Materials.Local where o.MaterialID == i.MaterialID select o).FirstOrDefault();

                context.StatusUpdates.Load();
                context.Statuses.Load();

                int sid = (from o in context.Statuses.Local where o.StatusName == "Added" select o).FirstOrDefault().StatusID;

                StatusUpdate su = (from o in context.StatusUpdates.Local where o.MaterialID == m.MaterialID && o.StatusID == sid select o).FirstOrDefault();
                dateAdded = su.DateModified;
            }

            AddDialog editDialog = new AddDialog(windowType, i.MaterialID, i.Title, i.AuthorsList, i.GenresList, i.FormatType, i.Price, i.SubscriptionName, i.DateReleased, i.Info, dateAdded);
            editDialog.ShowDialog();

            RefreshGrid();
            DeleteObsoleteFilters();
            InitializeFilters();
            MaterialsDataGrid_SelectionChanged(null, null);
        }

        private void DeleteMaterial_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this item?", "Deleting confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                using (BackloggerEntities context = new BackloggerEntities())
                {
                    dynamic i = MaterialsDataGrid.SelectedItem;

                    context.Materials.Load();
                    Material m = (from o in context.Materials.Local where o.MaterialID == i.MaterialID select o).FirstOrDefault();

                    context.Materials.Remove(m);
                    context.SaveChanges();

                    string fileName = App.projectPath + @"\Resources\Images\" + m.MaterialID.ToString();
                    List<string> extensions = new List<string> { ".png", ".jpg", ".jpeg", ".gif" };

                    foreach (var ext in extensions)
                    {
                        if (File.Exists(fileName + ext))
                        {
                            File.Delete(fileName + ext);
                        }
                    }
                    RefreshGrid();
                    DeleteObsoleteFilters();
                    InitializeFilters();
                    MaterialsDataGrid_SelectionChanged(null, null);
                }
            }
        }
        private void FilterPopup_Opened(object sender, EventArgs e)
        {
            var s = sender as Popup;
            var bd = s.Child as Border;
            var p = s.Parent as Grid;
            var c = p.Children[0] as TextBlock;

            string columnText = (string)c.Text;

            switch (columnText)
            {
                case "Title":
                    bd.Child = titleFilterList;
                    break;
                case "Authors":
                    bd.Child = authorFilterList;
                    break;
                case "Genres":
                    bd.Child = genreFilterList;
                    break;
                case "Price":
                    bd.Child = priceFilterList;
                    break;
                case "Subscription":
                    bd.Child = subscriptionFilterList;
                    break;
                case "Format":
                    bd.Child = formatFilterList;
                    break;
                case "Date released":
                    bd.Child = dateReleasedFilterList;
                    break;
                case "Current status":
                    bd.Child = statusFilterList;
                    break;
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            var s = sender as Button;
            var dp = s.Parent as Grid;
            var p = dp.Children[2] as Popup;
            p.IsOpen = true;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            var b = sender as Border;
            (b.Parent as Popup).IsOpen = false;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                SearchButton_Click(null, null);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (SearchTextBox.Text.Length == 0)
            SearchButton_Click(null, null);
        }

        private void EditStatusUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteStatusUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddStatusUpdate_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
