using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using zkemkeeper;
using ZKTeco.SDK.Model;
using static ZKTeco.SDK.MachineManager.ComTokenManager;

namespace ZKTeco.SDK.MachineManager
{
    public class STDDevComm
    {
        private CZKEMClass AloneSDK;
        public Machines dev { get; set; }
        private bool isConnected;
        private bool bolEventRegistered;
        private int _LockFunOn;
        private readonly int OperationSuccess = 1;
        private readonly int ErrUnknown = -999;
        private object ThreadLock = new object();
        private readonly int ErrOperationTimeOut = -7;
        private int LockTimeOut = 0xbb8;
        private static bool isPrint = false;
        public bool IsConnected { get; set; }
        public decimal FirmwareVersion { get; set; }

        public STDDevComm(Machines device)
        {
            this.dev = device;
            Thread thread = new Thread(new ThreadStart(this.CreadSDKObject));
            thread.Start();
            thread.Join();
        }
        private void CreadSDKObject()
        {

            this.AloneSDK = new CZKEMClass();

            this.AloneSDK.OnConnected += (new _IZKEMEvents_OnConnectedEventHandler(this.AloneSDK_OnConnected));
            this.AloneSDK.OnDisConnected += (new _IZKEMEvents_OnDisConnectedEventHandler(this.AloneSDK_OnDisConnected));
            this.AloneSDK.OnAlarm += (new _IZKEMEvents_OnAlarmEventHandler(this.AloneSDK_OnAlarm));
            this.AloneSDK.OnDoor += (new _IZKEMEvents_OnDoorEventHandler(this.AloneSDK_OnDoor));
            this.AloneSDK.OnAttTransactionEx += (new _IZKEMEvents_OnAttTransactionExEventHandler(this.AloneSDK_OnAttTransactionEx));
            this.AloneSDK.OnVerify += (new _IZKEMEvents_OnVerifyEventHandler(this.AloneSDK_OnVerify));
            this.AloneSDK.OnEnrollFinger += (new _IZKEMEvents_OnEnrollFingerEventHandler(this.AloneSDK_OnEnrollFinger));
            this.AloneSDK.OnEnrollFingerEx += (new _IZKEMEvents_OnEnrollFingerExEventHandler(this.AloneSDK_OnEnrollFingerEx));
            this.AloneSDK.OnFingerFeature += (new _IZKEMEvents_OnFingerFeatureEventHandler(this.AloneSDK_OnFingerFeature));
            this.AloneSDK.OnHIDNum += (new _IZKEMEvents_OnHIDNumEventHandler(this.AloneSDK_OnHIDNum));
        }

        #region Events
        private void AloneSDK_OnAlarm(int AlarmType, int EnrollNumber, int Verified)
        {
            if (null != this.OnAlarm)
            {
                this.OnAlarm(AlarmType, EnrollNumber, Verified);
            }
        }

        private void AloneSDK_OnAttTransactionEx(string EnrollNumber, int IsInValid, int AttState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second, int WorkCode)
        {
            if (null != this.OnAttTransaction)
            {
                this.OnAttTransaction(EnrollNumber, IsInValid, AttState, VerifyMethod, Year, Month, Day, Hour, Minute, Second, WorkCode);
            }
        }

        private void AloneSDK_OnConnected()
        {
            this.isConnected = true;
            if (null != this.OnConnected)
            {
                this.OnConnected();
            }
        }

        private void AloneSDK_OnDisConnected()
        {
            this.isConnected = false;
            this.bolEventRegistered = false;
            if (null != this.OnDisconnected)
            {
                this.OnDisconnected();
            }
        }

        private void AloneSDK_OnDoor(int EventType)
        {
            if (null != this.OnDoor)
            {
                this.OnDoor(EventType);
            }
        }

        private void AloneSDK_OnEMData(int DataType, int DataLen, ref sbyte DataBuffer)
        {
            if (null != this.OnEMData)
            {
                this.OnEMData(DataType, DataLen, ref DataBuffer);
            }
        }

        private void AloneSDK_OnEmptyCard(int ActionResult)
        {
            if (null != this.OnEmptyCard)
            {
                this.OnEmptyCard(ActionResult);
            }
        }

        private void AloneSDK_OnEnrollFinger(int EnrollNumber, int FingerIndex, int ActionResult, int TemplateLength)
        {
            if (null != this.OnEnrollFinger)
            {
                this.OnEnrollFinger(EnrollNumber.ToString(), FingerIndex, ActionResult, TemplateLength);
            }
        }

        private void AloneSDK_OnEnrollFingerEx(string EnrollNumber, int FingerIndex, int ActionResult, int TemplateLength)
        {
            if (null != this.OnEnrollFinger)
            {
                this.OnEnrollFinger(EnrollNumber, FingerIndex, ActionResult, TemplateLength);
            }
        }

        private void AloneSDK_OnFinger()
        {
            if (null != this.OnFinger)
            {
                this.OnFinger();
            }
        }

        private void AloneSDK_OnFingerFeature(int Score)
        {
            if (null != this.OnFingerFeature)
            {
                this.OnFingerFeature(Score);
            }
        }

        private void AloneSDK_OnHIDNum(int CardNumber)
        {
            if (null != this.OnHIDNum)
            {
                this.OnHIDNum(CardNumber);
            }
        }

        private void AloneSDK_OnNewUser(int EnrollNumber)
        {
            if (null != this.OnNewUser)
            {
                this.OnNewUser(EnrollNumber);
            }
        }

        private void AloneSDK_OnVerify(int UserID)
        {
            if (null != this.OnVerify)
            {
                this.OnVerify(UserID);
            }
        }

        private void AloneSDK_OnWriteCard(int EnrollNumber, int ActionResult, int Length)
        {
            if (null != this.OnWriteCard)
            {
                this.OnWriteCard(EnrollNumber, ActionResult, Length);
            }
        }
        public event AlarmEventHandler OnAlarm;

        public event AttTransactionEventHandler OnAttTransaction;

        public event ConnectedEventHandler OnConnected;

        public event DisconnectedEventHandler OnDisconnected;

        public event DoorEventHandler OnDoor;

        public event EMDataEventHandler OnEMData;

        public event EmptyCardEventHander OnEmptyCard;

        public event STD_OnEnrollFinger OnEnrollFinger;

        public event FingerEventHandler OnFinger;

        public event STD_FingerFeature OnFingerFeature;

        public event HIDNumEventHandler OnHIDNum;

        public event NewUserEventHandler OnNewUser;

        public event VerifyEventHandler OnVerify;

