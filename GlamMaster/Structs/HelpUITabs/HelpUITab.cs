using System;

namespace GlamMaster.Structs
{
    public class HelpUITab
    {
        public string TabName;
        public Action CallBack;

        public HelpUITab(string TabName, Action CallBack)
        {
            this.TabName = TabName;
            this.CallBack = CallBack;
        }
    }
}
