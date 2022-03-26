using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiskTest11
{
    public partial class Logging : Sunny.UI.UIPage
    {
        private delegate void SetLoggingEvent(int grade,string s);
        public Logging()
        {
            InitializeComponent();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            //设定字体大小为12px      

            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(120)));
            this.Turn_or_not.Checked = true;
        }

        private void Logging_Initialize(object sender, EventArgs e)
        {

        }
        public void LoggingEvent(int grade,string s)
        {
            if (this.file_name2.InvokeRequired)
            {
                SetLoggingEvent le = new SetLoggingEvent(LoggingEvent);
                this.Invoke(le, new object[] { grade,s });

            }
            else
            {
                setLoggingEvent(grade,s);
            }
        }
        public void setLoggingEvent(int grade,string s)
        {
            string grade_s;
            if (grade == 1)
                grade_s = "NORMAL: ";
            else if (grade == 2)
                grade_s = "EXCEPTION: ";
            else if (grade == 3)
                grade_s = "ERROR: ";
            else
                grade_s = "";
                
            if (file_name.Text != "")
            {
                if (file_name2.Text != "")
                {
                    String path = file_name.Text + "\\" + file_name2.Text + ".txt";
                    if (!System.IO.File.Exists(path))
                    {
                        FileStream fs1 = new FileStream(path, FileMode.Create, FileAccess.Write);//创建写入文件
                        System.IO.File.SetAttributes(path, FileAttributes.Normal);
                        fs1.Close();
                        StreamWriter sw = new StreamWriter(path,true, System.Text.Encoding.GetEncoding("GB2312"));
                        sw.WriteLine(grade_s+s);//开始写入值
                        sw.WriteLine("测试时间：" + DateTime.Now.ToString());
                        sw.Close();

                    }
                    else
                    {
                        FileStream fs1 = new FileStream(path, FileMode.Open, FileAccess.Write);//打开写入文件
                        System.IO.File.SetAttributes(path, FileAttributes.Normal);
                        fs1.Close();
                        StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.GetEncoding("GB2312"));
                        sw.WriteLine(grade_s + s);//开始写入值
                        sw.WriteLine("测试时间：" + DateTime.Now.ToString());
                        sw.Close();

                    }

                }
                else
                {
                    String path = file_name.Text + "\\"+"log"+".txt";
                    if (!System.IO.File.Exists(path))
                    {
                        FileStream fs1 = new FileStream(path, FileMode.Create, FileAccess.Write);//创建写入文件
                        System.IO.File.SetAttributes(path, FileAttributes.Normal);
                        fs1.Close();
                        StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.GetEncoding("GB2312"));
                        sw.WriteLine(grade_s + s);//开始写入值
                        sw.Close();

                    }
                    else
                    {
                        FileStream fs1 = new FileStream(path, FileMode.Open, FileAccess.Write);//打开写入文件
                        System.IO.File.SetAttributes(path, FileAttributes.Normal);
                        fs1.Close();
                        StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.GetEncoding("GB2312"));
                        sw.WriteLine(grade_s + s);//开始写入值
                        sw.WriteLine("测试时间："+DateTime.Now.ToString()); 
                        sw.Close();

                    }
                }
            }
          



        }
        private void browse_Click(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    file_name.Text = folderBrowserDialog.SelectedPath;
                }
            }

        }
        private void EnableAll(bool enable = true)
        {
            this.browse.Enabled = enable;
            this.file_name.Enabled = enable;
            this.file_name2.Enabled = enable;
            //this.singlefile.Enabled = enable;
            //this.stampedfile.Enabled = enable;
            this.label1.Enabled = enable;
            this.label2.Enabled = enable;
        }
        private void DisableAll()
        {
            EnableAll(false);
        }
        private void Turn_or_not_CheckedChanged(object sender, EventArgs e)
        {
            if (Turn_or_not.Checked == false)
            {
                DisableAll();
            }
            else
            {
                EnableAll(true);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
