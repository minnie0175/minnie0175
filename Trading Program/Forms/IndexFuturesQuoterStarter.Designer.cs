namespace Ciri.Forms
{
    partial class IndexFuturesQuoterStarter
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
            this.applyButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxCategory = new System.Windows.Forms.ComboBox();
            this.datalabel = new System.Windows.Forms.Label();
            this.comboBoxSelectStyle = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxSpreadMode = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // applyButton
            // 
            this.applyButton.BackColor = System.Drawing.Color.White;
            this.applyButton.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold);
            this.applyButton.Location = new System.Drawing.Point(29, 120);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(93, 26);
            this.applyButton.TabIndex = 18;
            this.applyButton.Text = "불러오기";
            this.applyButton.UseVisualStyleBackColor = false;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 17);
            this.label1.TabIndex = 17;
            this.label1.Text = "분류";
            // 
            // comboBoxCategory
            // 
            this.comboBoxCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCategory.FormattingEnabled = true;
            this.comboBoxCategory.Location = new System.Drawing.Point(58, 8);
            this.comboBoxCategory.Name = "comboBoxCategory";
            this.comboBoxCategory.Size = new System.Drawing.Size(87, 20);
            this.comboBoxCategory.TabIndex = 272;
            this.comboBoxCategory.SelectedIndexChanged += new System.EventHandler(this.comboBoxCategory_SelectedIndexChanged);
            // 
            // datalabel
            // 
            this.datalabel.AutoSize = true;
            this.datalabel.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold);
            this.datalabel.Location = new System.Drawing.Point(16, 95);
            this.datalabel.Name = "datalabel";
            this.datalabel.Size = new System.Drawing.Size(115, 17);
            this.datalabel.TabIndex = 273;
            this.datalabel.Text = "종목 개수 : 000개";
            this.datalabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // comboBoxSelectStyle
            // 
            this.comboBoxSelectStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSelectStyle.FormattingEnabled = true;
            this.comboBoxSelectStyle.Location = new System.Drawing.Point(58, 35);
            this.comboBoxSelectStyle.Name = "comboBoxSelectStyle";
            this.comboBoxSelectStyle.Size = new System.Drawing.Size(87, 20);
            this.comboBoxSelectStyle.TabIndex = 277;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(8, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 17);
            this.label3.TabIndex = 276;
            this.label3.Text = "Style";
            // 
            // checkBoxSpreadMode
            // 
            this.checkBoxSpreadMode.AutoSize = true;
            this.checkBoxSpreadMode.Location = new System.Drawing.Point(110, 65);
            this.checkBoxSpreadMode.Name = "checkBoxSpreadMode";
            this.checkBoxSpreadMode.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSpreadMode.TabIndex = 709;
            this.checkBoxSpreadMode.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(8, 63);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(91, 17);
            this.label8.TabIndex = 708;
            this.label8.Text = "Spread Mode";
            // 
            // IndexFuturesQuoterStarter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(156, 157);
            this.Controls.Add(this.checkBoxSpreadMode);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.comboBoxSelectStyle);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.datalabel);
            this.Controls.Add(this.comboBoxCategory);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.label1);
            this.Name = "IndexFuturesQuoterStarter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IndexFuturesQuoterStarter";
            this.Load += new System.EventHandler(this.StockQuoter2Starter_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxCategory;
        private System.Windows.Forms.Label datalabel;
        private System.Windows.Forms.ComboBox comboBoxSelectStyle;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBoxSpreadMode;
        private System.Windows.Forms.Label label8;
    }
}