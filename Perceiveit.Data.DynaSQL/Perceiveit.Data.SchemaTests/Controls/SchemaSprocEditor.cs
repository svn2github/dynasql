using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Perceiveit.Data.Schema;

namespace Perceiveit.Data.SchemaTests.Controls
{
    public partial class SchemaSprocEditor : UserControl, ISchemaEditor
    {
        private DBSchemaSproc _sproc;

        public SchemaSprocEditor()
        {
            InitializeComponent();
        }

        #region ISchemaEditor Members

        public Control UIControl
        {
            get { return this; }
        }

        public Perceiveit.Data.Schema.DBSchemaItem Item
        {
            get
            {
                return _sproc;
            }
            set
            {
                if (null == value)
                    _sproc = null;

                else if (value is Perceiveit.Data.Schema.DBSchemaSproc)
                    _sproc = (Perceiveit.Data.Schema.DBSchemaSproc)value;

                else
                    throw new InvalidCastException("This control can only handle Stored Procedure Schema items");
                this.UpdateUI();
            }
        }

        #endregion

        private void UpdateUI()
        {
            if (null == this._sproc)
            {
                this.Enabled = false;
                this.propertyGrid1.SelectedObject = null;
                this.dataGridView2.DataSource = null;
                this.dataGridView1.DataSource = null;
            }
            else
            {
                this.Enabled = true;
                this.propertyGrid1.SelectedObject = this._sproc;
                this.dataGridView1.AutoGenerateColumns = true;
                this.dataGridView1.ReadOnly = true;
                this.dataGridView1.DataSource = this._sproc.Parameters;

                this.dataGridView2.AutoGenerateColumns = true;
                this.dataGridView2.ReadOnly = true;
                this.dataGridView2.DataSource = this._sproc.Results;
            }

        }
    }
}
