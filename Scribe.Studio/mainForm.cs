﻿using Scribe.Studio.Configuration_Forms;
using Scribe.Studio.Logic;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.DataGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scribe.Studio
{
    public partial class mainForm : RibbonForm
    {
        public mainForm()
        {
            InitializeComponent();
            MessageBoxAdv.Office2016Theme = Office2016Theme.Colorful;
            MessageBoxAdv.MessageBoxStyle = MessageBoxAdv.Style.Office2016;
            try
            {
                Configuration.LoadConfiguration("configuration.json");
            }
            catch
            {
                MessageBoxAdv.Show("Error while reading configuration! Please click File and configure application!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void PopulateEnvironmentsGrid()
        {
            List<Logic.Environment> environments = new List<Logic.Environment>();
            environments.AddRange(Configuration.Parameters.Where(e => e.Key.StartsWith("ENV|")).Select(s => s.Value).ToList().Cast<Logic.Environment>());
            environmentsGrid.DataSource = environments;
            environmentsGrid.Columns.Remove(environmentsGrid.Columns.Where(c=>c.MappingName=="Password").FirstOrDefault());
        }

        //Configuration.Parameters = new List<KeyValuePair<string, object>>();
        //    Configuration.Parameters.Add(new KeyValuePair<string, object>("env1", new Logic.Environment("srv1", "db1", true)));
        //    Configuration.Parameters.Add(new KeyValuePair<string, object>("env2", new Logic.Environment("srv2", "db2", true)));
        //    Configuration.Parameters.Add(new KeyValuePair<string, object>("env3", new Logic.Environment("srv3", "db3", true)));
        //    Configuration.Parameters.Add(new KeyValuePair<string, object>("env4", new Logic.Environment("srv4", "db4", true)));
        //    Configuration.Parameters.Add(new KeyValuePair<string, object>("env5", new Logic.Environment("srv5", "db5", true)));
        //    Configuration.SaveConfiguration("test.json");
        //    Configuration.Parameters = new List<KeyValuePair<string, object>>();
        //    Configuration.LoadConfiguration("test.json");

        private void applicationMenuTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (applicationMenuTabs.SelectedIndex == 0)
            {
                PopulateEnvironmentsGrid();
            }
        }

        private void ribbonControl_MenuButtonClick(object sender, EventArgs e)
        {
            PopulateEnvironmentsGrid();
        }

        private void addEnvironmentBtn_Click(object sender, EventArgs e)
        {
            environmentForm environment = new environmentForm();
            if (environment.ShowDialog() == DialogResult.OK)
            {
                Configuration.Parameters.Add(new KeyValuePair<string, object>(string.Format("ENV|{0}", environment.GetEnvironment().Name), environment.GetEnvironment()));
                PopulateEnvironmentsGrid();
            }
        }

        private void environmentsGrid_QueryCellStyle(object sender, Syncfusion.WinForms.DataGrid.Events.QueryCellStyleEventArgs e)
        {
            e.Style.BackColor = ((Logic.Environment)e.DataRow.RowData).Color;
        }

        private void saveConfigBtn_Click(object sender, EventArgs e)
        {
            Configuration.SaveConfiguration("configuration.json");
        }

        private void environmentsGrid_CellDoubleClick(object sender, Syncfusion.WinForms.DataGrid.Events.CellClickEventArgs e)
        {
            environmentForm environment = new environmentForm((Logic.Environment)e.DataRow.RowData);
            if (environment.ShowDialog() == DialogResult.OK)
            {
                Configuration.Parameters.Remove(Configuration.Parameters.Where(p => p.Key == string.Format("ENV|{0}", ((Logic.Environment)e.DataRow.RowData).Name)).FirstOrDefault());
                Configuration.Parameters.Add(new KeyValuePair<string, object>(string.Format("ENV|{0}", environment.GetEnvironment().Name), environment.GetEnvironment()));
                PopulateEnvironmentsGrid();
            }
        }
    }
}
