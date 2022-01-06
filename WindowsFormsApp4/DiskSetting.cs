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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiskTest11
{
    public delegate void WriteBlockCompleteHandler(double speed);
    public delegate void ReadBlockComleteHandler(double speed);
    public delegate void NotifyEventHandler(int i,double speed,double written_MB,string now_time);//进度条
    /// <summary>
    /// 进度条百分比和当前时间的委托
    /// </summary>
    /// <param name="i"></param>
    /// <param name="written_MB"></param>
    public delegate void NotifyProcessAndTimeHandler(int i, string now_time);
    /// <summary>
    /// 速度和已写MB的委托
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="written_MB"></param>
    public delegate void NotifyWrittenAndSpeedHandler(double speed, double written_MB);
    public delegate void SwitchEventHandler(int i);//切换页面
    public delegate void LogEventHandler(string s);//日志
    public delegate void StartTimeEventHandler(string s);//日志
    public partial class DiskSetting : Sunny.UI.UIPage
    {
        private const int DEAFAUT_BLOCKSIZE = 512;
        private const int MB=1048576;
        /// <summary>
        /// 声明委托对象
        /// </summary>
        public NotifyEventHandler NotifyEvent;//进度条,速度等信息
        /// <summary>
        /// 进度条和时间的事件
        /// </summary>
        public NotifyProcessAndTimeHandler ProcessAndTimeEvent;
        /// <summary>
        /// 已写MB和速度的事件
        /// </summary>
        public NotifyWrittenAndSpeedHandler WrittenAndSpeedEvent;
        public SwitchEventHandler SwitchEvent;//切换界面
        public LogEventHandler LogEvent;//日志打印
        public StartTimeEventHandler StartTimeEvent;//传递开始时间

        public int now_index_framework=0;//当前点击的硬盘项

        public bool Test_Status;//当前测试状态，是停止，还是继续；
        private static AutoResetEvent resetEvent = new AutoResetEvent(true);
        /// <summary>
        /// 最后一个块的大小；
        /// </summary>
        private int Last_Block_Size;

        public Disk[] Ed; //创建用户控件，显示硬盘的控件

        public ArrayList Disk_Driver_List = new ArrayList();
        public ArrayList Disk_Choose_Information = new ArrayList();
        public ArrayList Disk_Choose_Information_List = new ArrayList();//新的choose数组
        public ArrayList Disk_Information_List = new ArrayList();
        public ChooseInformation Temp_Choose=new ChooseInformation();//临时choose变量
        public WriteBlockCompleteHandler GetWriteSpeed;
        public ReadBlockComleteHandler GetReadSpeed;
        /// <summary>
        /// 测试数组
        /// </summary>
        private byte[] TestArray;
        private byte[] CompareArray;              
        public DiskSetting()
        {           
            InitializeComponent();
            Init_Disk_Information();
            Init_Disk_Framework();
            Init_Choose_ArrayList();
            Test_Status = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;            
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            Show_Disk();
            for(int i=0;i<Disk_Information_List.Count;i++)
            {
                Ed[i].TransfChooseINF += Get_Transf_Choose_INF_Event;
            }
            if (Disk_Choose_Information_List.Count > 0 || Temp_Choose != null)
            {
                Disk_Choose_Information_List[0] = Temp_Choose;
            }
            else
            {
                MessageBox.Show("测试项为空--Temp_Choose==null");
            }
        }
        public void Init_Disk_Framework()
        {
            if(Disk_Information_List.Count>0)
            {
                Ed = new Disk[Disk_Information_List.Count];
                for(int i=0;i<Disk_Information_List.Count;i++)
                {
                    Ed[i] = new Disk();
                }
            }
        }
        public void  Init_Choose_ArrayList()
        {
            if(Disk_Information_List.Count>0)
            {
                for(int i=0;i<Disk_Information_List.Count;i++)
                {
                    ChooseInformation choose = new ChooseInformation();
                    Disk_Choose_Information_List.Add(choose);
                }
            }
        }
        
        public void Show_Disk()
        {
            if(Ed!=null)
            {
                Ed[0].Show();
                panel1.Controls.Add(Ed[0]);
            }
        }
        //增添Notify观察者对象
        public void AddNotifyObserver(NotifyEventHandler observer)
        {
            NotifyEvent += observer;
        }
        public void RemoteNotifyObserver(NotifyEventHandler observer)
        {
            NotifyEvent -= observer;
        }
        //增添Switch观察者对象
        public void AddSwitchObserver(SwitchEventHandler observer)
        {
            SwitchEvent += observer;
        }
        public void RemoteSwitchObserver(SwitchEventHandler observer)
        {
            SwitchEvent -= observer;
        }

        public void AddLogObserver(LogEventHandler observer)
        {
            LogEvent += observer;
        }
        public void RemoteLogObserver(LogEventHandler observer)
        {
            LogEvent -= observer;
        }

        public void AddStartTimeObserver(StartTimeEventHandler observer)
        {
            StartTimeEvent += observer;
        }
        public void RemoteStartTimeObserver(StartTimeEventHandler observer)
        {
            StartTimeEvent -= observer;
        }
        public void AddProcessAndTimeObserver(NotifyProcessAndTimeHandler observer)
        {
            ProcessAndTimeEvent += observer;
        }
        public void RemoteProcessAndTimeObserver(NotifyProcessAndTimeHandler observer)
        {
            ProcessAndTimeEvent += observer;
        }
        /// <summary>
        /// 添加观察者
        /// </summary>
        /// <param name="observer"></param>
        public void AddWrittenAndSpeedObserver(NotifyWrittenAndSpeedHandler observer)
        {
            WrittenAndSpeedEvent += observer;
        }
        public void RemoteProcessAndTimeObserver(NotifyWrittenAndSpeedHandler observer)
        {
            WrittenAndSpeedEvent += observer;
        }

        /// <summary>
        /// 广播速度，已读写量等信息，事件的具体实现，将这个组件的信息传给所有的观察者，让观察者执行相应的函数
        /// </summary>
        /// <param name="i"></param>
        /// <param name="speed"></param>
        /// <param name="wirtten_MB"></param>
        /// <param name="now_time"></param>
        public void PublishNotify(int i,double speed,double wirtten_MB,string now_time)
        {
            if (NotifyEvent != null)
            {
                NotifyEvent(i,speed,wirtten_MB,now_time);
            }
        }
        public void SwitchPage(int i)
        {
            if (SwitchEvent != null)
            {
                SwitchEvent(i);
            }
        }
        public void PrintLog(string s)
        {
            if(LogEvent!=null)
            {
                LogEvent(s);
            }
        }
        public void PublishStartTime(string s)
        {
            if(StartTimeEvent!=null)
            {
                StartTimeEvent(s);
            }
        }
        public void PublishProcessAndTime(int i,string now_time)
        {
            if(ProcessAndTimeEvent!=null)
            {
                ProcessAndTimeEvent(i, now_time);
            }
        }
        public void PublishWrittenAndSpeed(double speed,double written_MB)
        {
            if(WrittenAndSpeedEvent!=null)
            {
                WrittenAndSpeedEvent(speed, written_MB);
            }
        }
        /// <summary>
        /// 从Disk.cs中获取测试的选项信息
        /// </summary>
        /// <param name="value"></param>
        public void Get_Disk_Information_Event(ArrayList value)
        {
            Disk_Information_List = value;
        }
        public void Get_Transf_Choose_INF_Event(ChooseInformation choose)
        {
            Temp_Choose = choose;
            Disk_Choose_Information_List[now_index_framework] = choose;
        }
        /// <summary>
        /// 获取Stop按钮的信息，并改变当前的Test_Status，判断是暂停还是恢复
        /// </summary>
        /// <param name="status">Stop按钮传过来的状态参数</param>
        public void Get_Stop_Button_Status_Event(bool status)
        {
           if(Test_Status==false&&status==true)//当前是暂停，要恢复
           {
                Test_Status = status;
                resetEvent.Set();
           }
            else
            {
                Test_Status = status;
            }
        }
        private void Start_Test(Object obj)
        {
            if (Disk_Choose_Information_List.Count <= 0)
            {
                MessageBox.Show("测试信息数组为空");
            }
            for (int i = 0; i < Disk_Choose_Information_List.Count; i++)
            {
                ChooseInformation chooseInformation = (ChooseInformation)Disk_Choose_Information_List[i];
                DriverLoader driverLoader = (DriverLoader)Disk_Driver_List[i];
                if (chooseInformation.TestOrNot == true)
                {
                    //if (chooseInformation.TestMode == 0 && chooseInformation.BlockSize != 8)//随机读写验证
                    //{
                    //    RandomWriteAndVerify(i, chooseInformation.TestNum, chooseInformation.TestTime,2, chooseInformation.BlockSize);
                    //}
                    //else 
                    if(chooseInformation.TestMode==0&&chooseInformation.BlockSize<=256)
                    {
                        RandomWriteAndVerify_4KB(i, chooseInformation.TestNum, chooseInformation.TestTime, 2, chooseInformation.BlockSize);
                    }
                    else if (chooseInformation.TestMode == 0 && chooseInformation.BlockSize > 256)
                    {
                        RandomWriteAndVerify(i, chooseInformation.TestNum, chooseInformation.TestTime, 2, chooseInformation.BlockSize);
                    }
                    else if (chooseInformation.TestMode == 1)//随机只读
                    {
                        RandomOnlyRead(i, chooseInformation.TestNum, chooseInformation.TestTime, chooseInformation.BlockSize);
                    }
                    else if (chooseInformation.TestMode == 2)//随机只写
                    {
                        RandomOnlyWrite(i, chooseInformation.TestNum, chooseInformation.TestTime,2, chooseInformation.BlockSize);
                    }
                    else if (chooseInformation.TestMode == 3)//顺序读写验证
                    {
                        OrderWriteAndVerify(i, chooseInformation.TestPercent, chooseInformation.TestDataMode, chooseInformation.BlockSize, chooseInformation.TestCircle);
                    }
                    else if (chooseInformation.TestMode == 4)//顺序只读
                    {
                        OrderOnlyRead(i, chooseInformation.TestPercent, chooseInformation.TestDataMode, chooseInformation.BlockSize, chooseInformation.TestCircle);
                    }
                    else if (chooseInformation.TestMode == 5)//顺序只写
                    {
                        OrderOnlyWrite(i, chooseInformation.TestPercent, chooseInformation.TestDataMode, chooseInformation.BlockSize, chooseInformation.TestCircle);
                    }
                    else
                    {
                        MessageBox.Show("测试模式错误！");
                    }
                }
                else
                {
                    MessageBox.Show("该磁盘无法进行测试，请检查选项");
                }
            }
        }
        /// <summary>
        /// 开始测试的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartTest_Click(object sender, EventArgs e)
        {

            Init_Disk_Driver();
            GetWriteSpeed = OutWriteSpeed;
            System.Threading.Thread thr = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Start_Test));
            thr.Start();
            this.SwitchPage(201);
            this.PublishStartTime(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
        }
        /// <summary>
        /// 初始化读写测试IO--命名为Disk_Driver
        /// </summary>
        public void Init_Disk_Driver()
        {
            if (Disk_Information_List.Count <= 0)
            {
                MessageBox.Show("未检测到设备！");
                return;
            }
            for (int i = 0; i < Disk_Information_List.Count; i++)
            {
                DiskInformation information = (DiskInformation)Disk_Information_List[i];
                DriverLoader driver = new DriverLoader(information);
                Disk_Driver_List.Add(driver);
            }
        }
        /// <summary>
        /// 初始化测试参数
        /// </summary>
       
        /// <summary>
        /// 获取硬盘信息的函数
        /// </summary>
        public void Init_Disk_Information()
        {
            ManagementClass Diskobject = new ManagementClass("Win32_DiskDrive");//获取一个磁盘实例对象
            ManagementObjectCollection moc = Diskobject.GetInstances();//获取对象信息的集合            
            int id = 0;
            int i = 1;
            foreach (ManagementObject mo in moc)
            {
                if (mo.Properties["InterfaceType"].Value.ToString() == "USB")
                {
                    try
                    {
                        //产品名称
                        string name = mo.Properties["Name"].Value.ToString();
                        string sector_size_s = mo.Properties["TotalSectors"].Value.ToString();
                        long sector_size = Convert.ToInt64(sector_size_s);

                        string size_s = mo.Properties["Size"].Value.ToString();
                        double size_d = Convert.ToDouble(size_s) / (1024 * 1024 * 1024);
                        decimal size = decimal.Round(decimal.Parse("" + size_d), 2);
                        //long size = Convert.ToInt64(size_s);
                        DiskInformation d = new DiskInformation(name, sector_size, size, id++);
                        this.addColumn(name, size, sector_size);
                        Disk_Information_List.Add(d);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            if (id == 0)
            {
                MessageBox.Show("未检测到设备！");
            }
        }
        /// <summary>
        /// 输出当前的测试速度
        /// </summary>
        /// <param name="speed"></param>
        public void OutWriteSpeed(double speed)
        {
            Console.WriteLine(speed);
        }
        /// <summary>
        /// 在展示表格中添加一行信息
        /// </summary>
        /// <param name="name">硬盘的名字</param>
        /// <param name="size">硬盘的大小</param>
        /// <param name="sectorsize">扇区数</param>
        public void addColumn(string name, decimal size, long sectorsize)
        {
            int index = this.Disk_Information_Framework.Rows.Add();
            Disk_Information_Framework.Rows[index].Cells[0].Value = name;
            Disk_Information_Framework.Rows[index].Cells[1].Value = size;
            Disk_Information_Framework.Rows[index].Cells[2].Value = "";
            Disk_Information_Framework.Rows[index].Cells[3].Value = sectorsize;
            Disk_Information_Framework.Rows[index].Cells[4].Value = "512B";

        }
        /// <summary>
        /// 顺序读写验证,已修改测速模式，已验证速度无误
        /// </summary>
        /// <param name="driver_index">读写流的编号</param>
        /// <param name="percent">测试所占容量的百分比</param>
        /// <param name="test_data_mode">测试数据模式</param>
        /// <param name="block_size">块大小</param>
        /// <param name="circle">测试循环</param>
        public void OrderWriteAndVerify(int driver_index, int percent = 100, int test_data_mode = 0, int block_size = 1, int circle = 1)
        {
            if (Disk_Driver_List.Count <= 0)
            {
                MessageBox.Show("未检测到设备！");
                return;
            }
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            TestArray = new byte[DEAFAUT_BLOCKSIZE * block_size];
            CompareArray = new byte[DEAFAUT_BLOCKSIZE * block_size];
            //long actual_size = ((driver.DiskInformation.DiskSectorSize / block_size)*percent)/100;
            long actual_size = driver.DiskInformation.DiskSectorSize * percent / 100;
            long speed_start;//测试读写速度
            long speed_end;
            long speed_time;
            long total_speedtime=0;
            long _MB_num = 0;
            this.PublishNotify(0, 0, 0, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            if (test_data_mode == 0 || test_data_mode == 1)
            {
                int error_num = 0;
                Init_TestArray(block_size, test_data_mode);
                for (long i = 0; i < actual_size;)
                {
                    //添加状态判断语句
                     if (i + block_size > actual_size)//
                    {
                        //这时候i已经大于actual_size了，应该减去blocksize;                       
                        Last_Block_Size = Convert.ToInt32(actual_size - i);
                        int temp = DEAFAUT_BLOCKSIZE * Last_Block_Size;
                        TestArray = new byte[temp];
                        CompareArray = new byte[temp];
                        Init_TestArray(Last_Block_Size, test_data_mode);
                        speed_start = Environment.TickCount;
                        driver.WritSector(TestArray, i, Last_Block_Size);//记得将块的大小传进去
                        CompareArray = driver.ReadSector(i, Last_Block_Size);//传的是块大小
                        speed_end = Environment.TickCount;
                        speed_time = speed_end - speed_start;
                        error_num += VerifyArray(TestArray, CompareArray);
                        double now_MB_Last = (double)Last_Block_Size * 512 / (double)1048576;//最后一个块的MB数
                        double now_speed_Last = (double)(1000 * now_MB_Last);
                        now_speed_Last=now_speed_Last/(speed_time);//写速度的计算
                        now_MB_Last += (_MB_num / 1048576);
                        
                        string now_time_Last = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                        this.PublishNotify(100, now_speed_Last, now_MB_Last, now_time_Last);
                        Console.WriteLine("顺序读写测试完成，测试了" + actual_size + "个块未发生错误！");
                        this.PrintLog("顺序读写测试完成，测试了" + actual_size + "个块未发生错误！");
                        if (error_num == 0)
                        {
                            Console.WriteLine("顺序读写测试完成，测试了" + actual_size + "个扇区未发生错误！");
                            this.PrintLog("顺序读写测试完成，测试了" + actual_size + "个扇区未发生错误！");
                        }
                        break;
                    }
                    if (Test_Status == false)
                    {
                        resetEvent.WaitOne();
                    }
                    speed_start = Environment.TickCount;
                    driver.WritSector(TestArray, i, block_size);                    
                    CompareArray = driver.ReadSector(i, block_size);
                    speed_end = Environment.TickCount;
                    speed_time = speed_end - speed_start;
                    total_speedtime += speed_time;
                    _MB_num += DEAFAUT_BLOCKSIZE * block_size;
                    error_num += VerifyArray(TestArray, CompareArray);
                    i += block_size;
                    Console.WriteLine("向" + i + "扇区写入了数据");
                    if (_MB_num % (1048576 * 50) == 0)
                    {
                        double now_speed = ((double)(1000 * 50) / (double)(total_speedtime));
                        double now_MB = _MB_num / 1048576;
                        string now_time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                        total_speedtime = 0;
                        this.PublishNotify((int)(i * 100 / actual_size), now_speed, now_MB, now_time);
                    }
       
                }                
            }
            else if (test_data_mode == 2)
            {
                int error_num = 0;
                for (long i = 0; i < actual_size; )
                {
                    //添加状态判断语句
                    if (i + block_size > actual_size)//
                    {
                        //这时候i已经大于actual_size了，应该减去blocksize;                       
                        Last_Block_Size = Convert.ToInt32(actual_size - i);
                        int temp = DEAFAUT_BLOCKSIZE * Last_Block_Size;
                        TestArray = new byte[temp];
                        CompareArray = new byte[temp];
                        Init_TestArray(Last_Block_Size, test_data_mode);
                        speed_start = Environment.TickCount;
                        driver.WritSector(TestArray, i, Last_Block_Size);//记得将块的大小传进去
                        CompareArray = driver.ReadSector(i, Last_Block_Size);//传的是块大小
                        speed_end = Environment.TickCount;
                        speed_time = speed_end - speed_start;
                        error_num += VerifyArray(TestArray, CompareArray);
                        double now_MB_Last = (double)Last_Block_Size * 512 / (double)1048576;//最后一个块的MB数
                        
                        double now_speed_Last = (double)(1000 * now_MB_Last);
                        now_speed_Last = now_speed_Last / (speed_time);//写速度的计算
                        now_MB_Last += (_MB_num / 1048576);

                        string now_time_Last = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                        this.PublishNotify(100, now_speed_Last, now_MB_Last, now_time_Last);
                        Console.WriteLine("顺序读写测试完成，测试了" + actual_size + "个块未发生错误！");
                        this.PrintLog("顺序读写测试完成，测试了" + actual_size + "个块未发生错误！");
                        if (error_num == 0)
                        {
                            Console.WriteLine("顺序读写测试完成，测试了" + actual_size + "个扇区未发生错误！");
                            this.PrintLog("顺序读写测试完成，测试了" + actual_size + "个扇区未发生错误！");
                        }
                        break;
                    }
                        if (Test_Status == false)
                    {
                        resetEvent.WaitOne();
                    }
                    Init_TestArray(block_size, test_data_mode);
                    speed_start = Environment.TickCount;
                    driver.WritSector(TestArray, i, block_size);
                    CompareArray = driver.ReadSector(i, block_size);
                    error_num += VerifyArray(TestArray, CompareArray);
                    speed_end = Environment.TickCount;
                    speed_time = speed_end - speed_start;
                    total_speedtime += speed_time;
                    _MB_num += DEAFAUT_BLOCKSIZE * block_size;
                    i += block_size;
                    if (_MB_num % (1048576 * 50) == 0)
                    {
                        double now_speed = ((double)(1000 * 50) / (double)(total_speedtime));
                        double now_MB = _MB_num / 1048576;
                        string now_time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                        total_speedtime = 0;
                        this.PublishNotify((int)(i * 100 / actual_size), now_speed, now_MB, now_time);
                    }

                }
                this.PublishNotify(100, 0, 0, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                if (error_num == 0)
                {
                    Console.WriteLine("顺序读写测试完成，测试了" + actual_size + "个扇区未发生错误！");
                    this.PrintLog("顺序读写测试完成，测试了" + actual_size + "个扇区未发生错误！");
                }
                else
                {
                    Console.WriteLine("顺序读写测试完成，测试了" + actual_size + "个扇区出现了"+error_num+"个错误！");
                    this.PrintLog("顺序读写测试完成，测试了" + actual_size + "个扇区出现了" + error_num + "个错误！");
                }
            }
            else
            {
                Console.WriteLine("测试模式不存在，请重新选择!");
            }
        }
        /// <summary>
        /// 顺序只写,已修改测速模式
        /// </summary>
        /// <param name="driver_index">读写流的编号</param>
        /// <param name="percent">测试所占容量的百分比</param>
        /// <param name="test_data_mode">测试数据模式</param>
        /// <param name="block_size">块大小</param>
        /// <param name="circle">测试循环</param>
        public void OrderOnlyWrite(int driver_index, int percent = 100, int test_data_mode = 0, int block_size = 1, int circle = 1)
        {
            if (Disk_Driver_List.Count <= 0)
            {
                MessageBox.Show("未检测到设备！");
                return;
            }
            block_size = 2 * 1024 * 50;
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            TestArray = new byte[DEAFAUT_BLOCKSIZE * block_size];
            CompareArray = new byte[DEAFAUT_BLOCKSIZE * block_size];
            //long actual_size = ((driver.DiskInformation.DiskSectorSize / block_size)*percent)/100;
            long actual_size = driver.DiskInformation.DiskSectorSize * percent / 100;
            long speed_start;//测试读写速度
            long speed_end;
            long speed_time;
            long total_speedtime=0;
            long _MB_num = 0;
            this.PublishNotify(0, 0, 0, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            if (test_data_mode == 0 || test_data_mode == 1)
            {
                Init_TestArray(block_size, test_data_mode);
                long i;
                for (i=0; i < actual_size;)
                {
                    if (i +block_size> actual_size)//614400 610630
                    {
                        //这时候i已经大于actual_size了，应该减去blocksize;                       
                        Last_Block_Size = Convert.ToInt32(actual_size - i);
                        TestArray = new byte[DEAFAUT_BLOCKSIZE * Last_Block_Size];
                        Init_TestArray(Last_Block_Size, test_data_mode);
                        speed_start = Environment.TickCount;
                        driver.WritSector(TestArray, i ,Last_Block_Size);//记得将块的大小传进去
                        speed_end = Environment.TickCount;
                        speed_time = speed_end - speed_start;//最后一个块只有一次，只统计一次就行；
                        double now_MB_Last = (double)Last_Block_Size * 512 / (double)1048576;//最后一个块的MB数
                        
                        double now_speed_Last = ((double)(1000 * now_MB_Last) / (double)(speed_time));//写速度的计算
                        now_MB_Last += (_MB_num / 1048576);
                        string now_time_Last = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                        this.PublishNotify(100, now_speed_Last, now_MB_Last, now_time_Last);
                        Console.WriteLine("顺序只写测试完成，测试了" + actual_size + "个块未发生错误！");
                        this.PrintLog("顺序只写测试完成，测试了" + actual_size + "个块未发生错误！");
                        break;
                    }
                    //添加状态判断语句
                    if (Test_Status==false)
                    {
                        resetEvent.WaitOne();                        
                    }
                    //由于最后一个块比较大，最后一个块判断不能执行之后，将会有大量的块无法执行。
                    speed_start = Environment.TickCount;
                    driver.WritSector(TestArray, i, block_size);
                    speed_end = Environment.TickCount;
                    speed_time = speed_end - speed_start;
                    total_speedtime += speed_time;
                    _MB_num += DEAFAUT_BLOCKSIZE * block_size;
                    i += block_size;//注意这个i+=的位置要前移，否则进度条会很别扭
                    if (_MB_num % (1048576 * 50) == 0)//
                    {
                        double now_speed = ((double)(1000 * 50) / (double)(total_speedtime));
                        double now_MB = _MB_num / 1048576;
                        string now_time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                        total_speedtime = 0;
                        this.PublishNotify((int)(i*100/actual_size),now_speed,now_MB,now_time);
                    }
                }

                
            }
            else if (test_data_mode == 2)
            {
                for (long i = 0; i < actual_size;)
                {
                    if (i + block_size > actual_size)//614400 610630
                    {
                        //这时候i已经大于actual_size了，应该减去blocksize;                       
                        Last_Block_Size = Convert.ToInt32(actual_size - i);
                        TestArray = new byte[DEAFAUT_BLOCKSIZE * Last_Block_Size];
                        Init_TestArray(Last_Block_Size, test_data_mode);
                        speed_start = Environment.TickCount;
                        driver.WritSector(TestArray, i, Last_Block_Size);//记得将块的大小传进去
                        speed_end = Environment.TickCount;
                        speed_time = speed_end - speed_start;
                        double now_MB_Last = (double)Last_Block_Size * 512 / (double)1048576;//最后一个块的MB数
                        double now_speed_Last = ((double)(1000 * now_MB_Last) / (double)(speed_time));//写速度的计算
                        now_MB_Last += (_MB_num / 1048576);
                        string now_time_Last = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                        this.PublishNotify(100, now_speed_Last, now_MB_Last, now_time_Last);
                        Console.WriteLine("顺序只写测试完成，测试了" + actual_size + "个块未发生错误！");
                        this.PrintLog("顺序只写测试完成，测试了" + actual_size + "个块未发生错误！");
                        break;
                    }

                    //添加状态判断语句
                    if (Test_Status == false)
                    {
                        resetEvent.WaitOne();
                    }
                    Init_TestArray(block_size, test_data_mode);
                    speed_start = Environment.TickCount;
                    driver.WritSector(TestArray, i, block_size);
                    speed_end = Environment.TickCount;
                    speed_time = speed_end - speed_start;
                    total_speedtime += speed_time;
                    _MB_num += DEAFAUT_BLOCKSIZE * block_size;
                    i += block_size;
                    if (_MB_num % (1048576 * 50) == 0)//
                    {
                        double now_speed = ((double)(1000 * 50) / (double)(total_speedtime));
                        double now_MB = _MB_num / 1048576;
                        string now_time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                        GetWriteSpeed?.Invoke(now_speed);
                        total_speedtime = 0;
                        this.PublishNotify((int)(i * 100 / actual_size), now_speed, now_MB, now_time);
                    }
                }
            }
            else
            {
                Console.WriteLine("测试模式不存在，请重新选择!");
            }
            TestArray = null;
            CompareArray = null;
        }
        /// <summary>
        /// 顺序只读,已修改测速模式
        /// </summary>
        /// <param name="driver_index">读写流的编号</param>
        /// <param name="percent">测试所占容量的百分比</param>
        /// <param name="test_data_mode">测试数据模式</param>
        /// <param name="block_size">块大小</param>
        /// <param name="circle">测试循环</param>
        public void OrderOnlyRead(int driver_index, int percent = 100, int test_data_mode = 0, int block_size = 1, int circle = 1)
        {
            if (Disk_Driver_List.Count <= 0)
            {
                MessageBox.Show("未检测到设备！");
                return;
            }
            long start_time = Environment.TickCount;

            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            CompareArray = new byte[DEAFAUT_BLOCKSIZE * block_size];
            long actual_size = driver.DiskInformation.DiskSectorSize * percent / 100;
            long speed_start;//测试读写速度
            long speed_end;
            long speed_time;
            long total_speedtime=0;
            long _MB_num = 0;
            this.PublishNotify(0, 0, 0, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            for (long i = 0; i < actual_size;)
            {
                //添加状态判断语句
                if (i + block_size > actual_size)//
                {
                    //这时候i已经大于actual_size了，应该减去blocksize;                   
                    Last_Block_Size = Convert.ToInt32(actual_size - i);
                    int temp = DEAFAUT_BLOCKSIZE * Last_Block_Size;                   
                    CompareArray = new byte[temp];
                    speed_start = Environment.TickCount;
                    CompareArray = driver.ReadSector(i, Last_Block_Size);//传的是块大小
                    speed_end = Environment.TickCount;
                    speed_time = speed_end - speed_start;
                    double now_MB_Last = (double)Last_Block_Size * 512 / (double)1048576;//最后一个块的MB数
                    double now_speed_Last = (double)(1000 * now_MB_Last);
                    now_speed_Last = now_speed_Last / (speed_time);//写速度的计算
                    now_MB_Last += (_MB_num / MB);
                    string now_time_Last = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                    this.PublishNotify(100, now_speed_Last, now_MB_Last, now_time_Last);
                    Console.WriteLine("顺序只读测试完成，测试了" + actual_size + "次未发生错误！");
                    this.PrintLog("顺序只读测试完成，测试了" + actual_size + "次未发生错误！");
                    break;
                }
                if (Test_Status == false)
                {
                    resetEvent.WaitOne();
                }
                speed_start = Environment.TickCount;
                CompareArray = driver.ReadSector(i, block_size);
                speed_end = Environment.TickCount;
                speed_time = speed_end - speed_start;
                total_speedtime += speed_time;
                _MB_num += DEAFAUT_BLOCKSIZE * block_size;
                i += block_size;
                if (_MB_num % (MB * 50) == 0)
                {
                    double now_speed = ((double)(1000 * 50) / (double)(total_speedtime));
                    total_speedtime = 0;
                    double now_MB = _MB_num / MB;
                    string now_time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");                    
                    this.PublishNotify((int)(i * 100 / actual_size), now_speed, now_MB, now_time);
                }

            }


        }
        /// <summary>
        /// 随机读写验证,针对一次读写在256块以上的,已修改测速模式
        /// </summary>
        /// <param name="driver_index">读写IO流的编号</param>
        /// <param name="test_num">测试次数</param>
        /// <param name="test_time">测试时间</param>
        /// <param name="test_mode">测试模式--0代表全0,1代表全1，2随机数</param>
        public void RandomWriteAndVerify(int driver_index, long test_num = 0, long test_time = 0, int test_mode = 2,int block_size=8192)
        {
            if (Disk_Driver_List.Count <= 0)
            {
                MessageBox.Show("未检测到设备！");
                return;
            }
            Random R = new Random();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            long _MB_num = 0;
            this.PublishNotify(0, 0, 0, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            if (test_num == 0)
            {
                long start_time = Environment.TickCount;
                long speed_start;//测试读写速度
                long speed_end;
                long error_num = 0;
                long total_speedtime = 0;
                long speed_time;
                while (true)
                {
                    //添加状态判断语句
                    if (Test_Status == false)
                    {
                        resetEvent.WaitOne();
                    }
                    int temp_block = block_size;
                    int actual_block_size = DEAFAUT_BLOCKSIZE * temp_block;
                    TestArray = new byte[actual_block_size];
                    CompareArray = new byte[actual_block_size];
                    Init_TestArray(temp_block, test_mode);
                    long pos = NextLong(0, driver.DiskInformation.DiskSectorSize - temp_block);
                    //Console.WriteLine("写入" + pos + "扇区");
                    speed_start = Environment.TickCount;
                    driver.WritSector(TestArray, pos, temp_block);                   
                    CompareArray = driver.ReadSector(pos, temp_block);
                    speed_end = Environment.TickCount;//测试读写速度
                    speed_time = speed_end - speed_start;
                    total_speedtime += speed_time;
                    error_num += VerifyArray(TestArray, CompareArray);
                    long end_time = Environment.TickCount;
                    _MB_num += actual_block_size;
                    if ((end_time-start_time)%1000==0)//间隔一秒
                    {                   
                        double now_MB = _MB_num / 1048576;
                        double now_speed = ((double)(1000 * _MB_num) / (double)(total_speedtime) * 1048576);//累计读写字节除以时间
                        string now_time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                        GetWriteSpeed?.Invoke(now_speed);
                        total_speedtime = 0;
                        this.PublishNotify((int)(100*(end_time - start_time) / test_time), now_speed, now_MB, now_time);
                        _MB_num = 0;
                    }
                    if (end_time - start_time >= test_time)
                        break;
                }
                if (error_num == 0)
                {
                    Console.WriteLine("随机读写验证测试完成，测试了" + test_time + "毫秒未发生错误！");
                    this.PrintLog("随机读写验证测试完成，测试了" + test_time + "毫秒未发生错误！");
                }
                else
                {
                    Console.WriteLine("随机读写验证测试完成，测试了" + test_time + "毫秒出现了" + error_num + "个错误！");
                    this.PrintLog("随机读写验证测试完成，测试了" + test_time + "毫秒出现了" + error_num + "个错误！");
                }
            }
            else if (test_time == 0)
            {
                long temp_num = 1;
                int error_num = 0;
                long start_time = Environment.TickCount;
                long speed_start;//测试读写速度
                long speed_end;//算读写速度的
                long speed_time;
                long total_speedtime = 0;
                while (true)
                {
                    //添加状态判断语句
                    if (Test_Status == false)
                    {
                        resetEvent.WaitOne();
                    }
                    int temp_block = block_size;
                    int actual_block_size = DEAFAUT_BLOCKSIZE * temp_block;
                    TestArray = new byte[actual_block_size];
                    CompareArray = new byte[actual_block_size];
                    Init_TestArray(temp_block, test_mode);
                    long pos = NextLong(0, driver.DiskInformation.DiskSectorSize - temp_block);
                    Console.WriteLine("写入" + pos + "扇区");
                    speed_start = Environment.TickCount;
                    driver.WritSector(TestArray, pos, temp_block);                   
                    CompareArray = driver.ReadSector(pos, temp_block);
                    speed_end = Environment.TickCount;//测试读写速度
                    speed_time = speed_end - speed_start;
                    total_speedtime += speed_time;
                    error_num += VerifyArray(TestArray, CompareArray);
                    _MB_num += actual_block_size;
                    if (temp_num % 10 == 0)
                    {
                        string now_time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                        this.PublishProcessAndTime((int)(100 * temp_num / test_num), now_time);
                        double now_speed = ((double)1000 * 100 * actual_block_size / ((double)(total_speedtime) * MB));
                        double now_MB = (double)_MB_num / MB;
                        //string now_time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                        GetWriteSpeed?.Invoke(now_speed);
                        total_speedtime = 0;
                        this.PublishWrittenAndSpeed(now_speed, now_MB);
                        //this.PublishNotify((int)(100 * temp_num / test_num), now_speed, now_MB, now_time);                        
                    }
                    temp_num++;
                    if (temp_num > test_num)
                    {
                        break;
                    }
                }
                if (error_num == 0)
                {
                    Console.WriteLine("随机读写验证测试完成，测试了" + test_num + "次未发生错误！");
                    this.PrintLog("随机读写验证测试完成，测试了" + test_num + "次未发生错误！");
                }
                else
                {
                    Console.WriteLine("随机读写验证测试完成，测试了" + test_num + "次出现了" + error_num + "个错误！");
                    this.PrintLog("随机读写验证测试完成，测试了" + test_num + "次出现了" + error_num + "个错误！");
                }
            }
        }
        /// <summary>
        /// 随机读写验证，针对块大小在128及以下的，已修改测速模式
        /// </summary>
        /// <param name="driver_index"></param>
        /// <param name="test_num"></param>
        /// <param name="test_time"></param>
        /// <param name="test_mode"></param>
        /// <param name="block_size"></param>
        public void RandomWriteAndVerify_4KB(int driver_index, long test_num = 0, long test_time = 0, int test_mode = 2, int block_size = 8192)
        {
            //只计算读写的速度
            if (Disk_Driver_List.Count <= 0)
            {
                MessageBox.Show("未检测到设备！");
                return;
            }
            Random R = new Random();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            long Written_BlockSize = 0;
            long Temp_BlockSize = 0;//用于统计一秒钟读取的块数
            this.PublishNotify(0, 0, 0, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            if (test_num == 0)
            {
                long temp_num = 1;
                int error_num = 0;
                long start_time = Environment.TickCount;
                long end_time;
                long speed_start;//测试读写速度
                long speed_end;//算读写速度的
                ///这个是测试传递一次信息所累积的测试时间
                long total_speedtime=0;
                long speed_time;
                int percent=0;
                while (true)
                {
                    //添加状态判断语句
                    if (Test_Status == false)
                    {
                        resetEvent.WaitOne();
                    }
                    int temp_block = block_size;
                    int actual_block_size = DEAFAUT_BLOCKSIZE * temp_block;
                    TestArray = new byte[actual_block_size];
                    CompareArray = new byte[actual_block_size];
                    Init_TestArray(temp_block, test_mode);
                    long pos = NextLong(0, driver.DiskInformation.DiskSectorSize - temp_block);
                    Console.WriteLine("写入" + pos + "扇区");
                    speed_start = Environment.TickCount;
                    driver.WritSector(TestArray, pos, temp_block);
                    //调换位置
                    CompareArray = driver.ReadSector(pos, temp_block);
                    speed_end = Environment.TickCount;//测试读写速度
                    speed_time = speed_end - speed_start;
                    total_speedtime += speed_time;
                    error_num += VerifyArray(TestArray, CompareArray);                   
                    end_time = Environment.TickCount;
                    Written_BlockSize += actual_block_size;
                    if (temp_num % 10 == 0)
                    {
                        string now_time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                        //GetWriteSpeed?.Invoke(now_speed);
                        percent = (int)(100 * (end_time - start_time) / test_time);
                        this.PublishProcessAndTime(percent, now_time);
                        if (temp_num % 100 == 0)
                        {
                            double now_speed = ((double)1000 * 100 * actual_block_size / ((double)(total_speedtime) * MB));
                            double now_MB = (double)Written_BlockSize / MB;
                            //string now_time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                            GetWriteSpeed?.Invoke(now_speed);
                            total_speedtime=0;
                            this.PublishWrittenAndSpeed(now_speed, now_MB);
                        }
                    }                    
                    temp_num++;
                    if(end_time-start_time>test_time&&percent>100)
                    {
                        break;
                    }

                }
                if (error_num == 0)
                {
                    Console.WriteLine("随机读写验证测试完成，测试了" + test_time + "毫秒未发生错误！");
                    this.PrintLog("随机读写验证测试完成，测试了" + test_time + "毫秒未发生错误！");
                }
                else
                {
                    Console.WriteLine("随机读写验证测试完成，测试了" + test_time + "毫秒出现了" + error_num + "个错误！");
                    this.PrintLog("随机读写验证测试完成，测试了" + test_time + "毫秒出现了" + error_num + "个错误！");
                }
            }
            else if (test_time == 0)
            {
                long temp_num = 1;
                int error_num = 0;
                long start_time = Environment.TickCount;
                long speed_start;//测试读写速度
                long speed_end;//算读写速度的
                long total_speedtime=0;
                long speed_time;
                while (true)
                {
                    //添加状态判断语句
                    if (Test_Status == false)
                    {
                        resetEvent.WaitOne();
                    }                      
                    int temp_block = block_size;
                    int actual_block_size = DEAFAUT_BLOCKSIZE * temp_block;
                    TestArray = new byte[actual_block_size];
                    CompareArray = new byte[actual_block_size];
                    Init_TestArray(temp_block, test_mode);
                    long pos = NextLong(0, driver.DiskInformation.DiskSectorSize - temp_block);
                    Console.WriteLine("写入" + pos + "扇区");
                    speed_start = Environment.TickCount;
                    driver.WritSector(TestArray, pos, temp_block);
                    
                    CompareArray = driver.ReadSector(pos, temp_block);
                    speed_end = Environment.TickCount;//测试读写速度
                    speed_time = speed_end - speed_start;
                    total_speedtime += speed_time;
                    error_num += VerifyArray(TestArray, CompareArray);
                    
                    Written_BlockSize += actual_block_size;
                    if(temp_num%10==0)
                    {
                        string now_time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                        //GetWriteSpeed?.Invoke(now_speed);
                        this.PublishProcessAndTime((int)(100 * temp_num / test_num), now_time);
                        if (temp_num % 100 == 0)
                        {
                            double now_speed = ((double)1000 * 100 * actual_block_size / ((double)(total_speedtime) * MB));
                            double now_MB = (double)Written_BlockSize /MB;
                            //string now_time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                            GetWriteSpeed?.Invoke(now_speed);
                            total_speedtime = 0;
                            this.PublishWrittenAndSpeed(now_speed, now_MB);
                        }
                    }
                    temp_num++;
                    if (temp_num > test_num)
                    {
                        break;
                    }
                }
                if (error_num == 0)
                {
                    Console.WriteLine("随机读写验证测试完成，测试了" + test_num + "次未发生错误！");
                    this.PrintLog("随机读写验证测试完成，测试了" + test_num + "次未发生错误！");
                }
                else
                {
                    Console.WriteLine("随机读写验证测试完成，测试了" + test_num + "次出现了" + error_num + "个错误！");
                    this.PrintLog("随机读写验证测试完成，测试了" + test_num + "次出现了" + error_num + "个错误！");
                }
            }
        }
        /// <summary>
        /// 随机只读验证
        /// </summary>
        /// <param name="driver_index">读写IO流的编号--与硬盘对应</param>
        /// <param name="test_num">测试次数</param>
        /// <param name="test_time">测试时间</param>
        public void RandomOnlyRead(int driver_index, long test_num = 0, long test_time = 0,int block_size=8192)
        {
            if (Disk_Driver_List.Count <= 0)
            {
                MessageBox.Show("未检测到设备！");
                return;
            }
            Random R = new Random();
            this.PublishNotify(0, 0, 0, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            long start_time = Environment.TickCount;
            long speed_start;//测试读写速度
            long speed_end;//算读写速度的
            long speed_time;
            long total_speedtime = 0;
            long _MB_num = 0;
            long _MB_num_last = 0;
            long pos;
            int temp_block = block_size;
            int actual_block_size = DEAFAUT_BLOCKSIZE * temp_block;
            if (test_num == 0)
            {               
                while (true)
                {
                    //添加状态判断语句
                    if (Test_Status == false)
                    {
                        resetEvent.WaitOne();
                    }
                    //int temp_block = R.Next(1, 5);
                    CompareArray = new byte[actual_block_size];
                    pos = NextLong(0, driver.DiskInformation.DiskSectorSize - temp_block);
                    speed_start = Environment.TickCount;
                    CompareArray = driver.ReadSector(pos, temp_block);
                    speed_end = Environment.TickCount;
                    speed_time = speed_end - speed_start;
                    total_speedtime += speed_time;
                    long end_time = Environment.TickCount;
                    _MB_num += actual_block_size;
                    if ((end_time - start_time) % 1000 == 0)//间隔一秒
                    {
                        double now_MB = _MB_num / 1048576;
                        double now_speed = ((double)(1000 * _MB_num) / (double)(total_speedtime) * 1048576);//累计读写字节除以时间
                        string now_time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                        GetWriteSpeed?.Invoke(now_speed);
                        total_speedtime = 0;
                        this.PublishNotify((int)(100 * (end_time - start_time) / test_time), now_speed, now_MB, now_time);
                        _MB_num = 0;
                     }
                    if (end_time - start_time >= test_time)
                        break;
                }
                Console.WriteLine("随机只读测试完成，测试了" + test_time + "毫秒未发生错误！");
                this.PrintLog("随机只读测试完成，测试了" + test_time + "毫秒未发生错误！");

            }
            else if (test_time == 0)
            {
                long temp_num = 0;
                while (true)
                {
                    if (temp_num+1==test_num)//假如是最后一次测试
                    {
                        //这时候i已经大于actual_size了，应该减去blocksize;                                          
                        pos = NextLong(0, driver.DiskInformation.DiskSectorSize - temp_block);
                        speed_start = Environment.TickCount;
                        CompareArray = driver.ReadSector(pos, Last_Block_Size);//传的是块大小
                        speed_end = Environment.TickCount;
                        speed_time = speed_end - speed_start;
                        _MB_num += actual_block_size;
                        _MB_num_last += actual_block_size;
                        double now_speed = ((double)(1000*(_MB_num_last/MB)) / (double)(speed_time));//_MB_num_last/MB是最后一部分累积的MB
                        double now_MB = _MB_num / MB;
                        string now_time_Last = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                        this.PublishNotify(100, now_speed, now_MB, now_time_Last);
                        Console.WriteLine("随机只读测试完成，测试了" + test_num + "次未发生错误！");
                        this.PrintLog("随机只读测试完成，测试了" + test_num + "次未发生错误！");
                        break;
                    }
                    //添加状态判断语句
                    if (Test_Status == false)
                    {
                        resetEvent.WaitOne();
                    }
                    if (temp_num >= test_num)
                        break;
                    //int temp_block = R.Next(1, 5);
                    
                    pos = NextLong(0, driver.DiskInformation.DiskSectorSize - temp_block);
                    speed_start = Environment.TickCount;
                    CompareArray = driver.ReadSector(pos, block_size);
                    speed_end = Environment.TickCount;
                    speed_time = speed_end - speed_start;
                    total_speedtime += speed_time;
                    _MB_num += actual_block_size;
                    _MB_num_last += actual_block_size;
                    if (_MB_num % MB == 0)
                    {
                        double now_speed = ((double)(1000) / (double)(total_speedtime));
                        double now_MB = _MB_num / MB;
                        string now_time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                        total_speedtime = 0;
                        _MB_num_last = 0;
                        this.PublishNotify((int)(100 * temp_num / test_num), now_speed, now_MB, now_time);
                    }
                    temp_num++;
                }
            }
        }
        /// <summary>
        /// 随机只写验证,已修改测速模式
        /// </summary>
        /// <param name="driver_index">读写IO流的编号--与硬盘对应</param>
        /// <param name="test_num">测试次数</param>
        /// <param name="test_time">测试时间</param>
        /// <param name="test_mode">测试模式</param>
        public void RandomOnlyWrite(int driver_index, long test_num = 0, long test_time = 0, int test_mode = 2, int block_size = 8192)
        {
            if (Disk_Driver_List.Count <= 0)
            {
                MessageBox.Show("未检测到设备！");
                return;
            }
            Random R = new Random();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            long start_time = Environment.TickCount;
            long speed_start;//测试写速度
            long speed_end;
            long speed_time;
            long total_speedtime=0;
            long _MB_num = 0;
            if (test_num == 0)
            {

                while (true)
                {
                    //添加状态判断语句
                    if (Test_Status == false)
                    {
                        resetEvent.WaitOne();
                    }
                    //int temp_block = R.Next(1, 5);
                    int temp_block = block_size;
                    int actual_block_size = DEAFAUT_BLOCKSIZE * temp_block;
                    TestArray = new byte[actual_block_size];
                    Init_TestArray(temp_block, test_mode);
                    
                    long pos = NextLong(0, driver.DiskInformation.DiskSectorSize - temp_block);
                    speed_start = Environment.TickCount;
                    driver.WritSector(TestArray, pos, temp_block);
                    speed_end = Environment.TickCount;
                    speed_time = speed_end - speed_start;
                    total_speedtime += speed_time;
                    long end_time = Environment.TickCount;
                    _MB_num += actual_block_size;
                    if ((end_time - start_time) % 1000 == 0)
                    {
                        double now_speed = ((double)(1000 * 50) / (double)(total_speedtime));
                        double now_MB = _MB_num / 1048576;
                        string now_time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                        _MB_num = 0;
                        total_speedtime = 0;
                        this.PublishNotify((int)(100 * (end_time-start_time) / test_time), now_speed, now_MB, now_time);
                    }
                    if (end_time - start_time >= test_time)
                        break;
                }
                this.PublishNotify(100, 0, 0, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                Console.WriteLine("随机只写测试完成，测试了" + test_time + "毫秒！");
                this.PrintLog("随机只写测试完成，测试了" + test_time + "毫秒！");
            }
            else if (test_time == 0)
            {
                long temp_num = 0;
                while (true)
                {
                    //添加状态判断语句
                    if (Test_Status == false)
                    {
                        resetEvent.WaitOne();
                    }
                    if (temp_num >= test_num)
                        break;
                    //int temp_block = R.Next(1, 5);
                    int temp_block = block_size;
                    int actual_block_size = DEAFAUT_BLOCKSIZE * temp_block;
                    TestArray = new byte[actual_block_size];
                    Init_TestArray(temp_block, test_mode);
                    long pos = NextLong(0, driver.DiskInformation.DiskSectorSize - temp_block);
                    speed_start = Environment.TickCount;
                    driver.WritSector(TestArray, pos, temp_block);
                    speed_end = Environment.TickCount;
                    speed_time = speed_end - speed_start;
                    total_speedtime += speed_time;
                    _MB_num += actual_block_size;
                    if (_MB_num % MB == 0)
                    {
                        double now_speed = ((double)(1000) /(double)(total_speedtime));
                        double now_MB = _MB_num / MB;
                        string now_time = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                        total_speedtime = 0;
                        this.PublishNotify((int)(100 *temp_num/test_num), now_speed, now_MB, now_time);
                        
                    }
                    temp_num++;
                }
                this.PublishNotify(100, 0, 0, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                Console.WriteLine("随机只写测试完成，测试了" + test_num + "次！");
                this.PrintLog("随机只写测试完成，测试了" + test_num + "次！");
            }
        }
        public void Init_TestArray(int block_size, int mode)
        {
            if (block_size == 0)
            {
                MessageBox.Show("块大小不能为0");
                return;
            }
            if (mode == 0)
            {
                for (int i = 0; i < block_size * DEAFAUT_BLOCKSIZE; i++)
                {
                    TestArray[i] = 0;
                }
            }
            else if (mode == 1)
            {
                for (int i = 0; i < block_size * DEAFAUT_BLOCKSIZE; i++)
                {
                    TestArray[i] = 255;
                }
            }
            else if (mode == 2)
            {
                Random R = new Random();
                for (int i = 0; i < block_size * DEAFAUT_BLOCKSIZE; i++)
                {

                    TestArray[i] = (byte)R.Next(0, 255);
                }
            }

        }
        public int VerifyArray(byte[] testarray, byte[] comparearray)
        {
            if (testarray.Length != comparearray.Length)
            {
                Console.WriteLine("数组长度不匹配！");
            }
            int error_num = 0;
            for (int i = 0; i < testarray.Length; i++)
            {
                if (testarray[i] != comparearray[i])
                {
                    Console.WriteLine(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")+"当前位置" + i + "出错，正确数据为" + testarray[i] + "错误数据为：" + comparearray[i]);
                    this.PrintLog(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")+"当前位置" + i + "出错，正确数据为" + testarray[i] + "错误数据为：" + comparearray[i]);
                    error_num++;
                }
            }
            return error_num;

        }
        public long NextLong(long A, long B)
        {
            Random R = new Random((int)DateTime.Now.Ticks);
            long myResult = A;
            //-----
            long Max = B, Min = A;
            if (A > B)
            {
                Max = A;
                Min = B;
            }
            double Key = R.NextDouble();
            myResult = Min + (long)((Max - Min) * Key);
            //-----
            return myResult;
        }
        /// <summary>
        /// 硬盘数据点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Ed[e.RowIndex].Show();
                //Ed[e.RowIndex].TransfChooseINF += Get_Transf_Choose_INF_Event;
                //Disk_Choose_Information_List[e.RowIndex] = Temp_Choose;
                panel1.Controls.Clear();
                panel1.Controls.Add(Ed[e.RowIndex]);
                now_index_framework = e.RowIndex;
            }
            catch (Exception ee){
                MessageBox.Show("找不到硬盘");
             }

            
        }

        
    }
}
