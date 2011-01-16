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
    public partial class SchemaViewEditor : UserControl, ISchemaEditor
    {
        private DBSchemaView _view;

        public SchemaViewEditor()
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
                return _view;
            }
            set
            {
                if (null == value)
                    _view = null;

                else if (value is Perceiveit.Data.Schema.DBSchemaView)
                    _view = (Perceiveit.Data.Schema.DBSchemaView)value;

                else
                    throw new InvalidCastException("This control can only handle View Schema items");
                this.UpdateUI();
            }
        }

        #endregion

        private void UpdateUI()
        {
            if (null == this._view)
            {
                this.Enabled = false;
                this.propertyGrid1.SelectedObject = null;
                this.dataGridView1.DataSource = null;
            }
            else
            {
                this.Enabled = true;
                this.propertyGrid1.SelectedObject = this._view;
                this.dataGridView1.AutoGenerateColumns = true;
                this.dataGridView1.ReadOnly = true;
                this.dataGridView1.DataSource = this._view.Columns;
            }

        }
    }
}
