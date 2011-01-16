namespace Perceiveit.Data.SchemaTests
{
    partial class ClientConnection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientConnection));
            this.textConnection = new System.Windows.Forms.TextBox();
            this.labelConnection = new System.Windows.Forms.Label();
            this.labelProvider = new System.Windows.Forms.Label();
            this.comboProvider = new System.Windows.Forms.ComboBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelChecking = new System.Windows.Forms.Label();
            this.buttonTest = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.radioConfig = new System.Windows.Forms.RadioButton();
            this.radioCustom = new System.Windows.Forms.RadioButton();
            this.comboConfig = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // textConnection
            // 
            this.textConnection.Enabled = false;
            this.textConnection.Location = new System.Drawing.Point(132, 89);
            this.textConnection.Name = "textConnection";
            this.textConnection.Size = new System.Drawing.Size(362, 20);
            this.textConnection.TabIndex = 0;
            // 
            // labelConnection
            // 
            this.labelConnection.AutoSize = true;
            this.labelConnection.Enabled = false;
            this.labelConnection.Location = new System.Drawing.Point(36, 92);
            this.labelConnection.Name = "labelConnection";
            this.labelConnection.Size = new System.Drawing.Size(91, 13);
            this.labelConnection.TabIndex = 1;
            this.labelConnection.Text = "Connection String";
            // 
            // labelProvider
            // 
            this.labelProvider.AutoSize = true;
            this.labelProvider.Enabled = false;
            this.labelProvider.Location = new System.Drawing.Point(36, 125);
            this.labelProvider.Name = "labelProvider";
            this.labelProvider.Size = new System.Drawing.Size(77, 13);
            this.labelProvider.TabIndex = 2;
            this.labelProvider.Text = "Provider Name";
            // 
            // comboProvider
            // 
            this.comboProvider.DisplayMember = "InvariantName";
            this.comboProvider.Enabled = false;
            this.comboProvider.FormattingEnabled = true;
            this.comboProvider.Location = new System.Drawing.Point(132, 122);
            this.comboProvider.Name = "comboProvider";
            this.comboProvider.Size = new System.Drawing.Size(362, 21);
            this.comboProvider.TabIndex = 3;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(15, 169);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(419, 169);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // labelChecking
            // 
            this.labelChecking.Image = ((System.Drawing.Image)(resources.GetObject("labelChecking.Image")));
            this.labelChecking.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelChecking.Location = new System.Drawing.Point(297, 169);
            this.labelChecking.Name = "labelChecking";
            this.labelChecking.Size = new System.Drawing.Size(82, 23);
            this.labelChecking.TabIndex = 6;
            this.labelChecking.Text = "Checking....";
            this.labelChecking.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelChecking.Visible = false;
            // 
            // buttonTest
            // 
            this.buttonTest.Location = new System.Drawing.Point(216, 169);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(75, 23);
            this.buttonTest.TabIndex = 7;
            this.buttonTest.Text = "Test";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // radioConfig
            // 
            this.radioConfig.AutoSize = true;
            this.radioConfig.Checked = true;
            this.radioConfig.Location = new System.Drawing.Point(15, 12);
            this.radioConfig.Name = "radioConfig";
            this.radioConfig.Size = new System.Drawing.Size(148, 17);
            this.radioConfig.TabIndex = 10;
            this.radioConfig.TabStop = true;
            this.radioConfig.Text = "Known Config Connection";
            this.radioConfig.UseVisualStyleBackColor = true;
            // 
            // radioCustom
            // 
            this.radioCustom.AutoSize = true;
            this.radioCustom.Location = new System.Drawing.Point(15, 57);
            this.radioCustom.Name = "radioCustom";
            this.radioCustom.Size = new System.Drawing.Size(117, 17);
            this.radioCustom.TabIndex = 11;
            this.radioCustom.Text = "Custom Connection";
            this.radioCustom.UseVisualStyleBackColor = true;
            this.radioCustom.CheckedChanged += new System.EventHandler(this.radioCustom_CheckedChanged);
            // 
            // comboConfig
            // 
            this.comboConfig.DisplayMember = "InvariantName";
            this.comboConfig.FormattingEnabled = true;
            this.comboConfig.Location = new System.Drawing.Point(132, 35);
            this.comboConfig.Name = "comboConfig";
            this.comboConfig.Size = new System.Drawing.Size(362, 21);
            this.comboConfig.TabIndex = 12;
            this.comboConfig.SelectedIndexChanged += new System.EventHandler(this.comboConfig_SelectedIndexChanged);
            // 
            // ClientConnection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(506, 208);
            this.Controls.Add(this.comboConfig);
            this.Controls.Add(this.radioCustom);
            this.Controls.Add(this.radioConfig);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonTest);
            this.Controls.Add(this.labelChecking);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.comboProvider);
            this.Controls.Add(this.labelConnection);
            this.Controls.Add(this.labelProvider);
            this.Controls.Add(this.textConnection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClientConnection";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Client Connection";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textConnection;
        private System.Windows.Forms.Label labelConnection;
        private System.Windows.Forms.Label labelProvider;
        private System.Windows.Forms.ComboBox comboProvider;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label labelChecking;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.RadioButton radioConfig;
        private System.Windows.Forms.RadioButton radioCustom;
        private System.Windows.Forms.ComboBox comboConfig;
    }
}