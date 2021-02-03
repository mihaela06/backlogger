using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using Backlogger.Model;

namespace Backlogger.Windows
{
    /// <summary>
    /// Interaction logic for SuggestionsDialog.xaml
    /// </summary>
    public partial class SuggestionsDialog : Window
    {
        private string selHobby;
        private string selGenre;
        private string selStatus;
        private string selAuthor;
        private int hID;
        private Material selectedMaterial;
        public SuggestionsDialog()
        {
            InitializeComponent();
        }

        private void HobbyChoiceButton_Click(object sender, RoutedEventArgs e)
        {
            selHobby = ((sender as Button).Content as Label).Content.ToString();
            HobbySelectionGrid.Visibility = Visibility.Collapsed;
            StatusSelectionGrid.Visibility = Visibility.Visible;
        }

        private void RandomHobbyChoiceButton_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            int h = random.Next(1, 3);
            if (h == 1)
                selHobby = "Books";
            else if (h == 2)
                selHobby = "Movies";
            else
                selHobby = "Games";

            HobbySelectionGrid.Visibility = Visibility.Collapsed;
            StatusSelectionGrid.Visibility = Visibility.Visible;
        }
        private void StatusChoiceButton_Click(object sender, RoutedEventArgs e)
        {
            selStatus = null;
            if (e != null)
                switch (StatusComboBox.SelectedIndex)
                {
                    case 0:
                        selStatus = "Added";
                        break;
                    case 1:
                        selStatus = "On hold";
                        break;
                    case 2:
                        selStatus = "Finished";
                        break;
                    case 3:
                        selStatus = "Dropped";
                        break;
                }
            StatusSelectionGrid.Visibility = Visibility.Collapsed;
            GenreSelectionGrid.Visibility = Visibility.Visible;

            using (var context = new BackloggerEntities())
            {
                context.Materials.Load();
                context.Hobbies.Load();

                hID = (from o in context.Hobbies.Local
                       where o.HobbyName == selHobby
                       select o.HobbyID).FirstOrDefault();

                var filtered = (from o in context.Materials.Local
                                where o.HobbyID == hID
                                select o);

                if (selStatus != null)
                    filtered = (from o in filtered
                                where context.LastStatusUpdate(o.MaterialID).First().StatusName == selStatus
                                select o);

                foreach (var m in filtered.ToList())
                {
                    foreach (var g in m.Genres)
                    {
                        bool added = false;
                        foreach (var i in GenreComboBox.Items)
                            if (i.ToString() == g.GenreName)
                            {
                                added = true;
                                break;
                            }
                        if (!added)
                            GenreComboBox.Items.Add(g.GenreName);
                    }
                }
            }

            if (GenreComboBox.Items.Count == 0)
            {
                GenreSelectionGrid.Visibility = Visibility.Collapsed;
                NoResultGrid.Visibility = Visibility.Visible;
            }
            else
                GenreComboBox.SelectedIndex = 0;
        }

        private void RandomStatusChoiceButton_Click(object sender, RoutedEventArgs e)
        {
            StatusChoiceButton_Click(sender, null);
        }
        private void GenreChoiceButton_Click(object sender, RoutedEventArgs e)
        {
            selGenre = null;

            if (e != null)
                selGenre = GenreComboBox.SelectedItem.ToString();

            AuthorSelectionGrid.Visibility = Visibility.Visible;
            GenreSelectionGrid.Visibility = Visibility.Collapsed;

            using (var context = new BackloggerEntities())
            {
                context.Materials.Load();
                context.Hobbies.Load();
                context.Genres.Load();

                var filtered = (from o in context.Materials.Local
                                where o.HobbyID == hID
                                select o);

                if (selStatus != null)
                    filtered = (from o in filtered
                                where context.LastStatusUpdate(o.MaterialID).First().StatusName == selStatus
                                select o);


                if (selGenre != null)
                {
                    IEnumerable<Material> filteredG = new List<Material>();

                    foreach (var m in filtered.ToList())
                    {
                        var gr = m.Genres.ToList();
                        foreach (var g in gr)
                        {
                            if (g.GenreName == selGenre)
                            {
                                filteredG = filteredG.Append(m);
                                break;
                            }
                        }
                    }

                    filtered = filteredG;
                }

                foreach (var m in filtered.ToList())
                {
                    var al = (from o in context.Materials.Local
                              where o.MaterialID == m.MaterialID
                              select o.Authors).FirstOrDefault().ToList();
                    foreach (var a in al)
                    {
                        bool added = false;
                        foreach (var i in AuthorComboBox.Items)
                            if (i.ToString() == a.AuthorName)
                            {
                                added = true;
                                break;
                            }
                        if (!added)
                            AuthorComboBox.Items.Add(a.AuthorName);
                    }
                }
            }

            if (AuthorComboBox.Items.Count == 0)
            {
                AuthorSelectionGrid.Visibility = Visibility.Collapsed;
                NoResultGrid.Visibility = Visibility.Visible;
            }
            else
                AuthorComboBox.SelectedIndex = 0;
        }
        private void RandomGenreChoiceButton_Click(object sender, RoutedEventArgs e)
        {
            GenreChoiceButton_Click(sender, null);
        }

