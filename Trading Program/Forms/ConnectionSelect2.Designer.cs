namespace Ciri.Forms
{
    partial class ConnectionSelect2
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
            this.OAPanel = new System.Windows.Forms.TableLayoutPanel();
            this.serverPanel1 = new CiriData.Manage.ServerPanel();
            this.serverPanel2 = new CiriData.Manage.ServerPanel();
            this.serverPanel3 = new CiriData.Manage.ServerPanel();
            this.serverPanel4 = new CiriData.Manage.ServerPanel();
            this.serverPanel5 = new CiriData.Manage.ServerPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.OASisePanel = new System.Windows.Forms.Panel();
            this.buttonConnectSise = new System.Windows.Forms.Button();
            this.comboBoxSisePort = new System.Windows.Forms.ComboBox();
            this.comboBoxSiseServerIP = new System.Windows.Forms.ComboBox();
            this.buttonConnectComs = new System.Windows.Forms.Button();
            this.OAPanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.OASisePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // OAPanel
            // 
            this.OAPanel.AutoSize = true;
            this.OAPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.OAPanel.ColumnCount = 1;
            this.OAPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.OAPanel.Controls.Add(this.serverPanel1, 0, 0);
            this.OAPanel.Controls.Add(this.serverPanel2, 0, 1);
            this.OAPanel.Controls.Add(this.serverPanel3, 0, 2);
            this.OAPanel.Controls.Add(this.serverPanel4, 0, 3);
            this.OAPanel.Controls.Add(this.serverPanel5, 0, 4);
            this.OAPanel.Location = new System.Drawing.Point(30, 30);
            this.OAPanel.Name = "OAPanel";
            this.OAPanel.RowCount = 5;
            this.OAPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.OAPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.OAPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.OAPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.OAPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.OAPanel.Size = new System.Drawing.Size(489, 221);
            this.OAPanel.TabIndex = 0;
            // 
            // serverPanel1
            // 
            this.serverPanel1.Location = new System.Drawing.Point(4, 4);
            this.serverPanel1.Name = "serverPanel1";
            this.serverPanel1.ServerId = "SEOUL1";
            this.serverPanel1.Size = new System.Drawing.Size(481, 37);
            this.serverPanel1.TabIndex = 0;
            // 
            // serverPanel2
            // 
            this.serverPanel2.Location = new System.Drawing.Point(4, 48);
            this.serverPanel2.Name = "serverPanel2";
            this.serverPanel2.ServerId = "BUSAN1";
            this.serverPanel2.Size = new System.Drawing.Size(481, 37);
            this.serverPanel2.TabIndex = 1;
            // 
            // serverPanel3
            // 
            this.serverPanel3.Location = new System.Drawing.Point(4, 92);
            this.serverPanel3.Name = "serverPanel3";
            this.serverPanel3.ServerId = "BUSAN2";
            this.serverPanel3.Size = new System.Drawing.Size(481, 37);
            this.serverPanel3.TabIndex = 2;
            // 
            // serverPanel4
            // 
            this.serverPanel4.Location = new System.Drawing.Point(4, 136);
            this.serverPanel4.Name = "serverPanel4";
            this.serverPanel4.ServerId = "LOCAL";
            this.serverPanel4.Size = new System.Drawing.Size(481, 37);
            this.serverPanel4.TabIndex = 3;
            // 
            // serverPanel5
            // 
            this.serverPanel5.Location = new System.Drawing.Point(4, 180);
            this.serverPanel5.Name = "serverPanel5";
            this.serverPanel5.ServerId = "CUSTOM1";
            this.serverPanel5.Size = new System.Drawing.Size(481, 37);
            this.serverPanel5.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonAdd);
            this.groupBox1.Controls.Add(this.OAPanel);
            this.groupBox1.Location = new System.Drawing.Point(12, 110);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(665, 444);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "OracleArena";
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(538, 30);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(100, 35);
            this.buttonAdd.TabIndex = 10;
            this.buttonAdd.Text = "서버 추가";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // OASisePanel
            // 
            this.OASisePanel.BackColor = System.Drawing.SystemColors.Control;
            this.OASisePanel.Controls.Add(this.buttonConnectSise);
            this.OASisePanel.Controls.Add(this.comboBoxSisePort);
            this.OASisePanel.Controls.Add(this.comboBoxSiseServerIP);
            this.OASisePanel.Location = new System.Drawing.Point(46, 56);
            this.OASisePanel.Name = "OASisePanel";
            this.OASisePanel.Size = new System.Drawing.Size(509, 34);
            this.OASisePanel.TabIndex = 1;
            // 
            // buttonConnectSise
            // 
            this.buttonConnectSise.Location = new System.Drawing.Point(0, 0);
            this.buttonConnectSise.Name = "buttonConnectSise";
            this.buttonConnectSise.Size = new System.Drawing.Size(98, 34);
            this.buttonConnectSise.TabIndex = 2;
            this.buttonConnectSise.Text = "Sise";
            this.buttonConnectSise.UseVisualStyleBackColor = true;
            this.buttonConnectSise.Click += new System.EventHandler(this.buttonConnectSise_Click);
            // 
            // comboBoxSisePort
            // 
            this.comboBoxSisePort.FormattingEnabled = true;
            this.comboBoxSisePort.Items.AddRange(new object[] {
            "4504"});
            this.comboBoxSisePort.Location = new System.Drawing.Point(280, 4);
            this.comboBoxSisePort.Name = "comboBoxSisePort";
            this.comboBoxSisePort.Size = new System.Drawing.Size(80, 26);
            this.comboBoxSisePort.TabIndex = 9;
            // 
            // comboBoxSiseServerIP
            // 
            this.comboBoxSiseServerIP.FormattingEnabled = true;
            this.comboBoxSiseServerIP.Items.AddRange(new object[] {
            "192.168.245.118",
            "localhost"});
            this.comboBoxSiseServerIP.Location = new System.Drawing.Point(104, 4);
            this.comboBoxSiseServerIP.Name = "comboBoxSiseServerIP";
            this.comboBoxSiseServerIP.Size = new System.Drawing.Size(170, 26);
            this.comboBoxSiseServerIP.TabIndex = 6;
            // 
            // buttonConnectComs
            // 
            this.buttonConnectComs.Location = new System.Drawing.Point(46, 595);
            this.buttonConnectComs.Name = "buttonConnectComs";
            this.buttonConnectComs.Size = new System.Drawing.Size(100, 34);
            this.buttonConnectComs.TabIndex = 4;
            this.buttonConnectComs.Text = "C Oms";
            this.buttonConnectComs.UseVisualStyleBackColor = true;
            this.buttonConnectComs.Click += new System.EventHandler(this.buttonConnectComs_Click);
            // 
            // ConnectionSelect2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.ClientSize = new System.Drawing.Size(703, 1037);
            this.Controls.Add(this.buttonConnectComs);
            this.Controls.Add(this.OASisePanel);
            this.Controls.Add(this.groupBox1);
            this.Name = "ConnectionSelect2";
            this.Text = "ConnectionSelect";
            this.Load += new System.EventHandler(this.ConnectionSelect2_Load);
            this.OAPanel.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.OASisePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel OAPanel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel OASisePanel;
        private System.Windows.Forms.ComboBox comboBoxSiseServerIP;
        private System.Windows.Forms.Button buttonConnectSise;
        private System.Windows.Forms.Button buttonConnectComs;
        private System.Windows.Forms.ComboBox comboBoxSisePort;
        private System.Windows.Forms.Button buttonAdd;
        private CiriData.Manage.ServerPanel serverPanel1;
        private CiriData.Manage.ServerPanel serverPanel2;
        private CiriData.Manage.ServerPanel serverPanel3;
        private CiriData.Manage.ServerPanel serverPanel4;
        private CiriData.Manage.ServerPanel serverPanel5;
    }
}