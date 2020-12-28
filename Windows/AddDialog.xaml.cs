using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Backlogger.Model;

namespace Backlogger.Windows
{
    /// <summary>
    /// Interaction logic for AddDialog.xaml
    /// </summary>
    public partial class AddDialog : Window
    {
        public string windowType;
        private string wholePath;
        private int editID;
        public AddDialog(string type)
        {
            windowType = type;
            InitializeComponent();
            List<string> options = new List<string>
            {
                "None"
            };
            using (BackloggerEntities context = new BackloggerEntities())
            {
                switch (windowType)
                {
                    case "Books":
                        context.BooksSubscriptions.Load();
                        foreach (var s in context.BooksSubscriptions.Local)
                            if (s.IsActive)
                                options.Add(s.SubscriptionName);
                        break;
                    case "Movies":
                        context.MoviesSubscriptions.Load();
                        foreach (var s in context.MoviesSubscriptions.Local)
                            if (s.IsActive)
                                options.Add(s.SubscriptionName);
                        break;
                    case "Games":
                        context.GamesSubscriptions.Load();
                        foreach (var s in context.GamesSubscriptions.Local)
                            if (s.IsActive)
                                options.Add(s.SubscriptionName);
                        break;
                }
            }
            ComboBoxSubscription.ItemsSource = options;
            ComboBoxSubscription.SelectedIndex = 0;
            DatePickerDateAdded.DefaultValue = DateTime.Now;
            editID = -1;
        }

        public AddDialog(string type, int materialID, string title, string authors, string genres, string format, decimal? price,
                        string subName, DateTime? dateReleased, string details, DateTime? dateAdded)
        {
            windowType = type;
            InitializeComponent();
            List<string> options = new List<string>
            {
                "None"
            };
            using (BackloggerEntities context = new BackloggerEntities())
            {
                switch (windowType)
                {
                    case "Books":
                        context.BooksSubscriptions.Load();
                        foreach (var s in context.BooksSubscriptions.Local)
                            if (s.IsActive)
                                options.Add(s.SubscriptionName);
                        break;
                    case "Movies":
                        context.MoviesSubscriptions.Load();
                        foreach (var s in context.MoviesSubscriptions.Local)
                            if (s.IsActive)
                                options.Add(s.SubscriptionName);
                        break;
                    case "Games":
                        context.GamesSubscriptions.Load();
                        foreach (var s in context.GamesSubscriptions.Local)
                            if (s.IsActive)
                                options.Add(s.SubscriptionName);
                        break;
                }
            }
            ComboBoxSubscription.ItemsSource = options;
            TextBoxAuthor.Text = authors;
            TextBoxGenre.Text = genres;
            TextBoxInfo.Text = details;
            TextBoxMaterialFormat.Text = format;
            TextBoxPrice.Text = price.ToString();
            TextBoxTitle.Text = title;
            DatePickerDateAdded.DefaultValue = dateAdded;
            DatePickerDateReleased.SelectedDate = dateReleased;

            bool p(string s) { return s == subName; }
            ComboBoxSubscription.SelectedIndex = options.FindIndex(p);

            editID = materialID;

            string fileName = App.projectPath + @"\Resources\Images\" + editID.ToString();
            List<string> extensions = new List<string> { ".png", ".jpg", ".jpeg", ".gif" };

            foreach(var e in extensions)
            {
                if(File.Exists(fileName + e))
                {
                    var s = fileName + e;
                    wholePath = s;
                    if (s.Length < 60)
                        LabelFileSelected.Content = s;
                    else
                        LabelFileSelected.Content = s.Substring(0, 40) + "..." + s.Substring(s.Length - 25);
                    break;
                }
            }

        }

