
namespace DiskTest11
{
    partial class DiskSetting
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.StartTest = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Disk_Information_Framework = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Disk_Information_Framework)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.StartTest);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.Disk_Information_Framework);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Location = new System.Drawing.Point(28, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(767, 761);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Disk selection";
            // 
            // StartTest
            // 
            this.StartTest.Location = new System.Drawing.Point(258, 718);
            this.StartTest.Name = "StartTest";
            this.StartTest.Size = new System.Drawing.Size(156, 37);
            this.StartTest.TabIndex = 15;
            this.StartTest.Text = "开始测试";
            this.StartTest.UseVisualStyleBackColor = true;
            this.StartTest.Click += new System.EventHandler(this.StartTest_Click);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(34, 238);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(673, 474);
            this.panel1.TabIndex = 10;
            // 
            // Disk_Information_Framework
            // 
            this.Disk_Information_Framework.BackgroundColor = System.Drawing.SystemColors.Window;
            this.Disk_Information_Framework.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Disk_Information_Framework.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5});
            this.Disk_Information_Framework.Location = new System.Drawing.Point(34, 69);
            this.Disk_Information_Framework.Name = "Disk_Information_Framework";
            this.Disk_Information_Framework.RowHeadersWidth = 62;
            this.Disk_Information_Framework.RowTemplate.Height = 30;
            this.Disk_Information_Framework.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.Disk_Information_Framework.Size = new System.Drawing.Size(673, 163);
            this.Disk_Information_Framework.TabIndex = 9;
            this.Disk_Information_Framework.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Drive";
            this.Column1.MinimumWidth = 8;
            this.Column1.Name = "Column1";
            this.Column1.Width = 150;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Size";
            this.Column2.MinimumWidth = 8;
            this.Column2.Name = "Column2";
            this.Column2.Width = 150;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "属性";
            this.Column3.MinimumWidth = 8;
            this.Column3.Name = "Column3";
            this.Column3.Width = 150;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "扇区总数";
            this.Column4.MinimumWidth = 8;
            this.Column4.Name = "Column4";
            this.Column4.Width = 150;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "扇区大小";
            this.Column5.MinimumWidth = 8;
            this.Column5.Name = "Column5";
            this.Column5.Width = 150;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(34, 28);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(686, 35);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Automatically select all fixed drives at the start of testing";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // DiskSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 785);
            this.Controls.Add(this.groupBox1);
            this.Name = "DiskSetting";
            this.Text = "Disk";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Disk_Information_Framework)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.DataGridView Disk_Information_Framework;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button StartTest;
    }
}
