using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace DiskTestLib
{
    public class DriverLoader
    {

        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_READ = 0x00000001;
        private const uint FILE_SHARE_WRITE = 0x00000002;
        private const uint OPEN_EXISTING = 3;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern SafeFileHandle CreateFileA(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);
        private System.IO.FileStream _DirverStream;
        private SafeFileHandle _DirverHandle;
        private DiskInformation _DiskInformation;
        public delegate void SendExceptionLogHandler(int grade,string s);
        public SendExceptionLogHandler SendlongEvent;
        private int ID;
        /// <summary>
        /// 获取磁盘扇区信息
        /// </summary>
        /// <param name="DirverName">G:</param>
        public DriverLoader(DiskInformation d)
        {
            _DiskInformation = d;
            if (d == null && d.Physical_Name.Length == 0) return;
            _DirverHandle = CreateFileA(_DiskInformation.Physical_Name, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            
            _DirverStream = new System.IO.FileStream(_DirverHandle, System.IO.FileAccess.ReadWrite);
        }
        /// <summary>
        /// 扇区显示转换
        /// </summary>
        /// <param name="SectorBytes">扇区 长度512</param>
        /// <returns>EB 52 90 ......55 AA</returns>
        public string GetString(byte[] SectorBytes)
        {
            if (SectorBytes.Length != 512) return "";
            StringBuilder ReturnText = new StringBuilder();

            int RowCount = 0;
            for (int i = 0; i != 512; i++)
            {
                ReturnText.Append(SectorBytes[i].ToString("X02") + " ");

                if (RowCount == 15)
                {
                    ReturnText.Append("\r\n");
                    RowCount = -1;
                }

                RowCount++;
            }

            return ReturnText.ToString();

        }
        /// <summary>
        /// 读一个扇区
        /// </summary>
        /// <param name="SectorIndex">扇区号</param>
        /// <returns>如果扇区数字大于分区信息的扇区数 返回NULL</returns>
        public byte[] ReadSector(long SectorIndex,int blocksize=1)
        {
            if (SectorIndex > DiskInformation.DiskSectorSize) return null;

            _DirverStream.Position = SectorIndex * 512;
            byte[] ReturnByte = new byte[512*blocksize];
            _DirverStream.Read(ReturnByte, 0, 512*blocksize); //获取扇区
            //_DirverStream.Read(ReturnByte, 512, 512);
            return ReturnByte;
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="SectorBytes">扇区 长度512</param>
        /// <param name="SectorIndex">扇区位置</param>
        public void WritSector(byte[] SectorBytes, long SectorIndex,int blockszie=1)
        {
            //if (SectorBytes.Length != 512) return;
            _DirverStream.Position = SectorIndex * 512;
            try
            {

                _DirverStream.Write(SectorBytes, 0, 512*blockszie); //写入扇区  
                //_DirverStream.Position = SectorIndex * (512 + 1);
                //_DirverStream.Write(SectorBytes, 512, 512);
                _DirverStream.Flush();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                PrintLog(3, e.Message);
            }
        }
        public void addSendExceptionLogEvent(SendExceptionLogHandler el)
        {
            SendlongEvent += el;
        }
        public void PrintLog(int grade,string s)
        {
            SendlongEvent(grade,s);
        }
        public DiskInformation DiskInformation { get{ return _DiskInformation; } }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            _DirverStream.Close();
        }

    }
    public class DiskInformation
    {
        private string _DiskPhysicalName;//直接物理驱动器号
        private long _DiskSectorSize;
        private decimal _DiskSize;
        private int _ID;
        
        public DiskInformation()
        {
            _DiskPhysicalName = null;//直接物理驱动器号
            _DiskSectorSize = 0;
            _DiskSize = 0;
            _ID = 0;
          
        }
        public DiskInformation(string diskphysicalname,long disksector,decimal disksize,int id)
        {
            _DiskPhysicalName=diskphysicalname;//直接物理驱动器号
            _DiskSectorSize=disksector;
            _DiskSize=disksize;
            _ID = id;
        }        
        public string Physical_Name { get { return _DiskPhysicalName; } }
        public long DiskSectorSize { get { return _DiskSectorSize; } }
        public decimal DiskSize { get { return _DiskSize; } }
        public int ID { get { return _ID; } }
    }
    public class ChooseInformation
    {
        public bool TestOrNot;
        public int TestMode;
        public int TestDataMode;
        public int TestPercent;
        public int BlockSize;
        public long TestTime;
        public int TestCircle;
        public long TestNum;
        public bool Record;
        public int ThreadNum;
        public int DriverIndex;
        public ChooseInformation()
        {
            TestOrNot = false;
            TestMode = 0;
            TestDataMode = 0;
            TestPercent = 0;
            BlockSize = 0;
            TestTime = 0;
            TestCircle = 0;
            TestNum = 0;
            ThreadNum = 1;
            DriverIndex = 0;
            Record = false;
        }
        public void SetRandomParameters(bool testornot, int testmode, long testtime, long testnum,int blocksize,bool record,int threadnum)
        {
            TestOrNot = testornot;
            TestMode = testmode;
            TestTime = testtime;
            TestNum = testnum;
            BlockSize = blocksize;
            Record = record;
            ThreadNum = threadnum;
            if (testmode == -1||(testtime==0&&testnum==0))
                TestOrNot = false;
        }
        public void SetOrderParameters(bool testornot, int testmode, int testdatamode, int testpercent, int blocksize, long testtime, long testnum, int testcircle,bool record,int threadnum)
        {
            TestOrNot = testornot;
            TestMode = testmode;
            TestDataMode = testdatamode;
            TestPercent = testpercent;
            BlockSize = blocksize;
            TestTime = testtime;
            TestCircle = testcircle;
            TestNum = testnum;
            Record = record;
            ThreadNum = threadnum;
            if (testmode == -1 || blocksize == 0)
            {
                TestOrNot = false;
            }
        }
    }
    public class MultiThreadInfo
    {
        public ChooseInformation chooseInformation;
        public int threadIndex;
        public DriverLoader driverLoader;
        public MultiThreadInfo(DriverLoader driver,ChooseInformation choose,int index)
        {
            driverLoader = driver;
            chooseInformation = choose;
            threadIndex = index;
        }
    }
    public class TestInformation
    {
        public const int DATA_ERROR = 0;
        public const int DISK_ERROR = 1;
        public const int FINISH = 2;
        public string Driver_Name;
        public DateTime DateTime;
        public int Information_Mode;
        public byte Error_Data;
        public byte Correct_Data;
        public TestInformation(string name, DateTime time, int inf_mode, byte correct = 0, byte error = 0)
        {
            Driver_Name = name;
            DateTime = time;
            Information_Mode = inf_mode;
            Correct_Data = correct;
            Error_Data = error;
        }
    }
    
}
