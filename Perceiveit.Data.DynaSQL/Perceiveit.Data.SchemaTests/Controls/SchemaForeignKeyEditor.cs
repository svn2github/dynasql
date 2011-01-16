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
    public partial class SchemaForeignKeyEditor : UserControl, ISchemaEditor
    {
        private Perceiveit.Data.Schema.DBSchemaForeignKey _fk;

        public SchemaForeignKeyEditor()
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
                return _fk;
            }
            set
            {
                if (null == value)
                    _fk = null;

                else if (value is Perceiveit.Data.Schema.DBSchemaForeignKey)
                    _fk = (Perceiveit.Data.Schema.DBSchemaForeignKey)value;

                else
                    throw new InvalidCastException("This control can only handle Foreign Key Schema items");
                this.UpdateUI();
            }
        }

        #endregion

        private void UpdateUI()
        {
            if (null == this._fk)
            {
                this.Enabled = false;
                this.propertyGrid1.SelectedObject = null;
                this.dataGridView1.DataSource = null;
            }
            else
            {
                this.Enabled = true;
                this.propertyGrid1.SelectedObject = this._fk;
                this.dataGridView1.AutoGenerateColumns = true;
                this.dataGridView1.ReadOnly = true;
                this.dataGridView1.DataSource = this._fk.Mappings;
            }

        }
    }
}
