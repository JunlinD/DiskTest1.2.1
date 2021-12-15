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
    public partial class Form2 : Sunny.UI.UIAsideMainFrame
    {
        private DiskSetting diskSetting;
        private Test2 test2;
        private Log log;
        private Logging logging;
        //public PercentHandler GetPercent;
        public Form2()
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
            test2.addStopTestOberver(new StopTestEventHandler(diskSetting.Get_Stop_Button_Status_Event));
            diskSetting.AddStartTimeObserver(new StartTimeEventHandler(test2.GetStartTimeEvent));
            TreeNode parent = Aside.CreateNode("Setting", pageIndex);
            Aside.CreateChildNode(parent, AddPage(diskSetting, ++pageIndex));
            Aside.CreateChildNode(parent, AddPage(new Errors(), ++pageIndex));
            Aside.CreateChildNode(parent, AddPage(logging, ++pageIndex));
            pageIndex = 200;
            parent = Aside.CreateNode("Test", pageIndex);
            Aside.CreateChildNode(parent, AddPage(test2, ++pageIndex));
            Aside.CreateChildNode(parent, AddPage(new Test(), ++pageIndex));
            Aside.CreateChildNode(parent, AddPage(log, ++pageIndex));
            Aside.CreateChildNode(parent, AddPage(new Information(), ++pageIndex));
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
    }
}