        private void AuthorChoiceButton_Click(object sender, RoutedEventArgs e)
        {
            selAuthor = null;
            if (e != null)
                selAuthor = AuthorComboBox.SelectedItem.ToString();

            AuthorSelectionGrid.Visibility = Visibility.Collapsed;

            using (var context = new BackloggerEntities())
            {
                context.Materials.Load();
                context.Hobbies.Load();
                context.Genres.Load();

                var filtered = (from o in context.Materials.Local
                                where o.HobbyID == hID
                                select o);

                if (selStatus != null)
                    filtered = (from o in filtered
                                where context.LastStatusUpdate(o.MaterialID).First().StatusName == selStatus
                                select o);


                if (selGenre != null)
                {
                    IEnumerable<Material> filteredG = new List<Material>();

                    foreach (var m in filtered.ToList())
                    {
                        var gr = m.Genres.ToList();
                        foreach (var g in gr)
                        {
                            if (g.GenreName == selGenre)
                            {
                                filteredG = filteredG.Append(m);
                                break;
                            }
                        }
                    }

                    filtered = filteredG;
                }

                if (selAuthor != null)
                {
                    IEnumerable<Material> filteredA = new List<Material>();

                    foreach (var m in filtered.ToList())
                    {
                        var al = m.Authors.ToList();
                        foreach (var a in al)
                        {
                            if (a.AuthorName == selAuthor)
                            {
                                filteredA = filteredA.Append(m);
                                break;
                            }
                        }
                    }

                    filtered = filteredA;
                }

                int no = filtered.ToList().Count();
                if (no == 0)
                    NoResultGrid.Visibility = Visibility.Visible;
                else
                    ResultsGrid.Visibility = Visibility.Visible;

                Random random = new Random();
                int h = random.Next(0, no);
                selectedMaterial = filtered.ToList()[h];

                RecLabel.Content = selectedMaterial.Title;
            }
        }

        private void RandomAuthorChoiceButton_Click(object sender, RoutedEventArgs e)
        {
            AuthorChoiceButton_Click(sender, null);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            int id = selectedMaterial.MaterialID;

            using (var context = new BackloggerEntities())
            {
                context.Materials.Load();
                context.Statuses.Load();

                var inProgress = from o in context.Materials
                                 from up in context.LastStatusUpdate(o.MaterialID)
                                 where o.MaterialID == up.MaterialID && up.StatusName == "In progress"
                                 select new
                                 {
                                     o.MaterialID,
                                     o.Title
                                 };

                if (inProgress.ToList().Count > 0)
                {
                    MessageBoxResult res = MessageBox.Show("You can't do two things at once, now, can you? :)\n" + inProgress.ToList()[0].Title + " is currently in progress, do you want to pause it?",
                        "Something else is in progress", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                    if (res == MessageBoxResult.Cancel)
                        return;
                    else
                    {
                        int onHoldID = (from s in context.Statuses
                                        where s.StatusName == "On hold"
                                        select s).FirstOrDefault().StatusID;

                        StatusUpdate suo = new StatusUpdate
                        {
                            MaterialID = inProgress.ToList()[0].MaterialID,
                            DateModified = DateTime.Now,
                            StatusID = onHoldID
                        };

                        context.StatusUpdates.Add(suo);
                        context.SaveChanges();
                    }
                }

                int progressID = (from s in context.Statuses
                                  where s.StatusName == "In progress"
                                  select s).FirstOrDefault().StatusID;

                StatusUpdate sun = new StatusUpdate
                {
                    MaterialID = id,
                    DateModified = DateTime.Now,
                    StatusID = progressID
                };

                context.StatusUpdates.Add(sun);
                context.SaveChanges();

                MessageBox.Show(selectedMaterial.Title + " is now in progress!", "Done!", MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
            }
        }

        private void LaterButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
