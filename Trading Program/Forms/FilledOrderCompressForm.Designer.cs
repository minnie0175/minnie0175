using CommonLib.GridView;

namespace Ciri.Forms
{
    partial class FilledOrderCompressForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dgvFilledOrder = new CommonLib.GridView.FilledOrderGridView();
            this.서버 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.시간DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.개수 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.북코드 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.isinCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.종목명DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Purpose = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.수량DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.체결가DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.이론가DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.주문번호DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContractType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.comboBoxServer = new System.Windows.Forms.ComboBox();
            this.checkBoxFilterFix = new System.Windows.Forms.CheckBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTradingPnL = new System.Windows.Forms.TextBox();
            this.textBoxArbNetAmount = new System.Windows.Forms.TextBox();
            this.checkBoxScrollFix = new System.Windows.Forms.CheckBox();
            this.labelFillCount = new System.Windows.Forms.Label();
            this.labelCalc = new System.Windows.Forms.Label();
            this.textBoxCalcResult = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxCodeFilter = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilledOrder)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.dgvFilledOrder, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1278, 201);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // dgvFilledOrder
            // 
            this.dgvFilledOrder.AllowUserToAddRows = false;
            this.dgvFilledOrder.AllowUserToDeleteRows = false;
            this.dgvFilledOrder.AllowUserToResizeRows = false;
            this.dgvFilledOrder.AutoGenerateColumns = false;
            this.dgvFilledOrder.BackgroundColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFilledOrder.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvFilledOrder.ColumnHeadersHeight = 34;
            this.dgvFilledOrder.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvFilledOrder.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.서버,
            this.시간DataGridViewTextBoxColumn,
            this.개수,
            this.북코드,
            this.isinCode,
            this.종목명DataGridViewTextBoxColumn,
            this.Purpose,
            this.수량DataGridViewTextBoxColumn,
            this.체결가DataGridViewTextBoxColumn,
            this.이론가DataGridViewTextBoxColumn,
            this.주문번호DataGridViewTextBoxColumn,
            this.ContractType});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFilledOrder.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvFilledOrder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFilledOrder.Location = new System.Drawing.Point(4, 42);
            this.dgvFilledOrder.Margin = new System.Windows.Forms.Padding(4);
            this.dgvFilledOrder.Name = "dgvFilledOrder";
            this.dgvFilledOrder.RowHeadersVisible = false;
            this.dgvFilledOrder.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.dgvFilledOrder.RowTemplate.Height = 23;
            this.dgvFilledOrder.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvFilledOrder.Size = new System.Drawing.Size(1270, 155);
            this.dgvFilledOrder.TabIndex = 1;
            this.dgvFilledOrder.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvFilledOrder_CellMouseClick);
            this.dgvFilledOrder.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvFilledOrder_CellMouseDoubleClick);
            this.dgvFilledOrder.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvFilledOrder_RowsAdded);
            this.dgvFilledOrder.SelectionChanged += new System.EventHandler(this.dgvFilledOrder_SelectionChanged);
            this.dgvFilledOrder.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvFilledOrder_KeyDown);
            // 
            // 서버
            // 
            this.서버.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.서버.DataPropertyName = "서버";
            this.서버.HeaderText = "서버";
            this.서버.MinimumWidth = 8;
            this.서버.Name = "서버";
            this.서버.Width = 80;
            // 
            // 시간DataGridViewTextBoxColumn
            // 
            this.시간DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.시간DataGridViewTextBoxColumn.DataPropertyName = "시간";
            this.시간DataGridViewTextBoxColumn.HeaderText = "시간";
            this.시간DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.시간DataGridViewTextBoxColumn.Name = "시간DataGridViewTextBoxColumn";
            this.시간DataGridViewTextBoxColumn.Width = 80;
            // 
            // 개수
            // 
            this.개수.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.개수.DataPropertyName = "개수";
            this.개수.HeaderText = "#";
            this.개수.MinimumWidth = 8;
            this.개수.Name = "개수";
            this.개수.Width = 30;
            // 
            // 북코드
            // 
            this.북코드.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.북코드.DataPropertyName = "북코드";
            this.북코드.HeaderText = "북코드";
            this.북코드.MinimumWidth = 8;
            this.북코드.Name = "북코드";
            this.북코드.Width = 110;
            // 
            // isinCode
            // 
            this.isinCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.isinCode.DataPropertyName = "isinCode";
            this.isinCode.HeaderText = "isinCode";
            this.isinCode.MinimumWidth = 8;
            this.isinCode.Name = "isinCode";
            this.isinCode.Width = 10;
            // 
            // 종목명DataGridViewTextBoxColumn
            // 
            this.종목명DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.종목명DataGridViewTextBoxColumn.DataPropertyName = "종목명";
            this.종목명DataGridViewTextBoxColumn.HeaderText = "종목명";
            this.종목명DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.종목명DataGridViewTextBoxColumn.Name = "종목명DataGridViewTextBoxColumn";
            this.종목명DataGridViewTextBoxColumn.Width = 150;
            // 
            // Purpose
            // 
            this.Purpose.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Purpose.DataPropertyName = "Purpose";
            this.Purpose.HeaderText = "Purpose";
            this.Purpose.MinimumWidth = 8;
            this.Purpose.Name = "Purpose";
            this.Purpose.Width = 30;
            // 
            // 수량DataGridViewTextBoxColumn
            // 
            this.수량DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.수량DataGridViewTextBoxColumn.DataPropertyName = "수량";
            this.수량DataGridViewTextBoxColumn.HeaderText = "수량";
            this.수량DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.수량DataGridViewTextBoxColumn.Name = "수량DataGridViewTextBoxColumn";
            this.수량DataGridViewTextBoxColumn.Width = 60;
            // 
            // 체결가DataGridViewTextBoxColumn
            // 
            this.체결가DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.체결가DataGridViewTextBoxColumn.DataPropertyName = "체결가";
            this.체결가DataGridViewTextBoxColumn.HeaderText = "가격";
            this.체결가DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.체결가DataGridViewTextBoxColumn.Name = "체결가DataGridViewTextBoxColumn";
            this.체결가DataGridViewTextBoxColumn.Width = 55;
            // 
            // 이론가DataGridViewTextBoxColumn
            // 
            this.이론가DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.이론가DataGridViewTextBoxColumn.DataPropertyName = "이론가";
            this.이론가DataGridViewTextBoxColumn.HeaderText = "이론가";
            this.이론가DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.이론가DataGridViewTextBoxColumn.Name = "이론가DataGridViewTextBoxColumn";
            this.이론가DataGridViewTextBoxColumn.Width = 80;
            // 
            // 주문번호DataGridViewTextBoxColumn
            // 
            this.주문번호DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.주문번호DataGridViewTextBoxColumn.DataPropertyName = "주문번호";
            this.주문번호DataGridViewTextBoxColumn.HeaderText = "주문번호";
            this.주문번호DataGridViewTextBoxColumn.MinimumWidth = 8;
            this.주문번호DataGridViewTextBoxColumn.Name = "주문번호DataGridViewTextBoxColumn";
            this.주문번호DataGridViewTextBoxColumn.Width = 76;
            // 
            // ContractType
            // 
            this.ContractType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ContractType.DataPropertyName = "ContractType";
            this.ContractType.HeaderText = "ContractType";
            this.ContractType.MinimumWidth = 8;
            this.ContractType.Name = "ContractType";
            this.ContractType.Width = 150;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.comboBoxServer);
            this.panel1.Controls.Add(this.checkBoxFilterFix);
            this.panel1.Controls.Add(this.buttonClear);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.textBoxTradingPnL);
            this.panel1.Controls.Add(this.textBoxArbNetAmount);
            this.panel1.Controls.Add(this.checkBoxScrollFix);
            this.panel1.Controls.Add(this.labelFillCount);
            this.panel1.Controls.Add(this.labelCalc);
            this.panel1.Controls.Add(this.textBoxCalcResult);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBoxCodeFilter);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(4, 4);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1270, 30);
            this.panel1.TabIndex = 2;
            // 
            // comboBoxServer
            // 
            this.comboBoxServer.FormattingEnabled = true;
            this.comboBoxServer.Location = new System.Drawing.Point(0, 5);
            this.comboBoxServer.Name = "comboBoxServer";
            this.comboBoxServer.Size = new System.Drawing.Size(121, 26);
            this.comboBoxServer.TabIndex = 16;
            this.comboBoxServer.SelectedIndexChanged += new System.EventHandler(this.comboBoxServer_SelectedIndexChanged);
            // 
            // checkBoxFilterFix
            // 
            this.checkBoxFilterFix.AutoSize = true;
            this.checkBoxFilterFix.Checked = true;
            this.checkBoxFilterFix.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFilterFix.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxFilterFix.Location = new System.Drawing.Point(255, 9);
            this.checkBoxFilterFix.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxFilterFix.Name = "checkBoxFilterFix";
            this.checkBoxFilterFix.Size = new System.Drawing.Size(107, 21);
            this.checkBoxFilterFix.TabIndex = 15;
            this.checkBoxFilterFix.Text = "필터 고정";
            this.checkBoxFilterFix.UseVisualStyleBackColor = true;
            // 
            // buttonClear
            // 
            this.buttonClear.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.buttonClear.Location = new System.Drawing.Point(591, -1);
            this.buttonClear.Margin = new System.Windows.Forms.Padding(4);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(60, 34);
            this.buttonClear.TabIndex = 5;
            this.buttonClear.Text = "clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1143, 9);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 18);
            this.label3.TabIndex = 14;
            this.label3.Text = "TrdPnL";
            this.label3.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(922, 9);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 18);
            this.label2.TabIndex = 13;
            this.label2.Text = "ArbNet";
            this.label2.Visible = false;
            // 
            // textBoxTradingPnL
            // 
            this.textBoxTradingPnL.Location = new System.Drawing.Point(1213, 1);
            this.textBoxTradingPnL.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxTradingPnL.Name = "textBoxTradingPnL";
            this.textBoxTradingPnL.Size = new System.Drawing.Size(118, 28);
            this.textBoxTradingPnL.TabIndex = 12;
            this.textBoxTradingPnL.Visible = false;
            // 
            // textBoxArbNetAmount
            // 
            this.textBoxArbNetAmount.Location = new System.Drawing.Point(992, 1);
            this.textBoxArbNetAmount.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxArbNetAmount.Name = "textBoxArbNetAmount";
            this.textBoxArbNetAmount.Size = new System.Drawing.Size(141, 28);
            this.textBoxArbNetAmount.TabIndex = 11;
            this.textBoxArbNetAmount.Visible = false;
            this.textBoxArbNetAmount.TextChanged += new System.EventHandler(this.textBoxArbNetAmount_TextChanged);
            // 
            // checkBoxScrollFix
            // 
            this.checkBoxScrollFix.AutoSize = true;
            this.checkBoxScrollFix.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkBoxScrollFix.Location = new System.Drawing.Point(141, 9);
            this.checkBoxScrollFix.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxScrollFix.Name = "checkBoxScrollFix";
            this.checkBoxScrollFix.Size = new System.Drawing.Size(124, 21);
            this.checkBoxScrollFix.TabIndex = 10;
            this.checkBoxScrollFix.Text = "스크롤 고정";
            this.checkBoxScrollFix.UseVisualStyleBackColor = true;
            this.checkBoxScrollFix.CheckedChanged += new System.EventHandler(this.checkBoxScrollFix_CheckedChanged);
            // 
            // labelFillCount
            // 
            this.labelFillCount.AutoSize = true;
            this.labelFillCount.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelFillCount.Location = new System.Drawing.Point(849, 9);
            this.labelFillCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelFillCount.Name = "labelFillCount";
            this.labelFillCount.Size = new System.Drawing.Size(65, 17);
            this.labelFillCount.TabIndex = 9;
            this.labelFillCount.Text = "체결 : 0";
            // 
            // labelCalc
            // 
            this.labelCalc.AutoSize = true;
            this.labelCalc.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelCalc.Location = new System.Drawing.Point(651, 9);
            this.labelCalc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelCalc.Name = "labelCalc";
            this.labelCalc.Size = new System.Drawing.Size(40, 17);
            this.labelCalc.TabIndex = 8;
            this.labelCalc.Text = "Sum";
            this.labelCalc.Click += new System.EventHandler(this.labelCalc_Click);
            // 
            // textBoxCalcResult
            // 
            this.textBoxCalcResult.Location = new System.Drawing.Point(698, 1);
            this.textBoxCalcResult.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxCalcResult.Name = "textBoxCalcResult";
            this.textBoxCalcResult.Size = new System.Drawing.Size(141, 28);
            this.textBoxCalcResult.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(366, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "종목필터";
            // 
            // textBoxCodeFilter
            // 
            this.textBoxCodeFilter.Location = new System.Drawing.Point(446, 1);
            this.textBoxCodeFilter.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxCodeFilter.Name = "textBoxCodeFilter";
            this.textBoxCodeFilter.Size = new System.Drawing.Size(141, 28);
            this.textBoxCodeFilter.TabIndex = 5;
            this.textBoxCodeFilter.TextChanged += new System.EventHandler(this.textBoxCodeFilter_TextChanged);
            // 
            // FilledOrderCompressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1278, 201);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximumSize = new System.Drawing.Size(1300, 1592);
            this.MinimumSize = new System.Drawing.Size(1276, 56);
            this.Name = "FilledOrderCompressForm";
            this.Text = "당일체결내역2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FilledOrderCompressForm_FormClosing);
            this.Load += new System.EventHandler(this.FilledOrdersForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilledOrder)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private FilledOrderGridView dgvFilledOrder;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxCodeFilter;
        private System.Windows.Forms.Label labelCalc;
        private System.Windows.Forms.TextBox textBoxCalcResult;
        private System.Windows.Forms.Label labelFillCount;
        private System.Windows.Forms.CheckBox checkBoxScrollFix;
        private System.Windows.Forms.TextBox textBoxTradingPnL;
        private System.Windows.Forms.TextBox textBoxArbNetAmount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn 북코드DataGridViewTextBoxColumn;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.CheckBox checkBoxFilterFix;
        private System.Windows.Forms.DataGridViewTextBoxColumn 서버;
        private System.Windows.Forms.DataGridViewTextBoxColumn 시간DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn 개수;
        private System.Windows.Forms.DataGridViewTextBoxColumn 북코드;
        private System.Windows.Forms.DataGridViewTextBoxColumn isinCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn 종목명DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Purpose;
        private System.Windows.Forms.DataGridViewTextBoxColumn 수량DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn 체결가DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn 이론가DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn 주문번호DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContractType;
        private System.Windows.Forms.ComboBox comboBoxServer;
        /*
#region Windows Form Designer generated code

/// <summary>
/// Required method for Designer support - do not modify
/// the contents of this method with the code editor.
/// </summary>
private void InitializeComponent()
{
this.SuspendLayout();
// 
// FilledOrdersForm
// 
this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
this.BackColor = System.Drawing.SystemColors.ActiveCaption;
this.ClientSize = new System.Drawing.Size(637, 544);
this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
this.Name = "FilledOrdersForm";
this.Text = "FilledOrdersForm";
this.ResumeLayout(false);

}

#endregion
*/
    }
}