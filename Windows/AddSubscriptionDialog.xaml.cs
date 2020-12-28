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
using Backlogger.Model;

namespace Backlogger.Windows
{
    /// <summary>
    /// Interaction logic for AddSubscriptionDialog.xaml
    /// </summary>
    public partial class AddSubscriptionDialog : Window
    {
        string windowType;
        Subscription edited;
        public AddSubscriptionDialog(string type)
        {
            windowType = type;
            edited = null;
            InitializeComponent();
        }

        public AddSubscriptionDialog(string type, Subscription s)
        {
            windowType = type;
            InitializeComponent();

            TextBoxSubName.Text = s.SubscriptionName;
            TextBoxSubPrice.Text = s.Price.ToString();
            CheckBoxActive.IsChecked = s.IsActive;

            edited = s;
        }

        private void SaveSubscriptionButton_Click(object sender, RoutedEventArgs e)
        {
            // should verify format of fields

            using (BackloggerEntities context = new BackloggerEntities())
            {
                context.Hobbies.Load();
                context.Subscriptions.Load();

                if (edited == null)
                {
                    Subscription subscription = new Subscription()
                    {
                        HobbyID = (from o in context.Hobbies.Local where o.HobbyName == windowType select o).FirstOrDefault().HobbyID,
                        SubscriptionName = TextBoxSubName.Text,
                        Price = Convert.ToDecimal(TextBoxSubPrice.Text),
                        IsActive = (bool)CheckBoxActive.IsChecked
                    };

                    context.Subscriptions.Add(subscription);
                }
                else
                {
                    Subscription s = context.Subscriptions.Find(edited.SubscriptionID);
                    s.Price = Convert.ToDecimal(TextBoxSubPrice.Text);
                    s.IsActive = (bool)CheckBoxActive.IsChecked;
                    s.SubscriptionName = TextBoxSubName.Text;
                }
                context.SaveChanges();
                this.Close();
            }
        }

        private void CancelSubscriptionButton_Click(object sender, RoutedEventArgs e)
        {
            TextBoxSubName.Text = "";
            TextBoxSubPrice.Text = "";
            CheckBoxActive.IsChecked = false;
            this.Close();
        }
    }
}