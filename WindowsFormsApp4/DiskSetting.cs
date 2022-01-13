using DiskTestLib;
using System;
using System.Collections;
using System.Management;
using System.Threading;
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
        /// <summary>
        /// 默认块大小
        /// </summary>
        private const int DEAFAUT_BLOCKSIZE = 512;
        /// <summary>
        /// 1MB的实际字节数
        /// </summary>
        private const int MB=1048576;
        /// <summary>
        /// 一个静态全局变量，用于在Compute_OnceBlockTime函数中区分读，写，验证，VERTIFY是验证
        /// </summary>
        private const int VERTIFY = 0;
        /// <summary>
        /// 一个静态全局变量，用于在Compute_OnceBlockTime函数中区分读，写，验证，WRITE是写
        /// </summary>
        private const int WRITE = 1;
        /// <summary>
        /// 一个静态全局变量，用于在Compute_OnceBlockTime函数中区分读，写，验证，READ是读
        /// </summary>
        private const int READ = 2;
        /// <summary>
        /// 一个静态全局变量，Complete_Information_Print是在测试结束后信息打印的函数，该变量代表是随机验证
        /// </summary>
        private const int RANDOM_VERTIFY = 0;
        /// <summary>
        /// 一个静态全局变量，Complete_Information_Print是在测试结束后信息打印的函数，该变量代表是随机只读
        /// </summary>
        private const int RANDOM_READ = 1;
        /// <summary>
        /// 一个静态全局变量，Complete_Information_Print是在测试结束后信息打印的函数，该变量代表是随机只写
        /// </summary>
        private const int RANDOM_WRITE = 2;
        /// <summary>
        /// 一个静态全局变量，Complete_Information_Print是在测试结束后信息打印的函数，该变量代表是顺序验证
        /// </summary>
        private const int ORDER_VERTIFY = 3;
        /// <summary>
        /// 一个静态全局变量，Complete_Information_Print是在测试结束后信息打印的函数，该变量代表是顺序只读
        /// </summary>
        private const int ORDER_READ = 4;
        /// <summary>
        /// 一个静态全局变量，Complete_Information_Print是在测试结束后信息打印的函数，该变量代表是顺序只写
        /// </summary>
        private const int ORDER_WRITE = 5;
        /// <summary>
        /// 小间隔块数，用于计算读写速度
        /// </summary>
        private const int SMALL_INR = 1;
        /// <summary>
        /// 中等间隔块数用于计算读写速度
        /// </summary>
        private const int MIDDLE_INR = 10;
        /// <summary>
        /// 大间隔块数用于计算读写速度
        /// </summary>
        private const int BIG_INR = 25;
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
        /// <summary>
        /// 当前测试状态，是停止，还是继续；
        /// </summary>
        public bool Test_Status;
        private static AutoResetEvent resetEvent = new AutoResetEvent(true);
        /// <summary>
        /// 一个块包含的字节数
        /// </summary>
        private long Block_Bytes;
        /// <summary>
        /// 全部传输累积的字节数
        /// </summary>
        private long Total_Bytes;
        /// <summary>
        /// 单次传输累积的字节数
        /// </summary>
        private long Once_Bytes;
        /// <summary>
        /// 用于计算次数的中间变量
        /// </summary>
        private long Temp_Nums;
        /// <summary>
        /// 测试的开始时间
        /// </summary>
        private long Test_Start_Time;
        /// <summary>
        /// 测试的结束时间
        /// </summary>
        private long Test_End_Time;
        /// <summary>
        /// 用于统计错误出现的次数的变量
        /// </summary>
        private int Test_Error_Num;
        /// <summary>
        /// 测试进度，也是进度条的参数
        /// </summary>
        private int Percent;
        /// <summary>
        /// 当前测试到的位置，用于顺序测试
        /// </summary>
        private long Now_Pos;
        /// <summary>
        /// 快间隔，用于传输测试时间和进度条的信息
        /// </summary>
        private int Fast_INR;
        /// <summary>
        /// 慢间隔
        /// </summary>
        private int Slow_INR;
        /// <summary>
        /// 由于是通过选择容量百分比来决定测试的空间，所以这个变量表示的就是顺序测试中你要测试到的扇区数
        /// </summary>
        private long Order_Max_Block;
        /// <summary>
        /// 显示硬盘信息的对象数组
        /// </summary>
        public Disk[] Ed;
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
        struct SPEED_COMPUTE
        {
            public long Start_Time;
            public long End_Time;
            public long Once_Time;
            public long Total_Time;
            public static implicit operator SPEED_COMPUTE(long i)
            {
                return new SPEED_COMPUTE() { Start_Time = 0, End_Time=0,Once_Time=0,Total_Time=i };
            }
        }
        private SPEED_COMPUTE Speed_Compute;
        public DiskSetting()
        {           
            InitializeComponent();
            Init_Disk_Information();
            Init_Disk_Framework();
            Init_Choose_ArrayList();
            Speed_Compute = 0;
            Block_Bytes = 0;
            Total_Bytes = 0;
            Once_Bytes = 0;
            Temp_Nums = 0;
            Test_Start_Time = 0;
            Test_End_Time = 0;
            Test_Error_Num = 0;
            Percent = 0;
            Now_Pos = 0;
            Fast_INR = 0;
            Slow_INR = 0;
            Order_Max_Block = 0;
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
        //增添Switch观察者对象
        public void AddSwitchObserver(SwitchEventHandler observer)
        {
            SwitchEvent += observer;
        }
        public void AddLogObserver(LogEventHandler observer)
        {
            LogEvent += observer;
        }
        public void AddStartTimeObserver(StartTimeEventHandler observer)
        {
            StartTimeEvent += observer;
        }
        public void AddProcessAndTimeObserver(NotifyProcessAndTimeHandler observer)
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
                if (chooseInformation.TestOrNot == true)
                {
                    if(chooseInformation.TestMode==0)
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
        /// 计算最后一块的大小
        /// </summary>
        /// <param name="block_size"></param>
        /// <returns></returns>
        private int Compute_Last_Block_Size(int block_size)
        {
            if (Now_Pos + block_size >= Order_Max_Block) 
            {
                block_size = (int)(Order_Max_Block - Now_Pos);
                Block_Bytes = DEAFAUT_BLOCKSIZE * block_size;
                CompareArray = new byte[Block_Bytes];
                return block_size; 
            }
            else return block_size;
        }
        /// <summary>
        /// 写最后一个块
        /// </summary>
        /// <param name="Speed_Compute">计算速度的结构体</param>
        /// <param name="actual_size">每个块的包含的字节数</param>
        /// <param name="pos">当前写的位置</param>
        /// <param name="driver">硬盘读写测试类</param>
        /// <param name="test_data_mode">测试数据的模式</param>
        /// <param name="_MB_num">已测试的MB数</param>
        /// <summary>
        /// 进行一次读写
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="pos"></param>
        /// <param name="block_size"></param>
        /// <param name="compute_mode"></param>
        private void Compute_OnceBlockTime(DriverLoader driver,long pos,int block_size,int compute_mode)
        {
            Speed_Compute.Start_Time = Environment.TickCount;
            if(compute_mode==VERTIFY)
            {
                driver.WritSector(TestArray, pos, block_size);
                CompareArray = driver.ReadSector(pos, block_size);
            }
            else if(compute_mode==WRITE)
            {
                driver.WritSector(TestArray, pos, block_size);
            }
            else if(compute_mode==READ)
            {
                CompareArray = driver.ReadSector(pos, block_size);
            }
            Speed_Compute.End_Time = Environment.TickCount;
            Speed_Compute.Once_Time = Speed_Compute.End_Time - Speed_Compute.Start_Time;
            Speed_Compute.Total_Time += Speed_Compute.Once_Time;
        }
        /// <summary>
        /// 根据测试模式的不同，打印测试完成的信息
        /// </summary>
        /// <param name="Error_num">错误次数</param>
        /// <param name="Test_Mode">测试模式，有顺序读写验证，随机读写验证六种</param>
        /// <param name="time">测试时间，在随机测试中会使用到</param>
        /// <param name="num">测试次数，在随机测试中会使用到</param>
        /// <param name="max_sector">目标扇区块数</param>
        private void Complete_Information_Print(int Error_num, int Test_Mode, long time = 0, long num = 0,long max_sector=0)
        {
            if (Error_num == 0)
            {
                switch (Test_Mode)
                {
                    case RANDOM_VERTIFY:
                        Console.WriteLine("随机读写验证测试完成，未发生错误！");
                        this.PrintLog("随机读写验证测试完成，未发生错误！");
                        break;
                    case RANDOM_READ:
                        Console.WriteLine("随机读验证测试完成，未发生错误！");
                        this.PrintLog("随机读验证测试完成，未发生错误！");
                        break;
                    case RANDOM_WRITE:
                        Console.WriteLine("随机写验证测试完成，未发生错误！");
                        this.PrintLog("随机写验证测试完成，未发生错误！");
                        break;
                    case ORDER_VERTIFY:
                        Console.WriteLine("顺序读写验证测试完成，未发生错误！");
                        this.PrintLog("顺序读写验证测试完成，未发生错误！");
                        break;
                    case ORDER_READ:
                        Console.WriteLine("顺序读验证测试完成，未发生错误！");
                        this.PrintLog("顺序读验证测试完成，未发生错误！");
                        break;
                    case ORDER_WRITE:
                        Console.WriteLine("顺序写验证测试完成，未发生错误！");
                        this.PrintLog("顺序写验证测试完成，未发生错误！");
                        break;
                }
            }
            else
            {
                switch (Test_Mode)
                {
                    case RANDOM_VERTIFY:
                        Console.WriteLine("随机读写验证测试完成，出现了" + Error_num + "个错误！");
                        this.PrintLog("随机读写验证测试完成，出现了" + Error_num + "个错误！");
                        break;
                    case RANDOM_READ:
                        Console.WriteLine("随机读测试完成，出现了" + Error_num + "个错误！");
                        this.PrintLog("随机读测试完成，出现了" + Error_num + "个错误！");
                        break;
                    case RANDOM_WRITE:
                        Console.WriteLine("随机读写验证测试完成，出现了" + Error_num + "个错误！");
                        this.PrintLog("随机读写验证测试完成，出现了" + Error_num + "个错误！");
                        break;
                    case ORDER_VERTIFY:
                        Console.WriteLine("顺序读写验证测试完成，出现了" + Error_num + "个错误！");
                        this.PrintLog("随机读写验证测试完成，出现了" + Error_num + "个错误！");
                        break;
                    case ORDER_READ:
                        Console.WriteLine("顺序读测试完成，出现了" + Error_num + "个错误！");
                        this.PrintLog("顺序读测试完成，出现了" + Error_num + "个错误！");
                        break;
                    case ORDER_WRITE:
                        Console.WriteLine("顺序写测试完成，出现了" + Error_num + "个错误！");
                        this.PrintLog("顺序写验证测试完成，出现了" + Error_num + "个错误！");
                        break;
                }
            }
            if (num != 0&&time==0&&max_sector==0)
            {
                Console.WriteLine("测试次数：" + num + "次");
                this.PrintLog("测试次数：" + num + "次");
            }
            else if(num == 0 && time != 0 && max_sector == 0)
            {
                Console.WriteLine("测试时间：" + time + "ms");
                this.PrintLog("测试时间：" + time + "ms");
            }
            else
            {
                Console.WriteLine("测试到目标扇区：" + max_sector + "ms");
                this.PrintLog("测试到目标扇区：" + max_sector + "ms");
            }

        }
        /// <summary>
        /// 计算一次处理块的速度，并传输到Form
        /// </summary>
        private void Compute_OnceBlockSpeed()
        {
            double now_MB = Total_Bytes / MB;
            double once_mb = (double)Once_Bytes / (double)MB;
            double now_speed = ((1000 * once_mb) / (Speed_Compute.Total_Time));//累计读写字节除以时间                        
            GetWriteSpeed?.Invoke(now_speed);
            if (now_speed == 0)
            {
                Console.WriteLine("speed is 0!");
            }
            if (double.IsNaN(now_speed))
            {
                Console.WriteLine("speed is NaN!");
            }
            Speed_Compute.Total_Time = 0;
            this.PublishWrittenAndSpeed(now_speed, now_MB);
            Once_Bytes = 0;
        }
        /// <summary>
        /// 初始化测试参数，包括Test_Error_Num ，Test_Start_Time ，Temp_Nums，Total_Bytes，Once_Bytes，Percent，Block_Bytes
        /// </summary>
        /// <param name="block_size"></param>
        private void Init_Test_Param(int block_size)
        {
            Test_Error_Num = 0;
            Test_Start_Time = Environment.TickCount;
            Temp_Nums = 1;
            Total_Bytes = 0;
            Once_Bytes = 0;
            Percent = 0;
            Block_Bytes = DEAFAUT_BLOCKSIZE * block_size;
        }
        /// <summary>
        /// 检测是否有设备，如果没有则弹出提示框无设备
        /// </summary>
        private void Test_Driver_List()
        {
            if (Disk_Driver_List.Count <= 0)
            {
                MessageBox.Show("未检测到设备！");
                return;
            }
        }
        /// <summary>
        /// 单次传输字节数和总字节数的累加
        /// </summary>
        private void Add_Bytes()
        {
            Total_Bytes += Block_Bytes;
            Once_Bytes += Block_Bytes;
        }
        /// <summary>
        /// 顺序读写验证,已修改测速模式，已验证速度无误
        /// </summary>
        /// <param name="driver_index">读写流的编号</param>
        /// <param name="percent">测试所占容量的百分比</param>
        /// <param name="test_data_mode">测试数据模式</param>
        /// <param name="block_size">块大小</param>
        /// <param name="circle">测试循环</param>
        public void OrderWriteAndVerify(int driver_index, int percent_of_all_size = 100, int test_mode = 0, int block_size=0, int circle = 1)
        {
            Test_Driver_List();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            this.PublishNotify(0, 0, 0, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            Order_Max_Block = percent_of_all_size * driver.DiskInformation.DiskSectorSize / 100;
            Now_Pos = 0;
            Init_Test_Param(block_size);
            CompareArray = new byte[Block_Bytes];
            Fast_INR = (block_size >= 256) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (block_size >= 256) ? MIDDLE_INR : BIG_INR;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Status == false) resetEvent.WaitOne();                
                block_size = Compute_Last_Block_Size(block_size);
                Init_TestArray(block_size, test_mode);
                Compute_OnceBlockTime(driver, Now_Pos, block_size, VERTIFY);
                Test_Error_Num += VerifyArray(TestArray, CompareArray);
                Test_End_Time = Environment.TickCount;
                Add_Bytes();               
                Now_Pos += block_size;
                Percent = (int)(Now_Pos * 100 / Order_Max_Block);
                if (Temp_Nums % Fast_INR == 0||Percent==100)
                {                   
                    this.PublishProcessAndTime((int)(Percent), DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    if (Temp_Nums % Slow_INR == 0 || Percent >= 100) Compute_OnceBlockSpeed();
                }
                if (Now_Pos >= Order_Max_Block) break;
                else
                { 
                    Temp_Nums++;
                    continue; 
                }
            }
            Complete_Information_Print(Test_Error_Num, ORDER_VERTIFY, 0, 0,Order_Max_Block);
        }
        /// <summary>
        /// 顺序只写,已修改测速模式
        /// </summary>
        /// <param name="driver_index">读写流的编号</param>
        /// <param name="percent">测试所占容量的百分比</param>
        /// <param name="test_data_mode">测试数据模式</param>
        /// <param name="block_size">块大小</param>
        /// <param name="circle">测试循环</param>
        public void OrderOnlyWrite(int driver_index, int percent_of_all_size = 100, int test_mode = 0, int block_size = 1, int circle = 1)
        {
            Test_Driver_List();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            this.PublishNotify(0, 0, 0, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            Order_Max_Block = percent_of_all_size * driver.DiskInformation.DiskSectorSize / 100;
            Now_Pos = 0;
            Init_Test_Param(block_size);
            CompareArray = new byte[Block_Bytes];
            Fast_INR = (block_size >= 256) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (block_size >= 256) ? MIDDLE_INR : BIG_INR;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Status == false) resetEvent.WaitOne();
                block_size = Compute_Last_Block_Size(block_size);
                Init_TestArray(block_size, test_mode);
                Compute_OnceBlockTime(driver, Now_Pos, block_size, WRITE);
                Test_End_Time = Environment.TickCount;
                Add_Bytes();
                Now_Pos += block_size;
                Percent = (int)(Now_Pos * 100 / Order_Max_Block);
                if (Temp_Nums % Fast_INR == 0 || Percent == 100)
                {
                    this.PublishProcessAndTime((int)(Percent), DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    if (Temp_Nums % Slow_INR == 0 || Percent >= 100) Compute_OnceBlockSpeed();
                }
                if (Now_Pos >= Order_Max_Block) break;
                else
                {
                    Temp_Nums++;
                    continue;
                }
            }
            Complete_Information_Print(Test_Error_Num, ORDER_WRITE, 0, 0, Order_Max_Block);
        }
        /// <summary>
        /// 顺序只读,已修改测速模式
        /// </summary>
        /// <param name="driver_index">读写流的编号</param>
        /// <param name="percent">测试所占容量的百分比</param>
        /// <param name="test_data_mode">测试数据模式</param>
        /// <param name="block_size">块大小</param>
        /// <param name="circle">测试循环</param>
        public void OrderOnlyRead(int driver_index, int percent_of_all_size = 100, int test_mode = 0, int block_size = 1, int circle = 1)
        {
            Test_Driver_List();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            this.PublishNotify(0, 0, 0, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            Order_Max_Block = percent_of_all_size * driver.DiskInformation.DiskSectorSize / 100;
            Now_Pos = 0;
            Init_Test_Param(block_size);
            CompareArray = new byte[Block_Bytes];
            Fast_INR = (block_size >= 256) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (block_size >= 256) ? MIDDLE_INR : BIG_INR;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Status == false) resetEvent.WaitOne();
                block_size = Compute_Last_Block_Size(block_size);
                Init_TestArray(block_size, test_mode);
                Compute_OnceBlockTime(driver, Now_Pos, block_size, READ);
                Test_End_Time = Environment.TickCount;
                Add_Bytes();
                Now_Pos += block_size;
                Percent = (int)(Now_Pos * 100 / Order_Max_Block);
                if (Temp_Nums % Fast_INR == 0 || Percent == 100)
                {
                    this.PublishProcessAndTime((int)(Percent), DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    if (Temp_Nums % Slow_INR == 0 || Percent >= 100) Compute_OnceBlockSpeed();
                }
                if (Now_Pos >= Order_Max_Block) break;
                else
                {
                    Temp_Nums++;
                    continue;
                }
            }
            Complete_Information_Print(Test_Error_Num, ORDER_READ, 0, 0, Order_Max_Block);
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
            Test_Driver_List();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            this.PublishNotify(0, 0, 0, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            Init_Test_Param(block_size);            
            CompareArray = new byte[Block_Bytes];
            Fast_INR = (block_size >= 256) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (block_size >= 256) ? MIDDLE_INR : BIG_INR;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Status == false) resetEvent.WaitOne();
                Init_TestArray(block_size, test_mode);
                Now_Pos = NextLong(0, driver.DiskInformation.DiskSectorSize - block_size);
                Compute_OnceBlockTime(driver, Now_Pos, block_size, VERTIFY);
                Test_Error_Num += VerifyArray(TestArray, CompareArray);
                Test_End_Time = Environment.TickCount;
                Add_Bytes();
                if (Temp_Nums%Fast_INR==0)
                {
                    Percent = (test_num==0)?(int)(100 * (Test_End_Time - Test_Start_Time) / test_time): (int)(100 * Temp_Nums / test_num);
                    this.PublishProcessAndTime((int)(Percent), DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    if (Temp_Nums % Slow_INR == 0 || Percent >= 100) Compute_OnceBlockSpeed();
                }

                if ((test_num == 0) && (Test_End_Time - Test_Start_Time >= test_time || Percent > 100)) break;
                else if ((test_time == 0) && (Temp_Nums > test_num || Percent >= 100)) break;
                else
                {
                    Temp_Nums++; continue;
                }
            }
            if (test_num == 0) Complete_Information_Print(Test_Error_Num, RANDOM_VERTIFY, test_time, 0);
            else Complete_Information_Print(Test_Error_Num, RANDOM_VERTIFY, 0, test_num);
        }
        /// <summary>
        /// 随机只读验证
        /// </summary>
        /// <param name="driver_index">读写IO流的编号--与硬盘对应</param>
        /// <param name="test_num">测试次数</param>
        /// <param name="test_time">测试时间</param>
        public void RandomOnlyRead(int driver_index, long test_num = 0, long test_time = 0,int block_size=8192)
        {
            Test_Driver_List();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            this.PublishNotify(0, 0, 0, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            Init_Test_Param(block_size);
            CompareArray = new byte[Block_Bytes];
            Fast_INR = (block_size >= 256) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (block_size >= 256) ? MIDDLE_INR : BIG_INR;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Status == false) resetEvent.WaitOne();
                Now_Pos = NextLong(0, driver.DiskInformation.DiskSectorSize - block_size);
                Compute_OnceBlockTime(driver, Now_Pos, block_size, READ);
                Test_End_Time = Environment.TickCount;
                Add_Bytes();
                if (Temp_Nums % Fast_INR == 0)
                {
                    Percent = (test_num == 0) ? (int)(100 * (Test_End_Time - Test_Start_Time) / test_time) : (int)(100 * Temp_Nums / test_num);
                    this.PublishProcessAndTime((int)(Percent), DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    if (Temp_Nums % Slow_INR == 0 || Percent >= 100) Compute_OnceBlockSpeed();
                }
                Temp_Nums++;
                if ((test_num == 0) && (Test_End_Time - Test_Start_Time >= test_time || Percent > 100)) break;
                else if ((test_time == 0) && (Temp_Nums > test_num || Percent >= 100)) break;
                else continue;
            }
            if (test_num == 0) Complete_Information_Print(Test_Error_Num, RANDOM_READ, test_time, 0);
            else Complete_Information_Print(Test_Error_Num, RANDOM_READ, 0, test_num);
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
            Test_Driver_List();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            this.PublishNotify(0, 0, 0, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            Init_Test_Param(block_size);
            Fast_INR = (block_size >= 256) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (block_size >= 256) ? MIDDLE_INR : BIG_INR;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Status == false) resetEvent.WaitOne();
                Init_TestArray(block_size, test_mode);
                Now_Pos = NextLong(0, driver.DiskInformation.DiskSectorSize - block_size);
                Compute_OnceBlockTime(driver, Now_Pos, block_size, WRITE);
                Test_End_Time = Environment.TickCount;
                Add_Bytes();
                if (Temp_Nums % Fast_INR == 0)
                {
                    Percent = (test_num == 0) ? (int)(100 * (Test_End_Time - Test_Start_Time) / test_time) : (int)(100 * Temp_Nums / test_num);
                    this.PublishProcessAndTime((int)(Percent), DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    if (Temp_Nums % Slow_INR == 0 || Percent >= 100) Compute_OnceBlockSpeed();
                }
                Temp_Nums++;
                if ((test_num == 0) && (Test_End_Time - Test_Start_Time >= test_time || Percent > 100)) break;
                else if ((test_time == 0) && (Temp_Nums > test_num || Percent >= 100)) break;
                else continue;
            }
            if (test_num == 0) Complete_Information_Print(Test_Error_Num, RANDOM_WRITE, test_time, 0);
            else Complete_Information_Print(Test_Error_Num, RANDOM_WRITE, 0, test_num);
        }
        /// <summary>
        /// 初始化测试数组
        /// </summary>
        /// <param name="block_size">测试块大小</param>
        /// <param name="mode">测试模式</param>
        public void Init_TestArray(int block_size, int mode)
        {
            if (block_size == 0)
            {
                MessageBox.Show("块大小不能为0");
                return;
            }
            TestArray = new byte[Block_Bytes];
            if (mode == 0)
            {
                for (int i = 0; i < Block_Bytes; i++)
                {
                    TestArray[i] = 0;
                }
            }
            else if (mode == 1)
            {
                for (int i = 0; i < Block_Bytes; i++)
                {
                    TestArray[i] = 255;
                }
            }
            else if (mode == 2)
            {
                Random R = new Random();
                for (int i = 0; i < Block_Bytes; i++)
                {

                    TestArray[i] = (byte)R.Next(0, 255);
                }
            }

        }
        /// <summary>
        /// 验证数组中的数据
        /// </summary>
        /// <param name="testarray"></param>
        /// <param name="comparearray"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 获取long型随机数
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
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
