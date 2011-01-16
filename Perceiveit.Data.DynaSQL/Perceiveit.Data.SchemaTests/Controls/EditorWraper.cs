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
    /// <summary>
    /// wrapes the editor ui with standard functionality
    /// </summary>
    public partial class EditorWraper : UserControl, ISchemaEditor
    {
        #region public event EventHandler RequestClose;

        /// <summary>
        /// raises the event to request closing of the panel
        /// </summary>
        public event EventHandler RequestClose;

        protected virtual void OnRequestClose(EventArgs args)
        {
            if (null != this.RequestClose)
                this.RequestClose(this, args);
        }

        #endregion



        private ISchemaEditor _inner;
        /// <summary>
        /// Gets or sets the ISchemaEditor for this wrapper
        /// </summary>
        public ISchemaEditor InnerEditor
        {
            get
            {
                return _inner;
            }
            set
            {
                if (this.panelContent.Controls.Count > 0)
                    this.panelContent.Controls.Clear();
                _inner = value;

                if (null != _inner)
                {
                    Control ctl = _inner.UIControl;
                    this.panelContent.Controls.Add(ctl);
                    ctl.Dock = DockStyle.Fill;
                    this.labelTitle.Text = _inner.Item.FullName;
                    int index = this.GetIconForType(_inner.Item.Type);
                    if (index > -1)
                        this.pictureBox2.Image = this.imageList1.Images[index];
                    else
                        this.pictureBox2.Image = null;
                }
            }

        }

        public DBSchemaTypes EditorType
        {
            get
            {
                if (null == this._inner)
                    return (DBSchemaTypes) (-1);
                else
                    return this._inner.Item.Type;
            }
        }

        /// <summary>
        /// returns the index of the image in the ImageList based upon the schema type
        /// </summary>
        /// <param name="dBSchemaTypes"></param>
        /// <returns></returns>
        private int GetIconForType(DBSchemaTypes dBSchemaTypes)
        {
            int imgIndex;
            switch (dBSchemaTypes)
            {
                case DBSchemaTypes.Table:
                    imgIndex = 0;
                    break;
                case DBSchemaTypes.View:
                    imgIndex = 3;
                    break;
                case DBSchemaTypes.StoredProcedure:
                    imgIndex = 5;
                    break;
                case DBSchemaTypes.Function:
                    imgIndex = 4;
                    break;
                case DBSchemaTypes.Index:
                    imgIndex = 1;
                    break;
                case DBSchemaTypes.CommandScripts:
                    imgIndex = 6;
                    break;
                case DBSchemaTypes.ForeignKey:
                    imgIndex = 2;
                    break;
                default:
                    imgIndex = -1;
                    break;
            }
            return imgIndex;
        }


        public EditorWraper()
        {
            InitializeComponent();
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                this.OnRequestClose(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not close the editor: " + ex.Message);
            }
        }

        #region ISchemaEditor Members

        public Control UIControl
        {
            get {return this; }
        }

        public Perceiveit.Data.Schema.DBSchemaItem Item
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
