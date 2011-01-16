using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Perceiveit.Data;
using Perceiveit.Data.Schema;
using Perceiveit.Data.SchemaTests.Controls;

namespace Perceiveit.Data.SchemaTests
{
    public partial class SchemaExplorer : Form
    {
        /* ----------  SQLite Northwind Schema -------------- */
        //public const string SqliteDbConnection = @"data source=C:\Development\Perceiveit.Data.DynaSQL\Northwind_sqlite\Northwind.sl3";
        //public const string SqliteDbProvider = "System.Data.SQLite";

        /* ----------  Access 2007 Northwind Schema -------------- */
        //NOTE: x86 compilation only and script execution is not supported
        //public const string AccessDbConnection = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=""C:\Users\Richard Hewitson\Documents\Northwind07.accdb"";Persist Security Info=False;";
        //public const string AccessDbProvider = "System.Data.OleDb";

         /* ----------  MySQL Northwind Schema -------------- */
        //public const string MySQLDbConnection = "server=172.16.56.1;User Id=xan;Password=xan;Persist Security Info=True;database=xan";
        //public const string MySQLDbProvider = "MySql.Data.MySqlClient";
        
        /* ----------  MS SQL Server Northwind Schema -------------- */
        //public const string MsSQLDbConnection = @"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True;Pooling=False";
        //public const string MsSQLDbProvider = "System.Data.SqlClient";

        private static DBDatabase database = null;


        private class ImageIndexes
        {
            public const int TableIndex = 0;
            public const int IndexIndex = 1;
            public const int FKIndex = 2;
            public const int ViewIndex = 3;
            public const int FunctionIndex = 4;
            public const int ProcedureIndex = 4;
            public const int ScriptIndex = 4;
            public const int DatabaseIndex = 5;
            public const int FolderIndex = 6;
        }


        DBSchemaProvider provider = null;

