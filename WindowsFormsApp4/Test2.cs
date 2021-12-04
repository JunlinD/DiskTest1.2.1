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
    /// <summary>
    /// 传递点击按钮状态的委托
    /// </summary>
    /// <param name="status"></param>
    public delegate void StopTestEventHandler(bool status);
    public partial class Test2 : Sunny.UI.UIPage
    {
        private bool Stop_Button_Status;
        //public PercentHandler GetPercent;
        private delegate void ReceiveEventHandler(int now, double speed, double written_MB,string now_time);
        private delegate void GetStartTimeEventHandler(string s);
        public StopTestEventHandler StopTestEvent;
        public Test2()
        {
            Stop_Button_Status = false;
            InitializeComponent();

        }
        public void addStopTestOberver(StopTestEventHandler stopTestEvent)
        {
            StopTestEvent += stopTestEvent;
        }
        public void remoteStopTestOberver(StopTestEventHandler stopTestEvent)
        {
            StopTestEvent -= stopTestEvent;
        }
        /// <summary>
        /// 传递点击按钮的状态
        /// </summary>
        /// <param name="status">点击按钮的状态</param>
        public void PublishStopTest(bool status)
        {
            if(StopTestEvent!=null)
            {
                StopTestEvent(status);
            }
        }
        public void ReceiveEvent(int now, double speed, double written_MB,string now_time)
        {
            if(this.Now_Speed_Show_Label.InvokeRequired||this.Written_MB_Show_Label.InvokeRequired)
            {
                ReceiveEventHandler re = new ReceiveEventHandler(ReceiveEvent);
                this.Invoke(re, new object[] { now, speed, written_MB,now_time });
            }
            else
            {
                Receive(now, speed, written_MB,now_time);
            }
        }
        public void GetStartTimeEvent(string s)
        {
            if(this.Start_Time_Show.InvokeRequired)
            {
                GetStartTimeEventHandler st = new GetStartTimeEventHandler(GetStartTimeEvent);
                this.Invoke(st, new object[] { s });
            }
            else
            {
                GetStartTime(s);
            }
        }
        public void GetStartTime(string s)
        {
            this.Start_Time_Show.Text = s;
        }
        public void Receive(int now,double speed,double written_MB,string now_time)
        {
            Console.WriteLine("数据是：{0}", now);
            DateTime start_t = Convert.ToDateTime(this.Start_Time_Show.Text);
            DateTime now_t = Convert.ToDateTime(now_time);
            TimeSpan ts = now_t - start_t;

            this.uiProcessBar1.Value = now;
            this.uiProcessBar1.Maximum = 100;
            this.Duration_Time_Show.Text = ts.Minutes.ToString() + "m " + ts.Seconds.ToString() + "s";
            this.Now_Speed_Show_Label.Text= String.Format("{0:F}", speed);
            this.Written_MB_Show_Label.Text= String.Format("{0:F}", written_MB);
        }
        private void uiLabel17_Click(object sender, EventArgs e)
        {

        }

        private void uiGroupBox1_Click(object sender, EventArgs e)
        {

        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if(Stop_Button_Status==false)//假如这时候还没有被点击
            {
                PublishStopTest(Stop_Button_Status);//广播状态，暂停测试线程的执行,将false传给DiskSetting
                Stop_Button_Status = true;
                this.StopButton.Text = "Start";
                
            }
            else
            {
                PublishStopTest(Stop_Button_Status);
                Stop_Button_Status = false;
                this.StopButton.Text = "Stop";
            }
        }
    }
}
