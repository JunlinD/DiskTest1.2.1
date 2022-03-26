
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.file_name2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.browse = new System.Windows.Forms.Button();
            this.file_name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Turn_or_not = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.Turn_or_not);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1034, 646);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Test Log";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.file_name2);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.browse);
            this.groupBox2.Controls.Add(this.file_name);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(52, 91);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(0);
            this.groupBox2.Size = new System.Drawing.Size(802, 305);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // file_name2
            // 
            this.file_name2.Location = new System.Drawing.Point(210, 148);
            this.file_name2.Name = "file_name2";
            this.file_name2.Size = new System.Drawing.Size(392, 35);
            this.file_name2.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(19, 148);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 31);
            this.label2.TabIndex = 3;
            this.label2.Text = "File Name";
            // 
            // browse
            // 
            this.browse.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.browse.Location = new System.Drawing.Point(642, 99);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(111, 41);
            this.browse.TabIndex = 2;
            this.browse.Text = "Browse";
            this.browse.UseVisualStyleBackColor = true;
            this.browse.Click += new System.EventHandler(this.browse_Click);
            // 
            // file_name
            // 
            this.file_name.Location = new System.Drawing.Point(210, 99);
            this.file_name.Name = "file_name";
            this.file_name.Size = new System.Drawing.Size(392, 35);
            this.file_name.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(26, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "File location";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // Turn_or_not
            // 
            this.Turn_or_not.AutoSize = true;
            this.Turn_or_not.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Turn_or_not.Location = new System.Drawing.Point(52, 52);
            this.Turn_or_not.Margin = new System.Windows.Forms.Padding(0);
            this.Turn_or_not.Name = "Turn_or_not";
            this.Turn_or_not.Size = new System.Drawing.Size(22, 21);
            this.Turn_or_not.TabIndex = 0;
            this.Turn_or_not.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Turn_or_not.UseVisualStyleBackColor = true;
            this.Turn_or_not.CheckedChanged += new System.EventHandler(this.Turn_or_not_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(88, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(146, 28);
            this.label3.TabIndex = 1;
            this.label3.Text = "RecordOrNot";
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
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox file_name2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button browse;
        private System.Windows.Forms.TextBox file_name;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox Turn_or_not;
        private System.Windows.Forms.Label label3;
    }
}