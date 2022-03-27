using DiskTestLib;
using System;
using System.Collections;
using System.IO;
using System.Management;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using Timer = System.Windows.Forms.Timer;

namespace DiskTest11
{
    public delegate void WriteBlockCompleteHandler(double speed);
    public delegate void AvgSpeedEventHandler(double speed);
    public delegate void NotifyEventHandler(int i, double speed, double written_MB, string now_time);//进度条
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
    public delegate void LogEventHandler(int grade,string s);//日志
    public delegate void StartTimeEventHandler(string s);//日志
    public delegate void TestEndEventHandler();
    public delegate void CircleNumHandler(int circlenum);
    public delegate void RandomTestTimeHandler(long time);
    public partial class DiskSetting : Sunny.UI.UIPage
    {
        /// <summary>
        /// 信号量
        /// </summary>
        public static Mutex mutex = new Mutex();
        public static Mutex RandomTestMutex = new Mutex();
        public static Mutex TimeMutex = new Mutex();
        public static Mutex speed_mutex = new Mutex();
        public static Mutex ErrorNumMutex = new Mutex();
        private Timer timer;
        private double MinSpeed;
        /// <summary>
        /// 计算百分比的信号量
        /// </summary>
        public static Mutex percent_mutex = new Mutex();
        /// <summary>
        /// 默认块大小
        /// </summary>
        private const int DEAFAUT_BLOCKSIZE = 512;
        /// <summary>
        /// 1MB的实际字节数
        /// </summary>
        private const int MB = 1048576;
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
        private const int MIDDLE_INR = 5;
        /// <summary>
        /// 大间隔块数用于计算读写速度
        /// </summary>
        private const int BIG_INR = 15;
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
        public TestEndEventHandler TestEndEvent;
        public CircleNumHandler CircleNumEvent;
        /// <summary>
        /// 传递平均速度的事件
        /// </summary>
        public AvgSpeedEventHandler AvgSpeedEvent;
        /// <summary>
        /// 当前点击的硬盘项的数字
        /// </summary>
        public int Now_Index_Framework = 0;
        /// <summary>
        /// 当前测试状态，是停止，还是继续；
        /// </summary>
        public bool Test_Suspend_Status;
        public bool Test_Stop_Status;
        /// <summary>
        /// 用于判断判断是否复现上一次测试的标志位
        /// </summary>
        private bool Repeat_Status;
        /// <summary>
        /// 用于测试的暂停和开始的变量
        /// </summary>
        private static AutoResetEvent resetEvent = new AutoResetEvent(true);
        private static int NORMAL = 1;
        private static int EXCEPTION = 2;
        private static int ERROR = 3;
        private AutoResetEvent[] resetEvents;
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
        /// 用于多线程计算占的总百分比
        /// </summary>
        private long TOTAL_TEST;
        /// <summary>
        /// 暂停间隔时间
        /// </summary>
        private long Gap_Time;
        private long Gap_Start_Time;
        private long Gap_End_Time;
        /// <summary>
        /// 用于统计多线程MB的总数
        /// </summary>
        private double TOTAL_MB;
        /// <summary>
        /// 由于是通过选择容量百分比来决定测试的空间，所以这个变量表示的就是顺序测试中你要测试到的扇区数
        /// </summary>
        private long Order_Max_Block;
        private int NOW_CIRCLE;
        
