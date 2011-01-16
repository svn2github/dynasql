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
    public partial class SchemaFunctionEditor : UserControl, ISchemaEditor
    {
        private DBSchemaFunction _func;

        public SchemaFunctionEditor()
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
                return _func;
            }
            set
            {
                if (null == value)
                    _func = null;

                else if (value is Perceiveit.Data.Schema.DBSchemaFunction)
                    _func = (Perceiveit.Data.Schema.DBSchemaFunction)value;

                else
                    throw new InvalidCastException("This control can only handle Function Schema items");

                this.UpdateUI();
            }
        }

        #endregion

        private void UpdateUI()
        {
            if (null == this._func)
            {
                this.Enabled = false;
                this.propertyGrid1.SelectedObject = null;
                this.dataGridView1.DataSource = null;
            }
            else
            {
                this.Enabled = true;
                this.propertyGrid1.SelectedObject = this._func;
                this.dataGridView1.AutoGenerateColumns = true;
                this.dataGridView1.ReadOnly = true;
                this.dataGridView1.DataSource = this._func.Parameters;

                
            }

        }
    }
}
