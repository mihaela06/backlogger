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
using Backlogger.Model;

namespace Backlogger.Windows
{
    /// <summary>
    /// Interaction logic for StatisticsWindow.xaml
    /// </summary>
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow()
        {
            InitializeComponent();
        }

        private void StatisticsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.MainWindow.Show();
        }

        private void ReportViewer4_Load(object sender, EventArgs e)
        {
            //if (!IsReportViewerLoaded4)
            //{
            //    using (var db = new BackloggerEntities())
            //    {
            //        var data = db.MaterialsFullReports.ToList();
            //        var reportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource();
            //        reportDataSource.Name = "DataSet4";
            //        reportDataSource.Value = data;
            //        this.RepViewer4.LocalReport.DataSources.Add(reportDataSource);
            //        this.RepViewer4.LocalReport.ReportPath = App.projectPath + @"\Reports\MainReport4.rdlc";
            //        this.RepViewer4.RefreshReport();
            //        IsReportViewerLoaded4 = true;
            //    }
            //}
        }

        private void RepViewerTimeHobbies_Load(object sender, EventArgs e)
        {
            using (var db = new BackloggerEntities())
            {
                var data = db.TimeSpentOnHobbies.ToList();
                var reportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource();
                reportDataSource.Name = "DataSetTimeHobbies";
                reportDataSource.Value = data;
                this.RepViewerTimeHobbies.LocalReport.DataSources.Add(reportDataSource);
                this.RepViewerTimeHobbies.LocalReport.ReportPath = App.projectPath + @"\Reports\TimeSpentHobbies.rdlc";
                this.RepViewerTimeHobbies.RefreshReport();
            }
        }

        private void RepViewerTimeMaterials_Load(object sender, EventArgs e)
        {
            using (var db = new BackloggerEntities())
            {
                var data = db.TimeSpentOnMaterials.ToList();
                var reportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource();
                reportDataSource.Name = "DataSetTimeMaterials";
                reportDataSource.Value = data;
                this.RepViewerTimeMaterials.LocalReport.DataSources.Add(reportDataSource);
                this.RepViewerTimeMaterials.LocalReport.ReportPath = App.projectPath + @"\Reports\TimeSpentMaterials.rdlc";
                this.RepViewerTimeMaterials.RefreshReport();
            }
        }

        private void RepViewerFullReport_Load(object sender, EventArgs e)
        {
            using (var db = new BackloggerEntities())
            {
                var data = db.MaterialsFullReports.ToList();
                var reportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource();
                reportDataSource.Name = "DataSetCompleteReport";
                reportDataSource.Value = data;
                this.RepViewerFullReport.LocalReport.DataSources.Add(reportDataSource);
                this.RepViewerFullReport.LocalReport.ReportPath = App.projectPath + @"\Reports\CompleteReport.rdlc";
                this.RepViewerFullReport.RefreshReport();
            }
        }

        private void RepViewerSpendings_Load(object sender, EventArgs e)
        {
            using (var db = new BackloggerEntities())
            {
                var data = db.MaterialsFullReports.ToList();
                var reportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource();
                reportDataSource.Name = "DataSetSpendings";
                reportDataSource.Value = data;
                this.RepViewerSpendings.LocalReport.DataSources.Add(reportDataSource);
                this.RepViewerSpendings.LocalReport.ReportPath = App.projectPath + @"\Reports\Spendings.rdlc";
                this.RepViewerSpendings.RefreshReport();
            }
        }
    }
}
