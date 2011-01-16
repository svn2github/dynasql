using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Perceiveit.Data.SchemaTests.Controls
{
    public partial class SchemaTableEditor : UserControl, ISchemaEditor
    {
        private Perceiveit.Data.Schema.DBSchemaTable _table;

        public SchemaTableEditor()
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
                return _table;
            }
            set
            {
                if (null == value)
                    _table = null;

                else if (value is Perceiveit.Data.Schema.DBSchemaTable)
                    _table = (Perceiveit.Data.Schema.DBSchemaTable)value;

                else
                    throw new InvalidCastException("This control can only handle Table Schema items");
                this.UpdateUI();
            }
        }

        #endregion

        private void UpdateUI()
        {
            if (null == this._table)
            {
                this.Enabled = false;
                this.propertyGrid1.SelectedObject = null;
                this.dataGridView1.DataSource = null;
            }
            else
            {
                this.Enabled = true;
                this.propertyGrid1.SelectedObject = this._table;
                this.dataGridView1.AutoGenerateColumns = true;
                this.dataGridView1.ReadOnly = true;
                this.dataGridView1.DataSource = this._table.Columns;

                if (null == this._table.Columns)
                    this.tabPage1.Text = this.tabPage1.Text + " (0)";
                else
                    this.tabPage1.Text = this.tabPage1.Text + " (" + this._table.Columns.Count.ToString() + ")";

                this.dataGridView2.AutoGenerateColumns = true;
                this.dataGridView2.ReadOnly = true;
                this.dataGridView2.DataSource = this._table.ForeignKeys;
                if (null == this._table.ForeignKeys)
                    this.tabPage2.Text = this.tabPage2.Text + " (0)";
                else
                    this.tabPage2.Text = this.tabPage2.Text + " (" + this._table.ForeignKeys.Count.ToString() + ")";

                this.dataGridView3.AutoGenerateColumns = true;
                this.dataGridView3.ReadOnly = true;
                this.dataGridView3.DataSource = this._table.Indexes;
                if(null == this._table.Indexes)
                    this.tabPage3.Text = this.tabPage3.Text + " (0)";
                else
                    this.tabPage3.Text = this.tabPage3.Text + " (" + this._table.Indexes.Count.ToString() + ")";

            }

        }
    }
}
