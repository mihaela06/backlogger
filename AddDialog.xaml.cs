using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
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
    /// Interaction logic for AddDialog.xaml
    /// </summary>
    public partial class AddDialog : Window
    {
        public string windowType;
        private string wholePath;
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
                context.MaterialFormats.Load();
                context.Statuses.Load();
                context.StatusUpdates.Load();
                context.Subscriptions.Load();

                int hobbyID = (from o in context.Hobbies.Local where o.HobbyName == windowType select o).FirstOrDefault().HobbyID;
                int? subID = 0;
                try
                {
                    subID = (from o in context.Subscriptions.Local where 
                             o.SubscriptionName == ComboBoxSubscription.SelectedItem.ToString() && o.HobbyID == hobbyID 
                             select o).FirstOrDefault().SubscriptionID;
                }
                catch
                {
                    subID = null;
                }

                Decimal? price = null;
                if (TextBoxPrice.Text != "")
                    price = Convert.ToDecimal(TextBoxPrice.Text);

                string info = null;
                if (TextBoxInfo.Text != "")
                    info = TextBoxInfo.Text;

                int materialFormatID;
                try
                {
                    materialFormatID = (from o in context.MaterialFormats.Local where o.FormatType == TextBoxMaterialFormat.Text select o).FirstOrDefault().MaterialFormatID;
                }
                catch
                {
                    MaterialFormat materialFormat = new MaterialFormat()
                    {
                        FormatType = TextBoxMaterialFormat.Text
                    };

                    context.MaterialFormats.Add(materialFormat);
                    context.SaveChanges();
                    materialFormatID = (from o in context.MaterialFormats.Local where o.FormatType == TextBoxMaterialFormat.Text select o).FirstOrDefault().MaterialFormatID;
                }

                ObservableCollection<Author> authors = new ObservableCollection<Author>();
                ObservableCollection<Genre> genres = new ObservableCollection<Genre>();

                foreach (var a in TextBoxAuthor.Text.Split(','))
                {
                    int id;
                    try
                    {
                        id = (from o in context.Authors.Local where o.AuthorName == TextBoxAuthor.Text select o).FirstOrDefault().AuthorID;
                    }
                    catch
                    {
                        Author author = new Author
                        {
                            AuthorName = TextBoxAuthor.Text
                        };
                        context.Authors.Add(author);
                        context.SaveChanges();
                        id = (from o in context.Authors.Local where o.AuthorName == TextBoxAuthor.Text select o).FirstOrDefault().AuthorID;
                    }
                    authors.Add((from o in context.Authors.Local where o.AuthorID == id select o).FirstOrDefault());
                }

                foreach (var a in TextBoxGenre.Text.Split(','))
                {
                    int id;
                    try
                    {
                        id = (from o in context.Genres.Local where o.GenreName == TextBoxGenre.Text select o).FirstOrDefault().GenreID;
                    }
                    catch
                    {
                        Genre genre = new Genre
                        {
                            GenreName = TextBoxGenre.Text
                        };
                        context.Genres.Add(genre);
                        context.SaveChanges();
                        id = (from o in context.Genres.Local where o.GenreName == TextBoxGenre.Text select o).FirstOrDefault().GenreID;
                    }
                    genres.Add((from o in context.Genres.Local where o.GenreName == TextBoxGenre.Text select o).FirstOrDefault());
                }

                Material newMaterial = new Material()
                {
                    HobbyID = hobbyID,
                    Title = TextBoxTitle.Text,
                    MaterialFormatID = materialFormatID,
                    Price = price,
                    DateReleased = DatePickerDateReleased.SelectedDate,
                    Info = TextBoxInfo.Text,
                    Authors = authors,
                    Genres = genres,
                    SubscriptionID = subID
                    // check for correct format
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

                if (LabelFileSelected.Content.ToString() != "No file selected")
                {
                    int id = newMaterial.MaterialID;

                    // should modify path for release version

                    string path = LabelFileSelected.Content.ToString();
                    if (path.IndexOf("http") == 0)
                    {
                        Uri uri = new Uri(wholePath);

                        var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
                        var fileExtension = System.IO.Path.GetExtension(uriWithoutQuery);

                        using (WebClient client = new WebClient())
                        {
                            client.DownloadFile(uri, System.AppDomain.CurrentDomain.BaseDirectory + @"..\..\Resources\Images\" + id + fileExtension);
                        }
                    }
                    else
                    {
                        var fileExtension = System.IO.Path.GetExtension(wholePath);

                        File.Copy(wholePath, System.AppDomain.CurrentDomain.BaseDirectory + @"..\..\Resources\Images\" + id + fileExtension);
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

