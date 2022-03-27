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
    //public delegate void PercentHandler(int now);
    //测试
    public partial class DiskTest : Sunny.UI.UIAsideMainFrame
    {
        private DiskSetting diskSetting;
        private Test2 test2;
        private Log log;
        private Logging logging;
        //public PercentHandler GetPercent;
        public DiskTest()
        {
            InitializeComponent();
            diskSetting = new DiskSetting();
            test2 = new Test2();                      
            log = new Log();
            logging = new Logging();
            int pageIndex = 100;
            diskSetting.AddNotifyObserver(new NotifyEventHandler(test2.ReceiveEvent));
            diskSetting.AddProcessAndTimeObserver(new NotifyProcessAndTimeHandler(test2.ReceiveProcessAndTimeEvent));
            diskSetting.AddWrittenAndSpeedObserver(new NotifyWrittenAndSpeedHandler(test2.ReceiveWrittenAndProcessEvent));
            diskSetting.AddSwitchObserver(new SwitchEventHandler(Aside.SelectPage));
            diskSetting.AddLogObserver(new LogEventHandler(log.LogEvent));
            diskSetting.AddLogObserver(new LogEventHandler(logging.LoggingEvent));
            diskSetting.AddAvgSpeedObserver(new AvgSpeedEventHandler(test2.ReceiveAvgSpeedEvent));
            diskSetting.AddCircleNumObserver(new CircleNumHandler(test2.ReceiveCircleNumEvent));
            test2.addSuspendTestOberver(new SuspendTestEventHandler(diskSetting.Get_Suspend_Button_Status_Event));
            test2.addStopTestOberver(new StopTestEventHandler(diskSetting.Get_Stop_Button_Status_Event));
            diskSetting.AddStartTimeObserver(new StartTimeEventHandler(test2.GetStartTimeEvent));
            diskSetting.AddTestEndObserver(new TestEndEventHandler(test2.ReceiveStopTime));
            TreeNode parent = Aside.CreateNode("Setting", pageIndex);           
            Aside.CreateChildNode(parent, AddPage(diskSetting, ++pageIndex));
            
            //Aside.CreateChildNode(parent, AddPage(new Errors(), ++pageIndex));
            Aside.CreateChildNode(parent, AddPage(logging, ++pageIndex));
            //Aside.GetTreeNode(201).Text = "TestShow";
            pageIndex = 200;
            parent = Aside.CreateNode("Test", pageIndex);
            //Aside.Nodes[201].Name = "TestINF";
            TreeNode test_show = Aside.CreateChildNode(parent, AddPage(test2, ++pageIndex));
            test_show.Text = "TestShow";
            Aside.CreateChildNode(parent, AddPage(log, ++pageIndex));
            //Aside.CreateChildNode(parent, AddPage(new Information(), ++pageIndex));
            Aside.SelectPage(101);
        }

        private void Header_MenuItemClick(string itemText, int menuIndex, int pageIndex)
        {
            switch (pageIndex)
            {
                default:
                    Aside.SelectPage(pageIndex);
                    break;
            }
        }

        private void Aside_MenuItemClick(TreeNode node, Sunny.UI.NavMenuItem item, int pageIndex)
        {

        }
    }
}
