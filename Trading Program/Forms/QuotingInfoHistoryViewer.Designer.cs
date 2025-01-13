namespace Ciri.Forms
{
    partial class QuotingInfoHistoryViewer
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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.comboBoxServer = new System.Windows.Forms.ComboBox();
            this.textBoxUserId = new System.Windows.Forms.TextBox();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelCode = new System.Windows.Forms.Label();
            this.buttonPrevious = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.comboBoxPurpose = new System.Windows.Forms.ComboBox();
            this.comboBoxStyle = new System.Windows.Forms.ComboBox();
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.buttonLoadQuotingInfo1 = new System.Windows.Forms.Button();
            this.buttonLoadQuotingInfo2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelJsonString = new System.Windows.Forms.Label();
            this.labelQuotingInfo2 = new System.Windows.Forms.Label();
            this.labelQuotingInfo1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.DateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PurposeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StyleColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonSetQuoter1 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.buttonSetQuoter2 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.iGrid1RowTextColCellStyle = new TenTec.Windows.iGridLib.iGCellStyle(true);
            this.quotingInfoControl2 = new CiriData.Manage.QuotingInfoControl();
            this.quotingInfoControl1 = new CiriData.Manage.QuotingInfoControl();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(500, 101);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(292, 397);
            this.treeView1.TabIndex = 0;
            // 
            // comboBoxServer
            // 
            this.comboBoxServer.FormattingEnabled = true;
            this.comboBoxServer.Location = new System.Drawing.Point(91, 62);
            this.comboBoxServer.Name = "comboBoxServer";
            this.comboBoxServer.Size = new System.Drawing.Size(193, 26);
            this.comboBoxServer.TabIndex = 1;
            // 
            // textBoxUserId
            // 
            this.textBoxUserId.Location = new System.Drawing.Point(395, 60);
            this.textBoxUserId.Name = "textBoxUserId";
            this.textBoxUserId.Size = new System.Drawing.Size(100, 28);
            this.textBoxUserId.TabIndex = 2;
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(520, 62);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(90, 30);
            this.buttonLoad.TabIndex = 4;
            this.buttonLoad.Text = "불러오기";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 18);
            this.label2.TabIndex = 5;
            this.label2.Text = "서버: ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(319, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 18);
            this.label3.TabIndex = 6;
            this.label3.Text = "User Id: ";
            // 
            // labelCode
            // 
            this.labelCode.AutoSize = true;
            this.labelCode.BackColor = System.Drawing.SystemColors.ControlDark;
            this.labelCode.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelCode.Location = new System.Drawing.Point(35, 19);
            this.labelCode.Name = "labelCode";
            this.labelCode.Size = new System.Drawing.Size(166, 28);
            this.labelCode.TabIndex = 9;
            this.labelCode.Text = "종목명(종목코드)";
            // 
            // buttonPrevious
            // 
            this.buttonPrevious.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.buttonPrevious.Location = new System.Drawing.Point(79, 531);
            this.buttonPrevious.Name = "buttonPrevious";
            this.buttonPrevious.Size = new System.Drawing.Size(40, 30);
            this.buttonPrevious.TabIndex = 10;
            this.buttonPrevious.Text = "ᐊ";
            this.buttonPrevious.UseVisualStyleBackColor = true;
            this.buttonPrevious.Click += new System.EventHandler(this.buttonPrevious_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.buttonNext.Location = new System.Drawing.Point(269, 531);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(40, 30);
            this.buttonNext.TabIndex = 11;
            this.buttonNext.Text = "ᐅ";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // comboBoxPurpose
            // 
            this.comboBoxPurpose.FormattingEnabled = true;
            this.comboBoxPurpose.Location = new System.Drawing.Point(28, 52);
            this.comboBoxPurpose.Name = "comboBoxPurpose";
            this.comboBoxPurpose.Size = new System.Drawing.Size(81, 26);
            this.comboBoxPurpose.TabIndex = 12;
            this.comboBoxPurpose.SelectedIndexChanged += new System.EventHandler(this.comboBoxPurpose_SelectedIndexChanged);
            // 
            // comboBoxStyle
            // 
            this.comboBoxStyle.FormattingEnabled = true;
            this.comboBoxStyle.Location = new System.Drawing.Point(147, 52);
            this.comboBoxStyle.Name = "comboBoxStyle";
            this.comboBoxStyle.Size = new System.Drawing.Size(82, 26);
            this.comboBoxStyle.TabIndex = 13;
            this.comboBoxStyle.SelectedIndexChanged += new System.EventHandler(this.comboBoxStyle_SelectedIndexChanged);
            // 
            // treeView2
            // 
            this.treeView2.Location = new System.Drawing.Point(849, 101);
            this.treeView2.Name = "treeView2";
            this.treeView2.Size = new System.Drawing.Size(292, 397);
            this.treeView2.TabIndex = 14;
            // 
            // buttonLoadQuotingInfo1
            // 
            this.buttonLoadQuotingInfo1.BackColor = System.Drawing.SystemColors.Control;
            this.buttonLoadQuotingInfo1.Font = new System.Drawing.Font("맑은 고딕", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.buttonLoadQuotingInfo1.Location = new System.Drawing.Point(1351, 141);
            this.buttonLoadQuotingInfo1.Name = "buttonLoadQuotingInfo1";
            this.buttonLoadQuotingInfo1.Size = new System.Drawing.Size(240, 80);
            this.buttonLoadQuotingInfo1.TabIndex = 18;
            this.buttonLoadQuotingInfo1.Text = " Load Quoting Info 1";
            this.buttonLoadQuotingInfo1.UseVisualStyleBackColor = false;
            this.buttonLoadQuotingInfo1.Click += new System.EventHandler(this.buttonLoadQuotingInfo1_Click);
            // 
            // buttonLoadQuotingInfo2
            // 
            this.buttonLoadQuotingInfo2.BackColor = System.Drawing.SystemColors.Control;
            this.buttonLoadQuotingInfo2.Font = new System.Drawing.Font("맑은 고딕", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.buttonLoadQuotingInfo2.Location = new System.Drawing.Point(1351, 357);
            this.buttonLoadQuotingInfo2.Name = "buttonLoadQuotingInfo2";
            this.buttonLoadQuotingInfo2.Size = new System.Drawing.Size(240, 80);
            this.buttonLoadQuotingInfo2.TabIndex = 19;
            this.buttonLoadQuotingInfo2.Text = " Load Quoting Info 2";
            this.buttonLoadQuotingInfo2.UseVisualStyleBackColor = false;
            this.buttonLoadQuotingInfo2.Click += new System.EventHandler(this.buttonLoadQuotingInfo2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonLoadQuotingInfo2);
            this.groupBox1.Controls.Add(this.labelQuotingInfo2);
            this.groupBox1.Controls.Add(this.buttonLoadQuotingInfo1);
            this.groupBox1.Controls.Add(this.labelJsonString);
            this.groupBox1.Controls.Add(this.labelQuotingInfo1);
            this.groupBox1.Controls.Add(this.buttonPrevious);
            this.groupBox1.Controls.Add(this.buttonNext);
            this.groupBox1.Controls.Add(this.comboBoxPurpose);
            this.groupBox1.Controls.Add(this.comboBoxStyle);
            this.groupBox1.Controls.Add(this.treeView2);
            this.groupBox1.Controls.Add(this.treeView1);
            this.groupBox1.Location = new System.Drawing.Point(12, 119);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1651, 603);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Quoting Info 비교";
            // 
            // labelJsonString
            // 
            this.labelJsonString.AutoSize = true;
            this.labelJsonString.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelJsonString.Location = new System.Drawing.Point(844, 64);
            this.labelJsonString.Name = "labelJsonString";
            this.labelJsonString.Size = new System.Drawing.Size(92, 25);
            this.labelJsonString.TabIndex = 22;
            this.labelJsonString.Text = "jsonString";
            // 
            // labelQuotingInfo2
            // 
            this.labelQuotingInfo2.AutoSize = true;
            this.labelQuotingInfo2.Location = new System.Drawing.Point(1161, 392);
            this.labelQuotingInfo2.Name = "labelQuotingInfo2";
            this.labelQuotingInfo2.Size = new System.Drawing.Size(145, 18);
            this.labelQuotingInfo2.TabIndex = 21;
            this.labelQuotingInfo2.Text = "labelQuotingInfo2";
            // 
            // labelQuotingInfo1
            // 
            this.labelQuotingInfo1.AutoSize = true;
            this.labelQuotingInfo1.Location = new System.Drawing.Point(1161, 176);
            this.labelQuotingInfo1.Name = "labelQuotingInfo1";
            this.labelQuotingInfo1.Size = new System.Drawing.Size(145, 18);
            this.labelQuotingInfo1.TabIndex = 20;
            this.labelQuotingInfo1.Text = "labelQuotingInfo1";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DateColumn,
            this.PurposeColumn,
            this.StyleColumn,
            this.buttonSetQuoter1,
            this.buttonSetQuoter2});
            this.dataGridView1.Location = new System.Drawing.Point(32, 229);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 20;
            this.dataGridView1.Size = new System.Drawing.Size(350, 388);
            this.dataGridView1.TabIndex = 21;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // DateColumn
            // 
            this.DateColumn.HeaderText = "Date";
            this.DateColumn.MinimumWidth = 8;
            this.DateColumn.Name = "DateColumn";
            this.DateColumn.Width = 70;
            // 
            // PurposeColumn
            // 
            this.PurposeColumn.HeaderText = "Purpose";
            this.PurposeColumn.MinimumWidth = 8;
            this.PurposeColumn.Name = "PurposeColumn";
            this.PurposeColumn.Width = 30;
            // 
            // StyleColumn
            // 
            this.StyleColumn.HeaderText = "Style";
            this.StyleColumn.MinimumWidth = 8;
            this.StyleColumn.Name = "StyleColumn";
            this.StyleColumn.Width = 40;
            // 
            // buttonSetQuoter1
            // 
            this.buttonSetQuoter1.HeaderText = "1";
            this.buttonSetQuoter1.MinimumWidth = 8;
            this.buttonSetQuoter1.Name = "buttonSetQuoter1";
            this.buttonSetQuoter1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.buttonSetQuoter1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.buttonSetQuoter1.Text = "선택";
            this.buttonSetQuoter1.UseColumnTextForButtonValue = true;
            this.buttonSetQuoter1.Width = 40;
            // 
            // buttonSetQuoter2
            // 
            this.buttonSetQuoter2.HeaderText = "2";
            this.buttonSetQuoter2.MinimumWidth = 8;
            this.buttonSetQuoter2.Name = "buttonSetQuoter2";
            this.buttonSetQuoter2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.buttonSetQuoter2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.buttonSetQuoter2.Text = "선택";
            this.buttonSetQuoter2.UseColumnTextForButtonValue = true;
            this.buttonSetQuoter2.Width = 40;
            // 
            // quotingInfoControl2
            // 
            this.quotingInfoControl2.BackColor = System.Drawing.SystemColors.Control;
            this.quotingInfoControl2.Location = new System.Drawing.Point(1, 1082);
            this.quotingInfoControl2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.quotingInfoControl2.Name = "quotingInfoControl2";
            this.quotingInfoControl2.Size = new System.Drawing.Size(1683, 330);
            this.quotingInfoControl2.TabIndex = 16;
            // 
            // quotingInfoControl1
            // 
            this.quotingInfoControl1.BackColor = System.Drawing.SystemColors.Control;
            this.quotingInfoControl1.Location = new System.Drawing.Point(1, 729);
            this.quotingInfoControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.quotingInfoControl1.Name = "quotingInfoControl1";
            this.quotingInfoControl1.Size = new System.Drawing.Size(1683, 330);
            this.quotingInfoControl1.TabIndex = 15;
            // 
            // QuotingInfoHistoryViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1675, 1435);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.quotingInfoControl2);
            this.Controls.Add(this.quotingInfoControl1);
            this.Controls.Add(this.labelCode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.textBoxUserId);
            this.Controls.Add(this.comboBoxServer);
            this.Name = "QuotingInfoHistoryViewer";
            this.Text = "QuotingInfoHistoryViewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.QuotingInfoHistoryViewer_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ComboBox comboBoxServer;
        private System.Windows.Forms.TextBox textBoxUserId;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelCode;
        private System.Windows.Forms.Button buttonPrevious;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.ComboBox comboBoxPurpose;
        private System.Windows.Forms.ComboBox comboBoxStyle;
        private System.Windows.Forms.TreeView treeView2;
        private CiriData.Manage.QuotingInfoControl quotingInfoControl1;
        private CiriData.Manage.QuotingInfoControl quotingInfoControl2;
        private System.Windows.Forms.Button buttonLoadQuotingInfo1;
        private System.Windows.Forms.Button buttonLoadQuotingInfo2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label labelQuotingInfo1;
        private System.Windows.Forms.Label labelQuotingInfo2;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn PurposeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn StyleColumn;
        private System.Windows.Forms.DataGridViewButtonColumn buttonSetQuoter1;
        private System.Windows.Forms.DataGridViewButtonColumn buttonSetQuoter2;
        private TenTec.Windows.iGridLib.iGCellStyle iGrid1RowTextColCellStyle;
        private System.Windows.Forms.Label labelJsonString;
    }
}