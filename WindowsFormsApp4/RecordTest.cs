using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiskTestLib
{
    class RecordTest
    {
        public const int RANDOM = 0;
        public const int ORDER = 1;
        public FileStream RecordStream;
        public StreamWriter RecordWrite;
        public RecordTest()
        {
            RecordStream = new FileStream("Last_Test.txt", FileMode.Create, FileAccess.ReadWrite) ;
            RecordWrite = new StreamWriter(RecordStream);
        }
        public void Random_Record_Mode(int test_mode,int data_mode,int block_size,long test_time,long test_num,int thread_num)
        {
            RecordWrite.WriteLine(test_mode);
            RecordWrite.WriteLine(block_size);
            RecordWrite.WriteLine(test_time);
            RecordWrite.WriteLine(test_num);
            RecordWrite.WriteLine(thread_num);
        }
        public void Random_Record_Sector(long sector)
        {            
            RecordWrite.WriteLine(sector);
        }
        public void Order_Record_Mode(int test_mode,int data_mode,int block_size,int size,int thread_num,int circle_num)
        {
            RecordWrite.WriteLine(test_mode);
            RecordWrite.WriteLine(data_mode);
            RecordWrite.WriteLine(block_size);
            RecordWrite.WriteLine(size);
            RecordWrite.WriteLine(thread_num);
            RecordWrite.WriteLine(circle_num);
        }        
        public void Finish()
        {
            RecordWrite.Close();
            RecordStream.Close();
        }
    }
}
