namespace Perceiveit.Data.SchemaTests.Controls
{
    partial class SchemaTableEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDbType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPK = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colAutoAssign = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colNullable = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colReadonly = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colHasDefault = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colDefValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colDbType,
            this.colType,
            this.colSize,
            this.colPK,
            this.colAutoAssign,
            this.colNullable,
            this.colReadonly,
            this.colHasDefault,
            this.colDefValue});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(407, 128);
            this.dataGridView1.TabIndex = 0;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.HelpVisible = false;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(421, 150);
            this.propertyGrid1.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.propertyGrid1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(421, 314);
            this.splitContainer1.SplitterDistance = 150;
            this.splitContainer1.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(421, 160);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(413, 134);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Columns";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridView2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(413, 134);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Foreign Keys";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView2.Location = new System.Drawing.Point(3, 3);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(407, 128);
            this.dataGridView2.TabIndex = 1;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dataGridView3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(413, 134);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Indexes";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dataGridView3
            // 
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView3.Location = new System.Drawing.Point(3, 3);
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.Size = new System.Drawing.Size(407, 128);
            this.dataGridView3.TabIndex = 1;
            // 
            // colName
            // 
            this.colName.DataPropertyName = "Name";
            this.colName.HeaderText = "Name";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Width = 200;
            // 
            // colDbType
            // 
            this.colDbType.DataPropertyName = "DbType";
            this.colDbType.HeaderText = "DbType";
            this.colDbType.Name = "colDbType";
            this.colDbType.ReadOnly = true;
            // 
            // colType
            // 
            this.colType.DataPropertyName = "Type";
            this.colType.HeaderText = "Runtime Type";
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            this.colType.Width = 150;
            // 
            // colSize
            // 
            this.colSize.DataPropertyName = "Size";
            this.colSize.HeaderText = "Size";
            this.colSize.Name = "colSize";
            this.colSize.ReadOnly = true;
            this.colSize.Width = 50;
            // 
            // colPK
            // 
            this.colPK.DataPropertyName = "PrimaryKey";
            this.colPK.FalseValue = "False";
            this.colPK.HeaderText = "Primary Key";
            this.colPK.Name = "colPK";
            this.colPK.ReadOnly = true;
            this.colPK.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colPK.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colPK.TrueValue = "True";
            this.colPK.Width = 50;
            // 
            // colAutoAssign
            // 
            this.colAutoAssign.DataPropertyName = "AutoAssign";
            this.colAutoAssign.FalseValue = "False";
            this.colAutoAssign.HeaderText = "Auto Assign";
            this.colAutoAssign.Name = "colAutoAssign";
            this.colAutoAssign.ReadOnly = true;
            this.colAutoAssign.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colAutoAssign.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colAutoAssign.TrueValue = "True";
            this.colAutoAssign.Width = 50;
            // 
            // colNullable
            // 
            this.colNullable.DataPropertyName = "Nullable";
            this.colNullable.FalseValue = "False";
            this.colNullable.HeaderText = "Nullable";
            this.colNullable.Name = "colNullable";
            this.colNullable.ReadOnly = true;
            this.colNullable.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colNullable.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colNullable.TrueValue = "True";
            this.colNullable.Width = 50;
            // 
            // colReadonly
            // 
            this.colReadonly.DataPropertyName = "ReadOnly";
            this.colReadonly.FalseValue = "False";
            this.colReadonly.HeaderText = "Read only";
            this.colReadonly.Name = "colReadonly";
            this.colReadonly.ReadOnly = true;
            this.colReadonly.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colReadonly.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colReadonly.TrueValue = "True";
            this.colReadonly.Width = 50;
            // 
            // colHasDefault
            // 
            this.colHasDefault.DataPropertyName = "HasDefault";
            this.colHasDefault.FalseValue = "False";
            this.colHasDefault.HeaderText = "Has Default";
            this.colHasDefault.Name = "colHasDefault";
            this.colHasDefault.ReadOnly = true;
            this.colHasDefault.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colHasDefault.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colHasDefault.TrueValue = "True";
            this.colHasDefault.Width = 50;
            // 
            // colDefValue
            // 
            this.colDefValue.DataPropertyName = "DefaultValue";
            this.colDefValue.HeaderText = "DefaultValue";
            this.colDefValue.Name = "colDefValue";
            this.colDefValue.ReadOnly = true;
            // 
            // SchemaTableEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "SchemaTableEditor";
            this.Size = new System.Drawing.Size(427, 320);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDbType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSize;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colPK;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colAutoAssign;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colNullable;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colReadonly;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colHasDefault;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDefValue;
    }
}