        private void ButtonSaveNew_Click(object sender, RoutedEventArgs e)
        {
            using (BackloggerEntities context = new BackloggerEntities())
            {
                context.Hobbies.Load();
                context.Materials.Load();
                context.MaterialFormats.Load();
                context.Authors.Load();
                context.Genres.Load();
                context.Statuses.Load();
                context.StatusUpdates.Load();
                context.Subscriptions.Load();

                int hobbyID = (from o in context.Hobbies.Local where o.HobbyName == windowType select o).FirstOrDefault().HobbyID;
                int? subID = 0;
                try
                {
                    subID = (from o in context.Subscriptions.Local
                             where
                             o.SubscriptionName == ComboBoxSubscription.SelectedItem.ToString() && o.HobbyID == hobbyID
                             select o).FirstOrDefault().SubscriptionID;
                }
                catch
                {
                    subID = null;
                }

                Decimal? price = null;
                if (TextBoxPrice.Text != "")
                    price = Convert.ToDecimal(TextBoxPrice.Text.Trim());

                string info = null;
                if (TextBoxInfo.Text != "")
                    info = TextBoxInfo.Text;

                int materialFormatID;
                try
                {
                    materialFormatID = (from o in context.MaterialFormats.Local where o.FormatType == TextBoxMaterialFormat.Text.Trim() select o).FirstOrDefault().MaterialFormatID;
                }
                catch
                {
                    MaterialFormat materialFormat = new MaterialFormat()
                    {
                        FormatType = TextBoxMaterialFormat.Text.Trim()
                    };

                    context.MaterialFormats.Add(materialFormat);
                    context.SaveChanges();
                    materialFormatID = (from o in context.MaterialFormats.Local where o.FormatType == TextBoxMaterialFormat.Text.Trim() select o).FirstOrDefault().MaterialFormatID;
                }

                ObservableCollection<Author> authors = new ObservableCollection<Author>();
                ObservableCollection<Genre> genres = new ObservableCollection<Genre>();

                foreach (var a in TextBoxAuthor.Text.Split(','))
                {
                    var strTrimmed = a.Trim();
                    int id;
                    try
                    {
                        id = (from o in context.Authors.Local where o.AuthorName == strTrimmed select o).FirstOrDefault().AuthorID;
                    }
                    catch
                    {
                        Author author = new Author
                        {
                            AuthorName = strTrimmed
                        };
                        context.Authors.Add(author);
                        context.SaveChanges();
                        id = (from o in context.Authors.Local where o.AuthorName == strTrimmed select o).FirstOrDefault().AuthorID;
                    }
                    authors.Add((from o in context.Authors.Local where o.AuthorID == id select o).FirstOrDefault());
                }

                foreach (var a in TextBoxGenre.Text.Split(','))
                {
                    var strTrimmed = a.Trim();
                    int id; 
                    try
                    {
                        id = (from o in context.Genres.Local where o.GenreName == strTrimmed select o).FirstOrDefault().GenreID;
                    }
                    catch
                    {
                        Genre genre = new Genre
                        {
                            GenreName = strTrimmed
                        };
                        context.Genres.Add(genre);
                        context.SaveChanges();
                        id = (from o in context.Genres.Local where o.GenreName == strTrimmed select o).FirstOrDefault().GenreID;
                    }
                    genres.Add((from o in context.Genres.Local where o.GenreID == id select o).FirstOrDefault());
                }

                Material newMaterial = null;

                if (editID == -1)

                {
                    newMaterial = new Material()
                    {
                        HobbyID = hobbyID,
                        Title = TextBoxTitle.Text.Trim(),
                        MaterialFormatID = materialFormatID,
                        Price = price,
                        DateReleased = DatePickerDateReleased.SelectedDate,
                        Info = TextBoxInfo.Text.Trim(),
                        Authors = authors,
                        Genres = genres,
                        SubscriptionID = subID

                        // should check for correct format
                    };

                    context.Materials.Add(newMaterial);
                    context.SaveChanges();

                    int statusID = (from o in context.Statuses.Local where o.StatusName == "Added" select o).FirstOrDefault().StatusID;

                    StatusUpdate statusUpdate = new StatusUpdate()
                    {
                        MaterialID = newMaterial.MaterialID,
                        DateModified = (DateTime)DatePickerDateAdded.Value,
                        StatusID = statusID
                    };

                    context.StatusUpdates.Add(statusUpdate);
                    context.SaveChanges();

                    // should implement file reset
                }
                else
                {
                    newMaterial = (from o in context.Materials.Local where o.MaterialID == editID select o).FirstOrDefault();

                    var obsoleteAuthors = new ObservableCollection<Author>();

                    foreach(Author a in newMaterial.Authors)
                        if (authors.IndexOf(a) == -1)
                            obsoleteAuthors.Add(a);
                  
                    foreach (Author a in obsoleteAuthors)
                        newMaterial.Authors.Remove(a);

                    foreach (Author a in authors)
                        if (newMaterial.Authors.IndexOf(a) == -1)
                            newMaterial.Authors.Add(a);

                    var obsoleteGenres = new ObservableCollection<Genre>();

                    foreach (Genre g in newMaterial.Genres)
                        if (genres.IndexOf(g) == -1)
                            obsoleteGenres.Add(g);

                    foreach (Genre g in obsoleteGenres)
                        newMaterial.Genres.Remove(g);

                    foreach (Genre g in genres)
                        if (newMaterial.Genres.IndexOf(g) == -1)
                            newMaterial.Genres.Add(g);

                    context.SaveChanges();

                    newMaterial.Authors = authors;
                    newMaterial.Genres = genres;
                    newMaterial.Title = TextBoxTitle.Text;
                    newMaterial.Price = price;
                    newMaterial.DateReleased = DatePickerDateReleased.SelectedDate;
                    newMaterial.Info = TextBoxInfo.Text;
                    newMaterial.SubscriptionID = subID;
                    newMaterial.MaterialFormatID = materialFormatID;

                    int statusID = (from o in context.Statuses.Local where o.StatusName == "Added" select o).FirstOrDefault().StatusID;
                    StatusUpdate addedUpdate = (from o in context.StatusUpdates.Local
                                                where o.StatusID == statusID && o.MaterialID == editID
                                                select o).FirstOrDefault();

                    if(addedUpdate.DateModified != (DateTime)DatePickerDateAdded.Value)
                        addedUpdate.DateModified = (DateTime)DatePickerDateAdded.Value;

                    context.SaveChanges();
                }

                if (LabelFileSelected.Content.ToString() != "No file selected")
                {
                    int id = newMaterial.MaterialID;

                    string path = LabelFileSelected.Content.ToString();
                    if (path.IndexOf("http") == 0)
                    {
                        Uri uri = new Uri(wholePath);

                        var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
                        var fileExtension = System.IO.Path.GetExtension(uriWithoutQuery);

                        using (WebClient client = new WebClient())
                        {
                            client.DownloadFile(uri, App.projectPath + @"\Resources\Images\" + id + fileExtension);
                        }
                    }
                    else
                    {
                        var fileExtension = System.IO.Path.GetExtension(wholePath);

                        if(wholePath != App.projectPath + @"\Resources\Images\" + id + fileExtension)
                            File.Copy(wholePath, App.projectPath + @"\Resources\Images\" + id + fileExtension);
                    }
                }
                this.Close();
            }
        }

        private void ButtonCancelNew_Click(object sender, RoutedEventArgs e)
        {
            TextBoxAuthor.Text = "";
            TextBoxGenre.Text = "";
            TextBoxTitle.Text = "";
            TextBoxInfo.Text = "";

            this.Close();
        }

        private void ComboBoxSubscription_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxSubscription.SelectedItem.ToString() != "None")
            {
                TextBoxPrice.Text = "";
                TextBoxPrice.IsEnabled = false;
            }
            else
            {
                TextBoxPrice.IsEnabled = true;
            }
        }

        private void ButtonBrowseWeb_Click(object sender, RoutedEventArgs e)
        {
            BrowseImageWebDialog browseImageWebDialog = new BrowseImageWebDialog(TextBoxTitle.Text, TextBoxAuthor.Text, windowType);
            browseImageWebDialog.ShowDialog();
            string s = browseImageWebDialog.urlChosenImage;
            wholePath = s;
            if (s == null)
                LabelFileSelected.Content = "No file selected";
            else if (s.Length < 60)
                LabelFileSelected.Content = s;
            else
                LabelFileSelected.Content = s.Substring(0, 40) + "..." + s.Substring(s.Length - 25);
        }

        private void ButtonLocalFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string s = openFileDialog.FileName;
                wholePath = s;
                if (s.Length < 60)
                    LabelFileSelected.Content = s;
                else
                    LabelFileSelected.Content = s.Substring(0, 40) + "..." + s.Substring(s.Length - 25);
            }
        }
    }
}

