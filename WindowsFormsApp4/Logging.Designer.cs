
namespace DiskTest11
{
    partial class Logging
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.singlefile = new System.Windows.Forms.RadioButton();
            this.file_name2 = new System.Windows.Forms.TextBox();
            this.stampedfile = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.browse = new System.Windows.Forms.Button();
            this.file_name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Turn_or_not = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.Turn_or_not);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1034, 646);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Automatic Test";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.comboBox2);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.comboBox1);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(40, 301);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(957, 268);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Report Detail";
            // 
            // comboBox2
            // 
            this.comboBox2.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.comboBox2.Location = new System.Drawing.Point(336, 95);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(358, 39);
            this.comboBox2.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(247, 31);
            this.label4.TabIndex = 19;
            this.label4.Text = "Trace file detail level";
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.comboBox1.Location = new System.Drawing.Point(336, 44);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(358, 39);
            this.comboBox1.TabIndex = 18;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(227, 31);
            this.label3.TabIndex = 0;
            this.label3.Text = "Log file detail level";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.singlefile);
            this.groupBox2.Controls.Add(this.file_name2);
            this.groupBox2.Controls.Add(this.stampedfile);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.browse);
            this.groupBox2.Controls.Add(this.file_name);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(40, 68);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(0);
            this.groupBox2.Size = new System.Drawing.Size(957, 197);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Log file name";
            // 
            // singlefile
            // 
            this.singlefile.AutoSize = true;
            this.singlefile.Location = new System.Drawing.Point(393, 147);
            this.singlefile.Name = "singlefile";
            this.singlefile.Size = new System.Drawing.Size(150, 35);
            this.singlefile.TabIndex = 22;
            this.singlefile.TabStop = true;
            this.singlefile.Text = "Single file";
            this.singlefile.UseVisualStyleBackColor = true;
            // 
            // file_name2
            // 
            this.file_name2.Location = new System.Drawing.Point(210, 105);
            this.file_name2.Name = "file_name2";
            this.file_name2.Size = new System.Drawing.Size(392, 39);
            this.file_name2.TabIndex = 4;
            // 
            // stampedfile
            // 
            this.stampedfile.AutoSize = true;
            this.stampedfile.Location = new System.Drawing.Point(25, 147);
            this.stampedfile.Name = "stampedfile";
            this.stampedfile.Size = new System.Drawing.Size(253, 35);
            this.stampedfile.TabIndex = 21;
            this.stampedfile.TabStop = true;
            this.stampedfile.Text = "Time stamped files";
            this.stampedfile.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(198, 31);
            this.label2.TabIndex = 3;
            this.label2.Text = "Log name prefix";
            // 
            // browse
            // 
            this.browse.Location = new System.Drawing.Point(656, 51);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(111, 41);
            this.browse.TabIndex = 2;
            this.browse.Text = "Browse";
            this.browse.UseVisualStyleBackColor = true;
            this.browse.Click += new System.EventHandler(this.browse_Click);
            // 
            // file_name
            // 
            this.file_name.Location = new System.Drawing.Point(210, 53);
            this.file_name.Name = "file_name";
            this.file_name.Size = new System.Drawing.Size(392, 39);
            this.file_name.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "Log file name";
            // 
            // Turn_or_not
            // 
            this.Turn_or_not.AutoSize = true;
            this.Turn_or_not.Location = new System.Drawing.Point(40, 27);
            this.Turn_or_not.Name = "Turn_or_not";
            this.Turn_or_not.Size = new System.Drawing.Size(345, 35);
            this.Turn_or_not.TabIndex = 0;
            this.Turn_or_not.Text = "Turn automatic logging on";
            this.Turn_or_not.UseVisualStyleBackColor = true;
            this.Turn_or_not.CheckedChanged += new System.EventHandler(this.Turn_or_not_CheckedChanged);
            // 
            // Logging
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1047, 662);
            this.Controls.Add(this.groupBox1);
            this.Name = "Logging";
            this.Text = "Logging";
            this.Initialize += new System.EventHandler(this.Logging_Initialize);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox file_name2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button browse;
        private System.Windows.Forms.TextBox file_name;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox Turn_or_not;
        private System.Windows.Forms.RadioButton singlefile;
        private System.Windows.Forms.RadioButton stampedfile;
    }
}