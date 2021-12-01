
using System;

namespace DiskTest11
{
    partial class Disk
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.confirm = new System.Windows.Forms.Button();
            this.TestDataMode = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.CircleNumble = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.TestMode = new System.Windows.Forms.ComboBox();
            this.TestTime = new System.Windows.Forms.TextBox();
            this.TestNum = new System.Windows.Forms.TextBox();
            this.TestPercent = new System.Windows.Forms.TextBox();
            this.BlockSize = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TestOrNot = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.confirm);
            this.groupBox2.Controls.Add(this.TestDataMode);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.CircleNumble);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.TestMode);
            this.groupBox2.Controls.Add(this.TestTime);
            this.groupBox2.Controls.Add(this.TestNum);
            this.groupBox2.Controls.Add(this.TestPercent);
            this.groupBox2.Controls.Add(this.BlockSize);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.TestOrNot);
            this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(12, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(560, 444);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Edit details";
            // 
            // confirm
            // 
            this.confirm.Location = new System.Drawing.Point(198, 317);
            this.confirm.Name = "confirm";
            this.confirm.Size = new System.Drawing.Size(141, 33);
            this.confirm.TabIndex = 22;
            this.confirm.Text = "确定";
            this.confirm.UseVisualStyleBackColor = true;
            this.confirm.Click += new System.EventHandler(this.confirm_Click);
            // 
            // TestDataMode
            // 
            this.TestDataMode.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.TestDataMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TestDataMode.FormattingEnabled = true;
            this.TestDataMode.Items.AddRange(new object[] {
            "全0",
            "全1",
            "随机数"});
            this.TestDataMode.Location = new System.Drawing.Point(113, 121);
            this.TestDataMode.Name = "TestDataMode";
            this.TestDataMode.Size = new System.Drawing.Size(130, 32);
            this.TestDataMode.TabIndex = 21;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 121);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 24);
            this.label7.TabIndex = 20;
            this.label7.Text = "数据类型";
            // 
            // CircleNumble
            // 
            this.CircleNumble.Location = new System.Drawing.Point(368, 255);
            this.CircleNumble.Name = "CircleNumble";
            this.CircleNumble.Size = new System.Drawing.Size(130, 31);
            this.CircleNumble.TabIndex = 19;
            this.CircleNumble.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(280, 258);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 24);
            this.label6.TabIndex = 18;
            this.label6.Text = "循环次数";
            // 
            // TestMode
            // 
            this.TestMode.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.TestMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TestMode.FormattingEnabled = true;
            this.TestMode.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.TestMode.Items.AddRange(new object[] {
            "随机读写验证",
            "随机只读",
            "随机只写",
            "顺序读写验证",
            "顺序只读",
            "顺序只写"});
            this.TestMode.Location = new System.Drawing.Point(113, 66);
            this.TestMode.Name = "TestMode";
            this.TestMode.Size = new System.Drawing.Size(355, 32);
            this.TestMode.TabIndex = 17;
            // 
            // TestTime
            // 
            this.TestTime.Location = new System.Drawing.Point(113, 255);
            this.TestTime.Name = "TestTime";
            this.TestTime.Size = new System.Drawing.Size(130, 31);
            this.TestTime.TabIndex = 10;
            this.TestTime.Text = "0";
            // 
            // TestNum
            // 
            this.TestNum.Location = new System.Drawing.Point(368, 182);
            this.TestNum.Name = "TestNum";
            this.TestNum.Size = new System.Drawing.Size(130, 31);
            this.TestNum.TabIndex = 9;
            this.TestNum.Text = "0";
            // 
            // TestPercent
            // 
            this.TestPercent.Location = new System.Drawing.Point(368, 122);
            this.TestPercent.Name = "TestPercent";
            this.TestPercent.Size = new System.Drawing.Size(130, 31);
            this.TestPercent.TabIndex = 8;
            this.TestPercent.Text = "100";
            // 
            // BlockSize
            // 
            this.BlockSize.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.BlockSize.DisplayMember = "1";
            this.BlockSize.FormattingEnabled = true;
            this.BlockSize.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.BlockSize.Location = new System.Drawing.Point(110, 181);
            this.BlockSize.Name = "BlockSize";
            this.BlockSize.Size = new System.Drawing.Size(130, 32);
            this.BlockSize.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 255);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 24);
            this.label5.TabIndex = 6;
            this.label5.Text = "测试时间";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(280, 181);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 24);
            this.label4.TabIndex = 5;
            this.label4.Text = "测试次数";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 181);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 24);
            this.label3.TabIndex = 4;
            this.label3.Text = "Block size";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(280, 121);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 24);
            this.label2.TabIndex = 3;
            this.label2.Text = "测试容量";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 24);
            this.label1.TabIndex = 2;
            this.label1.Text = "测试模式";
            // 
            // TestOrNot
            // 
            this.TestOrNot.AutoSize = true;
            this.TestOrNot.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.TestOrNot.Location = new System.Drawing.Point(6, 27);
            this.TestOrNot.Name = "TestOrNot";
            this.TestOrNot.Size = new System.Drawing.Size(155, 28);
            this.TestOrNot.TabIndex = 0;
            this.TestOrNot.Text = "Test this drive";
            this.TestOrNot.UseVisualStyleBackColor = true;
            this.TestOrNot.CheckedChanged += new System.EventHandler(this.TestOrNot_CheckedChanged);
            // 
            // Disk
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(593, 456);
            this.Controls.Add(this.groupBox2);
            this.Name = "Disk";
            this.Text = "Disk";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }



        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.ComboBox TestDataMode;
        public System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox CircleNumble;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox TestMode;
        private System.Windows.Forms.TextBox TestTime;
        private System.Windows.Forms.TextBox TestNum;
        private System.Windows.Forms.TextBox TestPercent;
        private System.Windows.Forms.ComboBox BlockSize;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox TestOrNot;
        private System.Windows.Forms.Button confirm;
    }
}