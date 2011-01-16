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
    public partial class SchemaIndexEditor : UserControl, ISchemaEditor
    {
        private DBSchemaIndex _index;

        public SchemaIndexEditor()
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
                return _index;
            }
            set
            {
                _index = (DBSchemaIndex)value;
                this.UpdateUI();
            }
        }

        #endregion

        protected virtual void UpdateUI()
        {
            if (null == this._index)
            {
                this.propertyGrid1.SelectedObject = null;
                this.dataGridView1.DataSource = null;
                this.Enabled = false;
            }
            else
            {
                this.Enabled = true;
                this.propertyGrid1.SelectedObject = this._index;
                this.dataGridView1.AutoGenerateColumns = true;
                this.dataGridView1.ReadOnly = true;
                this.dataGridView1.DataSource = this._index.Columns;
            }

        }
    }
}
