using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKTeco.SDK.Model
{
    public class ObjDeviceParam
    {
        // Fields
        private string _BiometricMaxCount = "0:0:0:0:0:0:0:0:0";
        private string _BiometricType = "000000000";
        private string _BiometricUsedCount = "0:0:0:0:0:0:0:0:0";
        private string _BiometricVersion = "0:0:0:0:0:0:0:0:0";

        // Methods
        private string FormatBioProperty(string bioProperty)
        {
            string[] strArray = new string[9];
            string[] strArray2 = bioProperty.Split(new char[] { ':' });
            for (int i = 0; i < strArray2.Length; i++)
            {
                if (i < strArray.Length)
                {
                    strArray[i] = strArray2[i];
                }
            }
            return string.Join(":", strArray);
        }

        // Properties
        public int AcFun { get; set; }

        public int AntiPassback { get; set; }

        public int AuxInCount { get; set; }

        public int AuxOutCount { get; set; }

        public int BackupTime { get; set; }

        public string BiometricMaxCount
        {
            get
            {
                return this.FormatBioProperty(this._BiometricMaxCount);
            }
            set
            {
                this._BiometricMaxCount = this.FormatBioProperty(value);
            }
        }

        public string BiometricType
        {
            get
            {
                return this._BiometricType.ToString().PadRight(9, '0');
            }
            set
            {
                this._BiometricType = value.ToString().PadRight(9, '0');
            }
        }

        public string BiometricUsedCount
        {
            get
            {
                return this.FormatBioProperty(this._BiometricUsedCount);
            }
            set
            {
                this._BiometricUsedCount = this.FormatBioProperty(value);
            }
        }

        public string BiometricVersion
        {
            get
            {
                return this.FormatBioProperty(this._BiometricVersion);
            }
            set
            {
                this._BiometricVersion = this.FormatBioProperty(value);
            }
        }

        public int CardFormatFunOn { get; set; }

        public int CardFun { get; set; }

        public string CompatOldFirmware { get; set; }

        public string ComPwd { get; set; }

        public int DateFormat { get; set; }

        public int DateTime { get; set; }

        public int DeviceID { get; set; }

        public string DeviceName { get; set; }

        public int Door4ToDoor2 { get; set; }

        public string EventTypes { get; set; }

        public int Ext485ReaderFunOn { get; set; }

        public string ExtAuxBoardFunOn { get; set; }

        public int Face1_1Threshold { get; set; }

        public int Face1_NThreshold { get; set; }

        public int FaceCount { get; set; }

        public int FaceFunOn { get; set; }

        public string FingerFunOn { get; set; }

        public string FirmVer { get; set; }

        public int FP1_1Threshold { get; set; }

        public int FP1_NThreshold { get; set; }

        public int FreeTime { get; set; }

        public int FreeType { get; set; }

        public int FvFunOn { get; set; }

        public string GATEIPAddress { get; set; }

        public string IclockSvrFun { get; set; }

        public int InterLock { get; set; }

        public string IPAddress { get; set; }

        public int IsOnlyRFMachine { get; set; }

        public bool IsTFTMachine { get; set; }

        public int KeyPadBeep { get; set; }

        public string language { get; set; }

        public int LockCount { get; set; }

        public int MachineNumber { get; set; }

        public string MachineType { get; set; }

        public int MasterInbio485 { get; set; }

        public int MaxAttLogCount { get; set; }

        public int MaxExtAuxInCount { get; set; }

        public int MaxExtAuxOutCount { get; set; }

        public int MaxExtLockCount { get; set; }

        public int MaxExtReaderCount { get; set; }

        public int MaxUserCount { get; set; }

        public int MaxUserFingerCount { get; set; }

        public int Mifire { get; set; }

        public int MifireId { get; set; }

        public int MifireMustRegistered { get; set; }

        public string MThreshold { get; set; }

        public string NetMask { get; set; }

        public int NetOn { get; set; }

        public int NoDisplayFun { get; set; }

        public int Only1_1Mode { get; set; }

        public int OnlyCheckCard { get; set; }

        public string OverallAntiFunOn { get; set; }

        public string Parameters { get; set; }

        public int PC485AsInbio485 { get; set; }

        public int PinWidth { get; set; }

        public string Platform { get; set; }

        public int ReaderCount { get; set; }

        public int RFCardOn { get; set; }

        public string RS232BaudRate { get; set; }

        public int RS232On { get; set; }

        public int RS485On { get; set; }

        public string SerialNumber { get; set; }

        public int SimpleEventType { get; set; }

        public int TemplateCount { get; set; }

        public int TOMenu { get; set; }

        public int UILanguage { get; set; }

        public int UserCount { get; set; }

        public int UserExtFmt { get; set; }

        public string VerifyStyles { get; set; }

        public int VoiceTipsOn { get; set; }

        public int VOLUME { get; set; }

        public int VRYVH { get; set; }

        public int WatchDog { get; set; }

        public int WIFI { get; set; }

        public int WIFIDHCP { get; set; }

        public int WIFIOn { get; set; }

        public string WirelessAddr { get; set; }

        public string WirelessGateWay { get; set; }

        public string WirelessKey { get; set; }

        public string WirelessMask { get; set; }

        public string WirelessSSID { get; set; }

        public string ZKFPVersion { get; set; }
    }



}