        public SchemaExplorer()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!this.ChooseConnection())
                this.Close();
            else
                LoadSchemaTree();
        }

        private void LoadSchemaTree()
        {
            Dictionary<DBSchemaTypes, List<DBSchemaItemRef>> schema = new Dictionary<DBSchemaTypes, List<DBSchemaItemRef>>();
            this.FillSchema(schema);

            TreeNode db = new TreeNode(database.ConnectionString, ImageIndexes.DatabaseIndex, ImageIndexes.DatabaseIndex);
            this.tvSchemaTree.Nodes.Clear();
            this.tvSchemaTree.Nodes.Add(db);
            this.PopulateTree(db.Nodes, schema);
            db.Expand();
        }

        private bool ChooseConnection()
        {
            ClientConnection con = new ClientConnection();
            if (con.ShowDialog() == DialogResult.OK)
            {
                database = DBDatabase.Create(con.ConnectionString, con.ProviderName);
                provider = database.GetSchemaProvider();
                return true;
            }
            else
                return false;
        }

        private void FillSchema(Dictionary<DBSchemaTypes, List<DBSchemaItemRef>> schema)
        {
            List<DBSchemaItemRef> items = new List<DBSchemaItemRef>();

            IEnumerable<DBSchemaItemRef> tbls = FillAllReferencesAndReturnTables(items);
            
            foreach (DBSchemaItemRef item in items)
            {
                System.Diagnostics.Debug.WriteLine(item.ToString());
                DBSchemaTypes type = item.Type;
                List<DBSchemaItemRef> all;
                if (!schema.TryGetValue(type, out all))
                {
                    all = new List<DBSchemaItemRef>();
                    schema.Add(type, all);
                }
                all.Add(item);
            }

            foreach (DBSchemaItemRef tbl in tbls)
            {
                if (tbl.Name.Equals("Customers", StringComparison.OrdinalIgnoreCase))
                {
                    IEnumerable<DBSchemaItemRef> customerindexes = provider.GetAllIndexs(tbl);
                    foreach (DBSchemaItemRef idx in customerindexes)
                    {
                        System.Diagnostics.Debug.WriteLine(idx);
                    }
                }
            }

            foreach (List<DBSchemaItemRef> value in schema.Values)
            {
                value.Sort(delegate(DBSchemaItemRef one, DBSchemaItemRef two) { return one.Name.CompareTo(two.Name); });
            }

            

        }

        private IEnumerable<DBSchemaItemRef> FillAllReferencesAndReturnTables(List<DBSchemaItemRef> items)
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            IEnumerable<DBSchemaItemRef> tbls = null;

            try
            {
                tbls = provider.GetAllTables();
                items.AddRange(tbls);
                items.AddRange(provider.GetAllViews());
                items.AddRange(provider.GetAllIndexs());

                if((provider.SupportedSchemaTypes & DBSchemaTypes.StoredProcedure) > 0)
                    items.AddRange(provider.GetAllRoutines());

                items.AddRange(provider.GetAllForeignKeys());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Schema load failed : " + ex.Message);

            }

            sw.Stop();
            System.Diagnostics.Debug.WriteLine("Total load time: " + sw.Elapsed);

            return tbls;
        }

        private void PopulateTree(TreeNodeCollection addTo, Dictionary<DBSchemaTypes, List<DBSchemaItemRef>> schema)
        {
            TreeNode node;
            List<DBSchemaItemRef> all;

            //Tables
            node = this.GetFolderNode(DBSchemaTypes.Table);
            node.Expand();
            if (schema.TryGetValue(DBSchemaTypes.Table, out all))
            {
                this.PopulateSchemaType(node, all, ImageIndexes.TableIndex);
                foreach (TreeNode tblnode in node.Nodes)
                {
                    DBSchemaItemRef tref = (DBSchemaItemRef)tblnode.Tag;
                    
                    IEnumerable<DBSchemaItemRef> ixs = provider.GetAllIndexs(tref);

                    TreeNode ixparent = this.GetFolderNode(DBSchemaTypes.Index);
                    this.PopulateSchemaType(ixparent, ixs, ImageIndexes.IndexIndex);
                    tblnode.Nodes.Add(ixparent);

                    IEnumerable<DBSchemaItemRef> fks = provider.GetAllForeignKeys(tref);

                    TreeNode fkparent = this.GetFolderNode(DBSchemaTypes.ForeignKey);
                    this.PopulateSchemaType(fkparent, fks, ImageIndexes.FKIndex);
                    tblnode.Nodes.Add(fkparent);
                }
                
            }
            addTo.Add(node);

            //Views
            node = this.GetFolderNode(DBSchemaTypes.View);
            if (schema.TryGetValue(DBSchemaTypes.View, out all))
                this.PopulateSchemaType(node, all, ImageIndexes.ViewIndex);
            addTo.Add(node);

            //StoredProcedure
            node = this.GetFolderNode(DBSchemaTypes.StoredProcedure);
            if (schema.TryGetValue(DBSchemaTypes.StoredProcedure, out all))
                this.PopulateSchemaType(node, all, ImageIndexes.ScriptIndex);
            addTo.Add(node);

            //Functions
            node = this.GetFolderNode(DBSchemaTypes.Function);
            if (schema.TryGetValue(DBSchemaTypes.Function, out all))
                this.PopulateSchemaType(node, all, ImageIndexes.ScriptIndex);
            addTo.Add(node);

            
            
        }

        //private TreeNode GetDatabaseNode(DBDatabase database)
        //{
        //}

        private TreeNode GetFolderNode(DBSchemaTypes type)
        {
            TreeNode node = new TreeNode(type.ToString() + "s", ImageIndexes.FolderIndex, ImageIndexes.FolderIndex);
            return node;
        }

        private void PopulateSchemaType(TreeNode parent, IEnumerable<DBSchemaItemRef> items, int imageIndex)
        {
            foreach (DBSchemaItemRef item in items)
            {
                TreeNode node = new TreeNode(item.Name, imageIndex, imageIndex);
                node.ToolTipText = item.ToString();
                node.Tag = item;
                parent.Nodes.Add(node);
            }
        }

        private void tvSchemaTree_DoubleClick(object sender, EventArgs e)
        {
            if (this.tvSchemaTree.SelectedNode != null
                && this.tvSchemaTree.SelectedNode.Tag != null)
                this.EditNode(this.tvSchemaTree.SelectedNode);
        }

        protected virtual void EditNode(TreeNode node)
        {
            DBSchemaItemRef itemref = (DBSchemaItemRef)node.Tag;
            string fullname = itemref.ToString();
            if (this.tabControl1.TabPages.ContainsKey(fullname))
            {
                this.tabControl1.SelectedTab = this.tabControl1.TabPages[fullname];
            }
            else
            {
                ISchemaEditor editor = this.GetSchemaEditor(itemref);
                if (editor != null)
                {
                    try
                    {
                        DBSchemaItem item = this.provider.GetSchema(itemref);
                        editor.Item = item;
                        
                        Control ctl = editor.UIControl;
                        string full = itemref.ToString();
                        this.tabControl1.TabPages.Add(full, itemref.Name);
                        TabPage pg = this.tabControl1.TabPages[full];
                        Perceiveit.Data.SchemaTests.Controls.EditorWraper wrapper = new Perceiveit.Data.SchemaTests.Controls.EditorWraper();
                        wrapper.InnerEditor = editor;
                        wrapper.RequestClose += new EventHandler(wrapper_RequestClose);
                        pg.Controls.Add(wrapper);
                        ctl.Dock = DockStyle.Fill;
                        wrapper.Dock = DockStyle.Fill;
                        this.tabControl1.SelectedTab = pg;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Could not load the schema editor :" + ex.Message + "\r\n\r\n" + ex.StackTrace);
                    }
                }
            }
        }

        

        private ISchemaEditor GetSchemaEditor(DBSchemaItemRef itemref)
        {
            ISchemaEditor ed = null;
            switch (itemref.Type)
            {
                case DBSchemaTypes.Table:
                    ed = new Controls.SchemaTableEditor();
                    break;
                case DBSchemaTypes.View:
                    ed = new Controls.SchemaViewEditor();
                    break;
                case DBSchemaTypes.StoredProcedure:
                    ed = new Controls.SchemaSprocEditor();
                    break;
                case DBSchemaTypes.Function:
                    ed = new Controls.SchemaFunctionEditor();
                    break;
                case DBSchemaTypes.Index:
                    ed = new Controls.SchemaIndexEditor();
                    break;
                case(DBSchemaTypes.ForeignKey):
                    ed = new Controls.SchemaForeignKeyEditor();
                    break;
                case DBSchemaTypes.CommandScripts:
                    break;
                default:
                    break;
            }
            return ed;
        }

        void wrapper_RequestClose(object sender, EventArgs e)
        {
            EditorWraper wrapper = (EditorWraper)sender;
            TabPage found = null;

            foreach (TabPage tab in this.tabControl1.TabPages)
            {
                if(tab.Controls.Count == 0)
                    continue;

                Control contents = tab.Controls[0];
                if (contents == wrapper)
                {
                    found = tab;
                    break;
                }
            }

            if (null != found)
            {
                this.tabControl1.TabPages.Remove(found);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (this.tabControl1.TabPages.Count > 0)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to close the current database connection?", "Close connection?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.No)
                    return;
            }
            this.tvSchemaTree.Nodes.Clear();
            this.tabControl1.TabPages.Clear();
            database = null;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            ClientConnection con = new ClientConnection();
            if (con.ShowDialog(this) == DialogResult.OK)
            {
                database = DBDatabase.Create(con.ConnectionString, con.ProviderName);
                provider = database.GetSchemaProvider();
                this.LoadSchemaTree();
            }

        }

        

    }
}
