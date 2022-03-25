
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
            this.ThreadNumble = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.Record_CheckBox = new System.Windows.Forms.CheckBox();
            this.Repeat_Test_Checkbox = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
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
            this.groupBox2.Controls.Add(this.ThreadNumble);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.Record_CheckBox);
            this.groupBox2.Controls.Add(this.Repeat_Test_Checkbox);
            this.groupBox2.Controls.Add(this.label8);
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
            // ThreadNumble
            // 
            this.ThreadNumble.Location = new System.Drawing.Point(99, 247);
            this.ThreadNumble.Name = "ThreadNumble";
            this.ThreadNumble.Size = new System.Drawing.Size(130, 31);
            this.ThreadNumble.TabIndex = 27;
            this.ThreadNumble.Text = "1";
            this.ThreadNumble.TextChanged += new System.EventHandler(this.ThreadNumble_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 247);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(64, 24);
            this.label9.TabIndex = 26;
            this.label9.Text = "线程数";
            this.label9.Click += new System.EventHandler(this.label9_Click);
            // 
            // Record_CheckBox
            // 
            this.Record_CheckBox.AutoSize = true;
            this.Record_CheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Record_CheckBox.Location = new System.Drawing.Point(192, 27);
            this.Record_CheckBox.Name = "Record_CheckBox";
            this.Record_CheckBox.Size = new System.Drawing.Size(97, 28);
            this.Record_CheckBox.TabIndex = 25;
            this.Record_CheckBox.Text = "Record";
            this.Record_CheckBox.UseVisualStyleBackColor = true;
            // 
            // Repeat_Test_Checkbox
            // 
            this.Repeat_Test_Checkbox.AutoSize = true;
            this.Repeat_Test_Checkbox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Repeat_Test_Checkbox.Location = new System.Drawing.Point(316, 27);
            this.Repeat_Test_Checkbox.Name = "Repeat_Test_Checkbox";
            this.Repeat_Test_Checkbox.Size = new System.Drawing.Size(97, 28);
            this.Repeat_Test_Checkbox.TabIndex = 24;
            this.Repeat_Test_Checkbox.Text = "Repeat";
            this.Repeat_Test_Checkbox.UseVisualStyleBackColor = true;
            this.Repeat_Test_Checkbox.CheckedChanged += new System.EventHandler(this.Repeat_Test_CheckBoxChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(243, 161);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 24);
            this.label8.TabIndex = 23;
            this.label8.Text = "KB";
            // 
            // confirm
            // 
            this.confirm.Location = new System.Drawing.Point(15, 284);
            this.confirm.Name = "confirm";
            this.confirm.Size = new System.Drawing.Size(106, 52);
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
            this.TestDataMode.Location = new System.Drawing.Point(96, 102);
            this.TestDataMode.Name = "TestDataMode";
            this.TestDataMode.Size = new System.Drawing.Size(130, 32);
            this.TestDataMode.TabIndex = 21;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 102);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 24);
            this.label7.TabIndex = 20;
            this.label7.Text = "数据类型";
            // 
            // CircleNumble
            // 
            this.CircleNumble.Location = new System.Drawing.Point(368, 204);
            this.CircleNumble.Name = "CircleNumble";
            this.CircleNumble.Size = new System.Drawing.Size(130, 31);
            this.CircleNumble.TabIndex = 19;
            this.CircleNumble.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(280, 207);
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
            this.TestMode.Location = new System.Drawing.Point(96, 58);
            this.TestMode.Name = "TestMode";
            this.TestMode.Size = new System.Drawing.Size(355, 32);
            this.TestMode.TabIndex = 17;
            this.TestMode.SelectedIndexChanged += new System.EventHandler(this.TestMode_SelectedIndexChanged);
            // 
            // TestTime
            // 
            this.TestTime.Location = new System.Drawing.Point(99, 200);
            this.TestTime.Name = "TestTime";
            this.TestTime.Size = new System.Drawing.Size(130, 31);
            this.TestTime.TabIndex = 10;
            this.TestTime.Text = "0";
            // 
            // TestNum
            // 
            this.TestNum.Location = new System.Drawing.Point(368, 158);
            this.TestNum.Name = "TestNum";
            this.TestNum.Size = new System.Drawing.Size(130, 31);
            this.TestNum.TabIndex = 9;
            this.TestNum.Text = "0";
            // 
            // TestPercent
            // 
            this.TestPercent.Location = new System.Drawing.Point(368, 107);
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
            "4",
            "8",
            "16",
            "32",
            "64",
            "128",
            "256",
            "512",
            "1024",
            "2048",
            "4096",
            "8192",
            "16384",
            "32768",
            "65536"});
            this.BlockSize.Location = new System.Drawing.Point(96, 153);
            this.BlockSize.Name = "BlockSize";
            this.BlockSize.Size = new System.Drawing.Size(130, 32);
            this.BlockSize.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 200);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 24);
            this.label5.TabIndex = 6;
            this.label5.Text = "测试时间";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(280, 161);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 24);
            this.label4.TabIndex = 5;
            this.label4.Text = "测试次数";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 156);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 24);
            this.label3.TabIndex = 4;
            this.label3.Text = "块大小";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(280, 107);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 24);
            this.label2.TabIndex = 3;
            this.label2.Text = "测试容量";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 58);
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
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox Repeat_Test_Checkbox;
        private System.Windows.Forms.CheckBox Record_CheckBox;
        private System.Windows.Forms.TextBox ThreadNumble;
        private System.Windows.Forms.Label label9;
    }
}