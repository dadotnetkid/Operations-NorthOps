using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKTeco.SDK.Model
{
    public class ObjDevice
    {
        // Methods
        public ObjDevice()
        {
            this.ID = 1;
            this.ConnectType = 0;
            this.MachineAlias = "1";
            this.IP = "127.0.0.1";
            this.Port = 0x1112;
            this.SerialPort = 1;
            this.MachineNumber = 1;
            this.Baudrate = 0x1c200;
            this.CommPassword = "";
            this.Enabled = true;
            this.ZKFPVersion10 = true;
            this.Area_id = 0;
            this.DSTime_id = 1;
            this.DeviceType = 9;
            this.simpleEventType = 0;
            this.FvFunOn = 0;
            this.DevSDKType = SDKType.Undefined;
            this.SerialNumber = "";
        }

        public override string ToString()
        {
            if (this.ConnectType == ConnectType.Net)
            {
                return (this.IP.ToLower() + ":" + this.Port);
            }
            return (this.SerialPort.ToString() + "-" + this.MachineNumber.ToString());
        }

        // Properties
        public int Area_id { get; set; }

        public bool BatchUpdate { get; set; }

        public int Baudrate { get; set; }

        public int CardFun { get; set; }

        public string CommPassword { get; set; }

        public string CompatOldFirmware { get; set; }

        public ConnectType ConnectType { get; set; }

        public string DeviceName { get; set; }

        public int DeviceType { get; set; }

        public SDKType DevSDKType { get; set; }

        public int DSTime_id { get; set; }

        public bool Enabled { get; set; }

        public string ExtAuxBoardFunOn { get; set; }

        public int FaceFunOn { get; set; }

        public string FingerFunOn { get; set; }

        public decimal FirmwareVersion { get; set; }

        public int FPVersion { get; set; }

        public int FvFunOn { get; set; }

        public int ID { get; set; }

        public string IP { get; set; }

        public bool IsTFTMachine { get; set; }

        public int LockFunOn { get; set; }

        public string MachineAlias { get; set; }

        public int MachineNumber { get; set; }

        public string Platform { get; set; }

        public int Port { get; set; }

        public string SerialNumber { get; set; }

        public int SerialPort { get; set; }

        public int simpleEventType { get; set; }

        public int UserExtFmt { get; set; }

        public bool ZKFPVersion10 { get; set; }
    }
}
