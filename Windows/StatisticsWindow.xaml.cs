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
        private bool IsReportViewerLoaded;
        private bool IsReportViewerLoaded2;
        private bool IsReportViewerLoaded3;
        private bool IsReportViewerLoaded4;
        public StatisticsWindow()
        {
            InitializeComponent();
        }

        private void StatisticsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.MainWindow.Show();
        }

        private void ReportViewer_Load(object sender, EventArgs e)
        {
            if (!IsReportViewerLoaded)
            {
                using (var db = new BackloggerEntities())
                {
                    var data = db.TimeSpentOnHobbies.ToList();
                    var reportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource();
                    reportDataSource.Name = "DataSet";
                    reportDataSource.Value = data;
                    this.RepViewer.LocalReport.DataSources.Add(reportDataSource);
                    this.RepViewer.LocalReport.ReportPath = App.projectPath + @"\Reports\MainReport.rdlc";
                    this.RepViewer.RefreshReport();
                    IsReportViewerLoaded = true;
                }
            }
        }


        private void ReportViewer2_Load(object sender, EventArgs e)
        {
            if (!IsReportViewerLoaded2)
            {
                using (var db = new BackloggerEntities())
                {
                    var data = db.TimeSpentOnMaterials.ToList();
                    var reportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource();
                    reportDataSource.Name = "DataSet2";
                    reportDataSource.Value = data;
                    this.RepViewer2.LocalReport.DataSources.Add(reportDataSource);
                    this.RepViewer2.LocalReport.ReportPath = App.projectPath + @"\Reports\MainReport2.rdlc";
                    this.RepViewer2.RefreshReport();
                    IsReportViewerLoaded2 = true;
                }
            }
        }

        private void ReportViewer3_Load(object sender, EventArgs e)
        {
            if (!IsReportViewerLoaded3)
            {
                using (var db = new BackloggerEntities())
                {
                    var data = db.MaterialsFullReports.ToList();
                    var reportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource();
                    reportDataSource.Name = "DataSet3";
                    reportDataSource.Value = data;
                    this.RepViewer3.LocalReport.DataSources.Add(reportDataSource);
                    this.RepViewer3.LocalReport.ReportPath = App.projectPath + @"\Reports\MainReport3.rdlc";
                    this.RepViewer3.RefreshReport();
                    IsReportViewerLoaded3 = true;
                }
            }
        }

        private void ReportViewer4_Load(object sender, EventArgs e)
        {
            if (!IsReportViewerLoaded4)
            {
                using (var db = new BackloggerEntities())
                {
                    var data = db.MaterialsFullReports.ToList();
                    var reportDataSource = new Microsoft.Reporting.WinForms.ReportDataSource();
                    reportDataSource.Name = "DataSet4";
                    reportDataSource.Value = data;
                    this.RepViewer4.LocalReport.DataSources.Add(reportDataSource);
                    this.RepViewer4.LocalReport.ReportPath = App.projectPath + @"\Reports\MainReport4.rdlc";
                    this.RepViewer4.RefreshReport();
                    IsReportViewerLoaded4 = true;
                }
            }
        }
    }
}