        /// <summary>
        /// 一个线程计算的扇区数
        /// </summary>
        private long GapSectorNumble;
        /// <summary>
        /// 显示硬盘信息的对象数组
        /// </summary>
        public Disk[] Ed;
        /// <summary>
        /// 存放测试类的动态数组
        /// </summary>
        public ArrayList Disk_Driver_List = new ArrayList();
        /// <summary>
        /// 存放测试选项的类的数组
        /// </summary>
        public ArrayList Disk_Choose_Information_List = new ArrayList();
        public ArrayList Disk_Information_List = new ArrayList();
        /// <summary>
        /// 存放每个硬盘是否要重复测试的状态信息
        /// </summary>
        private ArrayList Repeat_Test_Status_List = new ArrayList();
        /// <summary>
        /// 临时的choose变量，用于choose_information的赋值
        /// </summary>
        public ChooseInformation Temp_Choose = new ChooseInformation();
        public WriteBlockCompleteHandler GetWriteSpeed;
        /// <summary>
        /// 写入的数组
        /// </summary>
        private byte[] TestArray;
        /// <summary>
        /// 读取的数组
        /// </summary>
        private byte[] CompareArray;
        /// <summary>
        /// 用来统计累积平均速度
        /// </summary>
        private double NOW_SPEED;
        private long NOW_SPEED_NUM;
        private Stopwatch[] stopwatches;
        private Stopwatch Time_StopWatch;
        /// <summary>
        /// 计算读写速度的结构体
        /// </summary>
        struct SPEED_COMPUTE
        {
            /// <summary>
            /// 开始时间
            /// </summary>
            public long Start_Time;
            /// <summary>
            /// 结束时间
            /// </summary>
            public long End_Time;
            /// <summary>
            /// 单次时间
            /// </summary>
            public long Once_Time;
            /// <summary>
            /// 总的时间
            /// </summary>
            public long Total_Time;
            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="i"></param>
            public static implicit operator SPEED_COMPUTE(long i)
            {
                return new SPEED_COMPUTE() { Start_Time = 0, End_Time = 0, Once_Time = 0, Total_Time = i };
            }
        }
        struct AVG_SPEED_COMPUTE
        {
            public double SPEED;
            public int NUM;
            public static implicit operator AVG_SPEED_COMPUTE(int i)
            {
                return new AVG_SPEED_COMPUTE() { NUM = i, SPEED = 0 };
            }
        }
        private SPEED_COMPUTE Speed_Compute;
        private AVG_SPEED_COMPUTE Avg_Speed_Compute;
        private FileStream RecordStream;
        public DiskSetting()
        {
            InitializeComponent();
            Init_Disk_Information();
            Init_Disk_Framework();
            Init_Choose_And_Repeat_Status_ArrayList();
            Init_Disk_Setting_Param();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(134)));
            Show_Disk();
            for (int i = 0; i < Disk_Information_List.Count; i++)
            {
                Ed[i].TransfChooseINF += Get_Transf_Choose_INF_Event;
                Ed[i].TransfRepeatTest += Get_Repeat_Test_Status_Event;
            }
            /*if (Disk_Choose_Information_List.Count > 0 || Temp_Choose != null)
            {
                //Disk_Choose_Information_List[0] = Temp_Choose;
            }
            else
            {
                MessageBox.Show("测试项为空--Temp_Choose==null");
            }*/
        }
        /// <summary>
        /// 初始化需要用到的变量
        /// </summary>
        private void Init_Disk_Setting_Param()
        {
            Speed_Compute = 0;
            Avg_Speed_Compute = 0;
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
            GapSectorNumble = 0;
            Order_Max_Block = 0;
            NOW_SPEED = 0;
            TOTAL_TEST = 0;
            NOW_SPEED_NUM = 0;
            TOTAL_MB = 0;
            Gap_Time = 0;
            Gap_End_Time = 0;
            Gap_Start_Time = 0;
            NOW_CIRCLE = 0;
            timer = new Timer();
            timer.Interval = 100;
            stopwatches = new Stopwatch[1];
            stopwatches[0] = new Stopwatch();
            Time_StopWatch = new Stopwatch();
            resetEvents = new AutoResetEvent[1];
            resetEvents[0] = new AutoResetEvent(true);
            Test_Suspend_Status = true;
            Test_Stop_Status = true;
            RecordStream = null;
            MinSpeed = 0.5;
        }
        public void Init_Disk_Framework()
        {
            if (Disk_Information_List.Count > 0)
            {
                Ed = new Disk[Disk_Information_List.Count];
                for (int i = 0; i < Disk_Information_List.Count; i++)
                {
                    Ed[i] = new Disk();
                }
            }
        }
        public void Init_Choose_And_Repeat_Status_ArrayList()
        {
            Disk_Choose_Information_List.Clear();
            if (Disk_Information_List.Count > 0)
            {
                for (int i = 0; i < Disk_Information_List.Count; i++)
                {
                    ChooseInformation choose = new ChooseInformation();
                    Disk_Choose_Information_List.Add(choose);
                    bool repeat_status = false;
                    Repeat_Test_Status_List.Add(repeat_status);
                }
            }
        }
        public void Show_Disk()
        {
            if (Ed != null)
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
        public void AddAvgSpeedObserver(AvgSpeedEventHandler observer)
        {
            AvgSpeedEvent += observer;
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
        public void AddTestEndObserver(TestEndEventHandler observer)
        {
            TestEndEvent += observer;
        }       
        public void AddCircleNumObserver(CircleNumHandler observer)
        {
            CircleNumEvent += observer;
        }
        public void SendTestTime()
        {
            TestEndEvent();
        }
        public void SendTestTime_Thread(object obj)
        {
            long starttime;
            long endtime;
            starttime = Environment.TickCount;
            Console.WriteLine("开始传秒");
            while(true)
            {
                if (Percent >= 100)
                    break;
                endtime = Environment.TickCount;
                if((endtime-starttime)%1000==0)
                {
                    TestEndEvent();
                }
            }
        }
        /// <summary>
        /// 广播速度，已读写量等信息，事件的具体实现，将这个组件的信息传给所有的观察者，让观察者执行相应的函数
        /// </summary>
        /// <param name="i"></param>
        /// <param name="speed"></param>
        /// <param name="wirtten_MB"></param>
        /// <param name="now_time"></param>
        public void PublishNotify(int i, double speed, double wirtten_MB, string now_time)
        {
            if (NotifyEvent != null)
            {
                NotifyEvent(i, speed, wirtten_MB, now_time);
            }
        }
        /// <summary>
        /// 切换页面的函数，将需要切换的的页码i传递到Form
        /// </summary>
        /// <param name="i"></param>
        public void SwitchPage(int i)
        {
            if (SwitchEvent != null)
            {
                SwitchEvent(i);
            }
        }
        /// <summary>
        /// 打印测试信息
        /// </summary>
        /// <param name="s">需要打印的信息的字符串</param>
        public void PrintLog(int grade,string s)
        {
            if (LogEvent != null)
            {
                LogEvent(grade,s);
            }
        }
        /// <summary>
        /// 向另一个控件Test2传递开始时间
        /// </summary>
        /// <param name="s"></param>
        public void PublishStartTime(string s)
        {
            if (StartTimeEvent != null)
            {
                StartTimeEvent(s);
            }
        }
        /// <summary>
        /// 从DiskSetting向Test2控件传输进度条和累积测试时间
        /// </summary>
        /// <param name="i">进度条的百分比</param>
        /// <param name="now_time"></param>
        public void PublishProcessAndTime(int i, string now_time)
        {
            if (ProcessAndTimeEvent != null)
            {
                ProcessAndTimeEvent(i, now_time);
            }
        }
        /// <summary>
        /// 从DiskSetting向Test2控件传输测试速度和已写MB数
        /// </summary>
        /// <param name="speed">测试速度</param>
        /// <param name="written_MB">已写MB数</param>
        public void PublishWrittenAndSpeed(double speed, double written_MB)
        {
            if (WrittenAndSpeedEvent != null)
            {
                WrittenAndSpeedEvent(speed, written_MB);
            }
        }
        public void PublishAvgSpeed(double speed)
        {
            if (AvgSpeedEvent != null) AvgSpeedEvent(speed);
        }
        public void PublishCircleNum(int circlenum)
        {
            if (CircleNumEvent != null) CircleNumEvent(circlenum);
        }
        public void PublishTestEnd()
        {
            TestEndEvent();
        }
        /// <summary>
        /// 从Disk.cs中获取测试的选项信息
        /// </summary>
        /// <param name="value"></param>
        public void Get_Transf_Choose_INF_Event(ChooseInformation choose)
        {
            //Temp_Choose = choose;
            Disk_Choose_Information_List[Now_Index_Framework] = choose;
        }
        /// <summary>
        /// 获取Stop按钮的信息，并改变当前的Test_Status，判断是暂停还是恢复
        /// </summary>
        /// <param name="status">Stop按钮传过来的状态参数</param>
        public void Get_Suspend_Button_Status_Event(bool status)
        {
            if (Test_Suspend_Status == false && status == true)//当前是暂停，要恢复
            {
                Test_Suspend_Status = status;
                //resetEvent.Set();
                for(int i=0;i<resetEvents.Length;i++)
                {
                    resetEvents[i].Set();
                    
                }
                if (!Time_StopWatch.IsRunning)
                    Time_StopWatch.Start();
                Gap_End_Time = Environment.TickCount;
                Gap_Time += Gap_End_Time - Gap_Start_Time;
            }
            else
            {
                Test_Suspend_Status = status;
                Console.WriteLine("--------------暂停按钮已经按了");
            }
        }
        public void Get_Stop_Button_Status_Event(bool status)
        {
            //if(Test_Stop_Status==true&&status==false&& Test_Suspend_Status == false)//当前属于暂停
            //{
                Test_Stop_Status = status;
                for (int i = 0; i < resetEvents.Length; i++)
                {
                    resetEvents[i].Set();

                }
            Console.WriteLine("--------------停止按钮已经按了");
            //}
        }
        /// <summary>
        /// 从Disk中获取Repeat_Test的状态
        /// </summary>
        /// <param name="status"></param>
        public void Get_Repeat_Test_Status_Event(bool status)
        {
            Repeat_Test_Status_List[Now_Index_Framework] = status;
        }
        public ChooseInformation Init_Repeat_Test_INF()
        {
            ChooseInformation choose = new ChooseInformation();
            ReadRecord readRecord = new ReadRecord();
            int test_mode = readRecord.Read_Test_Mode();
            if (test_mode >= RANDOM_VERTIFY && test_mode <= RANDOM_WRITE)
            {
                int block_size = readRecord.Read_Block_Size();
                long test_time = readRecord.Read_Test_Time();
                long test_num = readRecord.Read_Test_Num();
                int thread_num = readRecord.Read_Thread();
                choose.SetRandomParameters(true, test_mode, test_time, test_num, block_size, true,thread_num);
            }
            else
            {
                int data_mode = readRecord.Read_Data_Mode();
                int block_size = readRecord.Read_Block_Size();
                int size = readRecord.Read_Size();
                int thread_num = readRecord.Read_Thread();
                int circle_num = readRecord.Read_Circle();
                choose.SetOrderParameters(true, test_mode, data_mode, size, block_size, 0, 0, circle_num, true,thread_num);
            }
            readRecord.Finish();
            return choose;
        }
        public void StartRandomTestTimeInvoke(long time)
        {
            RandomTestTimeHandler rt = new RandomTestTimeHandler(StartRandomTestTime);
            this.Invoke(rt, new object[] { time });
        }
        public void StartRandomTestTime(long time)
        {
            timer.Start();
            

        }
        /// <summary>
        /// 开始测试的函数
        /// </summary>
        /// <param name="obj"></param>
        private void Start_Test(object obj)
        {
            Time_StopWatch.Reset();
            NOW_CIRCLE = 0;
            Test_Suspend_Status = true;
            Test_Stop_Status = true;
            if (Disk_Choose_Information_List.Count <= 0)
            {
                MessageBox.Show("测试信息数组为空");
            }
            for (int i = 0; i < Disk_Choose_Information_List.Count; i++)
            {
                ChooseInformation chooseInformation;
                if ((bool)Repeat_Test_Status_List[i])
                {
                    Console.WriteLine("Repeat last test!");
                    chooseInformation = Init_Repeat_Test_INF();
                    Repeat_Status = true;
                }
                else
                {
                    Console.WriteLine("No Repeat last test!");
                    chooseInformation = (ChooseInformation)Disk_Choose_Information_List[i];
                    Repeat_Status = false;
                }
                chooseInformation.DriverIndex = i;

                if (chooseInformation.TestOrNot == true)
                {
                    DiskInformation info = (DiskInformation)Disk_Information_List[i];
                    ShowInfoTip(info.Physical_Name+ " 驱动器开始测试");
                    if (chooseInformation.TestMode == RANDOM_VERTIFY)
                    {
                        this.PublishStartTime(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                        MultiRandomWriteAndVerifyCT(chooseInformation);
                        //RandomWriteAndVerify(i, chooseInformation.TestNum, chooseInformation.TestTime, 2, chooseInformation.BlockSize);
                    }
                    //else if (chooseInformation.TestMode == RANDOM_VERTIFY && Repeat_Status == true)
                    //{
                    //    RandomWriteAndVerify_Repeat(i, chooseInformation.TestNum, chooseInformation.TestTime, 2, chooseInformation.BlockSize);
                    //}
                    else if (chooseInformation.TestMode == RANDOM_READ)//随机只读
                    {
                        this.PublishStartTime(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                        MultiRandomOnlyReadCT(chooseInformation);
                        //RandomOnlyRead(i, chooseInformation.TestNum, chooseInformation.TestTime, chooseInformation.BlockSize);
                    }
                    //else if (chooseInformation.TestMode == RANDOM_READ && Repeat_Status == true)//随机只读
                    //{
                     //   RandomOnlyRead_Repeat(i, chooseInformation.TestNum, chooseInformation.TestTime, chooseInformation.BlockSize);
                    //}
                    else if (chooseInformation.TestMode == RANDOM_WRITE )//随机只写
                    {
                        this.PublishStartTime(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                        MultiRandomOnlyWriteCT(chooseInformation);
                        //RandomOnlyWrite(i, chooseInformation.TestNum, chooseInformation.TestTime, 2, chooseInformation.BlockSize);
                    }
                    //else if (chooseInformation.TestMode == RANDOM_WRITE && Repeat_Status == true)//随机只写
                    //{
                     //   RandomOnlyWrite_Repeat(i, chooseInformation.TestNum, chooseInformation.TestTime, 2, chooseInformation.BlockSize);
                    //}
                    else if (chooseInformation.TestMode == ORDER_VERTIFY)//顺序读写验证
                    {
                        //OrderWriteAndVerify(chooseInformation);
                        this.PublishStartTime(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                        MultiOrderWriteAndVerifyCT(chooseInformation);
                        //OrderWriteAndVertify_one(i, chooseInformation.TestPercent, chooseInformation.TestDataMode, chooseInformation.BlockSize, chooseInformation.TestCircle);
                    }
                   // else if (chooseInformation.TestMode == ORDER_VERTIFY && Repeat_Status == true)//顺序读写验证
                    //{
                     //   OrderWriteAndVerify_Repeat(i, chooseInformation.TestPercent, chooseInformation.TestDataMode, chooseInformation.BlockSize, chooseInformation.TestCircle);
                   //     
                   // }
                    else if (chooseInformation.TestMode == ORDER_READ )//顺序只读
                    {
                        this.PublishStartTime(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                        //for(int j=0;j<chooseInformation.TestCircle;j++)
                        //{
                        //TestDelegate test = MultiOrderOnlyReadCT;
                        //IAsyncResult asyncResult = test.BeginInvoke(chooseInformation, null, null);
                        //test.EndInvoke(asyncResult);
                        MultiOrderOnlyReadCT(chooseInformation);                           
                            //ShowInfoTip("第" + j + 1 + "次循环开始");
                            
                       // }
                        //MultiOrderOnlyReadCT(chooseInformation);
                        //OrderOnlyRead(i, chooseInformation.TestPercent, chooseInformation.TestDataMode, chooseInformation.BlockSize, chooseInformation.TestCircle);
                    }
                    //else if (chooseInformation.TestMode == ORDER_READ && Repeat_Status == true)//顺序只读
                    //{
                    //    OrderOnlyRead_Repeat(i, chooseInformation.TestPercent, chooseInformation.TestDataMode, chooseInformation.BlockSize, chooseInformation.TestCircle);
                    //}
                    else if (chooseInformation.TestMode == ORDER_WRITE )//顺序只写
                    {
                        this.PublishStartTime(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                        //TestDelegate test = MultiOrderOnlyWriteCT;
                        //IAsyncResult asyncResult=test.BeginInvoke(chooseInformation,null,null);
                        //test.EndInvoke(asyncResult);
                        MultiOrderOnlyWriteCT(chooseInformation);
                        //OrderOnlyWrite(i, chooseInformation.TestPercent, chooseInformation.TestDataMode, chooseInformation.BlockSize, chooseInformation.TestCircle);
                    }
                    //else if (chooseInformation.TestMode == ORDER_WRITE && Repeat_Status == true)//顺序只写
                    //{
                    //    OrderOnlyWrite_Repeat(i, chooseInformation.TestPercent, chooseInformation.TestDataMode, chooseInformation.BlockSize, chooseInformation.TestCircle);
                    //}
                    else
                    {
                        MessageBox.Show("测试模式错误！");
                    }
                }
                else
                {
                    
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
            //Init_Choose_And_Repeat_Status_ArrayList();
            //GetWriteSpeed = OutWriteSpeed;
            this.SwitchPage(201);
            
            Thread thr = new Thread(new ParameterizedThreadStart(Start_Test));
            thr.Start();
            
            
            
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
            Disk_Information_List.Clear();
            this.Disk_Information_Framework.Rows.Clear();
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
        private int Compute_Last_Block_Size_multi(int block_size, long now_pos, long max_pos)
        {
            if (now_pos + block_size >= max_pos)
            {
                block_size = (int)(max_pos - now_pos);
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
        private void Compute_OnceBlockTime(DriverLoader driver, long pos, int block_size, int compute_mode)
        {
            Speed_Compute.Start_Time = Environment.TickCount;
            if (compute_mode == VERTIFY)
            {
                driver.WritSector(TestArray, pos, block_size);
                CompareArray = driver.ReadSector(pos, block_size);
            }
            else if (compute_mode == WRITE)
            {
                driver.WritSector(TestArray, pos, block_size);
            }
            else if (compute_mode == READ)
            {
                CompareArray = driver.ReadSector(pos, block_size);
            }
            Speed_Compute.End_Time = Environment.TickCount;
            Speed_Compute.Once_Time = Speed_Compute.End_Time - Speed_Compute.Start_Time;
            Speed_Compute.Total_Time += Speed_Compute.Once_Time;
        }
        private void Compute_OnceBlockTime_multi(DriverLoader driver, long pos, int block_size, int compute_mode,SPEED_COMPUTE speed_compute)
        {
            speed_compute.Start_Time = Environment.TickCount;
            if (compute_mode == VERTIFY)
            {
                driver.WritSector(TestArray, pos, block_size);
                CompareArray = driver.ReadSector(pos, block_size);
            }
            else if (compute_mode == WRITE)
            {
                driver.WritSector(TestArray, pos, block_size);
            }
            else if (compute_mode == READ)
            {
                CompareArray = driver.ReadSector(pos, block_size);
            }
            speed_compute.End_Time = Environment.TickCount;
            speed_compute.Once_Time = speed_compute.End_Time - speed_compute.Start_Time;
            speed_compute.Total_Time += speed_compute.Once_Time;
        }
        private byte[] Compute_OnceBlockTimemulti(DriverLoader driver, long pos, int block_size, int compute_mode,byte []testarray,byte []comparearray)
        {
            //Speed_Compute.Start_Time = Environment.TickCount;
            if (compute_mode == VERTIFY)
            {
                //Console.WriteLine("准备获取信号量");
                mutex.WaitOne();
                //Console.WriteLine("获取到了信号量");
                //Console.WriteLine("comparearray.length: "+comparearray.Length);
                try
                {
                    driver.WritSector(testarray, pos, block_size);
                    comparearray = driver.ReadSector(pos, block_size);
                }
                catch(IOException ex)
                {
                    this.PrintLog(ERROR, "\n出现异常：" + ex.Message+"\n");
                }
                
                mutex.ReleaseMutex();
                //Console.WriteLine("释放了了信号量");
            }
            else if (compute_mode == WRITE)
            {
                mutex.WaitOne();
                try
                {
                    driver.WritSector(testarray, pos, block_size);
                }
                catch (IOException ex)
                {
                    this.PrintLog(ERROR, "\n出现异常：" + ex.Message + "\n");
                }
                mutex.ReleaseMutex();
            }
            else if (compute_mode == READ)
            {
                mutex.WaitOne();
                try
                {
                    comparearray = driver.ReadSector(pos, block_size);
                }
                catch (IOException ex)
                {
                    this.PrintLog(ERROR, "\n出现异常：" + ex.Message + "\n");
                }
                mutex.ReleaseMutex();
            }
            else
            {
                Console.WriteLine("读写模式不对");
            }
            //Speed_Compute.End_Time = Environment.TickCount;
            //Speed_Compute.Once_Time = Speed_Compute.End_Time - Speed_Compute.Start_Time;
            //Console.WriteLine("once_time: " + Speed_Compute.Once_Time);
            //Speed_Compute.Total_Time += Speed_Compute.Once_Time;
            return comparearray;
        }
        /// <summary>
        /// 计算平均速度
        /// </summary>
        /// <returns>返回平均速度</returns>
        private double Compute_Avg_Speed()
        {
            return NOW_SPEED / NOW_SPEED_NUM;
        }
        /// <summary>
        /// 根据测试模式的不同，打印测试完成的信息
        /// </summary>
        /// <param name="Error_num">错误次数</param>
        /// <param name="Test_Mode">测试模式，有顺序读写验证，随机读写验证六种</param>
        /// <param name="time">测试时间，在随机测试中会使用到</param>
        /// <param name="num">测试次数，在随机测试中会使用到</param>
        /// <param name="max_sector">目标扇区块数</param>
        private void Complete_Information_Print(MultiThreadInfo info,int Error_num, int Test_Mode, long time = 0, long num = 0, long max_sector = 0)
        {
            
            if (Error_num == 0)
            {
                switch (Test_Mode)
                {
                    case RANDOM_VERTIFY:
                        Console.Write(DateTime.Now.ToString() + "随机读写验证测试结束，未发生错误！\n测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数) \n");
                        this.PrintLog(NORMAL,DateTime.Now.ToString()+ "随机读写验证测试完成，未发生错误！\n测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数) \n");
                        break;
                    case RANDOM_READ:
                        Console.WriteLine(DateTime.Now.ToString() + "随机读验证测试完成，未发生错误！\n测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数) \n");
                        this.PrintLog(NORMAL, DateTime.Now.ToString() + "随机读验证测试完成，未发生错误！\n测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数) \n");
                        break;
                    case RANDOM_WRITE:
                        Console.WriteLine(DateTime.Now.ToString() + "随机写验证测试完成，未发生错误！\n测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数) \n");
                        this.PrintLog(NORMAL, DateTime.Now.ToString() + "随机写验证测试完成，未发生错误！\n 测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数) \n");
                        break;
                    case ORDER_VERTIFY:
                        Console.WriteLine(DateTime.Now.ToString() + "顺序读写验证测试结束，未发生错误！\n测试容量占硬盘百分比为：" + info.chooseInformation.TestPercent + " 测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数)\n测试循环为：" + info.chooseInformation.TestCircle + " ");
                        this.PrintLog(NORMAL, DateTime.Now.ToString() + "顺序读写验证测试结束，未发生错误！\n测试容量占硬盘百分比为：" + info.chooseInformation.TestPercent + " 测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数)\n测试循环为：" + info.chooseInformation.TestCircle + " ");
                        break;
                    case ORDER_READ:
                        Console.WriteLine(DateTime.Now.ToString() + "顺序读测试结束，未发生错误！\n测试容量占硬盘百分比为：" + info.chooseInformation.TestPercent + " 测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数)\n测试循环为：" + info.chooseInformation.TestCircle + " ");
                        this.PrintLog(NORMAL, DateTime.Now.ToString() + "顺序读测试结束，未发生错误\n测试容量占硬盘百分比为：" + info.chooseInformation.TestPercent + " 测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数)\n测试循环为：" + info.chooseInformation.TestCircle + " ");
                        break;
                    case ORDER_WRITE:
                        Console.WriteLine(DateTime.Now.ToString() + "顺序写测试结束，未发生错误！\n测试容量占硬盘百分比为：" + info.chooseInformation.TestPercent+" 测试的数据模式为："+info.chooseInformation.TestDataMode+ " (0:全0,1:全1,2:随机数)\n测试循环为：" + info.chooseInformation.TestCircle+" ");
                        this.PrintLog(NORMAL, DateTime.Now.ToString() + "顺序写测试结束，未发生错误\n测试容量占硬盘百分比为：" + info.chooseInformation.TestPercent + " 测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数)\n测试循环为：" + info.chooseInformation.TestCircle + " ");
                        break;
                }
            }
            else
            {
                switch (Test_Mode)
                {
                    case RANDOM_VERTIFY:
                        Console.WriteLine(DateTime.Now.ToString() + "随机读写验证测试完成，出现了" + Error_num + "个错误！\n测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数) \n");
                        this.PrintLog(ERROR, DateTime.Now.ToString() + "随机读写验证测试完成，出现了" + Error_num + "个错误！\n测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数) \n");
                        break;
                    case RANDOM_READ:
                        Console.WriteLine(DateTime.Now.ToString() + "随机读测试完成，出现了" + Error_num + "个错误！\n测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数) \n");
                        this.PrintLog(ERROR, DateTime.Now.ToString() + "随机读测试完成，出现了" + Error_num + "个错误！\n测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数) \n");
                        break;
                    case RANDOM_WRITE:
                        Console.WriteLine(DateTime.Now.ToString() + "随机读写验证测试完成，出现了" + Error_num + "个错误！\n测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数) \n");
                        this.PrintLog(ERROR, DateTime.Now.ToString() + "随机读写验证测试完成，出现了" + Error_num + "个错误！\n测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数) \n");
                        break;
                    case ORDER_VERTIFY:
                        Console.WriteLine(DateTime.Now.ToString() + "顺序读写验证测试完成，出现了" + Error_num + "个错误！\n测试容量占硬盘百分比为：" + info.chooseInformation.TestPercent + " 测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数)\n测试循环为：" + info.chooseInformation.TestCircle + " ");
                        this.PrintLog(ERROR, DateTime.Now.ToString() + "随机读写验证测试完成，出现了" + Error_num + "个错误！测试容量占硬盘百分比为：" + info.chooseInformation.TestPercent + " 测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数)\n测试循环为：" + info.chooseInformation.TestCircle + " ");
                        break;
                    case ORDER_READ:
                        Console.WriteLine(DateTime.Now.ToString() + "顺序读测试完成，出现了" + Error_num + "个错误！\n测试容量占硬盘百分比为：" + info.chooseInformation.TestPercent + " 测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数)\n测试循环为：" + info.chooseInformation.TestCircle + " ");
                        this.PrintLog(ERROR, DateTime.Now.ToString() + "顺序读测试完成，出现了" + Error_num + "个错误！\n测试容量占硬盘百分比为：" + info.chooseInformation.TestPercent + " 测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数)\n测试循环为：" + info.chooseInformation.TestCircle + " ");
                        break;
                    case ORDER_WRITE:
                        Console.WriteLine(DateTime.Now.ToString() + "顺序写测试完成，出现了" + Error_num + "个错误！\n测试容量占硬盘百分比为：" + info.chooseInformation.TestPercent + " 测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数)\n测试循环为：" + info.chooseInformation.TestCircle + " ");
                        this.PrintLog(ERROR, DateTime.Now.ToString() + "顺序写验证测试完成，出现了" + Error_num + "个错误！\n测试容量占硬盘百分比为：" + info.chooseInformation.TestPercent + " 测试的数据模式为：" + info.chooseInformation.TestDataMode + " (0:全0,1:全1,2:随机数)\n测试循环为：" + info.chooseInformation.TestCircle + " ");
                        break;
                }
            }
            if (num != 0 && time == 0 && max_sector == 0)
            {
                Console.WriteLine(" 测试次数：" + num + "次");
                this.PrintLog(NORMAL," 测试次数：" + num + "次");
            }
            else if (num == 0 && time != 0 && max_sector == 0)
            {
                Console.WriteLine(" 测试时间：" + time + "ms ");
                this.PrintLog(NORMAL, " 测试时间：" + time + "ms ");
            }
            else
            {
                Console.WriteLine(" 测试到目标扇区：" + max_sector + "块 ");
                this.PrintLog(NORMAL, "测试到目标扇区：" + max_sector + "块 ");
            }
            this.PublishAvgSpeed(Compute_Avg_Speed());
            this.PrintLog(NORMAL, "平均速度是："+ String.Format("{0:F}", Compute_Avg_Speed())+"\n");
            Console.WriteLine("平均速度是：" + Compute_Avg_Speed() + "MB/s");

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
            if (Percent == 99)
            {
                Console.WriteLine("Percent is 99");
            }
            if (now_speed == 0)
            {
                Console.WriteLine("speed is 0!");
                return;
            }
            if (now_speed > 10000000000)
            {
                Console.WriteLine("speed is NaN!");
                return;
            }
            Speed_Compute.Total_Time = 0;
            this.PublishWrittenAndSpeed(now_speed, now_MB);
            Add_Speed_And_Num(now_speed);
            Once_Bytes = 0;
        }
        private double Compute_OnceBlockSpeed_multi(SPEED_COMPUTE speed_compte,long total_bytes,long once_bytes)
        {
            double now_MB = total_bytes / MB;
            double once_mb = (double)once_bytes / (double)MB;
            double now_speed = ((1000 * once_mb) / (speed_compte.Total_Time));//累计读写字节除以时间                                   
            if (Percent == 99)
            {
                Console.WriteLine("Percent is 99");
            }
            if (now_speed == 0)
            {
                Console.WriteLine("speed is 0!");
                return 0;
            }
            if (now_speed > 10000000000)
            {
                Console.WriteLine("speed is NaN!");
                return 0;
            }
            //Speed_Compute.Total_Time = 0;//注意待会加一行清0
            
            speed_mutex.WaitOne();           
            NOW_SPEED += now_speed;
            NOW_SPEED_NUM++;
            TOTAL_MB += once_mb;
            speed_mutex.ReleaseMutex();
            Console.WriteLine("NOW_SPEED: " + NOW_SPEED + " NOW_SPEED_NUM: " + NOW_SPEED_NUM + " now_speed: " + now_speed+" oncebytes: "+once_bytes+" time: "+speed_compte.Total_Time+" ms");
            Add_Speed_And_Num(now_speed);
            return now_speed;
        }
        private void Add_Speed_And_Num(double speed)
        {
            Avg_Speed_Compute.SPEED += speed;
            Avg_Speed_Compute.NUM++;
        }
        /// <summary>
        /// 初始化测试参数，包括Test_Error_Num ，Test_Start_Time ，Temp_Nums，Total_Bytes，Once_Bytes，Percent，Block_Bytes
        /// </summary>
        /// <param name="block_size"></param>
        private void Init_Test_Param(int block_size)
        {
            Speed_Compute = 0;
            Avg_Speed_Compute = 0;
            Test_Error_Num = 0;
            Test_Start_Time = Environment.TickCount;
            Temp_Nums = 1;
            Total_Bytes = 0;
            Once_Bytes = 0;
            //Percent = 0;
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
        /// 初始化写入数组
        /// </summary>
        /// <param name="block_size"></param>
        /// <param name="mode"></param>
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
        public byte[] Init_TestArraymulti(int block_size, int mode,byte[] testarray)
        {
            if (block_size == 0)
            {
                MessageBox.Show("块大小不能为0");
                return null;
            }
            testarray = new byte[block_size];
            if (mode == 0)
            {
                for (int i = 0; i < block_size; i++)
                {
                    testarray[i] = 0;
                }
                return testarray;
            }
            else if (mode == 1)
            {
                for (int i = 0; i < block_size; i++)
                {
                    testarray[i] = 255;
                }
                return testarray;
            }
            else if (mode == 2)
            {
                Random R = new Random();
                for (int i = 0; i < block_size; i++)
                {
                    testarray[i] = (byte)R.Next(0, 255);
                }
                return testarray;
            }
            else
            {
                return null;
            }

        }
        /// <summary>
        /// 验证数组中的数据
        /// </summary>
        /// <param name="testarray"></param>
        /// <param name="comparearray"></param>
        /// <returns></returns>
        /// 
        public int VerifyArray(byte[] testarray, byte[] comparearray)
        {
            int error_num = 0;
            if (testarray.Length != comparearray.Length)
            {
                Console.WriteLine("数组长度不匹配！testarray.length = "+testarray.Length+" comparearray.length = "+comparearray.Length);
                return ++error_num; 
            }           
            for (int i = 0; i < testarray.Length; i++)
            {
                if (testarray[i] != comparearray[i])
                {
                    Console.WriteLine("testarray.length = " + testarray.Length + " comparearray.length = " + comparearray.Length);
                    Console.WriteLine(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "当前位置" + i + "出错，正确数据为" + testarray[i] + "错误数据为：" + comparearray[i]);
                    this.PrintLog(ERROR,DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "当前位置" + i + "出错，正确数据为" + testarray[i] + "错误数据为：" + comparearray[i]+"\n");
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
                Now_Index_Framework = e.RowIndex;
            }
            catch (Exception ee)
            {
                MessageBox.Show("找不到硬盘");
            }
        }
        /// <summary>
        /// 初始化测试的开始时间
        /// </summary>
        private void Init_Test_Label()
        {
            this.PublishNotify(0, 0, 0, DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            this.PublishAvgSpeed(0);
        }
        public void MultiOrderWriteAndVerifyCT(ChooseInformation info)
        {
            PublishCircleNum(NOW_CIRCLE + 1);
            if (!Repeat_Status&&info.Record)
            {
                RecordTest recordTest = new RecordTest();
                recordTest.Order_Record_Mode(ORDER_VERTIFY, info.TestDataMode, info.BlockSize, info.TestPercent, info.ThreadNum,info.TestCircle);
                recordTest.Finish();
            }
            Thread[] threads = new Thread[info.ThreadNum];
            DriverLoader driver = (DriverLoader)Disk_Driver_List[info.DriverIndex];
            stopwatches = new Stopwatch[info.ThreadNum];
            resetEvents = new AutoResetEvent[info.ThreadNum];
            Percent = 0;
            TOTAL_TEST = 0;
            if (NOW_CIRCLE == 0)
                TOTAL_MB = 0;
            NOW_SPEED = 0;
            NOW_SPEED_NUM = 0;

            for (int i=0;i< info.ThreadNum; i++)
            {
                threads[i] = new Thread(MultiOrderWriteAndVerify);
                threads[i].Start(new MultiThreadInfo(driver,info, i));
                resetEvents[i] = new AutoResetEvent(true);
            }
            for (int i = 0; i < info.ThreadNum; i++)
            {
                threads[i].Join();
            }
            Console.WriteLine("---------------wohoo,all is end!");
            NOW_CIRCLE++;
            if (NOW_CIRCLE < info.TestCircle)
                MultiOrderWriteAndVerifyCT(info);
            else
                PublishTestEnd();
        }
        /// <summary>
        /// 顺序读写验证,已修改测速模式，已验证速度无误
        /// </summary>
        /// <param name="driver_index">读写流的编号</param>
        /// <param name="percent">测试所占容量的百分比</param>
        /// <param name="test_data_mode">测试数据模式</param>
        /// <param name="block_size">块大小</param>
        /// <param name="circle">测试循环</param>
        /// <summary>
        /// 初始化测试数组
        /// </summary>
        /// <param name="block_size">测试块大小</param>
        /// <param name="mode">测试模式</param>
        public void MultiOrderWriteAndVerify(object o)//int driver_index, int percent_of_all_size = 100, int data_mode = 0, int block_size = 0, int circle = 1, int thread_num=1
        {
            MultiThreadInfo info = (MultiThreadInfo)o;
            Test_Driver_List();
            Init_Test_Label();
            int BLOCK_SIZE = info.chooseInformation.BlockSize;
            long total_sector = info.chooseInformation.TestPercent * info.driverLoader.DiskInformation.DiskSectorSize / 100;
            GapSectorNumble = info.chooseInformation.TestPercent * info.driverLoader.DiskInformation.DiskSectorSize / 100 / info.chooseInformation.ThreadNum;
            long now_pos = info.threadIndex * GapSectorNumble;
            long max_pos;
            if (total_sector % info.chooseInformation.ThreadNum != 0 && info.threadIndex == (info.chooseInformation.ThreadNum - 1))
            {
                max_pos = now_pos + GapSectorNumble + (total_sector % info.chooseInformation.ThreadNum);
            }
            else
                max_pos = now_pos + GapSectorNumble;
            Console.WriteLine("第 " + info.threadIndex + " 个线程的初始位置：" + now_pos+" 的最后位置是："+max_pos);            
            Init_Test_Param(BLOCK_SIZE);
            int BYTE_SIZE = BLOCK_SIZE * DEAFAUT_BLOCKSIZE;            
            Fast_INR = (BLOCK_SIZE > 512) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (BLOCK_SIZE > 512) ? SMALL_INR : BIG_INR;
            long total_bytes = 0;
            long once_bytes = 0;
            SPEED_COMPUTE speed_compute=0;
            TOTAL_TEST = 0;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Suspend_Status == false)
                {
                    //resetEvent.WaitOne();
                    resetEvents[info.threadIndex].WaitOne();

                }
                if (Test_Stop_Status == false)
                {
                    break;
                }
                BLOCK_SIZE =  Compute_Last_Block_Size_multi(BLOCK_SIZE, now_pos,max_pos);
                BYTE_SIZE= BLOCK_SIZE * DEAFAUT_BLOCKSIZE;
                byte[] comparearray = new byte[BYTE_SIZE];
                byte[] testarray = new byte[BYTE_SIZE];
                testarray=Init_TestArraymulti(BYTE_SIZE, info.chooseInformation.TestDataMode,testarray);
                //Console.WriteLine("线程 "+info.threadIndex+" 准备获取信号量---------------");
                speed_compute.Start_Time = Environment.TickCount;
                comparearray =Compute_OnceBlockTimemulti(info.driverLoader, now_pos, BLOCK_SIZE, VERTIFY,testarray,comparearray);
                speed_compute.End_Time = Environment.TickCount;
                speed_compute.Once_Time = speed_compute.End_Time - speed_compute.Start_Time;
                speed_compute.Total_Time += speed_compute.Once_Time;
                ErrorNumMutex.WaitOne();
                Test_Error_Num += VerifyArray(testarray, comparearray);
                ErrorNumMutex.ReleaseMutex();
                Test_End_Time = Environment.TickCount;
                Add_Bytes();
                total_bytes += BYTE_SIZE;
                once_bytes += BYTE_SIZE;
                //Console.WriteLine(now_pos+" 第 "+info.threadIndex+" 个线程的位置");
                now_pos += BLOCK_SIZE;
                percent_mutex.WaitOne();
                TOTAL_TEST += BLOCK_SIZE;
                percent_mutex.ReleaseMutex();
                Percent = (int)(TOTAL_TEST * 100 / total_sector);
                if (Temp_Nums % Fast_INR == 0 || now_pos >= max_pos)
                {
                    this.PublishProcessAndTime((int)(Percent), DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    if (Temp_Nums % Slow_INR == 0 || now_pos >= max_pos)
                    {
                        Console.Write("Threadid is : " + info.threadIndex+" ");
                        double speed = Compute_OnceBlockSpeed_multi(speed_compute, total_bytes, once_bytes);
                        this.PublishWrittenAndSpeed(speed, TOTAL_MB);
                        if (speed <= MinSpeed)
                            this.PrintLog(EXCEPTION, "Speed below minimum,speed is " + speed+"\n");
                        once_bytes = 0;
                        speed_compute.Total_Time = 0;
                    }
                }
                if (now_pos >= max_pos) break;
                else
                {
                    Temp_Nums++;
                    continue;
                }
            }
            Console.WriteLine("当前是第 " + info.threadIndex + " 个线程完成测试！");
            this.PrintLog(NORMAL, "当前驱动器：" + info.driverLoader.DiskInformation.Physical_Name + " 的第 " + info.threadIndex + " 个线程完成测试！\n");
            Complete_Information_Print(info,Test_Error_Num, ORDER_VERTIFY, 0, 0, max_pos);           
        }
        public void OrderWriteAndVertify(int driver_index, int percent_of_all_size = 100, int data_mode = 0, int block_size = 1, int circle = 1, int thread_num = 1)
        {
            RecordTest recordTest = new RecordTest();
            recordTest.Order_Record_Mode(ORDER_VERTIFY, data_mode, block_size, percent_of_all_size, thread_num,circle);
            recordTest.Finish();
            Test_Driver_List();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            Init_Test_Label();
            Order_Max_Block = percent_of_all_size * driver.DiskInformation.DiskSectorSize / 100;
            Now_Pos = 0;
            Init_Test_Param(block_size);
            CompareArray = new byte[Block_Bytes];
            Fast_INR = (block_size >= 256) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (block_size >= 256) ? MIDDLE_INR : BIG_INR;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Suspend_Status == false) resetEvent.WaitOne();
                block_size = Compute_Last_Block_Size(block_size);
                Init_TestArray(block_size, data_mode);
                Compute_OnceBlockTime(driver, Now_Pos, block_size, VERTIFY);
                Test_End_Time = Environment.TickCount;
                Add_Bytes();
                Now_Pos += block_size;
                Test_Error_Num += VerifyArray(TestArray, CompareArray);
                Percent = (int)(Now_Pos * 100 / Order_Max_Block);
                if (Temp_Nums % Fast_INR == 0 || Percent >= 100)
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
            //Complete_Information_Print(Test_Error_Num, ORDER_VERTIFY, 0, 0, Order_Max_Block);
        }
        /// <summary>
        /// 顺序读写验证复现版，无需记录本次测试过程
        /// </summary>
        /// <param name="driver_index"></param>
        /// <param name="percent_of_all_size"></param>
        /// <param name="data_mode"></param>
        /// <param name="block_size"></param>
        /// <param name="circle"></param>
        public void OrderWriteAndVerify_Repeat(int driver_index, int percent_of_all_size = 100, int data_mode = 0, int block_size = 0, int circle = 1)
        {
            Test_Driver_List();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            Init_Test_Label();
            Order_Max_Block = percent_of_all_size * driver.DiskInformation.DiskSectorSize / 100;
            Now_Pos = 0;
            Init_Test_Param(block_size);
            CompareArray = new byte[Block_Bytes];
            Fast_INR = (block_size >= 256) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (block_size >= 256) ? MIDDLE_INR : BIG_INR;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Suspend_Status == false) resetEvent.WaitOne();
                block_size = Compute_Last_Block_Size(block_size);
                Init_TestArray(block_size, data_mode);
                Compute_OnceBlockTime(driver, Now_Pos, block_size, VERTIFY);
                Test_Error_Num += VerifyArray(TestArray, CompareArray);
                Test_End_Time = Environment.TickCount;
                Add_Bytes();
                Console.WriteLine(Now_Pos);
                Now_Pos += block_size;
                Percent = (int)(Now_Pos * 100 / Order_Max_Block);
                if (Temp_Nums % Fast_INR == 0 || Percent >= 100)
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
            //Complete_Information_Print(Test_Error_Num, ORDER_VERTIFY, 0, 0, Order_Max_Block);
        }
        public void MultiOrderOnlyWriteCT(ChooseInformation info)
        {
            PublishCircleNum(NOW_CIRCLE + 1);
            if (!Repeat_Status && info.Record)
            {
                RecordTest recordTest = new RecordTest();
                recordTest.Order_Record_Mode(ORDER_WRITE, info.TestDataMode, info.BlockSize, info.TestPercent, info.ThreadNum,info.TestCircle);
                recordTest.Finish();
            }
            Thread[] threads = new Thread[info.ThreadNum];
            DriverLoader driver = (DriverLoader)Disk_Driver_List[info.DriverIndex];
            resetEvents = new AutoResetEvent[info.ThreadNum];
            Percent = 0;
            TOTAL_TEST = 0;
            if(NOW_CIRCLE==0)
                TOTAL_MB = 0;
            NOW_SPEED = 0;
            NOW_SPEED_NUM = 0;
            for (int i = 0; i < info.ThreadNum; i++)
            {
                threads[i] = new Thread(MultiOrderOnlyWrite);
                threads[i].Start(new MultiThreadInfo(driver, info, i));
                resetEvents[i] = new AutoResetEvent(true);
            }
            for (int i = 0; i < info.ThreadNum; i++)
            {
                threads[i].Join();
            }
            Console.WriteLine("---------------wohoo,all is end!");
            NOW_CIRCLE++;
            if (NOW_CIRCLE < info.TestCircle)
                MultiOrderOnlyWriteCT(info);
            else
                PublishTestEnd();
                //Console.WriteLine("---------------Test is final end!!!----------------");
        }
        public void MultiOrderOnlyWrite(object o)//int driver_index, int percent_of_all_size = 100, int data_mode = 0, int block_size = 0, int circle = 1, int thread_num=1
        {
            MultiThreadInfo info = (MultiThreadInfo)o;
            Test_Driver_List();
            Init_Test_Label();
            int BLOCK_SIZE = info.chooseInformation.BlockSize;
            long total_sector = info.chooseInformation.TestPercent * info.driverLoader.DiskInformation.DiskSectorSize / 100;
            GapSectorNumble = info.chooseInformation.TestPercent * info.driverLoader.DiskInformation.DiskSectorSize / 100 / info.chooseInformation.ThreadNum;
            long now_pos = info.threadIndex * GapSectorNumble;
            long max_pos;
            if (total_sector%info.chooseInformation.ThreadNum!=0&&info.threadIndex== (info.chooseInformation.ThreadNum-1))
            {
                max_pos = now_pos + GapSectorNumble+(total_sector % info.chooseInformation.ThreadNum);
            }
            else
                max_pos = now_pos + GapSectorNumble;            
            Console.WriteLine("第 " + info.threadIndex + " 个线程的初始位置：" + now_pos + " 的最后位置是：" + max_pos);
            Init_Test_Param(BLOCK_SIZE);
            int BYTE_SIZE = BLOCK_SIZE * DEAFAUT_BLOCKSIZE;
            Fast_INR = (BLOCK_SIZE > 512) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (BLOCK_SIZE > 512) ? SMALL_INR : BIG_INR;
            long total_bytes = 0;
            long once_bytes = 0;
            SPEED_COMPUTE speed_compute = 0;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Suspend_Status == false)
                {
                    //resetEvent.WaitOne();
                    resetEvents[info.threadIndex].WaitOne();
                }
                if (Test_Stop_Status == false)
                {
                    break;
                }
                BLOCK_SIZE = Compute_Last_Block_Size_multi(BLOCK_SIZE, now_pos, max_pos);
                BYTE_SIZE = BLOCK_SIZE * DEAFAUT_BLOCKSIZE;
                byte[] comparearray = new byte[BYTE_SIZE];
                byte[] testarray = new byte[BYTE_SIZE];
                testarray = Init_TestArraymulti(BYTE_SIZE, info.chooseInformation.TestDataMode, testarray);
                //Console.WriteLine("线程 "+info.threadIndex+" 准备获取信号量---------------");
                speed_compute.Start_Time = Environment.TickCount;
                Compute_OnceBlockTimemulti(info.driverLoader, now_pos, BLOCK_SIZE, WRITE, testarray, comparearray);
                speed_compute.End_Time = Environment.TickCount;
                speed_compute.Once_Time = speed_compute.End_Time - speed_compute.Start_Time;
                speed_compute.Total_Time += speed_compute.Once_Time;
                Test_End_Time = Environment.TickCount;
                Add_Bytes();
                total_bytes += BYTE_SIZE;
                once_bytes += BYTE_SIZE;
                //Console.WriteLine(now_pos+" 第 "+info.threadIndex+" 个线程的位置");
                now_pos += BLOCK_SIZE;
                percent_mutex.WaitOne();
                TOTAL_TEST += BLOCK_SIZE;
                percent_mutex.ReleaseMutex();
                Percent = (int)(TOTAL_TEST * 100 / total_sector);               
                if (Percent == 99)
                    Console.WriteLine("TOTAL_TEST is "+TOTAL_TEST+" total_sector is "+total_sector);
                if (Temp_Nums % Fast_INR == 0 || now_pos >= max_pos)
                {
                    this.PublishProcessAndTime((int)(Percent), DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    if (Temp_Nums % Slow_INR == 0 || now_pos >= max_pos)
                    {
                        Console.Write("Threadid is : " + info.threadIndex + " now_pos:"+now_pos);
                        double speed=Compute_OnceBlockSpeed_multi(speed_compute, total_bytes, once_bytes);
                        this.PublishWrittenAndSpeed(speed, TOTAL_MB);
                        if (speed <= MinSpeed)
                            this.PrintLog(EXCEPTION, "Speed below minimum,speed is " + speed+"\n");
                        once_bytes = 0;
                        speed_compute.Total_Time = 0;
                    }
                }
                if (now_pos >= max_pos) break;
                else
                {
                    Temp_Nums++;
                    continue;
                }
            }
            Console.WriteLine("当前是第 " + info.threadIndex + " 个线程完成测试！");
            this.PrintLog(NORMAL, "当前驱动器：" + info.driverLoader.DiskInformation.Physical_Name + " 的第 " + info.threadIndex + " 个线程完成测试！\n");
            Complete_Information_Print(info,Test_Error_Num, ORDER_WRITE, 0, 0, max_pos);
        }
        /// <summary>
        /// 顺序只写,已修改测速模式
        /// </summary>
        /// <param name="driver_index">读写流的编号</param>
        /// <param name="percent">测试所占容量的百分比</param>
        /// <param name="test_data_mode">测试数据模式</param>
        /// <param name="block_size">块大小</param>
        /// <param name="circle">测试循环</param>
        public void OrderOnlyWrite(int driver_index, int percent_of_all_size = 100, int data_mode = 0, int block_size = 1, int circle = 1,int thread_num=1)
        {
            RecordTest recordTest = new RecordTest();
            recordTest.Order_Record_Mode(ORDER_WRITE, data_mode, block_size, percent_of_all_size,thread_num,circle);
            recordTest.Finish();
            Test_Driver_List();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            Init_Test_Label();
            Order_Max_Block = percent_of_all_size * driver.DiskInformation.DiskSectorSize / 100;
            Now_Pos = 0;
            Init_Test_Param(block_size);
            CompareArray = new byte[Block_Bytes];
            Fast_INR = (block_size >= 256) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (block_size >= 256) ? MIDDLE_INR : BIG_INR;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Suspend_Status == false) resetEvent.WaitOne();
                block_size = Compute_Last_Block_Size(block_size);
                Init_TestArray(block_size, data_mode);
                Compute_OnceBlockTime(driver, Now_Pos, block_size, WRITE);
                Test_End_Time = Environment.TickCount;
                Add_Bytes();
                Now_Pos += block_size;
                Percent = (int)(Now_Pos * 100 / Order_Max_Block);
                if (Temp_Nums % Fast_INR == 0 || Percent >= 100)
                {
                    this.PublishProcessAndTime((int)(Percent), DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    if (Temp_Nums % Slow_INR == 0 || Percent >= 100) 
                        Compute_OnceBlockSpeed();
                }
                if (Now_Pos >= Order_Max_Block) break;
                else
                {
                    Temp_Nums++;
                    continue;
                }
            }
            //Complete_Information_Print(Test_Error_Num, ORDER_WRITE, 0, 0, Order_Max_Block);
        }
        /// <summary>
        /// 顺序只写复现版，无需记录本次测试过程
        /// </summary>
        /// <param name="driver_index"></param>
        /// <param name="percent_of_all_size"></param>
        /// <param name="test_mode"></param>
        /// <param name="block_size"></param>
        /// <param name="circle"></param>
        public void OrderOnlyWrite_Repeat(int driver_index, int percent_of_all_size = 100, int test_mode = 0, int block_size = 1, int circle = 1)
        {
            Test_Driver_List();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            Init_Test_Label();
            Order_Max_Block = percent_of_all_size * driver.DiskInformation.DiskSectorSize / 100;
            Now_Pos = 0;
            Init_Test_Param(block_size);
            CompareArray = new byte[Block_Bytes];
            Fast_INR = (block_size >= 256) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (block_size >= 256) ? MIDDLE_INR : BIG_INR;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Suspend_Status == false) resetEvent.WaitOne();
                block_size = Compute_Last_Block_Size(block_size);
                Init_TestArray(block_size, test_mode);
                Compute_OnceBlockTime(driver, Now_Pos, block_size, WRITE);
                Test_End_Time = Environment.TickCount;
                Add_Bytes();
                Now_Pos += block_size;
                Percent = (int)(Now_Pos * 100 / Order_Max_Block);
                if (Temp_Nums % Fast_INR == 0 || Percent >= 100)
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
            //Complete_Information_Print(Test_Error_Num, ORDER_WRITE, 0, 0, Order_Max_Block);
        }
        private delegate void TestDelegate(ChooseInformation choose);
        public void MultiOrderOnlyReadCT(ChooseInformation info)
        {
            PublishCircleNum(NOW_CIRCLE+1);
            if (!Repeat_Status && info.Record)
            {
                RecordTest recordTest = new RecordTest();
                recordTest.Order_Record_Mode(ORDER_READ, info.TestDataMode, info.BlockSize, info.TestPercent, info.ThreadNum,info.TestCircle);
                recordTest.Finish();
            }
            Thread[] threads = new Thread[info.ThreadNum];
            DriverLoader driver = (DriverLoader)Disk_Driver_List[info.DriverIndex];
            stopwatches = new Stopwatch[info.ThreadNum];
            resetEvents = new AutoResetEvent[info.ThreadNum];
            Percent = 0;
            TOTAL_TEST = 0;
            if (NOW_CIRCLE == 0)
                TOTAL_MB = 0;
            NOW_SPEED = 0;
            NOW_SPEED_NUM = 0;
            
            for (int i = 0; i < info.ThreadNum; i++)
            {
                threads[i] = new Thread(MultiOrderOnlyRead);
                threads[i].Start(new MultiThreadInfo(driver, info, i));
                
                resetEvents[i] = new AutoResetEvent(true);
                stopwatches[i] = new Stopwatch();
            }
            for (int i=0;i<info.ThreadNum;i++)
            {
                threads[i].Join();
            }
            Console.WriteLine("---------------wohoo,all is end!");
            NOW_CIRCLE++;
            if (NOW_CIRCLE < info.TestCircle)
                MultiOrderOnlyReadCT(info);
            else
                PublishTestEnd();
        }
        public void MultiOrderOnlyRead(object o)
        {
            MultiThreadInfo info = (MultiThreadInfo)o;
            Test_Driver_List();
            Init_Test_Label();
            int BLOCK_SIZE = info.chooseInformation.BlockSize;
            long total_sector = info.chooseInformation.TestPercent * info.driverLoader.DiskInformation.DiskSectorSize / 100;
            GapSectorNumble = info.chooseInformation.TestPercent * info.driverLoader.DiskInformation.DiskSectorSize / 100 / info.chooseInformation.ThreadNum;
            long now_pos = info.threadIndex * GapSectorNumble;
            long max_pos;
            if (total_sector % info.chooseInformation.ThreadNum != 0 && info.threadIndex == (info.chooseInformation.ThreadNum - 1))
            {
                max_pos = now_pos + GapSectorNumble + (total_sector % info.chooseInformation.ThreadNum);
            }
            else
                max_pos = now_pos + GapSectorNumble;
            Console.WriteLine("第 " + info.threadIndex + " 个线程的初始位置：" + now_pos + " 的最后位置是：" + max_pos);
            Init_Test_Param(BLOCK_SIZE);
            int BYTE_SIZE = BLOCK_SIZE * DEAFAUT_BLOCKSIZE;
            Fast_INR = (BLOCK_SIZE >= 256) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (BLOCK_SIZE >= 256) ? MIDDLE_INR : BIG_INR;
            long total_bytes = 0;
            long once_bytes = 0;
            SPEED_COMPUTE speed_compute = 0;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Suspend_Status == false)
                {
                    //resetEvent.WaitOne();
                    resetEvents[info.threadIndex].WaitOne();                    
                }
                if (Test_Stop_Status == false)
                {
                    break;
                }                    
                BLOCK_SIZE = Compute_Last_Block_Size_multi(BLOCK_SIZE, now_pos, max_pos);
                BYTE_SIZE = BLOCK_SIZE * DEAFAUT_BLOCKSIZE;
                byte[] comparearray = new byte[BYTE_SIZE];
                byte[] testarray = new byte[BYTE_SIZE];
                testarray = Init_TestArraymulti(BYTE_SIZE, info.chooseInformation.TestDataMode, testarray);
                //Console.WriteLine("线程 "+info.threadIndex+" 准备获取信号量---------------");
                speed_compute.Start_Time = Environment.TickCount;
                comparearray=Compute_OnceBlockTimemulti(info.driverLoader, now_pos, BLOCK_SIZE, READ, testarray, comparearray);
                speed_compute.End_Time = Environment.TickCount;
                speed_compute.Once_Time = speed_compute.End_Time - speed_compute.Start_Time;
                speed_compute.Total_Time += speed_compute.Once_Time;
                Test_End_Time = Environment.TickCount;
                Add_Bytes();
                total_bytes += BYTE_SIZE;
                once_bytes += BYTE_SIZE;
                //Console.WriteLine(now_pos+" 第 "+info.threadIndex+" 个线程的位置");
                now_pos += BLOCK_SIZE;
                percent_mutex.WaitOne();
                TOTAL_TEST += BLOCK_SIZE;
                percent_mutex.ReleaseMutex();
                Percent = (int)(TOTAL_TEST * 100 / total_sector);
                if (Temp_Nums % Fast_INR == 0 || now_pos >= max_pos)
                {
                    this.PublishProcessAndTime((int)(Percent), DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    if (Temp_Nums % Slow_INR == 0 || now_pos >= max_pos)
                    {
                        Console.Write("Threadid is : " + info.threadIndex + " now_pos: "+now_pos);
                        double speed = Compute_OnceBlockSpeed_multi(speed_compute, total_bytes, once_bytes);
                        this.PublishWrittenAndSpeed(speed, TOTAL_MB);
                        if (speed <= MinSpeed)
                            this.PrintLog(EXCEPTION, "Speed below minimum,speed is " + speed+"\n");
                        once_bytes = 0;
                        speed_compute.Total_Time = 0;
                    }
                }
                if (now_pos >= max_pos) break;
                else
                {
                    Temp_Nums++;
                    continue;
                }
            }
            Console.Write("当前是第 " + info.threadIndex + " 个线程完成测试！  ");
            this.PrintLog(NORMAL, "当前驱动器：" + info.driverLoader.DiskInformation.Physical_Name + " 的第 " + info.threadIndex + " 个线程完成测试！\n");
            Complete_Information_Print(info,Test_Error_Num, ORDER_READ, 0, 0, now_pos);
        }
        /// <summary>
        /// 顺序只读,已修改测速模式
        /// </summary>
        /// <param name="driver_index">读写流的编号</param>
        /// <param name="percent">测试所占容量的百分比</param>
        /// <param name="test_data_mode">测试数据模式</param>
        /// <param name="block_size">块大小</param>
        /// <param name="circle">测试循环</param>
        public void OrderOnlyRead(int driver_index, int percent_of_all_size = 100, int data_mode = 0, int block_size = 1, int circle = 1,int thread_num=1)
        {
            RecordTest recordTest = new RecordTest();
            recordTest.Order_Record_Mode(ORDER_READ, data_mode, block_size, percent_of_all_size,thread_num,circle);
            recordTest.Finish();
            Test_Driver_List();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            Init_Test_Label();
            Order_Max_Block = percent_of_all_size * driver.DiskInformation.DiskSectorSize / 100;
            Now_Pos = 0;
            Init_Test_Param(block_size);
            CompareArray = new byte[Block_Bytes];
            Fast_INR = (block_size >= 256) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (block_size >= 256) ? MIDDLE_INR : BIG_INR;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Suspend_Status == false) resetEvent.WaitOne();
                block_size = Compute_Last_Block_Size(block_size);
                Init_TestArray(block_size, data_mode);
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
            //Complete_Information_Print(Test_Error_Num, ORDER_READ, 0, 0, Order_Max_Block);
        }
        /// <summary>
        /// 顺序只读复现版，无需记录本次测试过程
        /// </summary>
        /// <param name="driver_index"></param>
        /// <param name="percent_of_all_size"></param>
        /// <param name="data_mode"></param>
        /// <param name="block_size"></param>
        /// <param name="circle"></param>
        public void OrderOnlyRead_Repeat(int driver_index, int percent_of_all_size = 100, int data_mode = 0, int block_size = 1, int circle = 1)
        {
            Test_Driver_List();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            Init_Test_Label();
            Order_Max_Block = percent_of_all_size * driver.DiskInformation.DiskSectorSize / 100;
            Now_Pos = 0;
            Init_Test_Param(block_size);
            CompareArray = new byte[Block_Bytes];
            Fast_INR = (block_size >= 256) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (block_size >= 256) ? MIDDLE_INR : BIG_INR;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Suspend_Status == false) resetEvent.WaitOne();
                block_size = Compute_Last_Block_Size(block_size);
                Init_TestArray(block_size, data_mode);
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
            //Complete_Information_Print(Test_Error_Num, ORDER_READ, 0, 0, Order_Max_Block);
        }
        public void MultiRandomWriteAndVerifyCT(ChooseInformation info)
        {
            if (!Repeat_Status && info.Record)
            {
                RecordTest recordTest = new RecordTest();
                recordTest.Random_Record_Mode(RANDOM_VERTIFY, info.TestDataMode, info.BlockSize, info.TestTime, info.TestNum, info.ThreadNum);
                recordTest.Finish();
            }
            Thread[] threads = new Thread[info.ThreadNum];
            DriverLoader driver = (DriverLoader)Disk_Driver_List[info.DriverIndex];
            resetEvents = new AutoResetEvent[info.ThreadNum];
            Percent = 0;
            TOTAL_TEST = 0;
            TOTAL_MB = 0;
            NOW_SPEED = 0;
            NOW_SPEED_NUM = 0;
            Console.WriteLine("当前块大小是： " + info.BlockSize);
            Time_StopWatch.Start();
            timer.Start();
            for (int i = 0; i < info.ThreadNum; i++)
            {
                threads[i] = new Thread(MultiRandomWriteAndVerify);
                threads[i].Start(new MultiThreadInfo(driver, info, i));
                resetEvents[i] = new AutoResetEvent(true);
            }
            for (int i = 0; i < info.ThreadNum; i++)
            {
                threads[i].Join();
            }
            Console.WriteLine("---------------wohoo,random all is end!");
            PublishTestEnd();
        }
        public void MultiRandomWriteAndVerify(object o)
        {
            MultiThreadInfo info = (MultiThreadInfo)o;
            Test_Driver_List();
            Init_Test_Label();
            int BLOCK_SIZE = info.chooseInformation.BlockSize;
            //Console.WriteLine("当前块大小是： " +BLOCK_SIZE);
            long total_sector = info.driverLoader.DiskInformation.DiskSectorSize;
            GapSectorNumble = total_sector / info.chooseInformation.ThreadNum;
            long left_pos = info.threadIndex * GapSectorNumble;
            long right_pos;
            long now_pos;
            if (total_sector % info.chooseInformation.ThreadNum != 0 && info.threadIndex == (info.chooseInformation.ThreadNum - 1))
            {
                right_pos = left_pos + GapSectorNumble + (total_sector % info.chooseInformation.ThreadNum);
            }
            else
                right_pos = left_pos + GapSectorNumble;
            Console.WriteLine("第 " + info.threadIndex + " 个线程的初始位置：" + left_pos + " 的最后位置是：" + right_pos);
            Init_Test_Param(BLOCK_SIZE);
            int BYTE_SIZE = BLOCK_SIZE * DEAFAUT_BLOCKSIZE;
            Fast_INR = (BLOCK_SIZE >= 256) ? SMALL_INR : SMALL_INR;
            Slow_INR = (BLOCK_SIZE >= 256) ? SMALL_INR : SMALL_INR;
            long total_bytes = 0;
            long once_bytes = 0;
            SPEED_COMPUTE speed_compute = 0;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Suspend_Status == false)
                {
                    //resetEvent.WaitOne();
                    Gap_End_Time = Environment.TickCount;                    
                    resetEvents[info.threadIndex].WaitOne();
                    if (Time_StopWatch.IsRunning)
                        Time_StopWatch.Stop();
                }
                if (Test_Stop_Status == false)
                {
                    break;
                }
                now_pos = NextLong(left_pos, right_pos - BLOCK_SIZE);
                //Console.Write("Threadid is : " + info.threadIndex+ "now_pos: " + now_pos + " ");
                //BLOCK_SIZE = Compute_Last_Block_Size_multi(BLOCK_SIZE, left_pos, right_pos);
                //BYTE_SIZE = BLOCK_SIZE * DEAFAUT_BLOCKSIZE;
                byte[] comparearray = new byte[BYTE_SIZE];
                byte[] testarray = new byte[BYTE_SIZE];
                testarray = Init_TestArraymulti(BYTE_SIZE, info.chooseInformation.TestDataMode, testarray);
                //Console.WriteLine("线程 "+info.threadIndex+" 准备获取信号量---------------");
                speed_compute.Start_Time = Environment.TickCount;
                comparearray=Compute_OnceBlockTimemulti(info.driverLoader, now_pos, BLOCK_SIZE, VERTIFY, testarray, comparearray);
                ErrorNumMutex.WaitOne();
                Test_Error_Num += VerifyArray(testarray, comparearray);
                ErrorNumMutex.ReleaseMutex();
                speed_compute.End_Time = Environment.TickCount;
                speed_compute.Once_Time = speed_compute.End_Time - speed_compute.Start_Time;
                speed_compute.Total_Time += speed_compute.Once_Time;
                TimeMutex.WaitOne();
                Test_End_Time = Environment.TickCount;
                TimeMutex.ReleaseMutex();
                Add_Bytes();
                total_bytes += BYTE_SIZE;
                once_bytes += BYTE_SIZE;
                //Console.WriteLine(now_pos+" 第 "+info.threadIndex+" 个线程的位置");
                //left_pos += BLOCK_SIZE;
                percent_mutex.WaitOne();
                TOTAL_TEST += BLOCK_SIZE;
                percent_mutex.ReleaseMutex();
                Time_StopWatch.Stop();
                /*timer.Tick += (sender1, e1) =>
                {
                    
                    //userCurve1.AddCurveData("B", random.Next(50, 201));
                };*/
                Percent = (info.chooseInformation.TestNum == 0) ? (int)(100 * (Time_StopWatch.ElapsedMilliseconds) / info.chooseInformation.TestTime) : (int)(100 * Temp_Nums / info.chooseInformation.TestNum);
                Time_StopWatch.Start();
                if (Temp_Nums % Fast_INR == 0 || Percent >= 100)
                {
                    this.PublishProcessAndTime((int)(Percent), DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    //if (Temp_Nums % Slow_INR == 0 || Percent >= 100)
                    //{
                        Console.Write("Threadid is : " + info.threadIndex + " now_pos:" + now_pos+" ");
                        double speed=Compute_OnceBlockSpeed_multi(speed_compute, total_bytes, once_bytes);
                        this.PublishWrittenAndSpeed(speed, TOTAL_MB);
                        if (speed <= MinSpeed)
                            this.PrintLog(EXCEPTION, "Speed below minimum,speed is " + speed + "\n");
                        once_bytes = 0;
                        speed_compute.Total_Time = 0;
                    //}
                }
                Time_StopWatch.Stop();
                if ((info.chooseInformation.TestNum == 0) && (Time_StopWatch.ElapsedMilliseconds >= info.chooseInformation.TestTime || Percent > 100)) break;
                else if ((info.chooseInformation.TestTime == 0) && (Temp_Nums > info.chooseInformation.TestNum || Percent >= 100)) break;
                else
                {
                    RandomTestMutex.WaitOne();
                    Temp_Nums++;
                    RandomTestMutex.ReleaseMutex();
                    Time_StopWatch.Start();
                    continue;
                }
            }
            this.PrintLog(NORMAL, "当前驱动器：" + info.driverLoader.DiskInformation.Physical_Name + " 的第 " + info.threadIndex + " 个线程完成测试！\n");
            if (info.chooseInformation.TestNum == 0) Complete_Information_Print(info,Test_Error_Num, RANDOM_VERTIFY, info.chooseInformation.TestTime, 0);
            else Complete_Information_Print(info,Test_Error_Num, RANDOM_VERTIFY, 0, info.chooseInformation.TestNum);
            Console.WriteLine("当前是第 " + info.threadIndex + " 个线程完成测试！");
            
            //Complete_Information_Print(info,Test_Error_Num, RANDOM_VERTIFY, 0, 0, right_pos);
        }
        /// <summary>
        /// 随机读写验证,针对一次读写在256块以上的,已添加记录函数
        /// </summary>
        /// <param name="driver_index">读写IO流的编号</param>
        /// <param name="test_num">测试次数</param>
        /// <param name="test_time">测试时间</param>
        /// <param name="test_mode">测试模式--0代表全0,1代表全1，2随机数</param>               
        public void RandomWriteAndVerify(int driver_index, long test_num = 0, long test_time = 0, int test_data_mode = 2, int block_size = 8192,int thread_num=1)
        {
            RecordTest recordTest = new RecordTest();
            recordTest.Random_Record_Mode(RANDOM_VERTIFY, test_data_mode, block_size, test_time, test_num,thread_num);
            Test_Driver_List();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            Init_Test_Label();
            Init_Test_Param(block_size);
            CompareArray = new byte[Block_Bytes];
            Fast_INR = (block_size >= 256) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (block_size >= 256) ? MIDDLE_INR : BIG_INR;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Suspend_Status == false) resetEvent.WaitOne();
                Init_TestArray(block_size, test_data_mode);
                Now_Pos = NextLong(0, driver.DiskInformation.DiskSectorSize - block_size);
                recordTest.Random_Record_Sector(Now_Pos);
                Compute_OnceBlockTime(driver, Now_Pos, block_size, VERTIFY);
                Test_Error_Num += VerifyArray(TestArray, CompareArray);
                Test_End_Time = Environment.TickCount;
                Add_Bytes();
                Percent = (test_num == 0) ? (int)(100 * (Test_End_Time - Test_Start_Time) / test_time) : (int)(100 * Temp_Nums / test_num);
                if (Temp_Nums % Fast_INR == 0 || Percent >= 100)
                {
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
            //if (test_num == 0) Complete_Information_Print(Test_Error_Num, RANDOM_VERTIFY, test_time, 0);
            //else Complete_Information_Print(Test_Error_Num, RANDOM_VERTIFY, 0, test_num);
            recordTest.Finish();
        }
        /// <summary>
        /// 随机读写验证复现版，无需记录上次测试结果
        /// </summary>
        /// <param name="driver_index"></param>
        /// <param name="test_num"></param>
        /// <param name="test_time"></param>
        /// <param name="test_data_mode"></param>
        /// <param name="block_size"></param>
        public void RandomWriteAndVerify_Repeat(int driver_index, long test_num = 0, long test_time = 0, int test_data_mode = 2, int block_size = 8192)
        {
            Test_Driver_List();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            Init_Test_Label();
            Init_Test_Param(block_size);
            CompareArray = new byte[Block_Bytes];
            Fast_INR = (block_size >= 256) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (block_size >= 256) ? MIDDLE_INR : BIG_INR;
            ReadRecord readRecord = new ReadRecord();
            for (int i = 0; i < 4; i++) readRecord.Read_Block_Size();
            while (true)
            {
                ///添加状态判断语句
                if (Test_Suspend_Status == false) resetEvent.WaitOne();
                Init_TestArray(block_size, test_data_mode);
                Now_Pos = readRecord.Read_Sector();
                //Console.WriteLine(Now_Pos);
                //recordTest.Random_Record_Sector(Now_Pos);
                Compute_OnceBlockTime(driver, Now_Pos, block_size, VERTIFY);
                Test_Error_Num += VerifyArray(TestArray, CompareArray);
                Test_End_Time = Environment.TickCount;
                Add_Bytes();
                Percent = (test_num == 0) ? (int)(100 * (Test_End_Time - Test_Start_Time) / test_time) : (int)(100 * Temp_Nums / test_num);
                if (Temp_Nums % Fast_INR == 0 || Percent >= 100)
                {
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
            //if (test_num == 0) Complete_Information_Print(Test_Error_Num, RANDOM_VERTIFY, test_time, 0);
            //else Complete_Information_Print(Test_Error_Num, RANDOM_VERTIFY, 0, test_num);
            readRecord.Finish();
            //recordTest.Finish();
        }
        public void MultiRandomOnlyWriteCT(ChooseInformation info)
        {
            if (!Repeat_Status && info.Record)
            {
                RecordTest recordTest = new RecordTest();
                recordTest.Random_Record_Mode(RANDOM_WRITE, info.TestDataMode, info.BlockSize, info.TestTime, info.TestNum, info.ThreadNum);
                recordTest.Finish();
            }
            Thread[] threads = new Thread[info.ThreadNum];
            DriverLoader driver = (DriverLoader)Disk_Driver_List[info.DriverIndex];
            resetEvents = new AutoResetEvent[info.ThreadNum];
            Percent = 0;
            TOTAL_TEST = 0;
            TOTAL_MB = 0;
            NOW_SPEED = 0;
            NOW_SPEED_NUM = 0;
            Time_StopWatch.Start();
            Console.WriteLine("当前块大小是： " + info.BlockSize);
            for (int i = 0; i < info.ThreadNum; i++)
            {
                threads[i] = new Thread(MultiRandomOnlyWrite);
                threads[i].Start(new MultiThreadInfo(driver, info, i));
                resetEvents[i] = new AutoResetEvent(true);
            }
            for (int i = 0; i < info.ThreadNum; i++)
            {
                threads[i].Join();
            }
            Console.WriteLine("---------------wohoo,random all is end!");
            PublishTestEnd();
        }
        public void MultiRandomOnlyWrite(object o)
        {
            MultiThreadInfo info = (MultiThreadInfo)o;
            Test_Driver_List();
            Init_Test_Label();
            int BLOCK_SIZE = info.chooseInformation.BlockSize;
            //Console.WriteLine("当前块大小是： " +BLOCK_SIZE);
            long total_sector = info.driverLoader.DiskInformation.DiskSectorSize;
            GapSectorNumble = total_sector / info.chooseInformation.ThreadNum;
            long left_pos = info.threadIndex * GapSectorNumble;
            long right_pos;
            long now_pos;
            if (total_sector % info.chooseInformation.ThreadNum != 0 && info.threadIndex == (info.chooseInformation.ThreadNum - 1))
            {
                right_pos = left_pos + GapSectorNumble + (total_sector % info.chooseInformation.ThreadNum);
            }
            else
                right_pos = left_pos + GapSectorNumble;
            //Console.WriteLine("第 " + info.threadIndex + " 个线程的初始位置：" + left_pos + " 的最后位置是：" + right_pos);
            Init_Test_Param(BLOCK_SIZE);
            int BYTE_SIZE = BLOCK_SIZE * DEAFAUT_BLOCKSIZE;
            Fast_INR = (BLOCK_SIZE >= 16) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (BLOCK_SIZE >= 16) ? SMALL_INR : BIG_INR;
            long total_bytes = 0;
            long once_bytes = 0;
            SPEED_COMPUTE speed_compute = 0;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Suspend_Status == false)
                {
                    //resetEvent.WaitOne();
                    Gap_End_Time = Environment.TickCount;
                    resetEvents[info.threadIndex].WaitOne();
                    if (Time_StopWatch.IsRunning)
                        Time_StopWatch.Stop();
                }
                if (Test_Stop_Status == false)
                {
                    break;
                }
                now_pos = NextLong(left_pos, right_pos - BLOCK_SIZE);
                //Console.Write("Threadid is : " + info.threadIndex+ "now_pos: " + now_pos + " ");
                //BLOCK_SIZE = Compute_Last_Block_Size_multi(BLOCK_SIZE, left_pos, right_pos);
                //BYTE_SIZE = BLOCK_SIZE * DEAFAUT_BLOCKSIZE;
                byte[] comparearray = new byte[BYTE_SIZE];
                byte[] testarray = new byte[BYTE_SIZE];
                testarray = Init_TestArraymulti(BYTE_SIZE, info.chooseInformation.TestDataMode, testarray);
                //Console.WriteLine("线程 "+info.threadIndex+" 准备获取信号量---------------");
                speed_compute.Start_Time = Environment.TickCount;
                Compute_OnceBlockTimemulti(info.driverLoader, now_pos, BLOCK_SIZE, WRITE, testarray, comparearray);
                speed_compute.End_Time = Environment.TickCount;
                speed_compute.Once_Time = speed_compute.End_Time - speed_compute.Start_Time;
                speed_compute.Total_Time += speed_compute.Once_Time;
                TimeMutex.WaitOne();
                Test_End_Time = Environment.TickCount;
                TimeMutex.ReleaseMutex();
                Add_Bytes();
                total_bytes += BYTE_SIZE;
                once_bytes += BYTE_SIZE;
                //Console.WriteLine(now_pos+" 第 "+info.threadIndex+" 个线程的位置");
                //left_pos += BLOCK_SIZE;
                percent_mutex.WaitOne();
                TOTAL_TEST += BLOCK_SIZE;
                percent_mutex.ReleaseMutex();
                Time_StopWatch.Stop();
                Percent = (info.chooseInformation.TestNum == 0) ? (int)(100 * (Time_StopWatch.ElapsedMilliseconds) / info.chooseInformation.TestTime) : (int)(100 * Temp_Nums / info.chooseInformation.TestNum);
                Time_StopWatch.Start();
                //Console.WriteLine("num: " + Temp_Nums);
                if (Temp_Nums % Fast_INR == 0 || Percent >= 100)
                {
                    this.PublishProcessAndTime((int)(Percent), DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    //if (Temp_Nums % Slow_INR == 0 || Percent >= 100)
                    //{
                        Console.Write("Threadid is : " + info.threadIndex + " now_pos:" + now_pos);
                        double speed=Compute_OnceBlockSpeed_multi(speed_compute, total_bytes, once_bytes);
                        this.PublishWrittenAndSpeed(speed, TOTAL_MB);
                        if (speed <= MinSpeed)
                            this.PrintLog(EXCEPTION, "Speed below minimum,speed is " + speed + "\n");
                        once_bytes = 0;
                        speed_compute.Total_Time = 0;
                    //}
                }

                Time_StopWatch.Stop();
                if ((info.chooseInformation.TestNum == 0) && (Time_StopWatch.ElapsedMilliseconds >= info.chooseInformation.TestTime || Percent > 100)) break;
                else if ((info.chooseInformation.TestTime == 0) && (Temp_Nums > info.chooseInformation.TestNum || Percent >= 100)) break;
                else
                {
                    RandomTestMutex.WaitOne();
                    Temp_Nums++;
                    RandomTestMutex.ReleaseMutex();
                    Time_StopWatch.Start();
                    continue;
                }
            }
            this.PrintLog(NORMAL, "当前驱动器：" + info.driverLoader.DiskInformation.Physical_Name + " 的第 " + info.threadIndex + " 个线程完成测试！\n");
            if (info.chooseInformation.TestNum == 0) Complete_Information_Print(info,Test_Error_Num, RANDOM_WRITE, info.chooseInformation.TestTime, 0);
            else Complete_Information_Print(info,Test_Error_Num, RANDOM_WRITE, 0, info.chooseInformation.TestNum);
            Console.WriteLine("当前是第 " + info.threadIndex + " 个线程完成测试！");
            
            //Complete_Information_Print(info,Test_Error_Num, RANDOM_VERTIFY, 0, 0, right_pos);
        }
        public void MultiRandomOnlyReadCT(ChooseInformation info)
        {
            if (!Repeat_Status && info.Record)
            {
                RecordTest recordTest = new RecordTest();
                recordTest.Random_Record_Mode(RANDOM_READ, info.TestDataMode, info.BlockSize, info.TestTime, info.TestNum, info.ThreadNum);
                recordTest.Finish();
            }
            Thread[] threads = new Thread[info.ThreadNum];
            DriverLoader driver = (DriverLoader)Disk_Driver_List[info.DriverIndex];
            resetEvents = new AutoResetEvent[info.ThreadNum];
            Percent = 0;
            TOTAL_TEST = 0;
            TOTAL_MB = 0;
            NOW_SPEED = 0;
            NOW_SPEED_NUM = 0;
            Console.WriteLine("当前块大小是： " + info.BlockSize);
            Time_StopWatch.Start();
            for (int i = 0; i < info.ThreadNum; i++)
            {
                threads[i] = new Thread(MultiRandomOnlyRead);
                threads[i].Start(new MultiThreadInfo(driver, info, i));
                resetEvents[i] = new AutoResetEvent(true);
            }
            for (int i = 0; i < info.ThreadNum; i++)
            {
                threads[i].Join();
            }
            Console.WriteLine("---------------wohoo,random all is end!");
            PublishTestEnd();
        }
        public void MultiRandomOnlyRead(object o)
        {
            MultiThreadInfo info = (MultiThreadInfo)o;
            Test_Driver_List();
            Init_Test_Label();
            int BLOCK_SIZE = info.chooseInformation.BlockSize;
            //Console.WriteLine("当前块大小是： " +BLOCK_SIZE);
            long total_sector = info.driverLoader.DiskInformation.DiskSectorSize;
            GapSectorNumble = total_sector / info.chooseInformation.ThreadNum;
            long left_pos = info.threadIndex * GapSectorNumble;
            long right_pos;
            long now_pos;
            if (total_sector % info.chooseInformation.ThreadNum != 0 && info.threadIndex == (info.chooseInformation.ThreadNum - 1))
            {
                right_pos = left_pos + GapSectorNumble + (total_sector % info.chooseInformation.ThreadNum);
            }
            else
                right_pos = left_pos + GapSectorNumble;
            //Console.WriteLine("第 " + info.threadIndex + " 个线程的初始位置：" + left_pos + " 的最后位置是：" + right_pos);
            Init_Test_Param(BLOCK_SIZE);
            int BYTE_SIZE = BLOCK_SIZE * DEAFAUT_BLOCKSIZE;
            Fast_INR = (BLOCK_SIZE >= 256) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (BLOCK_SIZE >= 256) ? MIDDLE_INR : BIG_INR;
            long total_bytes = 0;
            long once_bytes = 0;
            SPEED_COMPUTE speed_compute = 0;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Suspend_Status == false)
                {
                    //resetEvent.WaitOne();
                    Gap_End_Time = Environment.TickCount;
                    if (Time_StopWatch.IsRunning)
                        Time_StopWatch.Stop();
                    resetEvents[info.threadIndex].WaitOne();                   
                }
                if (Test_Stop_Status == false)
                {
                    break;
                }
                now_pos = NextLong(left_pos, right_pos - BLOCK_SIZE);
                //Console.Write("Threadid is : " + info.threadIndex+ "now_pos: " + now_pos + " ");
                //BLOCK_SIZE = Compute_Last_Block_Size_multi(BLOCK_SIZE, left_pos, right_pos);
                //BYTE_SIZE = BLOCK_SIZE * DEAFAUT_BLOCKSIZE;
                byte[] comparearray = new byte[BYTE_SIZE];
                byte[] testarray = new byte[BYTE_SIZE];
                //testarray = Init_TestArraymulti(BYTE_SIZE, info.chooseInformation.TestDataMode, testarray);
                //Console.WriteLine("线程 "+info.threadIndex+" 准备获取信号量---------------");
                speed_compute.Start_Time = Environment.TickCount;
                comparearray=Compute_OnceBlockTimemulti(info.driverLoader, now_pos, BLOCK_SIZE, READ, testarray, comparearray);
                speed_compute.End_Time = Environment.TickCount;
                speed_compute.Once_Time = speed_compute.End_Time - speed_compute.Start_Time;
                speed_compute.Total_Time += speed_compute.Once_Time;
                TimeMutex.WaitOne();
                Test_End_Time = Environment.TickCount;
                TimeMutex.ReleaseMutex();
                Add_Bytes();
                total_bytes += BYTE_SIZE;
                once_bytes += BYTE_SIZE;
                //Console.WriteLine(now_pos+" 第 "+info.threadIndex+" 个线程的位置");
                //left_pos += BLOCK_SIZE;
                percent_mutex.WaitOne();
                TOTAL_TEST += BLOCK_SIZE;
                percent_mutex.ReleaseMutex();
                Time_StopWatch.Stop();
                Percent = (info.chooseInformation.TestNum == 0) ? (int)(100 * (Time_StopWatch.ElapsedMilliseconds) / info.chooseInformation.TestTime) : (int)(100 * Temp_Nums / info.chooseInformation.TestNum);
                Time_StopWatch.Start();
                //Console.WriteLine("num: " + Temp_Nums);
                if (Temp_Nums % Fast_INR == 0 || Percent >= 100)
                {
                    this.PublishProcessAndTime((int)(Percent), DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    if (Temp_Nums % Slow_INR == 0 || Percent >= 100)
                    {
                        Console.Write("Threadid is : " + info.threadIndex + " now_pos:" + now_pos);
                        double speed=Compute_OnceBlockSpeed_multi(speed_compute, total_bytes, once_bytes);
                        this.PublishWrittenAndSpeed(speed, TOTAL_MB);
                        if (speed <= MinSpeed)
                            this.PrintLog(EXCEPTION, "Speed below minimum,speed is " + speed + "\n");
                        once_bytes = 0;
                        speed_compute.Total_Time = 0;
                    }
                }
                Time_StopWatch.Stop();
                if ((info.chooseInformation.TestNum == 0) && (Time_StopWatch.ElapsedMilliseconds >= info.chooseInformation.TestTime || Percent > 100)) break;
                else if ((info.chooseInformation.TestTime == 0) && (Temp_Nums > info.chooseInformation.TestNum || Percent >= 100)) break;
                else
                {
                    RandomTestMutex.WaitOne();
                    Temp_Nums++;
                    RandomTestMutex.ReleaseMutex();
                    Time_StopWatch.Start();
                    continue;
                }
            }
            this.PrintLog(NORMAL, "当前驱动器：" + info.driverLoader.DiskInformation.Physical_Name + " 的第 " + info.threadIndex + " 个线程完成测试！\n");
            if (info.chooseInformation.TestNum == 0) Complete_Information_Print(info,Test_Error_Num, RANDOM_READ, info.chooseInformation.TestTime, 0);
            else Complete_Information_Print(info,Test_Error_Num, RANDOM_READ, 0, info.chooseInformation.TestNum);
            Console.WriteLine("当前是第 " + info.threadIndex + " 个线程完成测试！");
        }
        /// <summary>
        /// 随机只读验证
        /// </summary>
        /// <param name="driver_index">读写IO流的编号--与硬盘对应</param>
        /// <param name="test_num">测试次数</param>
        /// <param name="test_time">测试时间</param>
        public void RandomOnlyRead(int driver_index, long test_num = 0, long test_time = 0, int block_size = 8192,int thread_num=1)
        {
            RecordTest recordTest = new RecordTest();
            recordTest.Random_Record_Mode(RANDOM_READ, 0, block_size, test_time, test_num,thread_num);
            Test_Driver_List();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            Init_Test_Label();
            Init_Test_Param(block_size);
            CompareArray = new byte[Block_Bytes];
            Fast_INR = (block_size >= 256) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (block_size >= 256) ? MIDDLE_INR : BIG_INR;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Suspend_Status == false) resetEvent.WaitOne();
                Now_Pos = NextLong(0, driver.DiskInformation.DiskSectorSize - block_size);
                recordTest.Random_Record_Sector(Now_Pos);
                Compute_OnceBlockTime(driver, Now_Pos, block_size, READ);
                Test_End_Time = Environment.TickCount;
                Add_Bytes();
                Percent = (test_num == 0) ? (int)(100 * (Test_End_Time - Test_Start_Time) / test_time) : (int)(100 * Temp_Nums / test_num);
                if (Temp_Nums % Fast_INR == 0 || Percent >= 100)
                {
                    this.PublishProcessAndTime((int)(Percent), DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    if (Temp_Nums % Slow_INR == 0 || Percent >= 100) Compute_OnceBlockSpeed();
                }
                Temp_Nums++;
                if ((test_num == 0) && (Test_End_Time - Test_Start_Time >= test_time || Percent > 100)) break;
                else if ((test_time == 0) && (Temp_Nums > test_num || Percent >= 100)) break;
                else continue;
            }
            //if (test_num == 0) Complete_Information_Print(Test_Error_Num, RANDOM_READ, test_time, 0);
            //else Complete_Information_Print(Test_Error_Num, RANDOM_READ, 0, test_num);
            recordTest.Finish();
        }
        /// <summary>
        /// 随机只读复现版，无需记录本次测试过程
        /// </summary>
        /// <param name="driver_index"></param>
        /// <param name="test_num"></param>
        /// <param name="test_time"></param>
        /// <param name="block_size"></param>
        public void RandomOnlyRead_Repeat(int driver_index, long test_num = 0, long test_time = 0, int block_size = 8192)
        {
            Test_Driver_List();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            Init_Test_Label();
            Init_Test_Param(block_size);
            CompareArray = new byte[Block_Bytes];
            Fast_INR = (block_size >= 256) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (block_size >= 256) ? MIDDLE_INR : BIG_INR;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Suspend_Status == false) resetEvent.WaitOne();
                Now_Pos = NextLong(0, driver.DiskInformation.DiskSectorSize - block_size);
                Compute_OnceBlockTime(driver, Now_Pos, block_size, READ);
                Test_End_Time = Environment.TickCount;
                Add_Bytes();
                Percent = (test_num == 0) ? (int)(100 * (Test_End_Time - Test_Start_Time) / test_time) : (int)(100 * Temp_Nums / test_num);
                if (Temp_Nums % Fast_INR == 0 || Percent >= 100)
                {
                    this.PublishProcessAndTime((int)(Percent), DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    if (Temp_Nums % Slow_INR == 0 || Percent >= 100) Compute_OnceBlockSpeed();
                }
                Temp_Nums++;
                if ((test_num == 0) && (Test_End_Time - Test_Start_Time >= test_time || Percent > 100)) break;
                else if ((test_time == 0) && (Temp_Nums > test_num || Percent >= 100)) break;
                else continue;
            }
            //if (test_num == 0) Complete_Information_Print(Test_Error_Num, RANDOM_READ, test_time, 0);
            //else Complete_Information_Print(Test_Error_Num, RANDOM_READ, 0, test_num);
        }
        /// <summary>
        /// 随机只写验证,已修改测速模式
        /// </summary>
        /// <param name="driver_index">读写IO流的编号--与硬盘对应</param>
        /// <param name="test_num">测试次数</param>
        /// <param name="test_time">测试时间</param>
        /// <param name="data_mode">测试模式</param>
        public void RandomOnlyWrite(int driver_index, long test_num = 0, long test_time = 0, int data_mode = 2, int block_size = 8192, int thread_num = 1)
        {
            RecordTest recordTest = new RecordTest();
            recordTest.Random_Record_Mode(RANDOM_WRITE, 0, block_size, test_time, test_num,thread_num);
            Test_Driver_List();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            Init_Test_Label();
            Init_Test_Param(block_size);
            Fast_INR = (block_size >= 256) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (block_size >= 256) ? MIDDLE_INR : BIG_INR;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Suspend_Status == false) resetEvent.WaitOne();
                Init_TestArray(block_size, data_mode);
                Now_Pos = NextLong(0, driver.DiskInformation.DiskSectorSize - block_size);
                recordTest.Random_Record_Sector(Now_Pos);
                Compute_OnceBlockTime(driver, Now_Pos, block_size, WRITE);
                Test_End_Time = Environment.TickCount;
                Add_Bytes();
                Percent = (test_num == 0) ? (int)(100 * (Test_End_Time - Test_Start_Time) / test_time) : (int)(100 * Temp_Nums / test_num);
                if (Temp_Nums % Fast_INR == 0 || Percent >= 100)
                {
                    this.PublishProcessAndTime((int)(Percent), DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    if (Temp_Nums % Slow_INR == 0 || Percent >= 100) Compute_OnceBlockSpeed();
                }
                Temp_Nums++;
                if ((test_num == 0) && (Test_End_Time - Test_Start_Time >= test_time || Percent > 100)) break;
                else if ((test_time == 0) && (Temp_Nums > test_num || Percent >= 100)) break;
                else continue;
            }
            //if (test_num == 0) Complete_Information_Print(Test_Error_Num, RANDOM_WRITE, test_time, 0);
            //else Complete_Information_Print(Test_Error_Num, RANDOM_WRITE, 0, test_num);
            recordTest.Finish();
        }
        public void RandomOnlyWrite_Repeat(int driver_index, long test_num = 0, long test_time = 0, int data_mode = 2, int block_size = 8192)
        {
            Test_Driver_List();
            DriverLoader driver = (DriverLoader)Disk_Driver_List[driver_index];
            Init_Test_Label();
            Init_Test_Param(block_size);
            Fast_INR = (block_size >= 256) ? SMALL_INR : MIDDLE_INR;
            Slow_INR = (block_size >= 256) ? MIDDLE_INR : BIG_INR;
            while (true)
            {
                ///添加状态判断语句
                if (Test_Suspend_Status == false) resetEvent.WaitOne();
                Init_TestArray(block_size, data_mode);
                Now_Pos = NextLong(0, driver.DiskInformation.DiskSectorSize - block_size);
                Compute_OnceBlockTime(driver, Now_Pos, block_size, WRITE);
                Test_End_Time = Environment.TickCount;
                Add_Bytes();
                Percent = (test_num == 0) ? (int)(100 * (Test_End_Time - Test_Start_Time) / test_time) : (int)(100 * Temp_Nums / test_num);
                if (Temp_Nums % Fast_INR == 0 || Percent >= 100)
                {
                    this.PublishProcessAndTime((int)(Percent), DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    if (Temp_Nums % Slow_INR == 0 || Percent >= 100) Compute_OnceBlockSpeed();
                }
                Temp_Nums++;
                if ((test_num == 0) && (Test_End_Time - Test_Start_Time >= test_time || Percent > 100)) break;
                else if ((test_time == 0) && (Temp_Nums > test_num || Percent >= 100)) break;
                else continue;
            }
            //if (test_num == 0) Complete_Information_Print(Test_Error_Num, RANDOM_WRITE, test_time, 0);
            //else Complete_Information_Print(Test_Error_Num, RANDOM_WRITE, 0, test_num);
        }

        private void Reflesh_Button_Click(object sender, EventArgs e)
        {
            //InitializeComponent();
            Init_Disk_Information();
            Init_Disk_Framework();
            Init_Choose_And_Repeat_Status_ArrayList();
            Init_Disk_Setting_Param();
            
            Show_Disk();
            for (int i = 0; i < Disk_Information_List.Count; i++)
            {
                Ed[i].TransfChooseINF += Get_Transf_Choose_INF_Event;
                Ed[i].TransfRepeatTest += Get_Repeat_Test_Status_Event;
            }
        }
    }
}