        public event WriteCardEventHandler OnWriteCard;
        #endregion
        public int GetAllTransaction(out List<Transactions> lstTransaction)
        {
            Exception exception;
            int operationSuccess = this.OperationSuccess;
            lstTransaction = new List<Transactions>();
            if (!Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                return (this.ErrOperationTimeOut - 0x186a0);
            }
            try
            {
                switch (this.dev.ConnectType)
                {
                    case ConnectType.Com:
                        {
                            ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                            if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                            {
                                break;
                            }
                            try
                            {
                                operationSuccess = this.GetAllTransactionWithoutLock(out lstTransaction);
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown - 0x186a0;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetAllTransactionCOM " + exception.Message);
                            }
                            Monitor.Exit(comToken);
                            goto Label_011F;
                        }
                    case ConnectType.Net:
                        operationSuccess = this.GetAllTransactionWithoutLock(out lstTransaction);
                        goto Label_011F;

                    case ConnectType.Usb:
                        operationSuccess = this.GetAllTransactionWithoutLock(out lstTransaction);
                        goto Label_011F;

                    default:
                        goto Label_011F;
                }
                operationSuccess = this.ErrOperationTimeOut - 0x186a0;
            }
            catch (Exception exception2)
            {
                exception = exception2;
                operationSuccess = this.ErrUnknown - 0x186a0;
                //  //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetAllTransaction " + exception.Message);
            }
            Label_011F:
            Monitor.Exit(this.ThreadLock);
            return operationSuccess;
        }
        protected int GetAllTransactionWithoutLock(out List<Transactions> lstTransaction)
        {
            int num2;
            int num4;
            int num5;
            int num6;
            int num7;
            int num8;
            int num9;
            int num10;
            int num11;
            int num12;
            bool flag;
            Transactions action;
            bool flag4;
            int operationSuccess = this.OperationSuccess;
            int dwWorkCode = 0;
            string dwEnrollNumber = "";
            lstTransaction = new List<Transactions>();
            if (!this.IsConnected)
            {
                operationSuccess = this.ConnectWithoutLock();
                if (operationSuccess != this.OperationSuccess)
                {
                    return (operationSuccess - 0x186a0);
                }
                operationSuccess = this.OperationSuccess;
            }
            operationSuccess = this.GetDeviceStatus(MachineDataStatusCode.AttRecordCount, out num2);
            if ((operationSuccess < 0) || (num2 <= 0))
            {
                return operationSuccess;
            }
            bool flag3 = this.dev.UserExtFmt > 0;
            bool flag2 = this.AloneSDK.IsTFTMachine(this.dev.MachineNumber);
            int num3 = num4 = num5 = num6 = num7 = num8 = num9 = num10 = num11 = num12 = 0;
            this.AloneSDK.DisableDeviceWithTimeOut(this.dev.MachineNumber, (this.dev.ConnectType == 0) ? (num2 * 2) : (num2 + 2));
            if (!this.AloneSDK.ReadAllGLogData(this.dev.MachineNumber))
            {
                this.AloneSDK.GetLastError(ref operationSuccess);
                this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                goto Label_03A4;
            }
            this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
            goto Label_039B;
            Label_02B8:
            if (num6 == 0xfc)
            {
                action.EventType = EventType.Type252;
            }
            else if (num6 == 0xfd)
            {
                action.EventType = EventType.Type253;
            }
            else if (num6 == 0xfe)
            {
                action.EventType = EventType.Type254;
            }
            else
            {
                action.EventType = (((num6 < 0) || (num6 > 5)) && (0xff != ((byte)num6))) ? EventType.ValidNoRight : EventType.Type0;
            }
            action.InOutState = (InOutState)((((num6 << 4) | ((((byte)num6) == 0xff) ? 2 : (num6 + 10)))) & 15);
            action.LogDateTime = new DateTime(num7, num8, num9, num10, num11, num12);//.ToString("yyyy-MM-dd HH:mm:ss");
            lstTransaction.Add(action);
            Label_039B:
            flag4 = true;
            if (flag2)
            {

                flag = this.AloneSDK.SSR_GetGeneralLogData(this.dev.MachineNumber, out dwEnrollNumber, out num5, out num6, out num7, out num8, out num9, out num10, out num11, out num12, ref dwWorkCode);
            }
            else
            {
                flag = this.AloneSDK.GetGeneralExtLogData(this.dev.MachineNumber, ref num3, ref num5, ref num6, ref num7, ref num8, ref num9, ref num10, ref num11, ref num12, ref dwWorkCode, ref num4);
                dwEnrollNumber = num3.ToString();
            }

            Debug.WriteLine($"{dwEnrollNumber}:{num5 }:{num6}:{num7}:{num8}:{num9}:{num10}:{num11}:{num12}:{dwWorkCode}  ");

            if (flag)
            {
                action = new Transactions
                {
                    Cardno = "",
                    DoorID = "1",
                    Pin = dwEnrollNumber
                };
                if (!(!flag3 || flag2))
                {
                    action.Verified = ((num5 >= 0) && (num5 <= 20)) ? ((VerifiedType)num5) : VerifiedType.Type0;
                }
                else
                {
                    switch (num5)
                    {
                        case 0:
                            action.Verified = VerifiedType.PW;
                            goto Label_02B8;

                        case 1:
                            action.Verified = VerifiedType.FP;
                            goto Label_02B8;

                        case 2:
                            action.Verified = VerifiedType.RF;
                            goto Label_02B8;

                        case 15:
                            action.Verified = VerifiedType.FACE;
                            goto Label_02B8;
                    }
                    action.Verified = VerifiedType.Others;
                    if (Enum.IsDefined(typeof(VerifiedType), num5))
                    {
                        action.Verified = (VerifiedType)num5;
                    }
                }
                goto Label_02B8;
            }
            Label_03A4:
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }
        protected int ConnectWithoutLock()
        {
            int operationSuccess = this.OperationSuccess;
            if (!this.isConnected)
            {
                try
                {
                    this.AloneSDK.SetCommProType(1);
                    if (!string.IsNullOrEmpty(this.dev.CommPassword))
                    {
                        this.AloneSDK.SetCommPassword(int.Parse(this.dev.CommPassword));
                    }
                    switch (this.dev.ConnectType)
                    {
                        case ConnectType.Com:
                            this.AloneSDK.PullMode = 1;
                            this.isConnected = this.AloneSDK.Connect_Com(this.dev.SerialPort, this.dev.MachineNumber, this.dev.Baudrate);
                            break;

                        case ConnectType.Net:
                            this.AloneSDK.PullMode = 0;
                            this.isConnected = this.AloneSDK.Connect_Net(this.dev.IP, this.dev.Port);
                            break;

                        case ConnectType.Usb:
                            this.AloneSDK.PullMode = 1;
                            this.isConnected = this.AloneSDK.Connect_USB(this.dev.MachineNumber);
                            break;
                    }
                    if (!this.isConnected)
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                        return operationSuccess;
                    }
                    this.GetParamInConn();
                    return operationSuccess;
                }
                catch (Exception exception)
                {
                    operationSuccess = this.ErrUnknown;
                    //
                }
                return operationSuccess;
            }
            this.GetParamInConn();
            return operationSuccess;
        }
        private void GetParamInConn()
        {
            int num;
            string str;
            if (this.AloneSDK.GetSysOption(this.dev.MachineNumber, "~UserExtFmt", out str) && int.TryParse(str, out num))
            {
                this.dev.UserExtFmt = num;
            }
            this.bolEventRegistered = true;
            if (!(this.AloneSDK.GetSysOption(this.dev.MachineNumber, "BuildVersion", out str) && !string.IsNullOrWhiteSpace(str)))
            {
                this.bolEventRegistered = this.AloneSDK.GetFirmwareVersion(this.dev.MachineNumber, ref str);
            }
            if (this.bolEventRegistered)
            {
                decimal result = 0M;
                if (str.Length > 8)
                {
                    str = str.Substring(4, 4);
                    str = str.EndsWith(".") ? str.Remove(str.Length - 1) : str;
                    decimal.TryParse(str, out result);
                }
                else if (str.Length > 4)
                {
                    decimal.TryParse(str.Substring(4, str.Length), out result);
                }
                this.FirmwareVersion = result;
            }
            if (this.AloneSDK.GetSysOption(this.dev.MachineNumber, "~Platform", out str))
            {
                this.dev.platform = str;
            }
            this.dev.IsTFTMachine = this.AloneSDK.IsTFTMachine(this.dev.MachineNumber);
            if (!this.AloneSDK.RegEvent(this.dev.MachineNumber, 0xffff))
            {
                this.bolEventRegistered = false;
            }
            else
            {
                this.bolEventRegistered = true;
            }
            if (this.AloneSDK.GetSysOption(this.dev.MachineNumber, "FaceFunOn", out str) && int.TryParse(str, out num))
            {
                this.dev.FaceFunOn = num;
            }
            num = 1;
            if (this.AloneSDK.GetCardFun(this.dev.MachineNumber, ref num))
            {
                this.dev.CardFun = num;
            }
            num = 0;
            if (this.AloneSDK.GetSysOption(this.dev.MachineNumber, "~LockFunOn", out str) && int.TryParse(str, out num))
            {
                this._LockFunOn = num;
            }
            if (this.AloneSDK.GetSysOption(this.dev.MachineNumber, "CompatOldFirmware", out str))
            {
                this.dev.CompatOldFirmware = str;
            }
            if (this.AloneSDK.GetSysOption(this.dev.MachineNumber, "BiometricType", out str))
            {
                this.dev.BiometricType = str;
            }
            if (this.AloneSDK.GetSysOption(this.dev.MachineNumber, "BiometricVersion", out str))
            {
                this.dev.BiometricVersion = str;
            }
            if (this.AloneSDK.GetSysOption(this.dev.MachineNumber, "BiometricMaxCount", out str))
            {
                this.dev.BiometricMaxCount = str;
            }
            if (this.AloneSDK.GetSysOption(this.dev.MachineNumber, "BiometricUsedCount", out str))
            {
                this.dev.BiometricUsedCount = str;
            }
            if (null != this.OnConnected)
            {
                this.OnConnected();
            }
        }
        public int GetDeviceStatus(MachineDataStatusCode code, out int count)
        {
            int operationSuccess = this.OperationSuccess;
            count = 0;
            if (!this.IsConnected)
            {
                operationSuccess = this.Connect();
                if (operationSuccess < 0)
                {
                    return operationSuccess;
                }
                operationSuccess = this.OperationSuccess;
            }
            if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                Exception exception;
                try
                {
                    if (this.dev.ConnectType == 0)
                    {
                        ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                        if (Monitor.TryEnter(comToken, this.LockTimeOut))
                        {
                            try
                            {
                                this.AloneSDK.DisableDeviceWithTimeOut(this.dev.MachineNumber, 5);
                                if (!this.AloneSDK.GetDeviceStatus(this.dev.MachineNumber, (int)code, ref count))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                                this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                            }
                            finally
                            {
                                Monitor.Exit(comToken);
                            }
                        }
                        else
                        {
                            operationSuccess = this.ErrOperationTimeOut;
                        }
                    }
                    else
                    {
                        this.AloneSDK.DisableDeviceWithTimeOut(this.dev.MachineNumber, 5);
                        if (!this.AloneSDK.GetDeviceStatus(this.dev.MachineNumber, (int)code, ref count))
                        {
                            this.AloneSDK.GetLastError(ref operationSuccess);
                        }
                        this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                }
                finally
                {
                    Monitor.Exit(this.ThreadLock);
                }
            }
            else
            {
                operationSuccess = this.ErrOperationTimeOut;
            }
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }
        public int Connect()
        {
            Exception exception;
            int operationSuccess = this.OperationSuccess;
            if (!Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                operationSuccess = this.ErrOperationTimeOut;
                goto Label_011C;
            }
            try
            {
                switch (this.dev.ConnectType)
                {
                    case ConnectType.Com:
                        {
                            ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                            if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                            {
                                break;
                            }
                            try
                            {
                                operationSuccess = this.ConnectWithoutLock();
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                            }
                            Monitor.Exit(comToken);
                            goto Label_0103;
                        }
                    case ConnectType.Net:
                        operationSuccess = this.ConnectWithoutLock();
                        goto Label_0103;

                    case ConnectType.Usb:
                        operationSuccess = this.ConnectWithoutLock();
                        goto Label_0103;

                    default:
                        goto Label_0103;
                }
                operationSuccess = this.ErrOperationTimeOut;
            }
            catch (Exception exception2)
            {
                exception = exception2;
                operationSuccess = this.ErrUnknown;
            }
            Label_0103:
            Monitor.Exit(this.ThreadLock);
            Label_011C:
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }

