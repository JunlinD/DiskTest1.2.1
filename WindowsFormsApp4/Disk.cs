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
            EnableAll(false);
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
            long testtime;
            bool isallnum = long.TryParse(TestTime.Text,out testtime);
            if (TestTime.Text == null || !isallnum)
                return 0;
            return testtime;
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
        
        private void EnableAll(bool enable = true)
        {
            TestMode.Enabled = enable;
            TestDataMode.Enabled = enable;
            TestPercent.Enabled = enable;
            BlockSize.Enabled = enable;
            TestNum.Enabled = enable;
            TestTime.Enabled = enable;
            CircleNumble.Enabled = enable;
            Repeat_Test_Checkbox.Enabled = enable;
            Record_CheckBox.Enabled = enable;
            ThreadNumble.Enabled = enable;
        }
        private void Enable_Repeat(bool enable = true)
        {
            TestMode.Enabled = enable;
            TestDataMode.Enabled = enable;
            TestPercent.Enabled = enable;
            BlockSize.Enabled = enable;
            TestNum.Enabled = enable;
            TestTime.Enabled = enable;
            CircleNumble.Enabled = enable;
            Record_CheckBox.Enabled = enable;
            ThreadNumble.Enabled = enable;
        }
        private void DisableAll()
        {
            EnableAll(false);
        }
        private void TestOrNot_CheckedChanged(object sender, EventArgs e)
        {
            if (TestOrNot.Checked == false)
            {
                DisableAll();
            }
            else
            {
                EnableAll(true);
            }
        }
        private void Repeat_Test_CheckBoxChanged(object sender, EventArgs e)
        {
            if(Repeat_Test_Checkbox.Checked==true)
            {
                Enable_Repeat(false);
            }
            else if(Repeat_Test_Checkbox.Checked == false)
            {
                Enable_Repeat(true);
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
            if (test_or_not&&!repeat_test)
            {
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
            }
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
                TestDataMode.Enabled = false;//测试数据模式
                TestPercent.Enabled = false;//测试容量
                BlockSize.Enabled = true;//块大小
                TestNum.Enabled = true;
                TestNum.Text = "1000";
                BlockSize.SelectedIndex = 10;
                CircleNumble.Enabled = false;
                TestTime.Enabled = true;
            }
            else if(TestMode.SelectedIndex==1)//随机只读
            {
                TestDataMode.Enabled = false;//测试数据模式
                TestPercent.Enabled = false;//测试容量
                BlockSize.Enabled = true;//块大小
                TestNum.Enabled = true;
                TestNum.Text = "1000";
                BlockSize.SelectedIndex = 10;
                CircleNumble.Enabled = false;
                TestTime.Enabled = true;
            }
            else if(TestMode.SelectedIndex==2)//随机只写
            {
                TestDataMode.Enabled = false;//测试数据模式
                TestPercent.Enabled = false;//测试容量
                BlockSize.Enabled = true;//块大小
                TestNum.Enabled = true;
                TestNum.Text = "1000";
                BlockSize.SelectedIndex = 10;
                CircleNumble.Enabled = false;
                TestTime.Enabled = true;
            }
            else if (TestMode.SelectedIndex == 3)//顺序读写
            {
                TestDataMode.Enabled = true;//测试数据模式
                TestPercent.Enabled = true;//测试容量
                TestPercent.Text = "1";
                TestDataMode.SelectedIndex = 1;
                BlockSize.Enabled = true;//块大小
                TestNum.Enabled = false;
                BlockSize.SelectedIndex = 10;
                CircleNumble.Enabled = true;//循环次数
                TestTime.Enabled = false;
            }
            else if (TestMode.SelectedIndex == 4)//顺序只读
            {
                TestDataMode.Enabled = false;//测试数据模式
                //TestDataMode.SelectedIndex = 0;
                TestPercent.Enabled = true;//测试容量
                TestPercent.Text = "1";
                BlockSize.Enabled = true;//块大小
                BlockSize.SelectedIndex = 10;
                TestNum.Enabled = false;
                CircleNumble.Enabled = true;//循环次数
                TestTime.Enabled = false;
            }
            else if (TestMode.SelectedIndex == 5)//顺序只写
            {
                TestDataMode.Enabled = true;//测试数据模式
                TestDataMode.SelectedIndex = 1;
                TestPercent.Enabled = true;//测试容量
                TestPercent.Text = "1";
                BlockSize.Enabled = true;//块大小
                TestNum.Enabled = false;
                BlockSize.SelectedIndex = 10;
                CircleNumble.Enabled = true;//循环次数
                TestTime.Enabled = false;
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void ThreadNumble_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
