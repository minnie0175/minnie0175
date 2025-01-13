namespace Ciri.Forms
{
    partial class QuotingInfoLoaderListForm
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
            TenTec.Windows.iGridLib.iGColPattern iGColPattern1 = new TenTec.Windows.iGridLib.iGColPattern();
            TenTec.Windows.iGridLib.iGColPattern iGColPattern2 = new TenTec.Windows.iGridLib.iGColPattern();
            TenTec.Windows.iGridLib.iGColPattern iGColPattern3 = new TenTec.Windows.iGridLib.iGColPattern();
            TenTec.Windows.iGridLib.iGColPattern iGColPattern4 = new TenTec.Windows.iGridLib.iGColPattern();
            TenTec.Windows.iGridLib.iGColPattern iGColPattern5 = new TenTec.Windows.iGridLib.iGColPattern();
            TenTec.Windows.iGridLib.iGColPattern iGColPattern6 = new TenTec.Windows.iGridLib.iGColPattern();
            TenTec.Windows.iGridLib.iGColPattern iGColPattern7 = new TenTec.Windows.iGridLib.iGColPattern();
            TenTec.Windows.iGridLib.iGColPattern iGColPattern8 = new TenTec.Windows.iGridLib.iGColPattern();
            TenTec.Windows.iGridLib.iGColPattern iGColPattern9 = new TenTec.Windows.iGridLib.iGColPattern();
            this.iGridQuotingInfoCol16CellStyle = new TenTec.Windows.iGridLib.iGCellStyle(true);
            this.iGridQuotingInfoCol16ColHdrStyle = new TenTec.Windows.iGridLib.iGColHdrStyle(true);
            this.iGridQuotingInfoCol4CellStyle = new TenTec.Windows.iGridLib.iGCellStyle(true);
            this.iGridQuotingInfoCol4ColHdrStyle = new TenTec.Windows.iGridLib.iGColHdrStyle(true);
            this.iGridQuotingInfoCol5CellStyle = new TenTec.Windows.iGridLib.iGCellStyle(true);
            this.iGridQuotingInfoCol5ColHdrStyle = new TenTec.Windows.iGridLib.iGColHdrStyle(true);
            this.iGridQuotingInfoCol6CellStyle = new TenTec.Windows.iGridLib.iGCellStyle(true);
            this.iGridQuotingInfoCol6ColHdrStyle = new TenTec.Windows.iGridLib.iGColHdrStyle(true);
            this.iGridQuotingInfoCol7CellStyle = new TenTec.Windows.iGridLib.iGCellStyle(true);
            this.iGridQuotingInfoCol7ColHdrStyle = new TenTec.Windows.iGridLib.iGColHdrStyle(true);
            this.iGridQuotingInfoCol10CellStyle = new TenTec.Windows.iGridLib.iGCellStyle(true);
            this.iGridQuotingInfoCol10ColHdrStyle = new TenTec.Windows.iGridLib.iGColHdrStyle(true);
            this.iGridQuotingInfoCol8CellStyle = new TenTec.Windows.iGridLib.iGCellStyle(true);
            this.iGridQuotingInfoCol8ColHdrStyle = new TenTec.Windows.iGridLib.iGColHdrStyle(true);
            this.buttonLoad = new System.Windows.Forms.Button();
            this.iGridQuotingInfo = new TenTec.Windows.iGridLib.iGrid();
            ((System.ComponentModel.ISupportInitialize)(this.iGridQuotingInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonLoad
            // 
            this.buttonLoad.BackColor = System.Drawing.Color.White;
            this.buttonLoad.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonLoad.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.buttonLoad.ForeColor = System.Drawing.Color.Black;
            this.buttonLoad.Location = new System.Drawing.Point(0, 556);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(618, 24);
            this.buttonLoad.TabIndex = 253;
            this.buttonLoad.Text = "불러오기";
            this.buttonLoad.UseVisualStyleBackColor = false;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // iGridQuotingInfo
            // 
            iGColPattern1.CellStyle = this.iGridQuotingInfoCol16CellStyle;
            iGColPattern1.ColHdrStyle = this.iGridQuotingInfoCol16ColHdrStyle;
            iGColPattern1.Key = "종류";
            iGColPattern1.Text = "종류";
            iGColPattern2.CellStyle = this.iGridQuotingInfoCol4CellStyle;
            iGColPattern2.ColHdrStyle = this.iGridQuotingInfoCol4ColHdrStyle;
            iGColPattern2.Key = "유형1";
            iGColPattern2.Text = "유형1";
            iGColPattern3.CellStyle = this.iGridQuotingInfoCol5CellStyle;
            iGColPattern3.ColHdrStyle = this.iGridQuotingInfoCol5ColHdrStyle;
            iGColPattern3.Key = "유형2";
            iGColPattern3.Text = "유형2";
            iGColPattern4.CellStyle = this.iGridQuotingInfoCol6CellStyle;
            iGColPattern4.ColHdrStyle = this.iGridQuotingInfoCol6ColHdrStyle;
            iGColPattern4.Key = "유형3";
            iGColPattern4.Text = "유형3";
            iGColPattern5.CellStyle = this.iGridQuotingInfoCol7CellStyle;
            iGColPattern5.ColHdrStyle = this.iGridQuotingInfoCol7ColHdrStyle;
            iGColPattern5.Key = "유형4";
            iGColPattern5.Text = "유형4";
            iGColPattern6.Key = "종목명";
            iGColPattern6.Text = "종목명";
            iGColPattern6.Width = 150;
            iGColPattern7.Key = "종목코드";
            iGColPattern7.SortType = TenTec.Windows.iGridLib.iGSortType.None;
            iGColPattern7.Text = "종목코드";
            iGColPattern7.Visible = false;
            iGColPattern7.Width = 100;
            iGColPattern8.CellStyle = this.iGridQuotingInfoCol10CellStyle;
            iGColPattern8.ColHdrStyle = this.iGridQuotingInfoCol10ColHdrStyle;
            iGColPattern8.Key = "Purpose";
            iGColPattern8.Text = "Purpose";
            iGColPattern9.CellStyle = this.iGridQuotingInfoCol8CellStyle;
            iGColPattern9.ColHdrStyle = this.iGridQuotingInfoCol8ColHdrStyle;
            iGColPattern9.Key = "Style";
            iGColPattern9.Text = "Style";
            this.iGridQuotingInfo.Cols.AddRange(new TenTec.Windows.iGridLib.iGColPattern[] {
            iGColPattern1,
            iGColPattern2,
            iGColPattern3,
            iGColPattern4,
            iGColPattern5,
            iGColPattern6,
            iGColPattern7,
            iGColPattern8,
            iGColPattern9});
            this.iGridQuotingInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iGridQuotingInfo.Header.Height = 22;
            this.iGridQuotingInfo.Location = new System.Drawing.Point(0, 0);
            this.iGridQuotingInfo.Name = "iGridQuotingInfo";
            this.iGridQuotingInfo.ReadOnly = true;
            this.iGridQuotingInfo.RowMode = true;
            this.iGridQuotingInfo.SelectionMode = TenTec.Windows.iGridLib.iGSelectionMode.MultiExtended;
            this.iGridQuotingInfo.Size = new System.Drawing.Size(618, 556);
            this.iGridQuotingInfo.StaySorted = true;
            this.iGridQuotingInfo.TabIndex = 255;
            // 
            // QuotingInfoLoaderListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 580);
            this.Controls.Add(this.iGridQuotingInfo);
            this.Controls.Add(this.buttonLoad);
            this.Name = "QuotingInfoLoaderListForm";
            this.Text = "QuotingInfoLoaderListForm";
            ((System.ComponentModel.ISupportInitialize)(this.iGridQuotingInfo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonLoad;
        private TenTec.Windows.iGridLib.iGrid iGridQuotingInfo;
        private TenTec.Windows.iGridLib.iGCellStyle iGridQuotingInfoCol16CellStyle;
        private TenTec.Windows.iGridLib.iGColHdrStyle iGridQuotingInfoCol16ColHdrStyle;
        private TenTec.Windows.iGridLib.iGCellStyle iGridQuotingInfoCol4CellStyle;
        private TenTec.Windows.iGridLib.iGColHdrStyle iGridQuotingInfoCol4ColHdrStyle;
        private TenTec.Windows.iGridLib.iGCellStyle iGridQuotingInfoCol5CellStyle;
        private TenTec.Windows.iGridLib.iGColHdrStyle iGridQuotingInfoCol5ColHdrStyle;
        private TenTec.Windows.iGridLib.iGCellStyle iGridQuotingInfoCol6CellStyle;
        private TenTec.Windows.iGridLib.iGColHdrStyle iGridQuotingInfoCol6ColHdrStyle;
        private TenTec.Windows.iGridLib.iGCellStyle iGridQuotingInfoCol10CellStyle;
        private TenTec.Windows.iGridLib.iGColHdrStyle iGridQuotingInfoCol10ColHdrStyle;
        private TenTec.Windows.iGridLib.iGCellStyle iGridQuotingInfoCol7CellStyle;
        private TenTec.Windows.iGridLib.iGColHdrStyle iGridQuotingInfoCol7ColHdrStyle;
        private TenTec.Windows.iGridLib.iGCellStyle iGridQuotingInfoCol8CellStyle;
        private TenTec.Windows.iGridLib.iGColHdrStyle iGridQuotingInfoCol8ColHdrStyle;
    }
}