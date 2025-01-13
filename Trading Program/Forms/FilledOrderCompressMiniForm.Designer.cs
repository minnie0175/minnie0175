using CommonLib.GridView;

namespace Ciri.Forms
{
    partial class FilledOrderCompressMiniForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dgvFilledOrder = new CommonLib.GridView.FilledOrderGridView();
            this.시간DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.개수 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.codeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.종목명DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Purpose = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.수량DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.체결가DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.주문번호DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.체결번호 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContractType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilledOrder)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.dgvFilledOrder, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 222F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(224, 222);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // dgvFilledOrder
            // 
            this.dgvFilledOrder.AllowUserToAddRows = false;
            this.dgvFilledOrder.AllowUserToDeleteRows = false;
            this.dgvFilledOrder.AllowUserToResizeRows = false;
            this.dgvFilledOrder.AutoGenerateColumns = false;
            this.dgvFilledOrder.BackgroundColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFilledOrder.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvFilledOrder.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFilledOrder.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.시간DataGridViewTextBoxColumn,
            this.개수,
            this.codeDataGridViewTextBoxColumn,
            this.종목명DataGridViewTextBoxColumn,
            this.Purpose,
            this.수량DataGridViewTextBoxColumn,
            this.체결가DataGridViewTextBoxColumn,
            this.주문번호DataGridViewTextBoxColumn,
            this.체결번호,
            this.ContractType});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFilledOrder.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvFilledOrder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFilledOrder.Location = new System.Drawing.Point(3, 3);
            this.dgvFilledOrder.Name = "dgvFilledOrder";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFilledOrder.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvFilledOrder.RowHeadersVisible = false;
            this.dgvFilledOrder.RowTemplate.Height = 23;
            this.dgvFilledOrder.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvFilledOrder.Size = new System.Drawing.Size(218, 216);
            this.dgvFilledOrder.TabIndex = 1;
            this.dgvFilledOrder.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvFilledOrder_CellMouseClick);
            // 
            // 시간DataGridViewTextBoxColumn
            // 
            this.시간DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.시간DataGridViewTextBoxColumn.DataPropertyName = "시간";
            this.시간DataGridViewTextBoxColumn.HeaderText = "시간";
            this.시간DataGridViewTextBoxColumn.Name = "시간DataGridViewTextBoxColumn";
            this.시간DataGridViewTextBoxColumn.Width = 80;
            // 
            // 개수
            // 
            this.개수.DataPropertyName = "개수";
            this.개수.HeaderText = "개수";
            this.개수.Name = "개수";
            this.개수.Visible = false;
            this.개수.Width = 30;
            // 
            // codeDataGridViewTextBoxColumn
            // 
            this.codeDataGridViewTextBoxColumn.DataPropertyName = "isinCode";
            this.codeDataGridViewTextBoxColumn.HeaderText = "isinCode";
            this.codeDataGridViewTextBoxColumn.Name = "codeDataGridViewTextBoxColumn";
            this.codeDataGridViewTextBoxColumn.Visible = false;
            this.codeDataGridViewTextBoxColumn.Width = 5;
            // 
            // 종목명DataGridViewTextBoxColumn
            // 
            this.종목명DataGridViewTextBoxColumn.DataPropertyName = "종목명";
            this.종목명DataGridViewTextBoxColumn.HeaderText = "종목명";
            this.종목명DataGridViewTextBoxColumn.Name = "종목명DataGridViewTextBoxColumn";
            this.종목명DataGridViewTextBoxColumn.Visible = false;
            this.종목명DataGridViewTextBoxColumn.Width = 5;
            // 
            // Purpose
            // 
            this.Purpose.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Purpose.DataPropertyName = "Purpose";
            this.Purpose.HeaderText = "Purpose";
            this.Purpose.Name = "Purpose";
            this.Purpose.Visible = false;
            this.Purpose.Width = 15;
            // 
            // 수량DataGridViewTextBoxColumn
            // 
            this.수량DataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.수량DataGridViewTextBoxColumn.DataPropertyName = "수량";
            this.수량DataGridViewTextBoxColumn.HeaderText = "수량";
            this.수량DataGridViewTextBoxColumn.Name = "수량DataGridViewTextBoxColumn";
            this.수량DataGridViewTextBoxColumn.Width = 60;
            // 
            // 체결가DataGridViewTextBoxColumn
            // 
            this.체결가DataGridViewTextBoxColumn.DataPropertyName = "체결가";
            this.체결가DataGridViewTextBoxColumn.HeaderText = "체결가";
            this.체결가DataGridViewTextBoxColumn.Name = "체결가DataGridViewTextBoxColumn";
            this.체결가DataGridViewTextBoxColumn.Visible = false;
            this.체결가DataGridViewTextBoxColumn.Width = 70;
            // 
            // 주문번호DataGridViewTextBoxColumn
            // 
            this.주문번호DataGridViewTextBoxColumn.DataPropertyName = "주문번호";
            this.주문번호DataGridViewTextBoxColumn.HeaderText = "주문번호";
            this.주문번호DataGridViewTextBoxColumn.Name = "주문번호DataGridViewTextBoxColumn";
            this.주문번호DataGridViewTextBoxColumn.Visible = false;
            this.주문번호DataGridViewTextBoxColumn.Width = 5;
            // 
            // 체결번호
            // 
            this.체결번호.DataPropertyName = "체결번호";
            this.체결번호.HeaderText = "체결번호";
            this.체결번호.Name = "체결번호";
            this.체결번호.Visible = false;
            this.체결번호.Width = 5;
            // 
            // ContractType
            // 
            this.ContractType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ContractType.DataPropertyName = "ContractType";
            this.ContractType.HeaderText = "ContractType";
            this.ContractType.Name = "ContractType";
            this.ContractType.Width = 90;
            // 
            // FilledOrderCompressMiniForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(224, 222);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.MaximumSize = new System.Drawing.Size(240, 1080);
            this.MinimumSize = new System.Drawing.Size(240, 39);
            this.Name = "FilledOrderCompressMiniForm";
            this.Text = "자세히 보기";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FilledOrderCompressMiniForm_FormClosing);
            this.Load += new System.EventHandler(this.FilledOrdersForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilledOrder)).EndInit();
            this.ResumeLayout(false);

        }

        private FilledOrderGridView dgvFilledOrder;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn 시간DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn 개수;
        private System.Windows.Forms.DataGridViewTextBoxColumn codeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn 종목명DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Purpose;
        private System.Windows.Forms.DataGridViewTextBoxColumn 수량DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn 체결가DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn 주문번호DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn 체결번호;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContractType;

        #endregion
    }
}