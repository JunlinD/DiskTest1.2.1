
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
            this.Choose_TestNumradioButton = new System.Windows.Forms.RadioButton();
            this.Choose_TestTimeradioButton = new System.Windows.Forms.RadioButton();
            this.ThreadNum_label = new System.Windows.Forms.Label();
            this.Record_CheckBox = new System.Windows.Forms.CheckBox();
            this.Repeat_Test_Checkbox = new System.Windows.Forms.CheckBox();
            this.KB_label = new System.Windows.Forms.Label();
            this.confirm = new System.Windows.Forms.Button();
            this.TestDataMode = new System.Windows.Forms.ComboBox();
            this.TestDataMode_label = new System.Windows.Forms.Label();
            this.CircleNum_label = new System.Windows.Forms.Label();
            this.TestMode = new System.Windows.Forms.ComboBox();
            this.TestTime_s = new System.Windows.Forms.TextBox();
            this.TestNum = new System.Windows.Forms.TextBox();
            this.BlockSize = new System.Windows.Forms.ComboBox();
            this.TestTime_label = new System.Windows.Forms.Label();
            this.TestNum_label = new System.Windows.Forms.Label();
            this.TestBlock_label = new System.Windows.Forms.Label();
            this.TestPercent_label = new System.Windows.Forms.Label();
            this.Test_Mode_label = new System.Windows.Forms.Label();
            this.TestOrNot = new System.Windows.Forms.CheckBox();
            this.TestTime_S_label = new System.Windows.Forms.Label();
            this.TestTime_M_label = new System.Windows.Forms.Label();
            this.TestTime_m = new System.Windows.Forms.TextBox();
            this.TestPercent = new System.Windows.Forms.NumericUpDown();
            this.ThreadNumble = new System.Windows.Forms.NumericUpDown();
            this.CircleNumble = new System.Windows.Forms.NumericUpDown();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TestPercent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ThreadNumble)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CircleNumble)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.CircleNumble);
            this.groupBox2.Controls.Add(this.ThreadNumble);
            this.groupBox2.Controls.Add(this.TestPercent);
            this.groupBox2.Controls.Add(this.TestTime_M_label);
            this.groupBox2.Controls.Add(this.TestTime_m);
            this.groupBox2.Controls.Add(this.TestTime_S_label);
            this.groupBox2.Controls.Add(this.Choose_TestNumradioButton);
            this.groupBox2.Controls.Add(this.Choose_TestTimeradioButton);
            this.groupBox2.Controls.Add(this.ThreadNum_label);
            this.groupBox2.Controls.Add(this.Record_CheckBox);
            this.groupBox2.Controls.Add(this.Repeat_Test_Checkbox);
            this.groupBox2.Controls.Add(this.KB_label);
            this.groupBox2.Controls.Add(this.confirm);
            this.groupBox2.Controls.Add(this.TestDataMode);
            this.groupBox2.Controls.Add(this.TestDataMode_label);
            this.groupBox2.Controls.Add(this.CircleNum_label);
            this.groupBox2.Controls.Add(this.TestMode);
            this.groupBox2.Controls.Add(this.TestTime_s);
            this.groupBox2.Controls.Add(this.TestNum);
            this.groupBox2.Controls.Add(this.BlockSize);
            this.groupBox2.Controls.Add(this.TestTime_label);
            this.groupBox2.Controls.Add(this.TestNum_label);
            this.groupBox2.Controls.Add(this.TestBlock_label);
            this.groupBox2.Controls.Add(this.TestPercent_label);
            this.groupBox2.Controls.Add(this.Test_Mode_label);
            this.groupBox2.Controls.Add(this.TestOrNot);
            this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(12, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(560, 444);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Edit details";
            // 
            // Choose_TestNumradioButton
            // 
            this.Choose_TestNumradioButton.AutoSize = true;
            this.Choose_TestNumradioButton.Font = new System.Drawing.Font("微软雅黑", 7.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Choose_TestNumradioButton.Location = new System.Drawing.Point(132, 153);
            this.Choose_TestNumradioButton.Name = "Choose_TestNumradioButton";
            this.Choose_TestNumradioButton.Size = new System.Drawing.Size(94, 23);
            this.Choose_TestNumradioButton.TabIndex = 29;
            this.Choose_TestNumradioButton.TabStop = true;
            this.Choose_TestNumradioButton.Text = "选择次数";
            this.Choose_TestNumradioButton.UseVisualStyleBackColor = true;
            this.Choose_TestNumradioButton.Visible = false;
            this.Choose_TestNumradioButton.CheckedChanged += new System.EventHandler(this.Choose_TestNumradioButton_CheckedChanged);
            // 
            // Choose_TestTimeradioButton
            // 
            this.Choose_TestTimeradioButton.AutoSize = true;
            this.Choose_TestTimeradioButton.Font = new System.Drawing.Font("微软雅黑", 7.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Choose_TestTimeradioButton.Location = new System.Drawing.Point(15, 153);
            this.Choose_TestTimeradioButton.Name = "Choose_TestTimeradioButton";
            this.Choose_TestTimeradioButton.Size = new System.Drawing.Size(94, 23);
            this.Choose_TestTimeradioButton.TabIndex = 28;
            this.Choose_TestTimeradioButton.TabStop = true;
            this.Choose_TestTimeradioButton.Text = "选择时间";
            this.Choose_TestTimeradioButton.UseVisualStyleBackColor = true;
            this.Choose_TestTimeradioButton.Visible = false;
            this.Choose_TestTimeradioButton.CheckedChanged += new System.EventHandler(this.Choose_TestTimeradioButton_CheckedChanged);
            // 
            // ThreadNum_label
            // 
            this.ThreadNum_label.AutoSize = true;
            this.ThreadNum_label.Location = new System.Drawing.Point(280, 247);
            this.ThreadNum_label.Name = "ThreadNum_label";
            this.ThreadNum_label.Size = new System.Drawing.Size(64, 24);
            this.ThreadNum_label.TabIndex = 26;
            this.ThreadNum_label.Text = "线程数";
            this.ThreadNum_label.Visible = false;
            this.ThreadNum_label.Click += new System.EventHandler(this.label9_Click);
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
            this.Record_CheckBox.Visible = false;
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
            this.Repeat_Test_Checkbox.Visible = false;
            this.Repeat_Test_Checkbox.CheckedChanged += new System.EventHandler(this.Repeat_Test_CheckBoxChanged);
            // 
            // KB_label
            // 
            this.KB_label.AutoSize = true;
            this.KB_label.Location = new System.Drawing.Point(501, 155);
            this.KB_label.Name = "KB_label";
            this.KB_label.Size = new System.Drawing.Size(32, 24);
            this.KB_label.TabIndex = 23;
            this.KB_label.Text = "KB";
            this.KB_label.Visible = false;
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
            this.TestDataMode.Visible = false;
            // 
            // TestDataMode_label
            // 
            this.TestDataMode_label.AutoSize = true;
            this.TestDataMode_label.Location = new System.Drawing.Point(11, 102);
            this.TestDataMode_label.Name = "TestDataMode_label";
            this.TestDataMode_label.Size = new System.Drawing.Size(82, 24);
            this.TestDataMode_label.TabIndex = 20;
            this.TestDataMode_label.Text = "数据类型";
            this.TestDataMode_label.Visible = false;
            // 
            // CircleNum_label
            // 
            this.CircleNum_label.AutoSize = true;
            this.CircleNum_label.Location = new System.Drawing.Point(280, 200);
            this.CircleNum_label.Name = "CircleNum_label";
            this.CircleNum_label.Size = new System.Drawing.Size(82, 24);
            this.CircleNum_label.TabIndex = 18;
            this.CircleNum_label.Text = "循环次数";
            this.CircleNum_label.Visible = false;
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
            this.TestMode.Visible = false;
            this.TestMode.SelectedIndexChanged += new System.EventHandler(this.TestMode_SelectedIndexChanged);
            // 
            // TestTime_s
            // 
            this.TestTime_s.Location = new System.Drawing.Point(179, 197);
            this.TestTime_s.Name = "TestTime_s";
            this.TestTime_s.Size = new System.Drawing.Size(32, 31);
            this.TestTime_s.TabIndex = 10;
            this.TestTime_s.Text = "0";
            this.TestTime_s.Visible = false;
            // 
            // TestNum
            // 
            this.TestNum.Location = new System.Drawing.Point(99, 247);
            this.TestNum.Name = "TestNum";
            this.TestNum.Size = new System.Drawing.Size(130, 31);
            this.TestNum.TabIndex = 9;
            this.TestNum.Text = "0";
            this.TestNum.Visible = false;
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
            this.BlockSize.Location = new System.Drawing.Point(365, 152);
            this.BlockSize.Name = "BlockSize";
            this.BlockSize.Size = new System.Drawing.Size(130, 32);
            this.BlockSize.TabIndex = 7;
            this.BlockSize.Visible = false;
            this.BlockSize.SelectedIndexChanged += new System.EventHandler(this.BlockSize_SelectedIndexChanged);
            // 
            // TestTime_label
            // 
            this.TestTime_label.AutoSize = true;
            this.TestTime_label.Location = new System.Drawing.Point(11, 200);
            this.TestTime_label.Name = "TestTime_label";
            this.TestTime_label.Size = new System.Drawing.Size(82, 24);
            this.TestTime_label.TabIndex = 6;
            this.TestTime_label.Text = "测试时间";
            this.TestTime_label.Visible = false;
            // 
            // TestNum_label
            // 
            this.TestNum_label.AutoSize = true;
            this.TestNum_label.Location = new System.Drawing.Point(11, 250);
            this.TestNum_label.Name = "TestNum_label";
            this.TestNum_label.Size = new System.Drawing.Size(82, 24);
            this.TestNum_label.TabIndex = 5;
            this.TestNum_label.Text = "测试次数";
            this.TestNum_label.Visible = false;
            // 
            // TestBlock_label
            // 
            this.TestBlock_label.AutoSize = true;
            this.TestBlock_label.Location = new System.Drawing.Point(280, 155);
            this.TestBlock_label.Name = "TestBlock_label";
            this.TestBlock_label.Size = new System.Drawing.Size(64, 24);
            this.TestBlock_label.TabIndex = 4;
            this.TestBlock_label.Text = "块大小";
            this.TestBlock_label.Visible = false;
            // 
            // TestPercent_label
            // 
            this.TestPercent_label.AutoSize = true;
            this.TestPercent_label.Location = new System.Drawing.Point(280, 107);
            this.TestPercent_label.Name = "TestPercent_label";
            this.TestPercent_label.Size = new System.Drawing.Size(82, 24);
            this.TestPercent_label.TabIndex = 3;
            this.TestPercent_label.Text = "测试容量";
            this.TestPercent_label.Visible = false;
            // 
            // Test_Mode_label
            // 
            this.Test_Mode_label.AutoSize = true;
            this.Test_Mode_label.Location = new System.Drawing.Point(11, 58);
            this.Test_Mode_label.Name = "Test_Mode_label";
            this.Test_Mode_label.Size = new System.Drawing.Size(82, 24);
            this.Test_Mode_label.TabIndex = 2;
            this.Test_Mode_label.Text = "测试模式";
            this.Test_Mode_label.Visible = false;
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
            // TestTime_S_label
            // 
            this.TestTime_S_label.AutoSize = true;
            this.TestTime_S_label.Location = new System.Drawing.Point(217, 200);
            this.TestTime_S_label.Name = "TestTime_S_label";
            this.TestTime_S_label.Size = new System.Drawing.Size(18, 24);
            this.TestTime_S_label.TabIndex = 30;
            this.TestTime_S_label.Text = "s";
            this.TestTime_S_label.Visible = false;
            this.TestTime_S_label.Click += new System.EventHandler(this.label4_Click);
            // 
            // TestTime_M_label
            // 
            this.TestTime_M_label.AutoSize = true;
            this.TestTime_M_label.Location = new System.Drawing.Point(146, 200);
            this.TestTime_M_label.Name = "TestTime_M_label";
            this.TestTime_M_label.Size = new System.Drawing.Size(27, 24);
            this.TestTime_M_label.TabIndex = 32;
            this.TestTime_M_label.Text = "m";
            this.TestTime_M_label.Visible = false;
            // 
            // TestTime_m
            // 
            this.TestTime_m.Location = new System.Drawing.Point(108, 197);
            this.TestTime_m.Name = "TestTime_m";
            this.TestTime_m.Size = new System.Drawing.Size(32, 31);
            this.TestTime_m.TabIndex = 31;
            this.TestTime_m.Text = "0";
            this.TestTime_m.Visible = false;
            // 
            // TestPercent
            // 
            this.TestPercent.Location = new System.Drawing.Point(365, 105);
            this.TestPercent.Name = "TestPercent";
            this.TestPercent.Size = new System.Drawing.Size(120, 31);
            this.TestPercent.TabIndex = 33;
            this.TestPercent.Tag = "0";
            this.TestPercent.ThousandsSeparator = true;
            this.TestPercent.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.TestPercent.Visible = false;
            // 
            // ThreadNumble
            // 
            this.ThreadNumble.Location = new System.Drawing.Point(365, 248);
            this.ThreadNumble.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.ThreadNumble.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ThreadNumble.Name = "ThreadNumble";
            this.ThreadNumble.Size = new System.Drawing.Size(120, 31);
            this.ThreadNumble.TabIndex = 34;
            this.ThreadNumble.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ThreadNumble.Visible = false;
            // 
            // CircleNumble
            // 
            this.CircleNumble.Location = new System.Drawing.Point(365, 197);
            this.CircleNumble.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.CircleNumble.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.CircleNumble.Name = "CircleNumble";
            this.CircleNumble.Size = new System.Drawing.Size(120, 31);
            this.CircleNumble.TabIndex = 35;
            this.CircleNumble.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.CircleNumble.Visible = false;
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
            ((System.ComponentModel.ISupportInitialize)(this.TestPercent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ThreadNumble)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CircleNumble)).EndInit();
            this.ResumeLayout(false);

        }



        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.ComboBox TestDataMode;
        public System.Windows.Forms.Label TestDataMode_label;
        private System.Windows.Forms.Label CircleNum_label;
        private System.Windows.Forms.ComboBox TestMode;
        private System.Windows.Forms.TextBox TestTime_s;
        private System.Windows.Forms.TextBox TestNum;
        private System.Windows.Forms.ComboBox BlockSize;
        private System.Windows.Forms.Label TestTime_label;
        private System.Windows.Forms.Label TestNum_label;
        private System.Windows.Forms.Label TestBlock_label;
        private System.Windows.Forms.Label TestPercent_label;
        private System.Windows.Forms.Label Test_Mode_label;
        private System.Windows.Forms.CheckBox TestOrNot;
        private System.Windows.Forms.Button confirm;
        private System.Windows.Forms.Label KB_label;
        private System.Windows.Forms.CheckBox Repeat_Test_Checkbox;
        private System.Windows.Forms.CheckBox Record_CheckBox;
        private System.Windows.Forms.Label ThreadNum_label;
        private System.Windows.Forms.RadioButton Choose_TestNumradioButton;
        private System.Windows.Forms.RadioButton Choose_TestTimeradioButton;
        private System.Windows.Forms.Label TestTime_S_label;
        private System.Windows.Forms.Label TestTime_M_label;
        private System.Windows.Forms.TextBox TestTime_m;
        private System.Windows.Forms.NumericUpDown TestPercent;
        private System.Windows.Forms.NumericUpDown ThreadNumble;
        private System.Windows.Forms.NumericUpDown CircleNumble;
    }
}