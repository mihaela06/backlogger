using System;
using System.Collections.Generic;
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
    /// Interaction logic for BrowseImageWebDialog.xaml
    /// </summary>
    public partial class BrowseImageWebDialog : Window
    {
        private string title;
        private string author;
        private string type;
        public string urlChosenImage;
        public BrowseImageWebDialog(string title, string author, string type)
        {
            this.title = title.Replace(" ", "%20");
            this.author = author.Replace(" ", "%20");
            this.type = (type.Replace(" ", "%20")).Remove(type.Length - 1);
            InitializeComponent();

            string urlToNavigate = "https://www.google.com/search?tbm=isch&q=" + this.title + "%20" + this.author + "%20" + this.type;
            webBrowser.Navigate(urlToNavigate);
        }

        private void ButtonImageCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonImageOK_Click(object sender, RoutedEventArgs e)
        {
            urlChosenImage = webBrowser.Source.AbsoluteUri;
            dynamic urlHtml = webBrowser.Document;
            string data = urlHtml.documentElement.InnerHtml;
            string hashID = urlChosenImage.Substring(urlChosenImage.Length - 14);
            string toSearch = "[1,[0,\"" + hashID + "\"";

            int index = data.IndexOf(toSearch);
            if (index > 0)
            {
                index = data.IndexOf("\n", index);
                if (data.IndexOf("http", index) == index + 4)
                {
                    urlChosenImage = data.Substring(index + 4).Split('"')[0];
                }
            }

            this.Close();
        }
    }
}
