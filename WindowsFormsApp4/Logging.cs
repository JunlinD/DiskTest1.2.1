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
        private delegate void SetLoggingEvent(string s);
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
        public void LoggingEvent(string s)
        {
            if (this.file_name2.InvokeRequired)
            {
                SetLoggingEvent le = new SetLoggingEvent(LoggingEvent);
                this.Invoke(le, new object[] { s });

            }
            else
            {
                setLoggingEvent(s);
            }
        }
        public void setLoggingEvent(string s)
        {
            if (file_name.Text != null)
            {
                if (file_name2.Text != null)
                {
                    String path = file_name.Text + "\\" + file_name2.Text + ".txt";
                    if (!System.IO.File.Exists(path))
                    {
                        FileStream fs1 = new FileStream(path, FileMode.Create, FileAccess.Write);//创建写入文件
                        System.IO.File.SetAttributes(path, FileAttributes.Hidden);
                        StreamWriter sw = new StreamWriter(fs1);
                        sw.WriteLine(s);//开始写入值
                        sw.Close();
                        fs1.Close();
                    }

                }
                else
                {
                    String path = file_name.Text + "\\log.txt";
                    if (!System.IO.File.Exists(path))
                    {
                        FileStream fs1 = new FileStream(file_name.Text, FileMode.Create, FileAccess.Write);//创建写入文件
                        System.IO.File.SetAttributes(path, FileAttributes.Hidden);
                        StreamWriter sw = new StreamWriter(fs1);
                        sw.WriteLine(s);//开始写入值
                        sw.Close();
                        fs1.Close();
                    }
                }
            }
            //file_name2.AppendText(s + "\r\n");



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
            //FileStream fs1 = new FileStream("D:\\Test.txt", FileMode.Create, FileAccess.Write);//创建写入文件
            //System.IO.File.SetAttributes(System.Windows.Forms.Application.StartupPath + "\\Test.txt", FileAttributes.Hidden);
            //StreamWriter sw = new StreamWriter(fs1);
            //MessageBox.Show(System.Windows.Forms.Application.StartupPath + "\\Test.txt");
            //sw.WriteLine("你好");//开始写入值
            //sw.Close();
            //fs1.Close();

        }
        private void EnableAll(bool enable = true)
        {
            this.browse.Enabled = enable;
            this.file_name.Enabled = enable;
            this.file_name2.Enabled = enable;
            this.singlefile.Enabled = enable;
            this.stampedfile.Enabled = enable;
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
    }
}