        public int GetSysOption(string OptionName, out string Value)
        {
            int operationSuccess = this.OperationSuccess;
            Value = string.Empty;
            if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                Exception exception;
                try
                {
                    if (this.dev.ConnectType == 0)
                    {
                        ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                        if (Monitor.TryEnter(comToken, this.LockTimeOut))
                        {
                            try
                            {
                                operationSuccess = this.GetSysOptionWithoutLock(OptionName, out Value);
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                            }
                            finally
                            {
                                Monitor.Exit(comToken);
                            }
                        }
                        else
                        {
                            operationSuccess = this.ErrOperationTimeOut;
                        }
                    }
                    else
                    {
                        operationSuccess = this.GetSysOptionWithoutLock(OptionName, out Value);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                }
                finally
                {
                    Monitor.Exit(this.ThreadLock);
                }
            }
            else
            {
                operationSuccess = this.ErrOperationTimeOut;
            }
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }

        protected int GetSysOptionWithoutLock(string OptionName, out string Value)
        {
            int operationSuccess = this.OperationSuccess;
            Value = string.Empty;
            if (!this.IsConnected)
            {
                operationSuccess = this.ConnectWithoutLock();
                if (operationSuccess != this.OperationSuccess)
                {
                    return operationSuccess;
                }
            }
            try
            {
                if (!this.AloneSDK.GetSysOption(this.dev.MachineNumber, OptionName, out Value))
                {
                    this.AloneSDK.GetLastError(ref operationSuccess);
                }
            }
            catch (Exception)
            {
                Value = string.Empty;
            }
            return operationSuccess;
        }
        public int GetDeviceIP(out string IPAddr)
        {
            int operationSuccess = this.OperationSuccess;
            IPAddr = string.Empty;
            if (!this.IsConnected)
            {
                operationSuccess = this.Connect();
                if (operationSuccess < 0)
                {
                    return operationSuccess;
                }
                operationSuccess = this.OperationSuccess;
            }
            if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                Exception exception;
                try
                {
                    if (this.dev.ConnectType == 0)
                    {
                        ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                        if (Monitor.TryEnter(comToken, this.LockTimeOut))
                        {
                            try
                            {
                                if (!this.AloneSDK.GetDeviceIP(this.dev.MachineNumber, ref IPAddr))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                            }
                            finally
                            {
                                Monitor.Exit(comToken);
                            }
                        }
                        else
                        {
                            operationSuccess = this.ErrOperationTimeOut;
                        }
                    }
                    else if (!this.AloneSDK.GetDeviceIP(this.dev.MachineNumber, ref IPAddr))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                }
                finally
                {
                    Monitor.Exit(this.ThreadLock);
                }
            }
            else
            {
                operationSuccess = this.ErrOperationTimeOut;
            }
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }

        public int STD_GetDeviceParam(Machines Model)
        {
            bool flag;
            int num3;
            string str;
            IPAddress address;
            IPAddress address2;
            int num2 = 0;
            ObjDeviceParam param = new ObjDeviceParam();

            int sysOption = num2;

            sysOption = this.GetSysOption("BiometricType", out str);
            if (sysOption >= num2)
            {
                param.BiometricType = str;
            }

            sysOption = this.GetSysOption("BiometricVersion", out str);
            if (sysOption >= num2)
            {
                param.BiometricVersion = str;
            }

            sysOption = this.GetSysOption("BiometricMaxCount", out str);
            if (sysOption >= num2)
            {
                param.BiometricMaxCount = str;
            }

            sysOption = this.GetSysOption("BiometricUsedCount", out str);
            if (sysOption >= num2)
            {
                param.BiometricUsedCount = str;
            }
            if (isPrint)
            {
                ////SysLogServer.WriteLogs(string.Concat(new object[] { "生物模人员数量：", param.BiometricUsedCount, "|", str, "|", sysOption }));
            }
            param.PinWidth = this.PinWidth;
            param.UserExtFmt = this.Device.UserExtFmt;
            if (isPrint)
            {
                //SysLogServer.WriteLogs(string.Concat(new object[] { "获取工号长度与验证模式完成：", param.UserExtFmt, "|", param.PinWidth, "|", sysOption }));
            }
            sysOption = this.IsTFTMachine(out flag);
            if (sysOption >= num2)
            {
                param.IsTFTMachine = flag;
            }
            if (isPrint)
            {
                //SysLogServer.WriteLogs(string.Concat(new object[] { "是否为彩屏机：", param.IsTFTMachine.ToString(), "|", str, "|", sysOption }));
            }
            sysOption = this.GetSysOption("~Platform", out str);
            if (sysOption >= num2)
            {
                param.Platform = str;
            }
            if (isPrint)
            {
                //SysLogServer.WriteLogs(string.Concat(new object[] { "获取平台：", param.Platform.ToString(), "|", str, "|", sysOption }));
            }
            sysOption = this.GetSysOption("~DeviceName", out str);
            if (sysOption >= num2)
            {
                param.DeviceName = str;
            }
            if (isPrint)
            {
                //SysLogServer.WriteLogs(string.Concat(new object[] { "获取设备名称：", param.DeviceName, "|", str, "|", sysOption }));
            }
            sysOption = this.GetDeviceStatus(MachineDataStatusCode.TemplateCapacity, out num3);
            if (sysOption >= num2)
            {
                param.MaxUserFingerCount = num3;
            }
            if (isPrint)
            {
                //SysLogServer.WriteLogs(string.Concat(new object[] { "获取指纹容量：", param.MaxUserFingerCount.ToString(), "|", str, "|", sysOption }));
            }
            sysOption = this.GetDeviceStatus(MachineDataStatusCode.TemplateCount, out num3);
            if (sysOption >= num2)
            {
                param.TemplateCount = num3;
            }
            if (isPrint)
            {
                //SysLogServer.WriteLogs(string.Concat(new object[] { "获取指纹数：", param.TemplateCount.ToString(), "|", str, "|", sysOption }));
            }
            sysOption = this.GetDeviceStatus(MachineDataStatusCode.FaceTemplateCount, out num3);
            if (sysOption >= num2)
            {
                param.FaceCount = num3;
            }
            if (isPrint)
            {
                //SysLogServer.WriteLogs(string.Concat(new object[] { "获取人脸数：", param.FaceCount.ToString(), "|", str, "|", sysOption }));
            }
            sysOption = this.GetDeviceStatus(MachineDataStatusCode.UserCapacity, out num3);
            if (sysOption >= num2)
            {
                param.MaxUserCount = num3;
            }
            if (isPrint)
            {
                //SysLogServer.WriteLogs(string.Concat(new object[] { "获取用户容量：", param.MaxUserCount.ToString(), "|", str, "|", sysOption }));
            }
            sysOption = this.GetDeviceStatus(MachineDataStatusCode.RegistedUserCount, out num3);
            if (sysOption >= num2)
            {
                param.UserCount = num3;
            }
            if (isPrint)
            {
                //SysLogServer.WriteLogs(string.Concat(new object[] { "获取用户数：", param.UserCount.ToString(), "|", str, "|", sysOption }));
            }
            sysOption = this.GetDeviceACFun(out num3);
            if (sysOption >= num2)
            {
                param.AcFun = num3;
            }
            if (isPrint)
            {
                //SysLogServer.WriteLogs(string.Concat(new object[] { "获取机器是否具有门禁功能：", param.AcFun.ToString(), "|", str, "|", sysOption }));
            }
            sysOption = this.GetSysOption("~LockFunOn", out str);
            if (!((sysOption < num2) || string.IsNullOrWhiteSpace(str)))
            {
                param.AcFun = Convert.ToInt32(str);
            }
            if (isPrint)
            {
                //SysLogServer.WriteLogs(string.Concat(new object[] { "获取机器是否具有门禁功能：", param.AcFun.ToString(), "|", str, "|", sysOption }));
            }
            sysOption = this.GetFirmwareVersion(out str);
            if (sysOption >= num2)
            {
                param.FirmVer = str;
            }
            if (isPrint)
            {
                //SysLogServer.WriteLogs(string.Concat(new object[] { "获取固件版本：", param.FirmVer.ToString(), "|", str, "|", sysOption }));
            }
            sysOption = this.GetSysOption("BuildVersion", out str);
            if (!((sysOption < num2) || string.IsNullOrWhiteSpace(str)))
            {
                param.FirmVer = str;
            }

            sysOption = this.GetSerialNumber(out str);
            if (sysOption >= num2)
            {
                param.SerialNumber = str;
            }

            sysOption = this.GetCardFun(out num3);
            if (sysOption >= num2)
            {
                param.CardFun = num3;
            }

            sysOption = this.GetSysOption("FaceFunOn", out str);
            if ((sysOption >= num2) && int.TryParse(str, out num3))
            {
                param.FaceFunOn = num3;
            }

            sysOption = this.GetSysOption("FvFunOn", out str);
            if ((sysOption >= num2) && int.TryParse(str, out num3))
            {
                param.FvFunOn = num3;
            }

            sysOption = this.GetDeviceIP(out str);
            if (sysOption >= num2)
            {
                param.IPAddress = str;
            }

            sysOption = this.GetSysOption("NetMask", out str);
            if ((sysOption >= num2) && IPAddress.TryParse(str, out address))
            {
                param.NetMask = str;
            }

            sysOption = this.GetSysOption("GATEIPAddress", out str);
            if ((sysOption >= num2) && IPAddress.TryParse(str, out address2))
            {
                param.GATEIPAddress = str;
            }

            sysOption = this.GetSysOption("MachineType", out str);
            if (sysOption >= num2)
            {
                param.MachineType = str;
            }

            sysOption = this.GetSysOption("~ZKFPVersion", out str);
            if (sysOption >= num2)
            {
                param.ZKFPVersion = str;
            }

            sysOption = this.GetDeviceInfo(DeviceInfoCode.MachineNumber, out num3);
            if (sysOption >= num2)
            {
                param.MachineNumber = num3;
            }

            sysOption = this.GetDeviceInfo(DeviceInfoCode.BaudRate, out num3);
            if (sysOption >= num2)
            {
                switch (num3)
                {
                    case 0:
                        param.RS232BaudRate = "1200";
                        goto Label_0F6C;

                    case 1:
                        param.RS232BaudRate = "2400";
                        goto Label_0F6C;

                    case 2:
                        param.RS232BaudRate = "4800";
                        goto Label_0F6C;

                    case 3:
                        param.RS232BaudRate = "9600";
                        goto Label_0F6C;

                    case 4:
                        param.RS232BaudRate = "19200";
                        goto Label_0F6C;

                    case 5:
                        param.RS232BaudRate = "38400";
                        goto Label_0F6C;

                    case 6:
                        param.RS232BaudRate = "57600";
                        goto Label_0F6C;
                }
                param.RS232BaudRate = "115200";
            }
            Label_0F6C:

            sysOption = this.GetDeviceInfo(DeviceInfoCode.Only1_1Mode, out num3);
            if (sysOption >= 0)
            {
                param.Only1_1Mode = num3;
            }

            sysOption = this.GetDeviceInfo(DeviceInfoCode.OnlyCheckCard, out num3);
            if (sysOption >= 0)
            {
                param.OnlyCheckCard = num3;
            }

            sysOption = this.GetDeviceInfo(DeviceInfoCode.MifareCardHaveToRegister, out num3);
            if (sysOption >= 0)
            {
                param.MifireMustRegistered = num3;
            }

            sysOption = this.GetSysOption("~RFCardOn", out str);
            if ((sysOption >= num2) && int.TryParse(str, out num3))
            {
                param.RFCardOn = num3;
            }

            sysOption = this.GetSysOption("~MIFARE", out str);
            if ((sysOption >= num2) && int.TryParse(str, out num3))
            {
                param.Mifire = num3;
            }

            sysOption = this.GetSysOption("~MIFAREID", out str);
            if ((sysOption >= num2) && int.TryParse(str, out num3))
            {
                param.MifireId = num3;
            }

            if (param.IsTFTMachine)
            {
                sysOption = this.GetSysOption("FreeTime", out str);
                if ((sysOption >= num2) && int.TryParse(str, out num3))
                {
                    param.FreeTime = num3;
                }

            }
            else
            {
                sysOption = this.GetDeviceInfo(DeviceInfoCode.IdleType, out num3);
                if (sysOption >= 0)
                {
                    param.FreeType = num3;
                }

                sysOption = this.GetDeviceInfo(DeviceInfoCode.IdleTime, out num3);
                if (sysOption >= 0)
                {
                    param.FreeTime = num3;
                }

            }
            sysOption = this.GetDeviceInfo(DeviceInfoCode.DateFormat, out num3);
            if (sysOption >= 0)
            {
                param.DateFormat = num3;
            }

            sysOption = this.GetSysOption("NoDisplayFun", out str);
            if ((sysOption >= num2) && int.TryParse(str, out num3))
            {
                param.NoDisplayFun = num3;
            }

            str = (param.NoDisplayFun == 1) ? "NewVoice" : "NewLng";
            sysOption = this.GetSysOption(str, out str);
            if ((sysOption >= num2) && int.TryParse(str, out num3))
            {
                param.UILanguage = num3;
            }

            sysOption = this.GetDeviceInfo(DeviceInfoCode.VoiceEnabled, out num3);
            if (sysOption >= 0)
            {
                param.VoiceTipsOn = num3;
            }

            sysOption = this.GetSysOption("VRYVH", out str);
            if ((sysOption >= num2) && int.TryParse(str, out num3))
            {
                param.VRYVH = num3;
            }

            sysOption = this.GetSysOption("KeyPadBeep", out str);
            if ((sysOption >= num2) && int.TryParse(str, out num3))
            {
                param.KeyPadBeep = num3;
            }

            sysOption = this.GetSysOption("TOMenu", out str);
            if ((sysOption >= num2) && int.TryParse(str, out num3))
            {
                param.TOMenu = num3;
            }

            sysOption = this.GetSysOption("VOLUME", out str);
            if ((sysOption >= num2) && int.TryParse(str, out num3))
            {
                param.VOLUME = num3;
            }

            sysOption = this.GetDeviceInfo(DeviceInfoCode.NetworkEnabled, out num3);
            if (sysOption >= 0)
            {
                param.NetOn = num3;
            }

            sysOption = this.GetDeviceInfo(DeviceInfoCode.RS232Enabled, out num3);
            if (sysOption >= 0)
            {
                param.RS232On = num3;
            }

            sysOption = this.GetDeviceInfo(DeviceInfoCode.RS485Enabled, out num3);
            if (sysOption >= 0)
            {
                param.RS485On = num3;
            }

            sysOption = this.GetSysOption("CompatOldFirmware", out str);
            if (sysOption >= 0)
            {
                param.CompatOldFirmware = str;
            }

            if (param.CompatOldFirmware == "0")
            {
                sysOption = this.GetSysOption("FingerFunOn", out str);
                if (sysOption >= 0)
                {
                    param.FingerFunOn = str;
                }

            }
            sysOption = this.GetSysOption("WIFI", out str);
            num3 = 0;
            if ((sysOption >= 0) && !int.TryParse(str, out num3))
            {
                num3 = 0;
            }
            param.WIFI = num3;

            sysOption = this.GetSysOption("WifiOn", out str);
            num3 = 0;
            if ((sysOption >= 0) && !int.TryParse(str, out num3))
            {
                num3 = 0;
            }
            param.WIFIOn = num3;

            sysOption = this.GetSysOption("WirelessDHCP", out str);
            num3 = 0;
            if ((sysOption >= 0) && !int.TryParse(str, out num3))
            {
                num3 = 0;
            }
            param.WIFIDHCP = num3;

            sysOption = this.GetSysOption("WirelessSSID", out str);
            if (sysOption >= 0)
            {
                param.WirelessSSID = str;
            }

            sysOption = this.GetSysOption("WirelessKey", out str);
            if (sysOption >= 0)
            {
                param.WirelessKey = str;
            }

            sysOption = this.GetSysOption("WirelessAddr", out str);
            if (sysOption >= 0)
            {
                param.WirelessAddr = str;
            }

            sysOption = this.GetSysOption("WirelessMask", out str);
            if (sysOption >= 0)
            {
                param.WirelessMask = str;
            }

            sysOption = this.GetSysOption("WirelessGateWay", out str);
            if (sysOption >= 0)
            {
                param.WirelessGateWay = str;
            }
            //if (isPrint)
            //{
            //    //SysLogServer.WriteLogs(string.Concat(new object[] { "获取wifi结束|", str, "|", sysOption }));
            //}
            Model.BiometricType = param.BiometricType;
            Model.BiometricVersion = param.BiometricVersion;
            Model.BiometricMaxCount = param.BiometricMaxCount;
            Model.BiometricUsedCount = param.BiometricUsedCount;
            Model.device_type = 0x3e8;
            Model.acpanel_type = 1;
            Model.door_count = 1;
            Model.reader_count = 1;
            Model.aux_in_count = 0;
            Model.aux_out_count = 0;
            Model.platform = param.Platform;
            Model.device_name = param.DeviceName;
            Model.IsOnlyRFMachine = (param.CardFun == 1) ? 1 : 0;
            Model.PinWidth = param.PinWidth;
            Model.IsTFTMachine = param.IsTFTMachine;
            Model.max_finger_count = param.MaxUserFingerCount;
            Model.fingercount = param.TemplateCount;
            Model.FaceCount = param.FaceCount;
            Model.max_user_count = param.MaxUserCount;
            Model.usercount = param.UserCount;
            Model.ACFun = param.AcFun;
            Model.FirmwareVersion = param.FirmVer;
            Model.sn = param.SerialNumber;
            Model.WIFI = param.WIFI;
            Model.WIFIOn = param.WIFIOn;
            Model.WIFIDHCP = param.WIFIDHCP;
            Model.WirelessSSID = param.WirelessSSID;
            Model.WirelessKey = param.WirelessKey;
            Model.WirelessAddr = param.WirelessAddr;
            Model.WirelessMask = param.WirelessMask;
            Model.WirelessGateWay = param.WirelessGateWay;
            Model.CardFun = param.CardFun;
            Model.FaceFunOn = param.FaceFunOn;
            Model.ipaddress = param.IPAddress;
            Model.subnet_mask = param.NetMask;
            Model.gateway = param.GATEIPAddress;
            int.TryParse(param.ZKFPVersion, out num3);
            Model.FpVersion = num3;
            if (param.CardFun == 1)
            {
                Model.FpVersion = 0;
            }
            Model.MachineNumber = param.MachineNumber;
            int.TryParse(param.RS232BaudRate, out num3);
            Model.Baudrate = num3;
            Model.UserExtFmt = param.UserExtFmt;
            Model.Only1_1Mode = param.Only1_1Mode;
            Model.OnlyCheckCard = param.OnlyCheckCard;
            Model.MifireMustRegistered = param.MifireMustRegistered;
            Model.RFCardOn = param.RFCardOn;
            Model.Mifire = param.Mifire;
            Model.MifireId = param.MifireId;
            Model.NetOn = param.NetOn;
            Model.RS232On = param.RS232On;
            Model.RS485On = param.RS485On;
            Model.FreeType = param.FreeType;
            Model.FreeTime = param.FreeTime;
            Model.DateFormat = (short)param.DateFormat;
            Model.NoDisplayFun = param.NoDisplayFun;
            Model.UILanguage = (short)param.UILanguage;
            Model.VoiceTipsOn = param.VoiceTipsOn;
            Model.TOMenu = param.TOMenu;
            Model.VOLUME = param.VOLUME;
            Model.VRYVH = param.VRYVH;
            Model.KeyPadBeep = param.KeyPadBeep;
            Model.CompatOldFirmware = param.CompatOldFirmware;
            Model.FingerFunOn = param.FingerFunOn;
            Model.FvFunOn = param.FvFunOn;
            //if (isPrint)
            //{
            //    //SysLogServer.WriteLogs("赋值给Model结束|" + sysOption);
            //}
            return sysOption;
        }
        public int GetFirmwareVersion(out string firmwareVer)
        {
            int operationSuccess = this.OperationSuccess;
            firmwareVer = string.Empty;
            if (!this.IsConnected)
            {
                operationSuccess = this.Connect();
                if (operationSuccess < 0)
                {
                    return operationSuccess;
                }
                operationSuccess = this.OperationSuccess;
            }
            if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                Exception exception;
                try
                {
                    if (this.dev.ConnectType == 0)
                    {
                        ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                        if (Monitor.TryEnter(comToken, this.LockTimeOut))
                        {
                            try
                            {
                                if (!this.AloneSDK.GetFirmwareVersion(this.dev.MachineNumber, ref firmwareVer))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                            }
                            finally
                            {
                                Monitor.Exit(comToken);
                            }
                        }
                        else
                        {
                            operationSuccess = this.ErrOperationTimeOut;
                        }
                    }
                    else if (!this.AloneSDK.GetFirmwareVersion(this.dev.MachineNumber, ref firmwareVer))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                }
                finally
                {
                    Monitor.Exit(this.ThreadLock);
                }
            }
            else
            {
                operationSuccess = this.ErrOperationTimeOut;
            }
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }
        public int GetDeviceInfo(DeviceInfoCode code, out int value)
        {
            int operationSuccess = this.OperationSuccess;
            value = 0;
            if (!this.IsConnected)
            {
                operationSuccess = this.Connect();
                if (operationSuccess < 0)
                {
                    return operationSuccess;
                }
                operationSuccess = this.OperationSuccess;
            }
            if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                Exception exception;
                try
                {
                    if (this.dev.ConnectType == 0)
                    {
                        ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                        if (Monitor.TryEnter(comToken, this.LockTimeOut))
                        {
                            try
                            {
                                if (!this.AloneSDK.GetDeviceInfo(this.dev.MachineNumber, (int)code, ref value))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                            }
                            finally
                            {
                                Monitor.Exit(comToken);
                            }
                        }
                        else
                        {
                            operationSuccess = this.ErrOperationTimeOut;
                        }
                    }
                    else if (!this.AloneSDK.GetDeviceInfo(this.dev.MachineNumber, (int)code, ref value))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                }
                finally
                {
                    Monitor.Exit(this.ThreadLock);
                }
            }
            else
            {
                operationSuccess = this.ErrOperationTimeOut;
            }
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }
        public int GetSerialNumber(out string SN)
        {
            int operationSuccess = this.OperationSuccess;
            SN = string.Empty;
            if (!this.IsConnected)
            {
                operationSuccess = this.Connect();
                if (operationSuccess < 0)
                {
                    return operationSuccess;
                }
                operationSuccess = this.OperationSuccess;
            }
            if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                Exception exception;
                try
                {
                    if (this.dev.ConnectType == 0)
                    {
                        ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                        if (Monitor.TryEnter(comToken, this.LockTimeOut))
                        {
                            try
                            {
                                if (!this.AloneSDK.GetSerialNumber(this.dev.MachineNumber, out SN))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                            }
                            finally
                            {
                                Monitor.Exit(comToken);
                            }
                        }
                        else
                        {
                            operationSuccess = this.ErrOperationTimeOut;
                        }
                    }
                    else if (!this.AloneSDK.GetSerialNumber(this.dev.MachineNumber, out SN))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                }
                finally
                {
                    Monitor.Exit(this.ThreadLock);
                }
            }
            else
            {
                operationSuccess = this.ErrOperationTimeOut;
            }
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }
        public int GetCardFun(out int CardFun)
        {
            int operationSuccess = this.OperationSuccess;
            CardFun = 0;
            if (!this.IsConnected)
            {
                operationSuccess = this.Connect();
                if (operationSuccess < 0)
                {
                    return operationSuccess;
                }
                operationSuccess = this.OperationSuccess;
            }
            if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                Exception exception;
                try
                {
                    if (this.dev.ConnectType == 0)
                    {
                        ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                        if (Monitor.TryEnter(comToken, this.LockTimeOut))
                        {
                            try
                            {
                                if (!this.AloneSDK.GetCardFun(this.dev.MachineNumber, ref CardFun))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                            }
                            finally
                            {
                                Monitor.Exit(comToken);
                            }
                        }
                        else
                        {
                            operationSuccess = this.ErrOperationTimeOut;
                        }
                    }
                    else if (!this.AloneSDK.GetCardFun(this.dev.MachineNumber, ref CardFun))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                }
                finally
                {
                    Monitor.Exit(this.ThreadLock);
                }
            }
            else
            {
                operationSuccess = this.ErrOperationTimeOut;
            }
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }
        public int GetDeviceACFun(out int count)
        {
            int operationSuccess = this.OperationSuccess;
            count = 0;
            if (!this.IsConnected)
            {
                operationSuccess = this.Connect();
                if (operationSuccess < 0)
                {
                    return operationSuccess;
                }
                operationSuccess = this.OperationSuccess;
            }
            if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                Exception exception;
                try
                {
                    if (this.dev.ConnectType == 0)
                    {
                        ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                        if (Monitor.TryEnter(comToken, this.LockTimeOut))
                        {
                            try
                            {
                                this.AloneSDK.DisableDeviceWithTimeOut(this.dev.MachineNumber, 5);
                                if (!this.AloneSDK.GetACFun(ref count))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                                this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                            }
                            finally
                            {
                                Monitor.Exit(comToken);
                            }
                        }
                        else
                        {
                            operationSuccess = this.ErrOperationTimeOut;
                        }
                    }
                    else
                    {
                        this.AloneSDK.DisableDeviceWithTimeOut(this.dev.MachineNumber, 5);
                        if (!this.AloneSDK.GetACFun(ref count))
                        {
                            this.AloneSDK.GetLastError(ref operationSuccess);
                        }
                        this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                }
                finally
                {
                    Monitor.Exit(this.ThreadLock);
                }
            }
            else
            {
                operationSuccess = this.ErrOperationTimeOut;
            }
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }
        public int PinWidth
        {
            get
            {
                int pINWidth = 0;
                if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
                {
                    try
                    {
                        pINWidth = this.AloneSDK.PINWidth;
                    }
                    catch (Exception exception)
                    {
                    }
                    finally
                    {
                        Monitor.Exit(this.ThreadLock);
                    }
                }
                return pINWidth;
            }
        }
        public Machines Device
        {
            get
            {
                return this.dev;
            }
            set
            {
                this.dev = value;
            }
        }
        public int IsTFTMachine(out bool bolIsTFT)
        {
            int operationSuccess = this.OperationSuccess;
            bolIsTFT = false;
            if (!this.IsConnected)
            {
                operationSuccess = this.Connect();
                if (operationSuccess < 0)
                {
                    return operationSuccess;
                }
                operationSuccess = this.OperationSuccess;
            }
            if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                try
                {
                    bolIsTFT = this.AloneSDK.IsTFTMachine(this.dev.MachineNumber);
                    this.AloneSDK.GetLastError(ref operationSuccess);
                }
                catch (Exception exception)
                {
                    operationSuccess = this.ErrUnknown;
                }
                finally
                {
                    Monitor.Exit(this.ThreadLock);
                }
            }
            else
            {
                operationSuccess = this.ErrOperationTimeOut;
            }
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }
        public int GetAllUserInfo(out List<ObjUser> lstUser)
        {
            int num = this.OperationSuccess;
            lstUser = new List<ObjUser>();
            if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                try
                {
                    switch (this.dev.ConnectType)
                    {
                        case 0:
                            ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                            if (Monitor.TryEnter((object)comToken, this.LockTimeOut))
                            {
                                try
                                {
                                    num = this.GetAllUserInfoWithoutLock(out lstUser);
                                }
                                catch (Exception ex)
                                {
                                    num = this.ErrUnknown - 100000;
                                }
                                Monitor.Exit((object)comToken);
                                break;
                            }
                            num = this.ErrOperationTimeOut - 100000;
                            break;
                        case ConnectType.Net:
                            num = this.GetAllUserInfoWithoutLock(out lstUser);
                            break;
                        case ConnectType.Usb:
                            num = this.GetAllUserInfoWithoutLock(out lstUser);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    num = this.ErrUnknown - 100000;
                }
                Monitor.Exit(this.ThreadLock);
            }
            else
                num = this.ErrOperationTimeOut - 100000;
            return num;
        }
        protected int GetAllUserInfoWithoutLock(out List<ObjUser> lstUser)
        {
            int operationSuccess1 = this.OperationSuccess;
            string ACardNumber = "";
            int num1 = 0;
            int Privilege;
            int dwEnrollNumber1 = Privilege = 0;
            bool Enabled = true;
            string str;
            string Password = str = "";
            string Name = str;
            string dwEnrollNumber2 = str;
            lstUser = new List<ObjUser>();
            if (!this.IsConnected)
            {
                int num2 = this.ConnectWithoutLock();
                if (num2 != this.OperationSuccess)
                    return num2 - 100000;
                operationSuccess1 = this.OperationSuccess;
            }
            int count;
            int deviceStatus = this.GetDeviceStatus(MachineDataStatusCode.RegistedUserCount, out count);
            if (deviceStatus < 0)
                return deviceStatus;
            int operationSuccess2 = this.OperationSuccess;
            int num3 = num1 + count;
            if (num3 <= 0)
                return 0;
            bool flag = this.AloneSDK.IsTFTMachine(this.dev.MachineNumber);
            this.AloneSDK.DisableDeviceWithTimeOut(this.dev.MachineNumber, this.dev.ConnectType == 0 ? num3 * 2 : num3 + 2);
            if (!this.AloneSDK.ReadAllUserID(this.dev.MachineNumber))
            {
                this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                this.AloneSDK.GetLastError(ref operationSuccess2);
            }
            else
            {
                this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                while (true)
                {
                    bool allUserInfo;
                    if (flag)
                    {
                        allUserInfo = this.AloneSDK.SSR_GetAllUserInfo(this.dev.MachineNumber, out dwEnrollNumber2, out Name, out Password, out Privilege, out Enabled);
                    }
                    else
                    {
                        allUserInfo = this.AloneSDK.GetAllUserInfo(this.dev.MachineNumber, ref dwEnrollNumber1, ref Name, ref Password, ref Privilege, ref Enabled);
                        dwEnrollNumber2 = dwEnrollNumber1.ToString();
                    }
                    if (allUserInfo)
                    {
                        int length1 = Name.IndexOf(char.MinValue);
                        if (length1 >= 0)
                            Name = Name.Substring(0, length1);
                        int length2 = Password.IndexOf(char.MinValue);
                        if (length2 >= 0)
                            Password = Password.Substring(0, length2);
                        ObjUser objUser = new ObjUser();
                        objUser.Pin = dwEnrollNumber2;
                        objUser.Name = Name;
                        objUser.Password = Password;
                        switch (Privilege)
                        {
                            case 0:
                                objUser.Privilege = 0;
                                break;
                            case 3:
                            case 14:
                                objUser.Privilege = 3;
                                break;
                            default:
                                objUser.Privilege = Privilege;
                                break;
                        }
                        this.AloneSDK.GetStrCardNumber(out ACardNumber);
                        if (!string.IsNullOrEmpty(ACardNumber) && ACardNumber != "0")
                        {
                            objUser.CardNo = ACardNumber;
                            /*switch (AccCommon.CodeVersion)
                            //{
                            //    case CodeVersionType.Main:
                            //        objUser.CardNo = ACardNumber;
                            //        break;
                            //    case CodeVersionType.JapanAF:
                            //        objUser.CardNo = ACardNumber;
                            //        try
                            //        {
                            //            ulong uint64 = Convert.ToUInt64(ACardNumber, 16);
                            //            objUser.CardNo = uint64.ToString("X");
                            //            break;
                            //        }
                            //        catch (Exception ex)
                            //        {
                            //            break;
                            //        }
                            }*/
                        }
                        lstUser.Add(objUser);
                    }
                    else
                        break;
                }
            }
            return operationSuccess2 == this.OperationSuccess ? 0 : operationSuccess2 - 100000;
        }


        protected int GetAllUserFPTemplateWithoutLock(out List<ObjTemplateV10> lstTemplate)
        {
            int operationSuccess1 = this.OperationSuccess;
            int TmpLength = 0;
            string TmpData = "";
            int num1 = 0;
            int Privilege;
            int dwEnrollNumber1 = Privilege = 0;
            bool Enabled = true;
            bool flag1 = true;
            string str;
            string Password = str = "";
            string Name = str;
            string dwEnrollNumber2 = str;
            lstTemplate = new List<ObjTemplateV10>();
            if (this.dev.CompatOldFirmware == "0")
            {
                if (this.dev.FingerFunOn == null || this.dev.FingerFunOn.Trim() != "1")
                    return 0;
            }
            else if (this.dev.CardFun == 1)
                return 0;
            if (!this.IsConnected)
            {
                int num2 = this.ConnectWithoutLock();
                if (num2 != this.OperationSuccess)
                    return num2 - 100000;
                operationSuccess1 = this.OperationSuccess;
            }
            bool flag2 = this.AloneSDK.IsTFTMachine(this.dev.MachineNumber);
            int count;
            int deviceStatus1 = this.GetDeviceStatus(MachineDataStatusCode.RegistedUserCount, out count);
            if (deviceStatus1 < 0)
                return deviceStatus1;
            operationSuccess1 = this.OperationSuccess;
            int num3 = num1 + count;
            int deviceStatus2 = this.GetDeviceStatus(MachineDataStatusCode.TemplateCount, out count);
            if (deviceStatus2 < 0)
                return deviceStatus2;
            int operationSuccess2 = this.OperationSuccess;
            int num4 = num3 + count;
            if (num4 == 0 || count == 0)
                return 0;
            this.AloneSDK.DisableDeviceWithTimeOut(this.dev.MachineNumber, this.dev.ConnectType == 0 ? num4 * 2 : num4 + 2);
            if (!this.AloneSDK.ReadAllUserID(this.dev.MachineNumber))
            {
                this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                this.AloneSDK.GetLastError(ref operationSuccess2);
            }
            else
            {
                if (!this.AloneSDK.ReadAllTemplate(this.dev.MachineNumber))
                {
                    flag1 = false;
                    this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                    this.AloneSDK.GetLastError(ref operationSuccess2);
                }
                if (flag1)
                {
                    this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                    do
                    {
                        bool allUserInfo;
                        if (flag2)
                        {
                            allUserInfo = this.AloneSDK.SSR_GetAllUserInfo(this.dev.MachineNumber, out dwEnrollNumber2, out Name, out Password, out Privilege, out Enabled);
                        }
                        else
                        {
                            allUserInfo = this.AloneSDK.GetAllUserInfo(this.dev.MachineNumber, ref dwEnrollNumber1, ref Name, ref Password, ref Privilege, ref Enabled);
                            dwEnrollNumber2 = dwEnrollNumber1.ToString();
                        }
                        if (allUserInfo)
                        {
                            for (int dwFingerIndex = 0; dwFingerIndex < 10; ++dwFingerIndex)
                            {
                                int Flag = 1;
                                bool flag3;
                                if (this.dev.FpVersion == 10 || this.FirmwareVersion >= new Decimal(66, 0, 0, false, (byte)1))
                                    flag3 = this.AloneSDK.GetUserTmpExStr(this.dev.MachineNumber, dwEnrollNumber2, dwFingerIndex, out Flag, out TmpData, out TmpLength);
                                else if (flag2)
                                {
                                    flag3 = this.AloneSDK.SSR_GetUserTmpStr(this.dev.MachineNumber, dwEnrollNumber2, dwFingerIndex, out TmpData, out TmpLength);
                                }
                                else
                                {
                                    flag3 = this.AloneSDK.GetUserTmpStr(this.dev.MachineNumber, dwEnrollNumber1, dwFingerIndex, ref TmpData, ref TmpLength);
                                    dwEnrollNumber2 = dwEnrollNumber1.ToString();
                                }
                                if (flag3)
                                    lstTemplate.Add(new ObjTemplateV10()
                                    {
                                        Valid = Flag != 3 ? ValidType.Type1 : ValidType.Type3,
                                        FingerID = (FingerType)(dwFingerIndex & 15),
                                        Pin = dwEnrollNumber2,
                                        Template = TmpData,
                                        Size = TmpLength
                                    });
                            }
                        }
                        else
                            break;
                    }
                    while (lstTemplate.Count != count);
                }
            }
            return operationSuccess2 == this.OperationSuccess ? 0 : operationSuccess2 - 100000;
        }

        public int GetAllUserFPTemplate(out List<ObjTemplateV10> lstTemplate)
        {
            int num = this.OperationSuccess;
            lstTemplate = new List<ObjTemplateV10>();
            if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                try
                {
                    switch (this.dev.ConnectType)
                    {
                        case 0:
                            ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                            if (Monitor.TryEnter((object)comToken, this.LockTimeOut))
                            {
                                try
                                {
                                    num = this.GetAllUserFPTemplateWithoutLock(out lstTemplate);
                                }
                                catch (Exception ex)
                                {
                                    num = this.ErrUnknown - 100000;
                                }
                                Monitor.Exit((object)comToken);
                                break;
                            }
                            num = this.ErrOperationTimeOut - 100000;
                            break;
                        case ConnectType.Net:
                            num = this.GetAllUserFPTemplateWithoutLock(out lstTemplate);
                            break;
                        case ConnectType.Usb:
                            num = this.GetAllUserFPTemplateWithoutLock(out lstTemplate);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    num = this.ErrUnknown - 100000;
                }
                Monitor.Exit(this.ThreadLock);
            }
            else
                num = this.ErrOperationTimeOut - 100000;
            return num;
        }

        public int SetDeviceTime(DateTime? dateTime = new DateTime?())
        {
            int operationSuccess = this.OperationSuccess;
            if (!this.IsConnected)
            {
                operationSuccess = this.Connect();
                if (operationSuccess < 0)
                {
                    return operationSuccess;
                }
                operationSuccess = this.OperationSuccess;
            }
            if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                Exception exception;
                try
                {
                    if (this.dev.ConnectType == 0)
                    {
                        ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                        if (Monitor.TryEnter(comToken, this.LockTimeOut))
                        {
                            try
                            {
                                if (!dateTime.HasValue)
                                {
                                    if (!((operationSuccess != this.OperationSuccess) || this.AloneSDK.SetDeviceTime(this.dev.MachineNumber)))
                                    {
                                        this.AloneSDK.GetLastError(ref operationSuccess);
                                    }
                                }
                                else if (!((operationSuccess != this.OperationSuccess) || this.AloneSDK.SetDeviceTime2(this.dev.MachineNumber, dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day, dateTime.Value.Hour, dateTime.Value.Minute, dateTime.Value.Second)))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                            }
                            finally
                            {
                                Monitor.Exit(comToken);
                            }
                        }
                        else
                        {
                            operationSuccess = this.ErrOperationTimeOut;
                        }
                    }
                    else if (!dateTime.HasValue)
                    {
                        if (!((operationSuccess != this.OperationSuccess) || this.AloneSDK.SetDeviceTime(this.dev.MachineNumber)))
                        {
                            this.AloneSDK.GetLastError(ref operationSuccess);
                        }
                    }
                    else if (!((operationSuccess != this.OperationSuccess) || this.AloneSDK.SetDeviceTime2(this.dev.MachineNumber, dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day, dateTime.Value.Hour, dateTime.Value.Minute, dateTime.Value.Second)))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                }
                finally
                {
                    Monitor.Exit(this.ThreadLock);
                }
            }
            else
            {
                operationSuccess = this.ErrOperationTimeOut;
            }
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }




        public int Unlock(int DelayTime = 10)
        {
            int dwErrorCode = this.OperationSuccess;
            if (!this.IsConnected)
            {
                int num = this.Connect();
                if (num < 0)
                    return num;
                dwErrorCode = this.OperationSuccess;
            }
            if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                try
                {
                    if (this.dev.ConnectType == 0)
                    {
                        ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                        if (Monitor.TryEnter((object)comToken, this.LockTimeOut))
                        {
                            try
                            {
                                if (!this.AloneSDK.ACUnlock(this.dev.MachineNumber, DelayTime))
                                    this.AloneSDK.GetLastError(ref dwErrorCode);
                            }
                            catch (Exception ex)
                            {
                                dwErrorCode = this.ErrUnknown;
                            }
                            finally
                            {
                                Monitor.Exit((object)comToken);
                            }
                        }
                        else
                            dwErrorCode = this.ErrOperationTimeOut;
                    }
                    else if (!this.AloneSDK.ACUnlock(this.dev.MachineNumber, DelayTime))
                        this.AloneSDK.GetLastError(ref dwErrorCode);
                }
                catch (Exception ex)
                {
                    dwErrorCode = this.ErrUnknown;
                }
                finally
                {
                    Monitor.Exit(this.ThreadLock);
                }
            }
            else
                dwErrorCode = this.ErrOperationTimeOut;
            return dwErrorCode == this.OperationSuccess ? 0 : dwErrorCode - 100000;
        }



        public int SetUserInfoWithoutLock(List<ObjUser> lstUser)
        {
            int operationSuccess = this.OperationSuccess;
            int num = 0;
            int count = lstUser.Count;
            if (count <= 0)
                return operationSuccess == this.OperationSuccess ? 0 : operationSuccess - 100000;
            int dwErrorCode = this.InitialCommunicationWithoutLock(this.dev.ConnectType == 0 ? count * 2 : count + 2, true);
            if (dwErrorCode != this.OperationSuccess)
                return dwErrorCode - 100000;
            bool flag = this.AloneSDK.IsTFTMachine(this.dev.MachineNumber);
            for (int index = 0; index < lstUser.Count; ++index)
            {
                ObjUser objUser = lstUser[index];
                int.Parse(objUser.Id);
                this.AloneSDK.SetStrCardNumber(objUser.CardNo);
                int result;
                int.TryParse(objUser.Group, out result);
                //  this.AloneSDK.AccGroup = result <= 0 || !dicPullGroupId_StdGroupId.ContainsKey(result) ? 1 : dicPullGroupId_StdGroupId[result];
                int Privilege;
                switch (objUser.Privilege)
                {
                    case 0:
                        Privilege = 0;
                        break;
                    case 3:
                    case 14:
                        Privilege = 3;
                        break;
                    default:
                        Privilege = 0;
                        break;
                }
                if (objUser.Privilege == 4 && this.dev.CompatOldFirmware == "0")
                    Privilege = 8;
                if (!flag ? this.AloneSDK.SetUserInfo(this.dev.MachineNumber, int.Parse(objUser.Pin), objUser.Name, objUser.Password, Privilege, true) : this.AloneSDK.SSR_SetUserInfo(this.dev.MachineNumber, objUser.Pin, objUser.Name, objUser.Password, Privilege, true))
                {
                    ++num;
                }
                else
                {
                    this.AloneSDK.GetLastError(ref dwErrorCode);
                    break;
                }
            }
            if (dwErrorCode == this.OperationSuccess)
                dwErrorCode = this.EndCommunicationWithoutLock(true, true);
            else if (dwErrorCode != this.ErrOperationTimeOut)
                this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
            return dwErrorCode == this.OperationSuccess ? num : dwErrorCode - 100000;
        }
        protected int InitialCommunicationWithoutLock(int WorkingTimeOut = 0, bool UseBatchMode = false)
        {
            int dwErrorCode = this.OperationSuccess;
            if (!this.IsConnected)
            {
                dwErrorCode = this.ConnectWithoutLock();
                if (dwErrorCode != this.OperationSuccess)
                    return dwErrorCode;
            }
            if (UseBatchMode && this.dev.BatchUpdate && !this.AloneSDK.BeginBatchUpdate(this.dev.MachineNumber, 1))
            {
                this.AloneSDK.GetLastError(ref dwErrorCode);
                return dwErrorCode;
            }
            if (WorkingTimeOut <= 0 || this.AloneSDK.DisableDeviceWithTimeOut(this.dev.MachineNumber, WorkingTimeOut))
                return dwErrorCode;
            this.AloneSDK.GetLastError(ref dwErrorCode);
            return dwErrorCode;
        }
        protected int EndCommunicationWithoutLock(bool UseBatchMode = false, bool RefreshData = false)
        {
            int operationSuccess = this.OperationSuccess;
            if ((operationSuccess == this.OperationSuccess || operationSuccess != this.ErrOperationTimeOut) && (UseBatchMode && this.dev.BatchUpdate && !this.AloneSDK.BatchUpdate(this.dev.MachineNumber) && operationSuccess == this.OperationSuccess))
                this.AloneSDK.GetLastError(ref operationSuccess);
            if (RefreshData && ((operationSuccess == this.OperationSuccess || operationSuccess != this.ErrOperationTimeOut) && !this.AloneSDK.RefreshData(this.dev.MachineNumber) && operationSuccess == this.OperationSuccess))
                this.AloneSDK.GetLastError(ref operationSuccess);
            if ((operationSuccess == this.OperationSuccess || operationSuccess != this.ErrOperationTimeOut) && !this.AloneSDK.EnableDevice(this.dev.MachineNumber, true) && operationSuccess == this.OperationSuccess)
                this.AloneSDK.GetLastError(ref operationSuccess);
            return operationSuccess;
        }
    }






    #region Delegates

    public delegate void AlarmEventHandler(int AlarmType, int EnrollNumber, int Verified);

    public delegate void AttTransactionEventHandler(string EnrollNumber, int IsInValid, int AttState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second, int WorkCode);

    public delegate void ConnectedEventHandler();

    public delegate void DisconnectedEventHandler();

    public delegate void DoorEventHandler(int EventType);

    public delegate void EMDataEventHandler(int DataType, int DataLen, ref sbyte DataBuffer);

    public delegate void EmptyCardEventHander(int ActionResult);

    public delegate void EnrollFingerEventHandler(string EnrollNumber, int FingerIndex, int ActionResult, int TemplateLength);

    public delegate void FingerEventHandler();

    public delegate void FingerFeatureEventHandler(int Score);

    public delegate void HIDNumEventHandler(int CardNumber);

    public delegate void NewUserEventHandler(int EnrollNumber);
    public delegate void VerifyEventHandler(int UserID);

    public delegate void WriteCardEventHandler(int EnrollNumber, int ActionResult, int Length);
    #endregion

}
