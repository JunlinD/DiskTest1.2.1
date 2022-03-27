using DiskTestLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiskTest11
{
    public delegate void TransfDelegate(ArrayList value);
    public delegate void TransfChooseINFDelegate(ChooseInformation choose);
    public delegate void TransfRepeatTestDelegate(bool status);
    public partial class Disk : Sunny.UI.UIPage
    {
        public event TransfChooseINFDelegate TransfChooseINF;
        public event TransfRepeatTestDelegate TransfRepeatTest;
        public Disk()
        {
            InitializeComponent();
            this.TestOrNot.Checked = false;
            VisibleAll(false);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

        }      
        public bool returnTestOrNot()
        {
            return this.TestOrNot.Checked;
        }
        public int returnTestMode()
        {
            if (this.TestMode.SelectedItem == null)
                return -1;
            return this.TestMode.SelectedIndex;
        }
        public int returnTestDataMode()
        {
            if (this.TestDataMode.SelectedItem == null)
                return -1;
            return this.TestDataMode.SelectedIndex;
        }
        public int returnTestPercent()
        {
            int percent;
            bool isallnum = int.TryParse(TestPercent.Text, out percent);
            if (CircleNumble.Text == null || isallnum == false)
            {
                return 0;
            }
            return percent;
        }
        public int returnBlockSize()
        {
            if (this.BlockSize.SelectedItem == null)
                return 0;
            return Convert.ToInt32(this.BlockSize.SelectedItem.ToString());
        }
        public long returnTestTime()
        {
            long testtime_s;
            long testtime_m;
            bool isallnum = long.TryParse(TestTime_s.Text,out testtime_s);
            bool isallnum1 = long.TryParse(TestTime_m.Text, out testtime_m);
            if (TestTime_s.Text == null || !isallnum||!isallnum1)
                return 0;
            return testtime_s*1000+testtime_m*60*1000;
        }
        public int returnTestCircle()
        {
            int circle;
            bool isallnum = int.TryParse(CircleNumble.Text, out circle);
            if(CircleNumble.Text==null||isallnum==false)
            {
                return 0;
            }
            return circle;
        }
        public long returnTestNum()
        {
            long testnum;
            bool isallnum = long.TryParse(TestNum.Text, out testnum);
            if (TestNum.Text == null || !isallnum)
                return 0;
            return testnum;
        }
        public int returnThreadNum()
        {
            int threadnum;
            bool isallnum = int.TryParse(ThreadNumble.Text, out threadnum);
            if (ThreadNumble.Text == null || !isallnum)
                return 0;
            return threadnum;
        }
        public bool returnRecordOrNot()
        {
            return this.Record_CheckBox.Checked;
        }
        public bool returnRepeatTestOrNot()
        {
            return this.Repeat_Test_Checkbox.Checked;
        }
        private void TestNum_TextChanged(object sender, EventArgs e)
        {
        }
        
        private void VisibleAll(bool enable = true)
        {
            TestMode.Visible = enable;
            Test_Mode_label.Visible = enable;
            Record_CheckBox.Visible = enable;
            Repeat_Test_Checkbox.Visible = enable;
            /*TestDataMode.Visible = enable;
            TestPercent.Visible = enable;
            BlockSize.Visible = enable;
            TestNum.Visible = enable;
            TestTime_s.Visible = enable;
            CircleNumble.Visible = enable;
            Repeat_Test_Checkbox.Visible = enable;
            Record_CheckBox.Visible = enable;
            ThreadNumble.Visible = enable;
            TestTime_m.Visible = enable;

            TestDataMode_label.Visible = enable;
            KB_label.Visible = enable;
            BlockSize.Visible = enable;
            Test_Mode_label.Visible = enable;
            ThreadNum_label.Visible = enable;
            CircleNum_label.Visible = enable;
            TestBlock_label.Visible = enable;
            TestPercent_label.Visible = enable;*/
        }
        private void Visible_Repeat(bool enable = true)
        {
            TestMode.Visible = enable;
            Test_Mode_label.Visible = enable;
            Record_CheckBox.Visible = enable;
            /*
            TestMode.Visible = enable;
            TestDataMode.Visible = enable;
            TestPercent.Visible = enable;
            BlockSize.Visible = enable;
            TestNum.Visible = enable;
            TestTime_s.Visible = enable;
            CircleNumble.Visible = enable;
            Record_CheckBox.Visible = enable;
            ThreadNumble.Visible = enable;
            TestTime_m.Visible = enable;

            TestDataMode_label.Visible = enable;
            KB_label.Visible = enable;
            BlockSize.Visible = enable;
            Test_Mode_label.Visible = enable;
            ThreadNum_label.Visible = enable;
            CircleNum_label.Visible = enable;
            TestBlock_label.Visible = enable;
            TestPercent_label.Visible = enable;*/
        }
        private void DisableAll(bool enable=false)
        {
            TestMode.Visible = enable;
            Test_Mode_label.Visible = enable;
            Record_CheckBox.Visible = enable;
            Repeat_Test_Checkbox.Visible = enable;
            
            TestMode.Visible = enable;
            TestDataMode.Visible = enable;
            TestPercent.Visible = enable;
            BlockSize.Visible = enable;
            TestNum.Visible = enable;
            TestTime_s.Visible = enable;
            CircleNumble.Visible = enable;
            Record_CheckBox.Visible = enable;
            ThreadNumble.Visible = enable;
            TestTime_m.Visible = enable;

            TestDataMode_label.Visible = enable;
            KB_label.Visible = enable;
            BlockSize.Visible = enable;
            Test_Mode_label.Visible = enable;
            ThreadNum_label.Visible = enable;
            CircleNum_label.Visible = enable;
            TestBlock_label.Visible = enable;
            TestPercent_label.Visible = enable;
        }
        private void TestOrNot_CheckedChanged(object sender, EventArgs e)
        {
            if (TestOrNot.Checked == false)
            {
                DisableAll(false);
            }
            else
            {
                VisibleAll(true);
            }
        }
        private void Repeat_Test_CheckBoxChanged(object sender, EventArgs e)
        {
            if(Repeat_Test_Checkbox.Checked==true)
            {
                Visible_Repeat(false);
            }
            else if(Repeat_Test_Checkbox.Checked == false)
            {
                Visible_Repeat(true);
            }
        }
        public event TransfDelegate TransfEvent;
        private void confirm_Click(object sender,EventArgs e)
        {
            ChooseInformation choose = new ChooseInformation();
            bool test_or_not=this.returnTestOrNot();
            int testmode=this.returnTestMode();
            int testdatamode=this.returnTestDataMode();
            int testpercent=this.returnTestPercent();
            int blocksize=this.returnBlockSize()*2;
            long testtime=this.returnTestTime();
            int testcircle=this.returnTestCircle();
            long testnum=this.returnTestNum();
            bool record_test = this.returnRecordOrNot();
            bool repeat_test = this.returnRepeatTestOrNot();
            int threadnum = this.returnThreadNum();
            if (testmode==0 || testmode == 1 || testmode == 2)
            {
                choose.SetRandomParameters(test_or_not, testmode, testtime, testnum,blocksize,record_test,threadnum);
            }
            else
            {
                choose.SetOrderParameters(test_or_not, testmode, testdatamode, testpercent, blocksize, testtime, testnum, testcircle,record_test,threadnum);
            }
            //Console.WriteLine("blocksize: " + blocksize);
            TransfChooseINF(choose);
            if(test_or_not)
            {
                TransfRepeatTest(repeat_test);
            }            
            MessageBox.Show("设置完成");
        }
        /// <summary>
        /// 下拉框选中改变选项状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(TestMode.SelectedIndex==0)
            {
                TestDataMode.Visible = false;//测试数据模式
                TestPercent.Visible = false;//测试容量
                BlockSize.Visible = true;//块大小
                //TestNum.Visible = true;
                //TestNum.Text = "1000";
                BlockSize.Visible = true;
                BlockSize.SelectedIndex = 10;
                CircleNumble.Visible = false;
                //TestTime_s.Visible = true;
                Choose_TestTimeradioButton.Visible = true;
                Choose_TestNumradioButton.Visible = true;
                TestBlock_label.Visible = true;

                TestPercent_label.Visible = false;
                CircleNum_label.Visible = false;
                TestDataMode_label.Visible = false;
            }
            else if(TestMode.SelectedIndex==1)//随机只读
            {
                TestDataMode.Visible = false;//测试数据模式
                TestPercent.Visible = false;//测试容量
                BlockSize.Visible = true;//块大小
                //TestNum.Visible = true;
                //TestNum.Text = "1000";
                BlockSize.SelectedIndex = 10;
                CircleNumble.Visible = false;
                //TestTime_s.Visible = true;
                Choose_TestTimeradioButton.Visible = true;
                Choose_TestNumradioButton.Visible = true;
                TestBlock_label.Visible = true;

                TestPercent_label.Visible = false;
                CircleNum_label.Visible = false;
                TestDataMode_label.Visible = false;
            }
            else if(TestMode.SelectedIndex==2)//随机只写
            {
                TestDataMode.Visible = false;//测试数据模式
                TestPercent.Visible = false;//测试容量
                BlockSize.Visible = true;//块大小
                //TestNum.Visible = true;
                TestNum.Text = "1000";
                BlockSize.SelectedIndex = 10;
                CircleNumble.Visible = false;
                //TestTime_s.Visible = true;
                Choose_TestTimeradioButton.Visible = true;
                Choose_TestNumradioButton.Visible = true;
                TestBlock_label.Visible = true;

                TestPercent_label.Visible = false;
                CircleNum_label.Visible = false;
                TestDataMode_label.Visible = false;
            }
            else if (TestMode.SelectedIndex == 3)//顺序读写
            {
                TestDataMode.Visible = true;//测试数据模式
                TestPercent.Visible = true;//测试容量
                TestPercent.Text = "1";
                TestDataMode.SelectedIndex = 1;
                BlockSize.Visible = true;//块大小
                BlockSize.SelectedIndex = 10;
                CircleNumble.Visible = true;//循环次数

                TestDataMode_label.Visible = true;
                CircleNum_label.Visible = true;
                ThreadNum_label.Visible = true;
                TestPercent_label.Visible = true;
                ThreadNumble.Visible = true;
                TestBlock_label.Visible = true;


                TestNum.Visible = false;
                TestNum_label.Visible = false;
                TestTime_s.Visible = false;
                TestTime_label.Visible = false;
                TestTime_m.Visible = false;
                TestTime_M_label.Visible = false;
                TestTime_S_label.Visible = false;
                Choose_TestTimeradioButton.Visible = false;
                Choose_TestNumradioButton.Visible = false;
            }
            else if (TestMode.SelectedIndex == 4)//顺序只读
            {
                TestDataMode.Visible = false;//测试数据模式
                //TestDataMode.SelectedIndex = 0;
                TestPercent.Visible = true;//测试容量
                TestPercent.Text = "1";
                BlockSize.Enabled = true;
                BlockSize.Visible = true;//块大小
                BlockSize.SelectedIndex = 10;
                //TestNum.Visible = false;
                CircleNumble.Visible = true;//循环次数

                TestDataMode_label.Visible = false;
                CircleNum_label.Visible = true;
                ThreadNum_label.Visible = true;
                TestPercent_label.Visible = true;
                ThreadNumble.Visible = true;
                TestBlock_label.Visible = true;

                TestNum.Visible = false;
                TestNum_label.Visible = false;
                TestTime_s.Visible = false;
                TestTime_label.Visible = false;
                TestTime_m.Visible = false;
                TestTime_M_label.Visible = false;
                TestTime_S_label.Visible = false;
                Choose_TestTimeradioButton.Visible = false;
                Choose_TestNumradioButton.Visible = false;
            }
            else if (TestMode.SelectedIndex == 5)//顺序只写
            {
                TestDataMode.Visible = true;//测试数据模式
                TestDataMode.Enabled = true;
                TestDataMode.SelectedIndex = 1;
                TestPercent.Visible = true;//测试容量
                TestPercent.Text = "1";
                BlockSize.Visible = true;//块大小
                //TestNum.Visible = false;
                BlockSize.SelectedIndex = 10;
                CircleNumble.Visible = true;//循环次数

                TestDataMode_label.Visible = true;
                CircleNum_label.Visible = true;
                ThreadNum_label.Visible = true;
                TestPercent_label.Visible = true;
                ThreadNumble.Visible = true;
                TestBlock_label.Visible = true;

                TestNum.Visible = false;
                TestNum_label.Visible = false;
                TestTime_s.Visible = false;
                TestTime_label.Visible = false;
                TestTime_m.Visible = false;
                TestTime_M_label.Visible = false;
                TestTime_S_label.Visible = false;
                Choose_TestTimeradioButton.Visible = false;
                Choose_TestNumradioButton.Visible = false;

                
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void ThreadNumble_TextChanged(object sender, EventArgs e)
        {

        }

        private void Choose_TestTimeradioButton_CheckedChanged(object sender, EventArgs e)
        {
            TestNum.Text = "0";
            TestNum.Visible=false;
            TestNum_label.Visible = false;
            TestTime_s.Visible = true;
            TestTime_label.Visible = true;
            TestTime_m.Visible = true;
            TestTime_M_label.Visible = true;
            TestTime_S_label.Visible = true;
        }

        private void Choose_TestNumradioButton_CheckedChanged(object sender, EventArgs e)
        {
            TestTime_s.Text = "0";
            TestTime_s.Visible = false;
            TestTime_label.Visible = false;
            TestTime_m.Visible = false;
            TestTime_m.Text = "0";
            TestTime_M_label.Visible = false;
            TestNum.Visible = true;
            TestNum_label.Visible = true;
            TestTime_S_label.Visible = false;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void BlockSize_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
