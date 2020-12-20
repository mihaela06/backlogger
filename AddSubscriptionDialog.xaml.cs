using System;
using System.Collections.Generic;
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
    /// Interaction logic for AddSubscriptionDialog.xaml
    /// </summary>
    public partial class AddSubscriptionDialog : Window
    {
        string windowType;
        BackloggerEntities context;
        public AddSubscriptionDialog(string type, BackloggerEntities context)
        {
            windowType = type;
            this.context = context;
            InitializeComponent();
        }

        private void SaveSubscriptionButton_Click(object sender, RoutedEventArgs e)
        {
            // should verify format of fields

            Subscription subscription = new Subscription()
            {
                HobbyID = (from o in context.Hobbies.Local where o.HobbyName == windowType select o).FirstOrDefault().HobbyID,
                SubscriptionName = TextBoxSubName.Text,
                Price = Convert.ToDecimal(TextBoxSubPrice.Text),
                IsActive = (bool)CheckBoxActive.IsChecked
            };

            context.Subscriptions.Add(subscription);
            context.SaveChanges();
            this.Close();
        }

        private void CancelSubscriptionButton_Click(object sender, RoutedEventArgs e)
        {
            TextBoxSubName.Text = "";
            TextBoxSubPrice.Text = "";
            CheckBoxActive.IsChecked = false;
        }
    }
}