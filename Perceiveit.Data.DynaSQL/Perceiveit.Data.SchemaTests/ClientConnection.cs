using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Perceiveit.Data.SchemaTests
{
    public partial class ClientConnection : Form
    {
        public string ConnectionString
        {
            get { return this.textConnection.Text; }
            set { this.textConnection.Text = value; }
        }

        public string ProviderName
        {
            get { return this.comboProvider.Text; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                foreach (DataRowView drv in ((DataView)this.comboProvider.DataSource))
                {
                    string val = drv[this.comboProvider.DisplayMember].ToString();
                    if (value == val)
                    {
                        this.comboProvider.SelectedItem = drv;
                        break;
                    }
                }
            }
        }

        public ClientConnection()
        {
            InitializeComponent();
            DataTable factories = System.Data.Common.DbProviderFactories.GetFactoryClasses();
            this.comboProvider.DataSource = factories.DefaultView;

            System.Configuration.ConnectionStringSettingsCollection constrings = System.Configuration.ConfigurationManager.ConnectionStrings;

            foreach (System.Configuration.ConnectionStringSettings constr in constrings)
            {
                this.comboConfig.Items.Add(constr.Name);
            }
            
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Exception ex;

            if (!this.CheckConnection(out ex))
                MessageBox.Show("Connection failed with error : " + ex.Message, "Connection failed", MessageBoxButtons.OK);
            else
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            this.labelChecking.Visible = true;
            this.Refresh();

            Exception ex;
            bool success = this.CheckConnection(out ex);

            if (!success)
                MessageBox.Show("Connection failed with error : " + ex.Message, "Connection failed", MessageBoxButtons.OK);
            else
                MessageBox.Show("Connection succeeded");

            this.labelChecking.Visible = false;
        }

        public bool CheckConnection(out Exception failure)
        {
            bool success = false;
            failure = null;
            try
            {
                DBDatabase db = DBDatabase.Create(this.ConnectionString, this.ProviderName);

                //validate that we can get the properties and a schema provider
                DBDatabaseProperties props = db.GetProperties();
                Schema.DBSchemaProvider prov = db.GetSchemaProvider();
                Schema.DBSchemaMetaDataCollectionSet collections = prov.Collections;

                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                failure = ex;
            }
            return success;

        }

        private void radioCustom_CheckedChanged(object sender, EventArgs e)
        {
            bool custEnable = this.radioCustom.Checked;
            bool configEnable = !custEnable;

            this.comboConfig.Enabled = configEnable;
            
            this.comboProvider.Enabled = custEnable;
            this.labelConnection.Enabled = custEnable;
            this.textConnection.Enabled = custEnable;
            this.labelProvider.Enabled = custEnable;
                
            
        }

        private void comboConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = this.comboConfig.Text;
            System.Configuration.ConnectionStringSettings configcon = System.Configuration.ConfigurationManager.ConnectionStrings[name];

            if (null == configcon)
                MessageBox.Show("Could not load the requested connection");
            else
            {
                this.ConnectionString = configcon.ConnectionString;
                this.ProviderName = string.IsNullOrEmpty(configcon.ProviderName) ? "System.Data.SqlClient" : configcon.ProviderName;
            }
        }
    }
}
