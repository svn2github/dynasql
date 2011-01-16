namespace Perceiveit.Data.SchemaTests
{
    partial class SchemaExplorer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SchemaExplorer));
            this.tvSchemaTree = new System.Windows.Forms.TreeView();
            this.ilDatabaseIcons = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvSchemaTree
            // 
            this.tvSchemaTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvSchemaTree.ImageIndex = 6;
            this.tvSchemaTree.ImageList = this.ilDatabaseIcons;
            this.tvSchemaTree.Location = new System.Drawing.Point(0, 25);
            this.tvSchemaTree.Name = "tvSchemaTree";
            this.tvSchemaTree.SelectedImageIndex = 0;
            this.tvSchemaTree.Size = new System.Drawing.Size(180, 406);
            this.tvSchemaTree.TabIndex = 0;
            this.tvSchemaTree.DoubleClick += new System.EventHandler(this.tvSchemaTree_DoubleClick);
            // 
            // ilDatabaseIcons
            // 
            this.ilDatabaseIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilDatabaseIcons.ImageStream")));
            this.ilDatabaseIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.ilDatabaseIcons.Images.SetKeyName(0, "Table_16.png");
            this.ilDatabaseIcons.Images.SetKeyName(1, "index_16.png");
            this.ilDatabaseIcons.Images.SetKeyName(2, "fk_16.png");
            this.ilDatabaseIcons.Images.SetKeyName(3, "view_16.png");
            this.ilDatabaseIcons.Images.SetKeyName(4, "Script_16.png");
            this.ilDatabaseIcons.Images.SetKeyName(5, "Database_16.png");
            this.ilDatabaseIcons.Images.SetKeyName(6, "Folder_16.png");
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvSchemaTree);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(540, 431);
            this.splitContainer1.SplitterDistance = 180;
            this.splitContainer1.TabIndex = 1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(180, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = global::Perceiveit.Data.SchemaTests.Properties.Resources.Database_24;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(133, 22);
            this.toolStripButton2.Text = "Change Connection...";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(356, 431);
            this.tabControl1.TabIndex = 0;
            // 
            // SchemaExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 431);
            this.Controls.Add(this.splitContainer1);
            this.Name = "SchemaExplorer";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvSchemaTree;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ImageList ilDatabaseIcons;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
    }
}

