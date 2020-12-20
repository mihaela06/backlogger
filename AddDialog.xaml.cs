using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for AddDialog.xaml
    /// </summary>
    public partial class AddDialog : Window
    {
        public string windowType;
        BackloggerEntities context;
        public AddDialog(string type, BackloggerEntities context)
        {
            windowType = type;
            this.context = context;
            InitializeComponent();
            DatePickerDateAdded.DefaultValue = DateTime.Now;
        }

        private void ButtonSaveNew_Click(object sender, RoutedEventArgs e)
        {
            //int hobbyID = (from o in context.Hobbies.Local where o.HobbyName == windowType select o).FirstOrDefault().HobbyID;
            int hobbyID = (from o in context.Hobbies.Local select o).FirstOrDefault().HobbyID;
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
                Price = Convert.ToDecimal(TextBoxPrice.Text),
                DateReleased = DatePickerDateReleased.SelectedDate,
                Info = TextBoxInfo.Text,
                Authors = authors,
                Genres = genres
                // Subscription
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
            this.Close();
        }

        private void ButtonCancelNew_Click(object sender, RoutedEventArgs e)
        {
            TextBoxAuthor.Text = "";
            TextBoxGenre.Text = "";
            TextBoxTitle.Text = "";
            TextBoxInfo.Text = "";

            this.Close();
        }
    }

}

