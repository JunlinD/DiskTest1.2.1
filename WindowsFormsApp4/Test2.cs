using System;
using System.Collections;
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
        private delegate void ReceiveProcessAndTimeHandler(int now, string now_time);
        private delegate void ReceiveWrittenAndSpeedHandler(double speed, double written_MB);
        private delegate void ReceiveAVGSpeedHandler(double speed);
        private delegate void ReceiveCircleNumHandler(int circlenum);
        private delegate void GetStartTimeEventHandler(string s);
        private delegate void ReceiveTestTimeCommandEventHandler();
        private DateTime Gap_Start_Time;
        private DateTime Gap_End_Time;
        private TimeSpan Gap_Time;
        private DateTime Start_Time;
        private DateTime End_Time;
        private double Max_Speed;
        private double Min_Speed;
        private bool Max_Set;
        private float[] data;
        private ArrayList floatarray;
        public StopTestEventHandler StopTestEvent;
        public Random random;
        public Test2()
        {
            Stop_Button_Status = false;
            Max_Set = false;
            data = new float[] { };
            Max_Speed = 5;
            Min_Speed = 1;
            floatarray = new ArrayList();
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
        public void ReceiveCircleNumEvent(int circlenum)
        {
            if(this.Circle_Num_Label.InvokeRequired)
            {
                ReceiveCircleNumHandler re = new ReceiveCircleNumHandler(ReceiveCircleNumEvent);
                this.Invoke(re, new object[] { circlenum });
            }
            else
            {
                ReceiveCircleNum(circlenum);
            }
        }
        public void ReceiveCircleNum(int circlenum)
        {
            this.Circle_Num_Label.Text = circlenum.ToString();
        }
        public void ReceiveTestTimeEvent()
        {
            if (this.Duration_Time_Show.InvokeRequired)
            {
                ReceiveProcessAndTimeHandler re = new ReceiveProcessAndTimeHandler(ReceiveProcessAndTimeEvent);
                this.Invoke(re, new object[] {  });
            }
            else
            {
                ReceiveTestTime();
            }
        }
        public void ReceiveTestTime()
        {
            TimeSpan ts = DateTime.Now - Start_Time - Gap_Time;
            Console.WriteLine("DateTime.Now: " + DateTime.Now.ToString());
            this.Duration_Time_Show.Text = ts.Minutes.ToString() + "m " + ts.Seconds.ToString() + "s";
        }
        /// <summary>
        /// 接受进度条和时间的委托事件
        /// </summary>
        /// <param name="now"></param>
        /// <param name="now_time"></param>
        public void ReceiveProcessAndTimeEvent(int now,string now_time)
        {
            if (this.Written_MB_Show_Label.InvokeRequired)
            {
                ReceiveProcessAndTimeHandler re = new ReceiveProcessAndTimeHandler(ReceiveProcessAndTimeEvent);
                this.Invoke(re, new object[] { now,  now_time });
            }
            else
            {
                ReceiveProcessAndTime(now,  now_time);
            }
        }
        /// <summary>
        /// 接受进度条和时间的事件
        /// </summary>
        /// <param name="now"></param>
        /// <param name="now_time"></param>
        private void ReceiveProcessAndTime(int now, string now_time)
        {
            //DateTime start_t = Convert.ToDateTime(this.Start_Time_Show.Text);
            //DateTime now_t = Convert.ToDateTime(now_time);
            //TimeSpan ts = DateTime.Now - start_t - Gap_Time;

            this.uiProcessBar1.Value = now;
            this.uiProcessBar1.Maximum = 100;
            //this.Duration_Time_Show.Text = ts.Minutes.ToString() + "m " + ts.Seconds.ToString() + "s";
        }
        /// <summary>
        /// 接受已写空间和速度的委托
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="written_MB"></param>
        public void ReceiveWrittenAndProcessEvent(double speed, double written_MB)
        {
            if (this.Now_Speed_Show_Label.InvokeRequired || this.Written_MB_Show_Label.InvokeRequired)
            {
                ReceiveWrittenAndSpeedHandler re = new ReceiveWrittenAndSpeedHandler(ReceiveWrittenAndProcessEvent);
                this.Invoke(re, new object[] { speed, written_MB });
            }
            else
            {
                ReceiveWrittenAndProcess(speed, written_MB);
            }
        }
        /// <summary>
        /// 接受已写空间和速度的方法
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="written_MB"></param>
        private void ReceiveWrittenAndProcess(double speed, double written_MB)
        {

            this.Now_Speed_Show_Label.Text = String.Format("{0:F}", speed)+" MB/s";
            this.Written_MB_Show_Label.Text = String.Format("{0:F}", written_MB) + " MB";
            if (speed > Max_Speed)
                Max_Speed = speed;
            else if (speed < Min_Speed)
                Min_Speed = speed;
            //Console.WriteLine("speed: " + speed + "----------------------------------");
            this.userCurve1.AddCurveData("SPEED", (float)speed);           
            userCurve1.ValueMaxLeft = (float)Math.Ceiling(Max_Speed);// 向上取整
            userCurve1.ValueMinLeft = (float)Math.Floor(Min_Speed);// 向下取整
        }
        public void ReceiveAvgSpeedEvent(double speed)
        {
            if(this.AVG_Speed_Show_Label.InvokeRequired)
            {
                ReceiveAVGSpeedHandler re = new ReceiveAVGSpeedHandler(ReceiveAvgSpeedEvent);
                this.Invoke(re, new object[] { speed });
            }
            else
            {
                ReceiveAvgSpeed(speed);
            }
        }
        private void ReceiveAvgSpeed(double speed)
        {
            this.AVG_Speed_Show_Label.Text= String.Format("{0:F}", speed) + " MB/s";
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
            Start_Time= Convert.ToDateTime(s);
            Gap_Time =DateTime.Now-DateTime.Now;
            
            userCurve1.RemoveCurve("SPEED");
            data=new float[] { };
            this.userCurve1.SetLeftCurve("SPEED", data, Color.Tomato);
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
                Gap_Start_Time = DateTime.Now;
            }
            else
            {
                PublishStopTest(Stop_Button_Status);
                Stop_Button_Status = false;
                this.StopButton.Text = "Stop";
                Gap_End_Time = DateTime.Now;
                TimeSpan span = Gap_End_Time - Gap_Start_Time;
                Gap_Time += span;
            }
        }

        private float[] GetRandomValueByCount(int count, float min, float max)
        {
            float[] data = new float[count];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (float)random.NextDouble() * (max - min) + min;
            }
            return data;
        }

        private void uiFlowLayoutPanel1_Click(object sender, EventArgs e)
        {

        }

        private void uiProcessBar1_ValueChanged(object sender, int value)
        {

        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
