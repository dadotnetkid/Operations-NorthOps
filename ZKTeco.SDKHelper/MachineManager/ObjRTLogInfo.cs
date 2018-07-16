using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKTeco.SDK.Model;

namespace ZKTeco.SDK.MachineManager
{
   public class ObjRTLogInfo
    {
        public string CardNo { get; set; }

        public DateTime Date { get; set; }

        public int DevID { get; set; }

        public string DoorID { get; set; }

        public string DoorStatus { get; set; }

        public EventType EType { get; set; }

        public string Index { get; set; }

        public string InOutStatus { get; set; }

        public string IP { get; set; }

        public string OnlineState { get; set; }

        public string Pin { get; set; }

        public string RelayState { get; set; }

        public string StatusInfo { get; set; }

        public string VerifyType { get; set; }

        public string WarningStatus { get; set; }
    }
}
