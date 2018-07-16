using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ZKTeco.SDK.Model
{
    public class Machines
    {
        public Machines()
        {

        }

        public Machines(string ip, ConnectType connectType = ConnectType.Net, int port = 4370, int machineNumber = 1)
        {
            this.IP = ip;
            this.ConnectType = connectType;
            this.Port = port;
            this.MachineNumber = machineNumber;

        }
        public int AccFun { get; set; }
        public int ACFun { get; set; }
        public int acpanel_type { get; set; }
        public string agent_ipaddress { get; set; }
        public string alg_ver { get; set; }
        public string alias { get; set; }
        public int area_id { get; set; }
        public int aux_in_count { get; set; }
        public int aux_out_count { get; set; }
        public bool BatchUpdate { get; set; }
        public int Baudrate { get; set; }
        public string BiometricMaxCount { get; set; }
        public string BiometricType { get; set; }
        public string BiometricUsedCount { get; set; }
        public string BiometricVersion { get; set; }
        public string brightness { get; set; }
        public int CardFun { get; set; }
        public string change_operator { get; set; }
        public DateTime? change_time { get; set; }
        public string city { get; set; }
        public int com_address { get; set; }
        public int com_port { get; set; }
        public int comm_type { get; set; }
        public string CommPassword { get; set; }
        public string CompatOldFirmware { get; set; }
        public ConnectType ConnectType { get; set; }
        public string create_operator { get; set; }
        public DateTime? create_time { get; set; }
        public short DateFormat { get; set; }
        public int delay { get; set; }
        public string delete_operator { get; set; }
        public DateTime? delete_time { get; set; }
        public string device_name { get; set; }
        public int device_type { get; set; }
        public byte[] deviceOption { get; set; }
        public SDKType DevSDKType { get; set; }
        public int door_count { get; set; }
        public int dstime_id { get; set; }
        public bool Enabled { get; set; }
        public bool encrypt { get; set; }
        public string EventTypes { get; set; }
        public int Ext485ReaderFunOn { get; set; }
        public string ExtAuxBoardFunOn { get; set; }
        public int FaceCount { get; set; }
        public int FaceFunOn { get; set; }
        public int fingercount { get; set; }
        public string FingerFunOn { get; set; }
        public string FirmwareVersion { get; set; }
        public string flash_size { get; set; }
        public bool four_to_two { get; set; }
        public int fp_mthreshold { get; set; }
        public int FpVersion { get; set; }
        public string free_flash_size { get; set; }
        public int FreeTime { get; set; }
        public int FreeType { get; set; }
        public int fvcount { get; set; }
        public int FvFunOn { get; set; }
        public string gateway { get; set; }
        public int ID { get; set; }
        public short Idle { get; set; }
        public short InOutRecordWarn { get; set; }
        public string IP { get; set; }
        public string ipaddress { get; set; }
        public string is_tft { get; set; }
        public bool IsHost { get; set; }
        public int IsIfChangeConfigServer2 { get; set; }
        public int IsOnlyRFMachine { get; set; }
        public bool IsTFTMachine { get; set; }
        public bool IsWireless { get; set; }
        public int KeyPadBeep { get; set; }
        public string language { get; set; }
        public DateTime? last_activity { get; set; }
        public string lng_encode { get; set; }
        public short LockControl { get; set; }
        public string log_stamp { get; set; }
        public string MachineAlias { get; set; }
        public int MachineNumber { get; set; }
        public string main_time { get; set; }
        public int managercount { get; set; }
        public int max_attlog_count { get; set; }
        public int max_comm_count { get; set; }
        public int max_comm_size { get; set; }
        public int max_finger_count { get; set; }
        public int max_user_count { get; set; }
        public int MaxExtAuxInCount { get; set; }
        public int MaxExtAuxOutCount { get; set; }
        public int MaxExtLockCount { get; set; }
        public int MaxExtReaderCount { get; set; }
        public int Mifire { get; set; }
        public int MifireId { get; set; }
        public int MifireMustRegistered { get; set; }
        public int NetOn { get; set; }
        public int NoDisplayFun { get; set; }
        public string oem_vendor { get; set; }
        public int Only1_1Mode { get; set; }
        public int OnlyCheckCard { get; set; }
        public byte[] oplog_stamp { get; set; }
        public byte[] ParamValues { get; set; }
        public byte[] photo_stamp { get; set; }
        public string PhotoStamp { get; set; }
        public int PinWidth { get; set; }
        public string platform { get; set; }
        public int Port { get; set; }
        public int ProduceKind { get; set; }
        public string ProductType { get; set; }
        public short Purpose { get; set; }
        public int pushver { get; set; }
        public int reader_count { get; set; }
        public bool realtime { get; set; }
        public int RFCardOn { get; set; }
        public int RS232On { get; set; }
        public int RS485On { get; set; }
        public int SecretCount { get; set; }
        public int SerialPort { get; set; }
        public int simpleEventType { get; set; }
        public string sn { get; set; }
        public int status { get; set; }
        public string subnet_mask { get; set; }
        public bool SupportFace { get; }
        public bool SupportFingerprint { get; }
        public bool SupportFingerVein { get; }
        public bool SupportRFCard { get; }
        public List<int> SupportVerifyTypes { get; }
        public bool sync_time { get; set; }
        public int TOMenu { get; set; }
        public string trans_times { get; set; }
        public int transaction_count { get; set; }
        public int TransInterval { get; set; }
        public int TZAdj { get; set; }
        public string UDataTableDescription { get; set; }
        public short UILanguage { get; set; }
        public string UpdateDB { get; set; }
        public int usercount { get; set; }
        public int UserExtFmt { get; set; }
        public string VerifyStyles { get; set; }
        public string video_login { get; set; }
        public short Voice { get; set; }
        public int VoiceTipsOn { get; set; }
        public string volume { get; set; }
        public int VOLUME { get; set; }
        public int VRYVH { get; set; }
        public int WIFI { get; set; }
        public int WIFIDHCP { get; set; }
        public int WIFIOn { get; set; }
        public string WirelessAddr { get; set; }
        public string WirelessGateWay { get; set; }
        public string WirelessKey { get; set; }
        public string WirelessMask { get; set; }
        public string WirelessSSID { get; set; }
        public ObjDevice ToDeviceModel()
        {
            return new ObjDevice
            {
                Area_id = this.area_id,
                BatchUpdate = this.BatchUpdate,
                Baudrate = this.Baudrate,
                CardFun = this.CardFun,
                CommPassword = this.CommPassword,
                CompatOldFirmware = this.CompatOldFirmware,
                ConnectType = this.ConnectType,
                DeviceName = this.device_name,
                DeviceType = this.device_type,
                DevSDKType = this.DevSDKType,
                DSTime_id = this.dstime_id,
                Enabled = this.Enabled,
                FaceFunOn = this.FaceFunOn,
                FingerFunOn = this.FingerFunOn,
                FPVersion = this.FpVersion,
                FvFunOn = this.FvFunOn,
                ID = this.ID,
                IP = this.IP,
                IsTFTMachine = this.IsTFTMachine,
                MachineAlias = this.MachineAlias,
                MachineNumber = this.MachineNumber,
                Platform = this.platform,
                Port = this.Port,
                SerialNumber = this.sn,
                SerialPort = this.SerialPort,
                simpleEventType = this.simpleEventType,
                UserExtFmt = this.UserExtFmt,
                ZKFPVersion10 = this.FpVersion == 10,
                ExtAuxBoardFunOn = this.ExtAuxBoardFunOn
            };
        }

    }
}
