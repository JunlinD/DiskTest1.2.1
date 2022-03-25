using System;
using System.Collections.Generic;
using System.IO;

namespace DiskTestLib
{
    class ReadRecord
    {
        public FileStream RecordStream;
        public StreamReader RecordRead;
        public ReadRecord()
        {
            RecordStream = (!File.Exists("Last_Test.txt")) ? new FileStream("Last_Test.txt", FileMode.Create, FileAccess.ReadWrite) : new FileStream("Last_Test.txt", FileMode.Open, FileAccess.ReadWrite);
            RecordRead = new StreamReader(RecordStream);
        }      
        public int Read_Test_Mode()
        {
            int test_mode = Convert.ToInt32(RecordRead.ReadLine());
            return test_mode;
        }
        public int Read_Data_Mode()
        {
            int test_data_mode = Convert.ToInt32(RecordRead.ReadLine());
            return test_data_mode;
        }
        public int Read_Block_Size()
        {
            int block_size = Convert.ToInt32(RecordRead.ReadLine());
            return block_size;
        }
        public int Read_Size()
        {
            int size = Convert.ToInt32(RecordRead.ReadLine());
            return size;
        }
        public long Read_Test_Num()
        {
            long testnum = Convert.ToInt64(RecordRead.ReadLine());
            return testnum;
        }
        public long Read_Test_Time()
        {
            long testtime = Convert.ToInt64(RecordRead.ReadLine());
            return testtime;
        }
        public long Read_Sector()
        {
            long sector= Convert.ToInt64(RecordRead.ReadLine());
            return sector;
        }
        public int Read_Thread()
        {
            int threadnum = Convert.ToInt32(RecordRead.ReadLine());
            return threadnum;
        }
        public int Read_Circle()
        {
            int circle = Convert.ToInt32(RecordRead.ReadLine());
            return circle;
        }
        public void Finish()
        {
            RecordRead.Close();
            RecordStream.Close();
        }
    }
}
