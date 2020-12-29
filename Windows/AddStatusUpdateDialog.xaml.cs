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
    /// Interaction logic for AddStatusUpdateDialog.xaml
    /// </summary>
    public partial class AddStatusUpdateDialog : Window
    {
        int editID = -1;
        int materialID = -1;
        public AddStatusUpdateDialog(int materialID)
        {
            InitializeComponent();
            List<string> statuses = new List<string> { "Added", "On hold", "In progress", "Finished", "Dropped" };
            ComboBoxStatus.ItemsSource = statuses;
            DatePickerDateStatus.DefaultValue = DateTime.Now;
            this.materialID = materialID;
            ComboBoxStatus.SelectedItem = ComboBoxStatus.Items[0];
        }

        public AddStatusUpdateDialog(int materialID, int updateID, DateTime oldDate, string oldStatus)
        {
            InitializeComponent();
            List<string> statuses = new List<string> { "Added", "On hold", "In progress", "Finished", "Dropped" };
            ComboBoxStatus.ItemsSource = statuses;
            DatePickerDateStatus.DefaultValue = oldDate;
            foreach(var item in ComboBoxStatus.Items)
                if(item.ToString() == oldStatus)
                {
                    ComboBoxStatus.SelectedItem = item;
                    break;
                }
            editID = updateID;
            this.materialID = materialID;
        }

        private void SaveStatusUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new BackloggerEntities())
            {
                context.StatusUpdates.Load();
                context.Statuses.Load();

                int sID = (from o in context.Statuses.Local
                           where o.StatusName == ComboBoxStatus.SelectedItem.ToString()
                           select o).FirstOrDefault().StatusID;

                if (editID == -1)
                {
                    StatusUpdate su = new StatusUpdate
                    { 
                        StatusID = sID,
                        DateModified = (DateTime)DatePickerDateStatus.Value,
                        MaterialID = materialID
                    };
                    context.StatusUpdates.Add(su);
                    context.SaveChanges();
                }
                else
                {
                    StatusUpdate su = (from o in context.StatusUpdates
                                       where o.UpdateID == editID
                                       select o).FirstOrDefault();

                    su.StatusID = sID;
                    su.DateModified = (DateTime)DatePickerDateStatus.Value;
                    context.SaveChanges();
                }
            }
            Close();
        }

        private void CancelStatusUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
