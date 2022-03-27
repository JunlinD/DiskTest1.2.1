using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiskTest11
{
    public partial class Log : Sunny.UI.UIPage
    {
        private delegate void SetLogEvent(int grade,string s);
        public Log()
        {
            InitializeComponent();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            //设定字体大小为12px      

            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));

        }
        public void LogEvent(int grade,string s)
        {
            if(this.ActivityLogTextBox.InvokeRequired)
            {
                SetLogEvent le = new SetLogEvent(LogEvent);
                this.Invoke(le, new object[] { grade,s });
                
            }
            else
            {
                setLogEvent(grade,s);
            }
        }
        public void setLogEvent(int grade,string s)
        {
            if(grade==2)
                ActivityLogTextBox.SelectionColor = Color.Orange;
            else if(grade==3)
                ActivityLogTextBox.SelectionColor = Color.Red;
            else
                ActivityLogTextBox.SelectionColor = Color.Black;
            ActivityLogTextBox.AppendText(s);
        }

        private void ActivityLogTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
