using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using zkemkeeper;
using ZKTeco.SDK.Model;
using static ZKTeco.SDK.MachineManager.ComTokenManager;

namespace ZKTeco.SDK.MachineManager
{
 public   class STDDevComm
    {
        // Fields
        private int _LockFunOn;
        private CZKEMClass AloneSDK;
        private bool bolEventRegistered = false;
        private Machines dev;
        private readonly int Err_No_Data = 0;
        private readonly int ErrDel_No_Data = -4993;
        private readonly int ErrOperationTimeOut = -7;
        private readonly int ErrUnknown = -999;
        private decimal FirmwareVersion;
        private bool isConnected;
        private int LockTimeOut = 0xbb8;
        private readonly int OperationSuccess = 1;
        private object ThreadLock = new object();

        // Events
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

        // Methods
        public STDDevComm(Machines device)
        {
            this.dev = device;
            Thread thread = new Thread(new ThreadStart(this.CreadSDKObject));
            thread.Start();
            thread.Join();
        }

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

        private List<BioTemplate> AnalyzeBiotemplateBuffer(string buffer)
        {
            List<BioTemplate> list = new List<BioTemplate>();
            string[] strArray = buffer.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length > 0)
            {
                int num;
                int num2;
                int num3;
                int num4;
                int num5;
                int num6;
                int num7;
                int num8;
                int num9;
                int num10;
                this.GetFieldIndex(strArray[0], out num, out num2, out num3, out num4, out num5, out num6, out num7, out num8, out num9, out num10);
                for (int i = 1; i < strArray.Length; i++)
                {
                    BioTemplate item = new BioTemplate();
                    string[] strArray2 = strArray[i].Split(new char[] { ',' });
                    item.BadgeNumber = strArray2[num];
                    item.BioType = Convert.ToInt32(strArray2[num4]);
                    item.IsDuress = strArray2[num3];
                    item.TemplateData = strArray2[num10];
                    item.ValidFlag = strArray2[num2];
                    item.DataFormat = new int?(Convert.ToInt32(strArray2[num7]));
                    item.TemplateNO = Convert.ToInt32(strArray2[num8]);
                    item.TemplateNOIndex = Convert.ToInt32(strArray2[num9]);
                    item.Version = strArray2[num5] + "." + strArray2[num6];
                    item.nOldType = 0;
                    list.Add(item);
                }
            }
            return list;
        }

        //private string AssemblesFingerVeinInfo(List<FingerVein> lstFingerVein)
        //{
        //    StringBuilder builder = new StringBuilder();
        //    foreach (FingerVein vein in lstFingerVein)
        //    {
        //        builder.Append(string.Format("Pin={0}\tValid={1}\tDuress={2}\tType={3}\tMajorVer={4}\tMinorVer={5}\tFormat={6}\tNo={7}\tIndex={8}\tTmp={9}\r\n", new object[] { vein.Pin, vein.Valid, vein.DuressFlag, 7, vein.MayorVer, vein.MinorVer, vein.Format, vein.FingerID, vein.Fv_ID_Index, Convert.ToBase64String(vein.Template) }));
        //    }
        //    return builder.ToString();
        //}

        public int CancelOperation()
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
                                if (!this.AloneSDK.CancelOperation())
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on CancelOperation_Com " + exception.Message);
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
                    else if (!this.AloneSDK.CancelOperation())
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on CancelOperation " + exception.Message);
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

        public int ClearAdministrators()
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
                                if (!this.AloneSDK.ClearAdministrators(this.dev.MachineNumber))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on ClearAdministrators_Com " + exception.Message);
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
                    else if (!this.AloneSDK.ClearAdministrators(this.dev.MachineNumber))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on ClearAdministrators " + exception.Message);
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

        public int ClearFaceTemplate()
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
                    case 0:
                        {
                            ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                            if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                            {
                                break;
                            }
                            try
                            {
                                operationSuccess = this.ClearFaceTemplateWithoutLock();
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on ClearFaceTemplateWithoutLockCOM " + exception.Message);
                            }
                            Monitor.Exit(comToken);
                            goto Label_0103;
                        }
                    case 1:
                        operationSuccess = this.ClearFaceTemplateWithoutLock();
                        goto Label_0103;

                    case 2:
                        operationSuccess = this.ClearFaceTemplateWithoutLock();
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
                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on ClearFaceTemplateWithoutLock " + exception.Message);
            }
            Label_0103:
            Monitor.Exit(this.ThreadLock);
            Label_011C:
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }

        protected int ClearFaceTemplateWithoutLock()
        {
            bool flag2;
            string str2;
            string str3;
            bool flag3;
            int operationSuccess = this.OperationSuccess;
            if (this.dev.FaceFunOn != 1)
            {
                return this.OperationSuccess;
            }
            operationSuccess = this.InitialCommunicationWithoutLock(0, false);
            if (operationSuccess != this.OperationSuccess)
            {
                return operationSuccess;
            }
            int dwEnrollNumber = 0;
            int privilege = 0;
            int num4 = 0;
            int count = 0;
            int num6 = 0;
            int num7 = 0;
            bool enabled = true;
            string name = str2 = str3 = "";
            operationSuccess = this.GetDeviceStatus(MachineDataStatusCode.RegistedUserCount, out count);
            if (operationSuccess < 0)
            {
                return (operationSuccess + 0x186a0);
            }
            if (count <= 0)
            {
                return this.OperationSuccess;
            }
            operationSuccess = this.OperationSuccess;
            operationSuccess = this.GetDeviceStatus(MachineDataStatusCode.FaceTemplateCount, out count);
            if (operationSuccess < 0)
            {
                return (operationSuccess + 0x186a0);
            }
            if (count <= 0)
            {
                return this.OperationSuccess;
            }
            operationSuccess = this.OperationSuccess;
            num4 = count;
            num6 += count;
            if (!this.AloneSDK.DisableDeviceWithTimeOut(this.dev.MachineNumber, (this.dev.ConnectType == 0) ? (num6 * 2) : (num6 + 2)))
            {
                this.AloneSDK.GetLastError(ref operationSuccess);
                return operationSuccess;
            }
            if (!this.AloneSDK.ReadAllUserID(this.dev.MachineNumber))
            {
                this.AloneSDK.GetLastError(ref operationSuccess);
                goto Label_025F;
            }
            Label_0256:
            flag3 = true;
            if (this.AloneSDK.IsTFTMachine(this.dev.MachineNumber))
            {
                flag2 = this.AloneSDK.SSR_GetAllUserInfo(this.dev.MachineNumber, out str3, out name, out str2, out privilege, out enabled);
            }
            else
            {
                flag2 = this.AloneSDK.GetAllUserInfo(this.dev.MachineNumber, ref dwEnrollNumber, ref name, ref str2, ref privilege, ref enabled);
                str3 = dwEnrollNumber.ToString();
            }
            if (flag2)
            {
                if (this.AloneSDK.DelUserFace(this.dev.MachineNumber, str3, 50))
                {
                    num7++;
                    if (num7 >= num4)
                    {
                        goto Label_025F;
                    }
                    goto Label_0256;
                }
                this.AloneSDK.GetLastError(ref operationSuccess);
            }
            Label_025F:
            if (operationSuccess == this.OperationSuccess)
            {
                operationSuccess = this.EndCommunicationWithoutLock(false, false);
            }
            else if (operationSuccess != this.ErrOperationTimeOut)
            {
                this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
            }
            return operationSuccess;
        }

        public int ClearFPTemplate()
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
                    case 0:
                        {
                            ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                            if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                            {
                                break;
                            }
                            try
                            {
                                operationSuccess = this.ClearFPTemplateWithoutLock();
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on ClearFPTemplateCOM " + exception.Message);
                            }
                            Monitor.Exit(comToken);
                            goto Label_0103;
                        }
                    case 1:
                        operationSuccess = this.ClearFPTemplateWithoutLock();
                        goto Label_0103;

                    case 2:
                        operationSuccess = this.ClearFPTemplateWithoutLock();
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
                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on ClearFPTemplate " + exception.Message);
            }
            Label_0103:
            Monitor.Exit(this.ThreadLock);
            Label_011C:
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }

        protected int ClearFPTemplateWithoutLock()
        {
            int operationSuccess = this.OperationSuccess;
            if (this.dev.CompatOldFirmware == "0")
            {
                if ((this.dev.FingerFunOn == null) || (this.dev.FingerFunOn.Trim() != "1"))
                {
                    return 0;
                }
            }
            else if (this.dev.CardFun == 1)
            {
                return this.OperationSuccess;
            }
            operationSuccess = this.InitialCommunicationWithoutLock(10, false);
            if (operationSuccess == this.OperationSuccess)
            {
                if (!this.AloneSDK.ClearData(this.dev.MachineNumber, 2))
                {
                    this.AloneSDK.GetLastError(ref operationSuccess);
                }
                if (operationSuccess == this.OperationSuccess)
                {
                    operationSuccess = this.EndCommunicationWithoutLock(false, false);
                }
                else if (operationSuccess != this.ErrOperationTimeOut)
                {
                    this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                }
            }
            return operationSuccess;
        }

        public int ClearGLog()
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
                    case 0:
                        {
                            ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                            if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                            {
                                break;
                            }
                            try
                            {
                                operationSuccess = this.ClearGLogWithoutLock();
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on ClearUserCOM " + exception.Message);
                            }
                            Monitor.Exit(comToken);
                            goto Label_0103;
                        }
                    case 1:
                        operationSuccess = this.ClearGLogWithoutLock();
                        goto Label_0103;

                    case 2:
                        operationSuccess = this.ClearGLogWithoutLock();
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
                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on ClearUser " + exception.Message);
            }
            Label_0103:
            Monitor.Exit(this.ThreadLock);
            Label_011C:
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }

        protected int ClearGLogWithoutLock()
        {
            int operationSuccess = this.OperationSuccess;
            operationSuccess = this.InitialCommunicationWithoutLock(10, false);
            if (operationSuccess == this.OperationSuccess)
            {
                if (!this.AloneSDK.ClearGLog(this.dev.MachineNumber))
                {
                    this.AloneSDK.GetLastError(ref operationSuccess);
                }
                if (operationSuccess == this.OperationSuccess)
                {
                    operationSuccess = this.EndCommunicationWithoutLock(false, false);
                }
                else if (operationSuccess != this.ErrOperationTimeOut)
                {
                    this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                }
            }
            return operationSuccess;
        }

        public int ClearKeeperData()
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
                                if (!this.AloneSDK.ClearKeeperData(this.dev.MachineNumber))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on ClearKeeperData_Com " + exception.Message);
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
                    else if (!this.AloneSDK.ClearKeeperData(this.dev.MachineNumber))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on ClearKeeperData " + exception.Message);
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

        public int ClearUser()
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
                    case 0:
                        {
                            ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                            if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                            {
                                break;
                            }
                            try
                            {
                                operationSuccess = this.ClearUserWithoutLock();
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on ClearUserCOM " + exception.Message);
                            }
                            Monitor.Exit(comToken);
                            goto Label_0103;
                        }
                    case 1:
                        operationSuccess = this.ClearUserWithoutLock();
                        goto Label_0103;

                    case 2:
                        operationSuccess = this.ClearUserWithoutLock();
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
                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on ClearUser " + exception.Message);
            }
            Label_0103:
            Monitor.Exit(this.ThreadLock);
            Label_011C:
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }

        protected int ClearUserWithoutLock()
        {
            int operationSuccess = this.OperationSuccess;
            operationSuccess = this.InitialCommunicationWithoutLock(10, false);
            if (operationSuccess == this.OperationSuccess)
            {
                if (!this.AloneSDK.ClearData(this.dev.MachineNumber, 5))
                {
                    this.AloneSDK.GetLastError(ref operationSuccess);
                }
                if (operationSuccess == this.OperationSuccess)
                {
                    operationSuccess = this.EndCommunicationWithoutLock(false, false);
                }
                else if (operationSuccess != this.ErrOperationTimeOut)
                {
                    this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                }
            }
            return operationSuccess;
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
                    case 0:
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
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on ConnectCOM " + exception.Message);
                            }
                            Monitor.Exit(comToken);
                            goto Label_0103;
                        }
                    case 1:
                        operationSuccess = this.ConnectWithoutLock();
                        goto Label_0103;

                    case 2:
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
                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on Connect " + exception.Message);
            }
            Label_0103:
            Monitor.Exit(this.ThreadLock);
            Label_011C:
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
                        case 0:
                            this.AloneSDK.PullMode = 1;
                            this.isConnected = this.AloneSDK.Connect_Com(this.dev.SerialPort, this.dev.MachineNumber, this.dev.Baudrate);
                            break;

                        case 1:
                            this.AloneSDK.PullMode = 0;
                            this.isConnected = this.AloneSDK.Connect_Net(this.dev.IP, this.dev.Port);
                            break;

                        case 2:
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
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on ConnectWithoutLock " + exception.Message);
                }
                return operationSuccess;
            }
            this.GetParamInConn();
            return operationSuccess;
        }

        protected string ConvertLstBioTemplate2StringBuff(List<BioTemplate> lstBioTemplate)
        {
            StringBuilder builder = new StringBuilder();
            foreach (BioTemplate template in lstBioTemplate)
            {
                string[] strArray = template.Version.Split(new char[] { '.' });
                builder.Append(string.Format("Pin={0}\tValid={1}\tDuress={2}\tType={3}\tMajorVer={4}\tMinorVer={5}\tFormat={6}\tNo={7}\tIndex={8}\tTmp={9}\r\n", new object[] { template.BadgeNumber, template.ValidFlag, template.IsDuress, template.BioType, strArray[0], strArray[1], template.DataFormat, template.TemplateNO, template.TemplateNOIndex, template.TemplateData }));
            }
            return builder.ToString();
        }

        private void CreadSDKObject()
        {
            try
            {
                this.AloneSDK = new CZKEMClass();
            }
            catch (COMException)
            {
                if (File.Exists("zkemkeeper.dll"))
                {
                    Process process = new Process
                    {
                        StartInfo = { FileName = "cmd.exe", UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true }
                    };
                    process.Start();
                    process.StandardInput.WriteLine("regsvr32 zkemkeeper.dll /s");
                    process.StandardInput.WriteLine("exit");
                    try
                    {
                        this.AloneSDK = new CZKEMClass();
                    }
                    catch (Exception exception2)
                    {
                        //SysLogServer.WriteLog("Create SDK Error:" + exception2.Message);
                    }
                }
            }
            catch (Exception exception3)
            {
                //SysLogServer.WriteLog("Create SDK Error:" + exception3.Message);
            }
            this.AloneSDK.add_OnConnected(new _IZKEMEvents_OnConnectedEventHandler(this.AloneSDK_OnConnected));
            this.AloneSDK.add_OnDisConnected(new _IZKEMEvents_OnDisConnectedEventHandler(this.AloneSDK_OnDisConnected));
            this.AloneSDK.add_OnAlarm(new _IZKEMEvents_OnAlarmEventHandler(this.AloneSDK_OnAlarm));
            this.AloneSDK.add_OnDoor(new _IZKEMEvents_OnDoorEventHandler(this.AloneSDK_OnDoor));
            this.AloneSDK.add_OnAttTransactionEx(new _IZKEMEvents_OnAttTransactionExEventHandler(this.AloneSDK_OnAttTransactionEx));
            this.AloneSDK.add_OnVerify(new _IZKEMEvents_OnVerifyEventHandler(this.AloneSDK_OnVerify));
            this.AloneSDK.add_OnEnrollFinger(new _IZKEMEvents_OnEnrollFingerEventHandler(this.AloneSDK_OnEnrollFinger));
            this.AloneSDK.add_OnEnrollFingerEx(new _IZKEMEvents_OnEnrollFingerExEventHandler(this.AloneSDK_OnEnrollFingerEx));
            this.AloneSDK.add_OnFingerFeature(new _IZKEMEvents_OnFingerFeatureEventHandler(this.AloneSDK_OnFingerFeature));
            this.AloneSDK.add_OnHIDNum(new _IZKEMEvents_OnHIDNumEventHandler(this.AloneSDK_OnHIDNum));
        }

        //protected int DeleteUserBioTemplateWithoutLock(List<BioTemplate> lstBioTemplate, BioTemplateType bioTemplateType)
        //{
        //    int operationSuccess = this.OperationSuccess;
        //    int count = lstBioTemplate.Count;
        //    if (this.dev.BiometricType[(int)bioTemplateType] == '0')
        //    {
        //        return 0;
        //    }
        //    operationSuccess = this.InitialCommunicationWithoutLock((this.dev.ConnectType == 0) ? (count * 2) : (count + 2), false);
        //    if (operationSuccess == this.OperationSuccess)
        //    {
        //        if (((lstBioTemplate != null) && (lstBioTemplate.Count > 0)) && !this.AloneSDK.SSR_DeleteDeviceData(this.dev.MachineNumber, Tables.pers_biotemplate.ToString(), string.Format("Type={0}", (int)bioTemplateType), ""))
        //        {
        //            this.AloneSDK.GetLastError(ref operationSuccess);
        //        }
        //        if (operationSuccess == this.OperationSuccess)
        //        {
        //            operationSuccess = this.EndCommunicationWithoutLock(false, false);
        //        }
        //        else if (operationSuccess != this.ErrOperationTimeOut)
        //        {
        //            this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
        //        }
        //    }
        //    return operationSuccess;
        //}

        //public int DeleteUserFaceTemplate(List<ObjFaceTemp> lstTemplate)
        //{
        //    Exception exception;
        //    int operationSuccess = this.OperationSuccess;
        //    if (!Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
        //    {
        //        operationSuccess = this.ErrOperationTimeOut;
        //        goto Label_011F;
        //    }
        //    try
        //    {
        //        switch (this.dev.ConnectType)
        //        {
        //            case 0:
        //                {
        //                    ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
        //                    if (!Monitor.TryEnter(comToken, this.LockTimeOut))
        //                    {
        //                        break;
        //                    }
        //                    try
        //                    {
        //                        operationSuccess = this.DeleteUserFaceTemplateWithoutLock(lstTemplate);
        //                    }
        //                    catch (Exception exception1)
        //                    {
        //                        exception = exception1;
        //                        operationSuccess = this.ErrUnknown;
        //                        //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on DeleteUserFaceTemplateCOM " + exception.Message);
        //                    }
        //                    Monitor.Exit(comToken);
        //                    goto Label_0106;
        //                }
        //            case 1:
        //                operationSuccess = this.DeleteUserFaceTemplateWithoutLock(lstTemplate);
        //                goto Label_0106;

        //            case 2:
        //                operationSuccess = this.DeleteUserFaceTemplateWithoutLock(lstTemplate);
        //                goto Label_0106;

        //            default:
        //                goto Label_0106;
        //        }
        //        operationSuccess = this.ErrOperationTimeOut;
        //    }
        //    catch (Exception exception2)
        //    {
        //        exception = exception2;
        //        operationSuccess = this.ErrUnknown;
        //        //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on DeleteUserFaceTemplate " + exception.Message);
        //    }
        //    Label_0106:
        //    Monitor.Exit(this.ThreadLock);
        //    Label_011F:
        //    return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        //}

        //protected int DeleteUserFaceTemplateWithoutLock(List<ObjFaceTemp> lstTemplate)
        //{
        //    int operationSuccess = this.OperationSuccess;
        //    int count = lstTemplate.Count;
        //    if (this.dev.FaceFunOn != 1)
        //    {
        //        return this.OperationSuccess;
        //    }
        //    operationSuccess = this.InitialCommunicationWithoutLock((this.dev.ConnectType == 0) ? (count * 2) : (count + 2), false);
        //    if (operationSuccess == this.OperationSuccess)
        //    {
        //        if ((lstTemplate != null) && (lstTemplate.Count > 0))
        //        {
        //            for (int i = 0; i < lstTemplate.Count; i++)
        //            {
        //                ObjFaceTemp temp = lstTemplate[i];
        //                if (!this.AloneSDK.DelUserFace(this.dev.MachineNumber, temp.Pin, 50))
        //                {
        //                    this.AloneSDK.GetLastError(ref operationSuccess);
        //                    break;
        //                }
        //            }
        //        }
        //        if (operationSuccess == this.OperationSuccess)
        //        {
        //            operationSuccess = this.EndCommunicationWithoutLock(false, false);
        //        }
        //        else if (operationSuccess != this.ErrOperationTimeOut)
        //        {
        //            this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
        //        }
        //    }
        //    return operationSuccess;
        //}

        public int DeleteUserFPTemplate(List<Template> lstTemplate)
        {
            Exception exception;
            int operationSuccess = this.OperationSuccess;
            if (!Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                operationSuccess = this.ErrOperationTimeOut;
                goto Label_011F;
            }
            try
            {
                switch (this.dev.ConnectType)
                {
                    case 0:
                        {
                            ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                            if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                            {
                                break;
                            }
                            try
                            {
                                operationSuccess = this.DeleteUserFPTemplateWithoutLock(lstTemplate);
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on DeleteUserFPTemplateCOM " + exception.Message);
                            }
                            Monitor.Exit(comToken);
                            goto Label_0106;
                        }
                    case 1:
                        operationSuccess = this.DeleteUserFPTemplateWithoutLock(lstTemplate);
                        goto Label_0106;

                    case 2:
                        operationSuccess = this.DeleteUserFPTemplateWithoutLock(lstTemplate);
                        goto Label_0106;

                    default:
                        goto Label_0106;
                }
                operationSuccess = this.ErrOperationTimeOut;
            }
            catch (Exception exception2)
            {
                exception = exception2;
                operationSuccess = this.ErrUnknown;
                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on DeleteUserFPTemplate " + exception.Message);
            }
            Label_0106:
            Monitor.Exit(this.ThreadLock);
            Label_011F:
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }

        protected int DeleteUserFPTemplateWithoutLock(List<Template> lstTemplate)
        {
            int operationSuccess = this.OperationSuccess;
            int count = lstTemplate.Count;
            if (this.dev.CompatOldFirmware == "0")
            {
                if ((this.dev.FingerFunOn == null) || (this.dev.FingerFunOn.Trim() != "1"))
                {
                    return 0;
                }
            }
            else if (this.dev.CardFun == 1)
            {
                return this.OperationSuccess;
            }
            operationSuccess = this.InitialCommunicationWithoutLock((this.dev.ConnectType == 0) ? (count * 2) : (count + 2), false);
            if (operationSuccess == this.OperationSuccess)
            {
                if ((lstTemplate != null) && (lstTemplate.Count > 0))
                {
                    for (int i = 0; i < lstTemplate.Count; i++)
                    {
                        Template template = lstTemplate[i];
                        if (this.AloneSDK.IsTFTMachine(this.dev.MachineNumber))
                        {
                            if (!this.AloneSDK.SSR_DelUserTmpExt(this.dev.MachineNumber, template.Pin, template.FINGERID))
                            {
                                this.AloneSDK.GetLastError(ref operationSuccess);
                                break;
                            }
                        }
                        else if (template.FINGERID == 13)
                        {
                            for (int j = 0; j < 10; j++)
                            {
                                if (!this.AloneSDK.DelUserTmp(this.dev.MachineNumber, int.Parse(template.Pin), j))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                    break;
                                }
                            }
                        }
                        else if (!this.AloneSDK.DelUserTmp(this.dev.MachineNumber, int.Parse(template.Pin), template.FINGERID))
                        {
                            this.AloneSDK.GetLastError(ref operationSuccess);
                            break;
                        }
                    }
                }
                if (operationSuccess == this.OperationSuccess)
                {
                    operationSuccess = this.EndCommunicationWithoutLock(false, false);
                }
                else if (operationSuccess != this.ErrOperationTimeOut)
                {
                    this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                }
            }
            return operationSuccess;
        }

        public int DeleteUserInfo(List<ObjUser> lstUser)
        {
            Exception exception;
            int operationSuccess = this.OperationSuccess;
            if (!Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                operationSuccess = this.ErrOperationTimeOut;
                goto Label_011F;
            }
            try
            {
                switch (this.dev.ConnectType)
                {
                    case 0:
                        {
                            ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                            if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                            {
                                break;
                            }
                            try
                            {
                                operationSuccess = this.DeleteUserInfoWithoutLock(lstUser);
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on DeleteUserInfoCOM " + exception.Message);
                            }
                            Monitor.Exit(comToken);
                            goto Label_0106;
                        }
                    case 1:
                        operationSuccess = this.DeleteUserInfoWithoutLock(lstUser);
                        goto Label_0106;

                    case 2:
                        operationSuccess = this.DeleteUserInfoWithoutLock(lstUser);
                        goto Label_0106;

                    default:
                        goto Label_0106;
                }
                operationSuccess = this.ErrOperationTimeOut;
            }
            catch (Exception exception2)
            {
                exception = exception2;
                operationSuccess = this.ErrUnknown;
                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on DeleteUserInfo " + exception.Message);
            }
            Label_0106:
            Monitor.Exit(this.ThreadLock);
            Label_011F:
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }

        protected int DeleteUserInfoWithoutLock(List<ObjUser> lstUser)
        {
            int operationSuccess = this.OperationSuccess;
            bool flag = true;
            int count = lstUser.Count;
            operationSuccess = this.InitialCommunicationWithoutLock((this.dev.ConnectType == 0) ? (count * 2) : (count + 2), false);
            if (operationSuccess == this.OperationSuccess)
            {
                if ((lstUser != null) && (lstUser.Count > 0))
                {
                    for (int i = 0; i < lstUser.Count; i++)
                    {
                        ObjUser user = lstUser[i];
                        if (this.AloneSDK.IsTFTMachine(this.dev.MachineNumber))
                        {
                            flag = this.AloneSDK.SSR_DeleteEnrollDataExt(this.dev.MachineNumber, user.Pin, 12);
                            if (((flag || (this.dev.device_name == null)) || !this.dev.device_name.ToUpper().Contains("BIOCAM")) || !this.dev.device_name.ToUpper().Contains("300"))
                            {
                                goto Label_019A;
                            }
                            int dwErrorCode = this.OperationSuccess;
                            this.AloneSDK.GetLastError(ref operationSuccess);
                            if (this.AloneSDK.SSR_DelUserTmpExt(this.dev.MachineNumber, user.Pin, 0))
                            {
                                flag = false;
                            }
                            else
                            {
                                this.AloneSDK.GetLastError(ref dwErrorCode);
                            }
                            break;
                        }
                        flag = this.AloneSDK.DeleteEnrollData(this.dev.MachineNumber, int.Parse(user.Pin), this.dev.MachineNumber, 12);
                        Label_019A:
                        if (!flag)
                        {
                            this.AloneSDK.GetLastError(ref operationSuccess);
                            operationSuccess = this.ReturnMemoryNotEnough(operationSuccess);
                            if (this.OperationSuccess == operationSuccess)
                            {
                                flag = true;
                            }
                            break;
                        }
                    }
                }
                if (operationSuccess == this.OperationSuccess)
                {
                    this.AloneSDK.RefreshData(this.dev.MachineNumber);
                    operationSuccess = this.EndCommunicationWithoutLock(false, false);
                }
                else if (operationSuccess != this.ErrOperationTimeOut)
                {
                    this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                }
            }
            return operationSuccess;
        }

        public void Disconnect()
        {
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
                                this.AloneSDK.Disconnect();
                                Thread.Sleep(500);
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on Disconnect_Com " + exception.Message);
                            }
                            finally
                            {
                                Monitor.Exit(comToken);
                            }
                        }
                    }
                    else
                    {
                        this.AloneSDK.Disconnect();
                    }
                    this.isConnected = false;
                    this.bolEventRegistered = false;
                    if (null != this.OnDisconnected)
                    {
                        this.OnDisconnected();
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on disconnect. " + exception.Message);
                }
                finally
                {
                    Monitor.Exit(this.ThreadLock);
                }
            }
        }

        public int EnableDevice(bool Enabled)
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
                                if (!this.AloneSDK.EnableDevice(this.dev.MachineNumber, Enabled))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on EnableDevice_Com " + exception.Message);
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
                    else if (!this.AloneSDK.EnableDevice(this.dev.MachineNumber, Enabled))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on EnableDevice " + exception.Message);
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

        protected int EndCommunicationWithoutLock(bool UseBatchMode = false, bool RefreshData = false)
        {
            int operationSuccess = this.OperationSuccess;
            if ((((operationSuccess == this.OperationSuccess) || (operationSuccess != this.ErrOperationTimeOut)) && (UseBatchMode && this.dev.BatchUpdate)) && (!this.AloneSDK.BatchUpdate(this.dev.MachineNumber) && (operationSuccess == this.OperationSuccess)))
            {
                this.AloneSDK.GetLastError(ref operationSuccess);
            }
            if ((RefreshData && ((operationSuccess == this.OperationSuccess) || (operationSuccess != this.ErrOperationTimeOut))) && (!this.AloneSDK.RefreshData(this.dev.MachineNumber) && (operationSuccess == this.OperationSuccess)))
            {
                this.AloneSDK.GetLastError(ref operationSuccess);
            }
            if ((operationSuccess == this.OperationSuccess) || (operationSuccess != this.ErrOperationTimeOut))
            {
                if (this.AloneSDK.EnableDevice(this.dev.MachineNumber, true))
                {
                    return operationSuccess;
                }
                if (operationSuccess == this.OperationSuccess)
                {
                    this.AloneSDK.GetLastError(ref operationSuccess);
                }
            }
            return operationSuccess;
        }

        public int GetAllTransaction(out List<ObjTransAction> lstTransaction)
        {
            Exception exception;
            int operationSuccess = this.OperationSuccess;
            lstTransaction = new List<ObjTransAction>();
            if (!Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                return (this.ErrOperationTimeOut - 0x186a0);
            }
            try
            {
                switch (this.dev.ConnectType)
                {
                    case 0:
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
                    case 1:
                        operationSuccess = this.GetAllTransactionWithoutLock(out lstTransaction);
                        goto Label_011F;

                    case 2:
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
                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetAllTransaction " + exception.Message);
            }
            Label_011F:
            Monitor.Exit(this.ThreadLock);
            return operationSuccess;
        }

        protected int GetAllTransactionWithoutLock(out List<ObjTransAction> lstTransaction)
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
            ObjTransAction action;
            bool flag4;
            int operationSuccess = this.OperationSuccess;
            int dwWorkCode = 0;
            string dwEnrollNumber = "";
            lstTransaction = new List<ObjTransAction>();
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
            action.InOutState = ((num6 << 4) | ((((byte)num6) == 0xff) ? 2 : (num6 + 10))).ToString();
            action.Time_second = new DateTime(num7, num8, num9, num10, num11, num12).ToString("yyyy-MM-dd HH:mm:ss");
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
            if (flag)
            {
                action = new ObjTransAction
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

        public int GetAllUserBioTemplate(out List<BioTemplate> lstBioTemplate, BioTemplateType BioTpye)
        {
            Exception exception;
            lstBioTemplate = new List<BioTemplate>();
            int operationSuccess = this.OperationSuccess;
            if (!Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                return (this.ErrOperationTimeOut - 0x186a0);
            }
            try
            {
                switch (this.dev.ConnectType)
                {
                    case 0:
                        {
                            ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                            if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                            {
                                break;
                            }
                            try
                            {
                                operationSuccess = this.GetAllUserBioTemplateWithoutLock(out lstBioTemplate, BioTpye);
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown - 0x186a0;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetAllUserBioTemplate : BioTemplateType=" + BioTpye);
                                //SysLogServer.WriteLog(" Exception Infomation : " + exception.Message);
                            }
                            Monitor.Exit(comToken);
                            goto Label_014E;
                        }
                    case 1:
                        operationSuccess = this.GetAllUserBioTemplateWithoutLock(out lstBioTemplate, BioTpye);
                        goto Label_014E;

                    case 2:
                        operationSuccess = this.GetAllUserBioTemplateWithoutLock(out lstBioTemplate, BioTpye);
                        goto Label_014E;

                    default:
                        goto Label_014E;
                }
                operationSuccess = this.ErrOperationTimeOut - 0x186a0;
            }
            catch (Exception exception2)
            {
                exception = exception2;
                operationSuccess = this.ErrUnknown - 0x186a0;
                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetAllUserBioTemplate : BioTemplateType=" + BioTpye);
                //SysLogServer.WriteLog(" Exception Infomation : " + exception.Message);
            }
            Label_014E:
            Monitor.Exit(this.ThreadLock);
            return operationSuccess;
        }

        protected int GetAllUserBioTemplateWithoutLock(out List<BioTemplate> lstBioTemplate, BioTemplateType BioTpye)
        {
            int num3;
            int operationSuccess = this.OperationSuccess;
            bool flag = false;
            int bioTemplateVersion = 0;
            int num2 = 0;
            lstBioTemplate = new List<BioTemplate>();
            if (this.dev.BiometricType[(int)BioTpye] == '0')
            {
                return 0;
            }
            bioTemplateVersion = this.GetBioTemplateVersion(this.dev.BiometricVersion.Split(new char[] { ':' }), BioTpye);
            if (bioTemplateVersion <= 0)
            {
                return 0;
            }
            bool flag2 = this.AloneSDK.IsTFTMachine(this.dev.MachineNumber);
            if (!this.IsConnected)
            {
                operationSuccess = this.ConnectWithoutLock();
                if (operationSuccess != this.OperationSuccess)
                {
                    return (operationSuccess - 0x186a0);
                }
                operationSuccess = this.OperationSuccess;
            }
            operationSuccess = this.GetDeviceStatus(MachineDataStatusCode.RegistedUserCount, out num3);
            if (operationSuccess < 0)
            {
                return operationSuccess;
            }
            operationSuccess = this.OperationSuccess;
            num2 += num3;
            int num5 = this.AloneSDK.SSR_GetDeviceDataCount(Tables.pers_biotemplate.ToString(), string.Format("Type={0}", (int)BioTpye), "");
            if (num5 <= 0)
            {
                return 0;
            }
            num2 += num5;
            if ((num2 == 0) || (num5 == 0))
            {
                return 0;
            }
            this.AloneSDK.DisableDeviceWithTimeOut(this.dev.MachineNumber, (this.dev.ConnectType == 0) ? (num2 * 2) : (num2 + 2));
            if ((this.dev.BiometricVersion.Split(new char[] { ':' })[(int)BioTpye] != "0") && (num5 > 0))
            {
                int bufferSize = (num5 * 0xdac) * ((BioTpye == BioTemplateType.Type1) ? 10 : ((BioTpye == BioTemplateType.Type2) ? 12 : 0));
                string buffer = string.Empty;
                if (bioTemplateVersion != 7)
                {
                    if ((bioTemplateVersion == 8) && (this.AloneSDK.SSR_GetDeviceData(this.dev.MachineNumber, out buffer, bufferSize, Tables.pers_biotemplate.ToString(), "*", string.Format("Type={0}", (int)BioTpye), "") && (buffer.Length > 0)))
                    {
                        lstBioTemplate = this.AnalyzeBiotemplateBuffer(buffer);
                    }
                }
                else if (!this.AloneSDK.ReadAllUserID(this.dev.MachineNumber))
                {
                    this.AloneSDK.GetLastError(ref operationSuccess);
                    this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                }
                else
                {
                    this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                    while (true)
                    {
                        int num7;
                        string str4;
                        string str5;
                        int dwEnrollNumber = num7 = 0;
                        string str3 = str4 = str5 = "";
                        bool enabled = true;
                        if (flag2)
                        {
                            flag = this.AloneSDK.SSR_GetAllUserInfo(this.dev.MachineNumber, out str3, out str4, out str5, out num7, out enabled);
                        }
                        else
                        {
                            flag = this.AloneSDK.GetAllUserInfo(this.dev.MachineNumber, ref dwEnrollNumber, ref str4, ref str5, ref num7, ref enabled);
                            str3 = dwEnrollNumber.ToString();
                        }
                        if (!flag)
                        {
                            break;
                        }
                        if (this.AloneSDK.SSR_GetDeviceData(this.dev.MachineNumber, out buffer, bufferSize, Tables.pers_biotemplate.ToString(), "*", string.Format("Pin={0}\tType={1}", str3, (int)BioTpye), ""))
                        {
                            List<BioTemplate> list = new List<BioTemplate>();
                            list = this.AnalyzeBiotemplateBuffer(buffer);
                            foreach (BioTemplate template in list)
                            {
                                lstBioTemplate.Add(template);
                            }
                            if (lstBioTemplate.Count == (num5 * 12))
                            {
                                break;
                            }
                        }
                    }
                }
                if ((lstBioTemplate != null) && (lstBioTemplate.Count > 0))
                {
                    lstBioTemplate = (from a in lstBioTemplate
                                      orderby a.BadgeNumber, a.BioType, a.Version, a.IsDuress, a.TemplateNO, a.TemplateNOIndex
                                      select a).ToList<BioTemplate>();
                }
            }
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }

        protected int GetAllUserFaceTemplateWithoutLock(out List<BioTemplate> lstBioTemplate, BioTemplateType BioTpye)
        {
            int num3;
            int num7;
            int operationSuccess = this.OperationSuccess;
            int num2 = 0;
            lstBioTemplate = new List<BioTemplate>();
            if (this.dev.FaceFunOn != 1)
            {
                return 0;
            }
            if (!this.IsConnected)
            {
                operationSuccess = this.ConnectWithoutLock();
                if (operationSuccess != this.OperationSuccess)
                {
                    return (operationSuccess - 0x186a0);
                }
                operationSuccess = this.OperationSuccess;
            }
            bool flag3 = this.AloneSDK.IsTFTMachine(this.dev.MachineNumber);
            operationSuccess = this.GetDeviceStatus(MachineDataStatusCode.RegistedUserCount, out num3);
            if (operationSuccess < 0)
            {
                return operationSuccess;
            }
            operationSuccess = this.OperationSuccess;
            num2 += num3;
            operationSuccess = this.GetDeviceStatus(MachineDataStatusCode.FaceTemplateCount, out num7);
            if (operationSuccess < 0)
            {
                return operationSuccess;
            }
            operationSuccess = this.OperationSuccess;
            num2 += num7;
            if ((num2 == 0) || (num7 == 0))
            {
                return 0;
            }
            this.AloneSDK.DisableDeviceWithTimeOut(this.dev.MachineNumber, (this.dev.ConnectType == 0) ? (num2 * 2) : (num2 + 2));
            string[] strArray = this.dev.BiometricVersion.Split(new char[] { ':' });
            string[] strArray2 = this.dev.BiometricUsedCount.Split(new char[] { ':' });
            string buffer = string.Empty;
            int result = 0;
            int.TryParse(strArray2[1], out result);
            if ((strArray[1] != "0") && (result > 0))
            {
                int bufferSize = result * 0xdac;
                if (this.AloneSDK.SSR_GetDeviceData(this.dev.MachineNumber, out buffer, bufferSize, Tables.pers_biotemplate.ToString(), "*", string.Format("Type={0}", BioTpye), ""))
                {
                    lstBioTemplate = this.AnalyzeBiotemplateBuffer(buffer);
                }
            }
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }

        public int GetAllUserFPBioTemplate(out List<BioTemplate> lstBioTemplate, BioTemplateType BioTpye)
        {
            Exception exception;
            lstBioTemplate = new List<BioTemplate>();
            int operationSuccess = this.OperationSuccess;
            if (!Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                return (this.ErrOperationTimeOut - 0x186a0);
            }
            try
            {
                switch (this.dev.ConnectType)
                {
                    case 0:
                        {
                            ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                            if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                            {
                                break;
                            }
                            try
                            {
                                operationSuccess = this.GetAllUserFPTemplateWithoutLock(out lstBioTemplate, BioTpye);
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown - 0x186a0;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetAllUserBioTemplate " + exception.Message);
                            }
                            Monitor.Exit(comToken);
                            goto Label_0122;
                        }
                    case 1:
                        operationSuccess = this.GetAllUserFPTemplateWithoutLock(out lstBioTemplate, BioTpye);
                        goto Label_0122;

                    case 2:
                        operationSuccess = this.GetAllUserFPTemplateWithoutLock(out lstBioTemplate, BioTpye);
                        goto Label_0122;

                    default:
                        goto Label_0122;
                }
                operationSuccess = this.ErrOperationTimeOut - 0x186a0;
            }
            catch (Exception exception2)
            {
                exception = exception2;
                operationSuccess = this.ErrUnknown - 0x186a0;
                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetAllUserFPTemplate " + exception.Message);
            }
            Label_0122:
            Monitor.Exit(this.ThreadLock);
            return operationSuccess;
        }

        protected int GetAllUserFPTemplateWithoutLock(out List<BioTemplate> lstBioTemplate, BioTemplateType BioTpye)
        {
            int deviceDataCount;
            int operationSuccess = this.OperationSuccess;
            int num2 = 0;
            lstBioTemplate = new List<BioTemplate>();
            if (this.dev.CompatOldFirmware == "0")
            {
                if (((this.dev.FingerFunOn == null) || (this.dev.FingerFunOn.Trim() != "1")) && ((this.dev.BiometricType.Length >= BioTpye) && (this.dev.BiometricType[(int)BioTpye] == '0')))
                {
                    return 0;
                }
            }
            else if (this.dev.CardFun == 1)
            {
                return 0;
            }
            if (!this.IsConnected)
            {
                operationSuccess = this.ConnectWithoutLock();
                if (operationSuccess != this.OperationSuccess)
                {
                    return (operationSuccess - 0x186a0);
                }
                operationSuccess = this.OperationSuccess;
            }
            bool flag = this.AloneSDK.IsTFTMachine(this.dev.MachineNumber);
            operationSuccess = this.GetDeviceStatus(MachineDataStatusCode.RegistedUserCount, out deviceDataCount);
            if (operationSuccess < 0)
            {
                return operationSuccess;
            }
            operationSuccess = this.OperationSuccess;
            num2 += deviceDataCount;
            switch (BioTpye)
            {
                case BioTemplateType.Type1:
                    operationSuccess = this.GetDeviceStatus(MachineDataStatusCode.TemplateCount, out deviceDataCount);
                    break;

                case BioTemplateType.Type7:
                    deviceDataCount = this.GetDeviceDataCount(Tables.pers_biotemplate.ToString(), BiometricType.FingerVein);
                    break;
            }
            if (operationSuccess < 0)
            {
                return operationSuccess;
            }
            operationSuccess = this.OperationSuccess;
            num2 += deviceDataCount;
            if ((num2 == 0) || (deviceDataCount == 0))
            {
                return 0;
            }
            this.AloneSDK.DisableDeviceWithTimeOut(this.dev.MachineNumber, (this.dev.ConnectType == 0) ? (num2 * 2) : (num2 + 2));
            string buffer = string.Empty;
            string[] strArray = this.dev.BiometricVersion.Split(new char[] { ':' });
            string[] strArray2 = this.dev.BiometricUsedCount.Split(new char[] { ':' });
            int result = 0;
            int.TryParse(strArray2[1], out result);
            if (result > deviceDataCount)
            {
                deviceDataCount = result;
            }
            if (deviceDataCount > 0)
            {
                int bufferSize = deviceDataCount * 0xdac;
                if (this.AloneSDK.SSR_GetDeviceData(this.dev.MachineNumber, out buffer, bufferSize, Tables.pers_biotemplate.ToString(), "*", string.Format("Type={0}", (int)BioTpye), ""))
                {
                    lstBioTemplate = this.AnalyzeBiotemplateBuffer(buffer);
                }
                if ((lstBioTemplate != null) && (lstBioTemplate.Count > 0))
                {
                    lstBioTemplate = (from a in lstBioTemplate
                                      orderby a.BadgeNumber, a.BioType, a.Version, a.IsDuress, a.TemplateNO, a.TemplateNOIndex
                                      select a).ToList<BioTemplate>();
                }
            }
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }

        public int GetAllUserInfo(out List<ObjUser> lstUser)
        {
            Exception exception;
            int operationSuccess = this.OperationSuccess;
            lstUser = new List<ObjUser>();
            if (!Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                return (this.ErrOperationTimeOut - 0x186a0);
            }
            try
            {
                switch (this.dev.ConnectType)
                {
                    case 0:
                        {
                            ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                            if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                            {
                                break;
                            }
                            try
                            {
                                operationSuccess = this.GetAllUserInfoWithoutLock(out lstUser);
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown - 0x186a0;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetAllUserInfoCOM " + exception.Message);
                            }
                            Monitor.Exit(comToken);
                            goto Label_011F;
                        }
                    case 1:
                        operationSuccess = this.GetAllUserInfoWithoutLock(out lstUser);
                        goto Label_011F;

                    case 2:
                        operationSuccess = this.GetAllUserInfoWithoutLock(out lstUser);
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
                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetAllUserInfo " + exception.Message);
            }
            Label_011F:
            Monitor.Exit(this.ThreadLock);
            return operationSuccess;
        }

        protected int GetAllUserInfoWithoutLock(out List<ObjUser> lstUser)
        {
            int num3;
            int num4;
            bool flag2;
            string str2;
            string str3;
            ObjUser user;
            bool flag4;
            int operationSuccess = this.OperationSuccess;
            string aCardNumber = "";
            int num2 = 0;
            int dwEnrollNumber = num4 = 0;
            bool enabled = true;
            string str = str2 = str3 = "";
            lstUser = new List<ObjUser>();
            if (!this.IsConnected)
            {
                operationSuccess = this.ConnectWithoutLock();
                if (operationSuccess != this.OperationSuccess)
                {
                    return (operationSuccess - 0x186a0);
                }
                operationSuccess = this.OperationSuccess;
            }
            operationSuccess = this.GetDeviceStatus(MachineDataStatusCode.RegistedUserCount, out num3);
            if (operationSuccess < 0)
            {
                return operationSuccess;
            }
            operationSuccess = this.OperationSuccess;
            num2 += num3;
            if (num2 <= 0)
            {
                return 0;
            }
            bool flag3 = this.AloneSDK.IsTFTMachine(this.dev.MachineNumber);
            this.AloneSDK.DisableDeviceWithTimeOut(this.dev.MachineNumber, (this.dev.ConnectType == 0) ? (num2 * 2) : (num2 + 2));
            if (!this.AloneSDK.ReadAllUserID(this.dev.MachineNumber))
            {
                this.AloneSDK.GetLastError(ref operationSuccess);
                this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                goto Label_030F;
            }
            this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
            goto Label_0306;
            Label_0263:
            this.AloneSDK.GetStrCardNumber(out aCardNumber);
            if (!string.IsNullOrEmpty(aCardNumber) && (aCardNumber != "0"))
            {
                user.CardNo = aCardNumber;
                switch (AccCommon.CodeVersion)
                {
                    case CodeVersionType.Main:
                        user.CardNo = aCardNumber;
                        break;

                    case CodeVersionType.JapanAF:
                        user.CardNo = aCardNumber;
                        try
                        {
                            user.CardNo = Convert.ToUInt64(aCardNumber, 0x10).ToString("X");
                        }
                        catch (Exception)
                        {
                        }
                        break;
                }
            }
            lstUser.Add(user);
            Label_0306:
            flag4 = true;
            if (flag3)
            {
                flag2 = this.AloneSDK.SSR_GetAllUserInfo(this.dev.MachineNumber, out str, out str2, out str3, out num4, out enabled);
            }
            else
            {
                flag2 = this.AloneSDK.GetAllUserInfo(this.dev.MachineNumber, ref dwEnrollNumber, ref str2, ref str3, ref num4, ref enabled);
                str = dwEnrollNumber.ToString();
            }
            if (flag2)
            {
                int index = str2.IndexOf('\0');
                if (index >= 0)
                {
                    str2 = str2.Substring(0, index);
                }
                index = str3.IndexOf('\0');
                if (index >= 0)
                {
                    str3 = str3.Substring(0, index);
                }
                user = new ObjUser
                {
                    Pin = str,
                    Name = str2,
                    Password = str3
                };
                switch (num4)
                {
                    case 0:
                        user.Privilege = 0;
                        goto Label_0263;

                    case 3:
                    case 14:
                        user.Privilege = 3;
                        goto Label_0263;

                    default:
                        user.Privilege = num4;
                        goto Label_0263;
                }
            }
            Label_030F:
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }

        private int GetBioTemplateVersion(string[] biometricVersion, BioTemplateType BioTpye)
        {
            double result = 0.0;
            biometricVersion = this.dev.BiometricVersion.Split(new char[] { ':' });
            double.TryParse(biometricVersion[(int)BioTpye], NumberStyles.Float, CultureInfo.InvariantCulture, out result);
            return Convert.ToInt32(result);
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
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetCardFun_Com " + exception.Message);
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
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetCardFun " + exception.Message);
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
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetDeviceStatus_Com " + exception.Message);
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
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetDeviceStatus " + this.dev.MachineAlias + ". " + exception.Message);
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

        public int GetDeviceDataCount(string tableName, BiometricType aBioType)
        {
            string filter = string.Format("Type={0}", (int)aBioType);
            string options = "";
            return this.AloneSDK.SSR_GetDeviceDataCount(tableName, filter, options);
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
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetDeviceInfo_Com " + exception.Message);
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
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetDeviceInfo " + code.ToString() + ". " + exception.Message);
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
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetDeviceIP_Com " + exception.Message);
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
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetDeviceIP " + exception.Message);
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
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetDeviceStatus_Com " + exception.Message);
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
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetDeviceStatus " + code.ToString() + ". " + exception.Message);
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

        public int GetDoorState(out int StateCode)
        {
            int operationSuccess = this.OperationSuccess;
            StateCode = 0;
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
                                if (!this.AloneSDK.GetDoorState(this.dev.MachineNumber, ref StateCode))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetDoorState_Com " + exception.Message);
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
                    else if (!this.AloneSDK.GetDoorState(this.dev.MachineNumber, ref StateCode))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetDoorState " + exception.Message);
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

        private void GetFieldIndex(string bufferLine, out int iPin, out int iValid, out int iDuress, out int iType, out int iMaxVer, out int iMinVer, out int iFormat, out int iNo, out int iIndex, out int iTmp)
        {
            string[] array = bufferLine.Split(new char[] { ',' });
            iPin = Array.IndexOf<string>(array, "Pin");
            if (iPin < 0)
            {
                iPin = Array.IndexOf<string>(array, "PIN2");
            }
            iValid = Array.IndexOf<string>(array, "Valid");
            iDuress = Array.IndexOf<string>(array, "Duress");
            iType = Array.IndexOf<string>(array, "Type");
            iMaxVer = Array.IndexOf<string>(array, "MajorVer");
            iMinVer = Array.IndexOf<string>(array, "MinorVer");
            iFormat = Array.IndexOf<string>(array, "Format");
            iNo = Array.IndexOf<string>(array, "No");
            iIndex = Array.IndexOf<string>(array, "Index");
            iTmp = Array.IndexOf<string>(array, "Tmp");
        }

       /* public bool GetFingerVein(out List<FingerVein> lstFingerVein)
        {
            lstFingerVein = new List<FingerVein>();
            string buffer = string.Empty;
            int bufferSize = 0x5000000;
            string tableName = "pers_biotemplate";
            string fieldNames = "*";
            string filter = string.Format("Type={0}", 7);
            string options = "";
            if (!this.AloneSDK.SSR_GetDeviceData(this.dev.MachineNumber, out buffer, bufferSize, tableName, fieldNames, filter, options))
            {
                int dwErrorCode = 0;
                this.AloneSDK.GetLastError(ref dwErrorCode);
                return false;
            }
            if (!string.IsNullOrWhiteSpace(buffer))
            {
                string[] strArray = buffer.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if ((strArray != null) && (strArray.Length > 0))
                {
                    string str6 = strArray[0];
                    for (int i = 1; i < strArray.Length; i++)
                    {
                        FingerVein item = new FingerVein();
                        string[] strArray2 = str6.Split(new char[] { ',' });
                        string[] strArray3 = strArray[i].Split(new char[] { ',' });
                        if ((strArray2.Length > 0) && (strArray2.Length == strArray3.Length))
                        {
                            for (int j = 0; j < strArray2.Length; j++)
                            {
                                int duressFlag;
                                switch (strArray2[j].ToLowerInvariant())
                                {
                                    case "tmp":
                                        item.Template = Convert.FromBase64String(strArray3[j]);
                                        break;

                                    case "pin":
                                        item.Pin = strArray3[j];
                                        break;

                                    case "duress":
                                        duressFlag = item.DuressFlag;
                                        int.TryParse(strArray3[j], out duressFlag);
                                        item.DuressFlag = duressFlag;
                                        break;

                                    case "no":
                                        duressFlag = item.FingerID;
                                        int.TryParse(strArray3[j], out duressFlag);
                                        item.FingerID = duressFlag;
                                        break;

                                    case "index":
                                        duressFlag = item.Fv_ID_Index;
                                        int.TryParse(strArray3[j], out duressFlag);
                                        item.Fv_ID_Index = duressFlag;
                                        break;
                                }
                            }
                            item.Size = item.Template.Length;
                        }
                        lstFingerVein.Add(item);
                    }
                }
            }
            return true;
        }
        */
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
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetFirmwareVersion_Com " + exception.Message);
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
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetFirmwareVersion " + exception.Message);
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

        private void GetParamInConn()
        {
            int num=0;
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

        public int GetSDKVersion(out string SDKVer)
        {
            int operationSuccess = this.OperationSuccess;
            SDKVer = string.Empty;
            try
            {
                if (!this.AloneSDK.GetSDKVersion(ref SDKVer))
                {
                    this.AloneSDK.GetLastError(ref operationSuccess);
                }
            }
            catch (Exception exception)
            {
                operationSuccess = this.ErrUnknown;
                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetSDKVersion " + exception.Message);
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
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetSerialNumber_Com " + exception.Message);
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
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetSerialNumber " + exception.Message);
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
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetSysOption_Com " + exception.Message);
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
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetSysOption " + exception.Message);
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
/*
        public int GetUserBioPalmVeinTemplate(out List<BioTemplate> lstBioTemplate, BioTemplateType BioTpye)
        {
            int deviceDataCount;
            int operationSuccess = this.OperationSuccess;
            int num2 = 0;
            lstBioTemplate = new List<BioTemplate>();
            if (this.dev.BiometricType[(int)BioTpye] == '0')
            {
                return 0;
            }
            if (!this.IsConnected)
            {
                operationSuccess = this.ConnectWithoutLock();
                if (operationSuccess != this.OperationSuccess)
                {
                    return (operationSuccess - 0x186a0);
                }
                operationSuccess = this.OperationSuccess;
            }
            operationSuccess = this.GetDeviceStatus(MachineDataStatusCode.RegistedUserCount, out deviceDataCount);
            if (operationSuccess < 0)
            {
                return operationSuccess;
            }
            operationSuccess = this.OperationSuccess;
            num2 += deviceDataCount;
            deviceDataCount = this.GetDeviceDataCount(Tables.pers_biotemplate.ToString(), BiometricType.PalmVein);
            num2 += deviceDataCount;
            if ((num2 == 0) || (deviceDataCount == 0))
            {
                return 0;
            }
            this.AloneSDK.DisableDeviceWithTimeOut(this.dev.MachineNumber, (this.dev.ConnectType == 0) ? (num2 * 2) : (num2 + 2));
            string buffer = string.Empty;
            string[] strArray = this.dev.BiometricVersion.Split(new char[] { ':' });
            string[] strArray2 = this.dev.BiometricUsedCount.Split(new char[] { ':' });
            int result = 0;
            int.TryParse(strArray2[1], out result);
            if (result > deviceDataCount)
            {
                deviceDataCount = result;
            }
            if (deviceDataCount > 0)
            {
                int bufferSize = deviceDataCount * 0xdac;
                if (this.AloneSDK.SSR_GetDeviceData(this.dev.MachineNumber, out buffer, bufferSize, Tables.pers_biotemplate.ToString(), "*", string.Format("Type={0}", (int)BioTpye), ""))
                {
                    lstBioTemplate = this.AnalyzeBiotemplateBuffer(buffer);
                }
            }
            if ((lstBioTemplate != null) && (lstBioTemplate.Count > 0))
            {
                lstBioTemplate = (from a in lstBioTemplate
                                  orderby a.BadgeNumber, a.BioType, a.Version, a.IsDuress, a.TemplateNO, a.TemplateNOIndex
                                  select a).ToList<BioTemplate>();
            }
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }

        public int GetUserFPTemplate(int Pin, int FingerId, out ObjTemplateV10 Template)
        {
            int operationSuccess = this.OperationSuccess;
            int num3 = 0;
            int tmpLength = 0;
            string tmpData = "";
            int num2 = 0;
            Template = null;
            if (this.dev.CompatOldFirmware == "0")
            {
                if ((this.dev.FingerFunOn == null) || (this.dev.FingerFunOn.Trim() != "1"))
                {
                    return 0;
                }
            }
            else if (this.dev.CardFun == 1)
            {
                return 0;
            }
            if (!this.IsConnected)
            {
                operationSuccess = this.Connect();
                if (operationSuccess < 0)
                {
                    return operationSuccess;
                }
                operationSuccess = this.OperationSuccess;
            }
            bool flag2 = false;
            bool flag = this.AloneSDK.IsTFTMachine(this.dev.MachineNumber);
            num2 = 1;
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
                                this.AloneSDK.DisableDeviceWithTimeOut(this.dev.MachineNumber, (this.dev.ConnectType == 0) ? (num2 * 2) : (num2 + 2));
                                num3 = 1;
                                if ((this.dev.FpVersion == 10) || (this.FirmwareVersion >= 6.6M))
                                {
                                    flag2 = this.AloneSDK.GetUserTmpExStr(this.dev.MachineNumber, Pin.ToString(), FingerId & 15, out num3, out tmpData, out tmpLength);
                                }
                                else if (flag)
                                {
                                    flag2 = this.AloneSDK.SSR_GetUserTmpStr(this.dev.MachineNumber, Pin.ToString(), FingerId & 15, out tmpData, out tmpLength);
                                }
                                else
                                {
                                    flag2 = this.AloneSDK.GetUserTmpStr(this.dev.MachineNumber, Pin, FingerId & 15, ref tmpData, ref tmpLength);
                                }
                                if (!flag2)
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                    this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                                }
                                else
                                {
                                    Template = new ObjTemplateV10();
                                    Template.FingerID = ((FingerType)FingerId) & (FingerType.Type9 | FingerType.Type6);
                                    Template.Pin = Pin.ToString();
                                    Template.Template = tmpData;
                                    Template.Size = tmpLength;
                                    if (num3 == 3)
                                    {
                                        Template.Valid = ValidType.Type3;
                                    }
                                    else
                                    {
                                        Template.Valid = ValidType.Type1;
                                    }
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetUserFPTemplate_Com " + exception.Message);
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
                        this.AloneSDK.DisableDeviceWithTimeOut(this.dev.MachineNumber, (this.dev.ConnectType == 0) ? (num2 * 2) : (num2 + 2));
                        num3 = 1;
                        if ((this.dev.FpVersion == 10) || (this.FirmwareVersion >= 6.6M))
                        {
                            flag2 = this.AloneSDK.GetUserTmpExStr(this.dev.MachineNumber, Pin.ToString(), FingerId & 15, out num3, out tmpData, out tmpLength);
                        }
                        else if (flag)
                        {
                            flag2 = this.AloneSDK.SSR_GetUserTmpStr(this.dev.MachineNumber, Pin.ToString(), FingerId & 15, out tmpData, out tmpLength);
                        }
                        else
                        {
                            flag2 = this.AloneSDK.GetUserTmpStr(this.dev.MachineNumber, Pin, FingerId & 15, ref tmpData, ref tmpLength);
                        }
                        if (!flag2)
                        {
                            this.AloneSDK.GetLastError(ref operationSuccess);
                            this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                        }
                        else
                        {
                            Template = new ObjTemplateV10();
                            Template.FingerID = ((FingerType)FingerId) & (FingerType.Type9 | FingerType.Type6);
                            Template.Pin = Pin.ToString();
                            Template.Template = tmpData;
                            Template.Size = tmpLength;
                            if (num3 == 3)
                            {
                                Template.Valid = ValidType.Type3;
                            }
                            else
                            {
                                Template.Valid = ValidType.Type1;
                            }
                        }
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetUserFPTemplate " + exception.Message);
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
        */
        protected int InitialCommunicationWithoutLock(int WorkingTimeOut = 0, bool UseBatchMode = false)
        {
            int operationSuccess = this.OperationSuccess;
            if (!this.IsConnected)
            {
                operationSuccess = this.ConnectWithoutLock();
                if (operationSuccess != this.OperationSuccess)
                {
                    return operationSuccess;
                }
            }
            if ((UseBatchMode && this.dev.BatchUpdate) && !this.AloneSDK.BeginBatchUpdate(this.dev.MachineNumber, 1))
            {
                this.AloneSDK.GetLastError(ref operationSuccess);
                return operationSuccess;
            }
            if ((WorkingTimeOut > 0) && !this.AloneSDK.DisableDeviceWithTimeOut(this.dev.MachineNumber, WorkingTimeOut))
            {
                this.AloneSDK.GetLastError(ref operationSuccess);
                return operationSuccess;
            }
            return operationSuccess;
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
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on IsTFTMachine. " + exception.Message);
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

        public int PowerOffDevice()
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
                                if (!this.AloneSDK.PowerOffDevice(this.dev.MachineNumber))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on PowerOffDevice_Com " + exception.Message);
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
                    else if (!this.AloneSDK.PowerOffDevice(this.dev.MachineNumber))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on PowerOffDevice " + exception.Message);
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

        public int RaiseRTEvent()
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
                    case 0:
                        {
                            ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                            if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                            {
                                break;
                            }
                            try
                            {
                                operationSuccess = this.RaiseRTEventWithoutLock();
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on RaiseRTEventCOM " + exception.Message);
                            }
                            Monitor.Exit(comToken);
                            goto Label_0103;
                        }
                    case 1:
                        operationSuccess = this.RaiseRTEventWithoutLock();
                        goto Label_0103;

                    case 2:
                        operationSuccess = this.RaiseRTEventWithoutLock();
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
                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on RaiseRTEvent " + exception.Message);
            }
            Label_0103:
            Monitor.Exit(this.ThreadLock);
            Label_011C:
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }

        protected int RaiseRTEventWithoutLock()
        {
            int operationSuccess = this.OperationSuccess;
            if (!this.IsConnected)
            {
                operationSuccess = this.ConnectWithoutLock();
                if (operationSuccess != this.OperationSuccess)
                {
                    return operationSuccess;
                }
            }
            if (!this.AloneSDK.ReadRTLog(this.dev.MachineNumber))
            {
                this.AloneSDK.GetLastError(ref operationSuccess);
                return operationSuccess;
            }
            while (this.AloneSDK.GetRTLog(this.dev.MachineNumber))
            {
            }
            return operationSuccess;
        }

        public int RebootDevice()
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
                                if (!this.AloneSDK.RestartDevice(this.dev.MachineNumber))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on RebootDevice_Com " + exception.Message);
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
                    else if (!this.AloneSDK.RestartDevice(this.dev.MachineNumber))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on RebootDevice " + exception.Message);
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

        public int RefreshData()
        {
            return this.EndCommunicationWithoutLock(false, true);
        }

        public int RegEvent(RegRTEventCode EventMask)
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
                                if (!this.AloneSDK.RegEvent(this.dev.MachineNumber, (int)EventMask))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                    this.bolEventRegistered = false;
                                }
                                else
                                {
                                    this.bolEventRegistered = true;
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on RegEvent_Com " + exception.Message);
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
                    else if (!this.AloneSDK.RegEvent(this.dev.MachineNumber, (int)EventMask))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                        this.bolEventRegistered = false;
                    }
                    else
                    {
                        this.bolEventRegistered = true;
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on RegEvent " + exception.Message);
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

        private int ReturnMemoryNotEnough(int result)
        {
            if ((((result == -4990) || (result == -4)) || ((result == -2001) || (result == this.Err_No_Data))) || (result == this.ErrDel_No_Data))
            {
                result = this.OperationSuccess;
            }
            return result;
        }

        public int SearchDevice(out string buffer)
        {
            int operationSuccess = this.OperationSuccess;
            string commType = "UDP";
            string address = "255.255.255.255";
            int devBufferSize = 0x100000;
            buffer = "";
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
                                if (!this.AloneSDK.SearchDevice(commType, address, out buffer, devBufferSize))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(" Exception on SearchDevice " + exception.Message);
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
                    else if (!this.AloneSDK.SearchDevice(commType, address, out buffer, devBufferSize))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetDeviceStatus " + exception.Message);
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

        public int SendFile(string FileName, int EnabledTimeOut = 600)
        {
            int operationSuccess = this.OperationSuccess;
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
                                operationSuccess = this.ConnectWithoutLock();
                                if (operationSuccess != this.OperationSuccess)
                                {
                                    return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
                                }
                                if (!this.AloneSDK.DisableDeviceWithTimeOut(this.dev.MachineNumber, EnabledTimeOut))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                                else if (!this.AloneSDK.SendFile(this.dev.MachineNumber, FileName))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                                else if (!this.AloneSDK.EnableDevice(this.dev.MachineNumber, true))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SendFile_Com " + exception.Message);
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
                        operationSuccess = this.ConnectWithoutLock();
                        if (operationSuccess != this.OperationSuccess)
                        {
                            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
                        }
                        if (!this.AloneSDK.DisableDeviceWithTimeOut(this.dev.MachineNumber, EnabledTimeOut))
                        {
                            this.AloneSDK.GetLastError(ref operationSuccess);
                        }
                        else if (!this.AloneSDK.SendFile(this.dev.MachineNumber, FileName))
                        {
                            this.AloneSDK.GetLastError(ref operationSuccess);
                        }
                        else if (!this.AloneSDK.EnableDevice(this.dev.MachineNumber, true))
                        {
                            this.AloneSDK.GetLastError(ref operationSuccess);
                        }
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SendFile " + exception.Message);
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

        public int SetDeviceCommPwd(int CommPwd)
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
                                if (operationSuccess == this.OperationSuccess)
                                {
                                    if (!this.AloneSDK.SetDeviceCommPwd(this.dev.MachineNumber, CommPwd))
                                    {
                                        this.AloneSDK.GetLastError(ref operationSuccess);
                                    }
                                    else
                                    {
                                        this.dev.CommPassword = CommPwd.ToString();
                                        this.AloneSDK.SetCommPassword(CommPwd);
                                    }
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetDeviceCommPwd_Com " + exception.Message);
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
                    else if (operationSuccess == this.OperationSuccess)
                    {
                        if (!this.AloneSDK.SetDeviceCommPwd(this.dev.MachineNumber, CommPwd))
                        {
                            this.AloneSDK.GetLastError(ref operationSuccess);
                        }
                        else
                        {
                            this.dev.CommPassword = CommPwd.ToString();
                            this.AloneSDK.SetCommPassword(CommPwd);
                        }
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetDeviceCommPwd " + exception.Message);
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

        public int SetDeviceData(string data, out string persList)
        {
            int num = 0;
            int operationSuccess = this.OperationSuccess;
            persList = string.Empty;
            if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                Exception exception;
                try
                {
                    switch (this.dev.ConnectType)
                    {
                        case 0:
                            {
                                ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                                if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                                {
                                    break;
                                }
                                try
                                {
                                    num = this.SetUserBioTemplateWithoutLock(data, out persList);
                                }
                                catch (Exception exception1)
                                {
                                    exception = exception1;
                                    operationSuccess = this.ErrUnknown;
                                    //SysLogServer.WriteLog(this.dev.MachineAlias + "Exception on SetUserBioTemplateCOM, Exception Infomation : " + exception.Message);
                                }
                                Monitor.Exit(comToken);
                                goto Label_00E8;
                            }
                        case 1:
                            num = this.SetUserBioTemplateWithoutLock(data, out persList);
                            goto Label_00E8;

                        case 2:
                            num = this.SetUserBioTemplateWithoutLock(data, out persList);
                            goto Label_00E8;

                        default:
                            goto Label_00E8;
                    }
                    operationSuccess = this.ErrOperationTimeOut - 0x186a0;
                    Label_00E8:
                    if (num < 0)
                    {
                        operationSuccess = num + 0x186a0;
                    }
                    else
                    {
                        operationSuccess = this.OperationSuccess;
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetUserBioTemplate, Exception Infomation : " + exception.Message);
                }
                Monitor.Exit(this.ThreadLock);
            }
            else
            {
                operationSuccess = this.ErrOperationTimeOut;
            }
            return ((operationSuccess == this.OperationSuccess) ? num : (operationSuccess - 0x186a0));
        }

        public int SetDeviceInfo(DeviceInfoCode code, int value)
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
                                if (!((operationSuccess != this.OperationSuccess) || this.AloneSDK.SetDeviceInfo(this.dev.MachineNumber, (int)code, value)))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetDeviceInfo_Com " + exception.Message);
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
                    else if (!((operationSuccess != this.OperationSuccess) || this.AloneSDK.SetDeviceInfo(this.dev.MachineNumber, (int)code, value)))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetDeviceInfo " + code.ToString() + ". " + exception.Message);
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

        public int SetDeviceIP(string IPAddr)
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
                                if (!((operationSuccess != this.OperationSuccess) || this.AloneSDK.SetDeviceIP(this.dev.MachineNumber, IPAddr)))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                                else
                                {
                                    this.dev.IP = IPAddr;
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetDeviceIP_Com " + exception.Message);
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
                    else if (!((operationSuccess != this.OperationSuccess) || this.AloneSDK.SetDeviceIP(this.dev.MachineNumber, IPAddr)))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                    else
                    {
                        this.dev.IP = IPAddr;
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetDeviceIP " + exception.Message);
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
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetDeviceTime_Com " + exception.Message);
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
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetDeviceTime " + exception.Message);
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

    /*    public bool SetFingerVein(List<FingerVein> lstFingerVein, out List<FingerVein> failedList)
        {
            string tableName = "pers_biotemplate";
            string options = "";
            int num = 100;
            List<FingerVein> list = new List<FingerVein>();
            failedList = new List<FingerVein>();
            if ((lstFingerVein != null) && (lstFingerVein.Count > 0))
            {
                int num2;
                foreach (FingerVein vein in lstFingerVein)
                {
                    list.Add(vein);
                    if (list.Count >= num)
                    {
                        if (!this.AloneSDK.SSR_SetDeviceData(this.dev.MachineNumber, tableName, this.AssemblesFingerVeinInfo(list), options))
                        {
                            num2 = 0;
                            this.AloneSDK.GetLastError(ref num2);
                            foreach (FingerVein vein2 in list)
                            {
                                failedList.Add(vein2);
                            }
                        }
                        list.Clear();
                    }
                }
                if (!this.AloneSDK.SSR_SetDeviceData(this.dev.MachineNumber, tableName, this.AssemblesFingerVeinInfo(list), options))
                {
                    num2 = 0;
                    this.AloneSDK.GetLastError(ref num2);
                    foreach (FingerVein vein2 in list)
                    {
                        failedList.Add(vein2);
                    }
                }
            }
            return true;
        }

 /*       public int SetGroup(List<Group> lstGroup)
        {
            Exception exception;
            int operationSuccess = this.OperationSuccess;
            if (this._LockFunOn < 2)
            {
                return 0;
            }
            if (!Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                operationSuccess = this.ErrOperationTimeOut;
                goto Label_0139;
            }
            try
            {
                switch (this.dev.ConnectType)
                {
                    case 0:
                        {
                            ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                            if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                            {
                                break;
                            }
                            try
                            {
                                operationSuccess = this.SetGroupWithoutLock(lstGroup);
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetGroupCOM " + exception.Message);
                            }
                            Monitor.Exit(comToken);
                            goto Label_0120;
                        }
                    case 1:
                        operationSuccess = this.SetGroupWithoutLock(lstGroup);
                        goto Label_0120;

                    case 2:
                        operationSuccess = this.SetGroupWithoutLock(lstGroup);
                        goto Label_0120;

                    default:
                        goto Label_0120;
                }
                operationSuccess = this.ErrOperationTimeOut;
            }
            catch (Exception exception2)
            {
                exception = exception2;
                operationSuccess = this.ErrUnknown;
                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetGroup " + exception.Message);
            }
            Label_0120:
            Monitor.Exit(this.ThreadLock);
            Label_0139:
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }*/

       /* protected int SetGroupWithoutLock(List<Group> lstGroup)
        {
            int operationSuccess = this.OperationSuccess;
            if (this._LockFunOn < 2)
            {
                return 0;
            }
            bool flag = true;
            int count = lstGroup.Count;
            operationSuccess = this.InitialCommunicationWithoutLock((this.dev.ConnectType == 0) ? (count * 2) : (count + 2), false);
            if (operationSuccess == this.OperationSuccess)
            {
                if ((lstGroup != null) && (lstGroup.Count > 0))
                {
                    for (int i = 0; i < lstGroup.Count; i++)
                    {
                        Group group = lstGroup[i];
                        if (group.GroupNo > 0)
                        {
                            if (this.AloneSDK.IsTFTMachine(this.dev.MachineNumber))
                            {
                                flag = this.AloneSDK.SSR_SetGroupTZ(this.dev.MachineNumber, group.GroupNo, group.TimeZoneId1, group.TimeZoneId2, group.TimeZoneId3, group.ValidOnHoliday, group.VerifyMode);
                            }
                            else
                            {
                                flag = this.AloneSDK.SetGroupTZStr(this.dev.MachineNumber, group.GroupNo, string.Format("{0}:{1}:{2}", group.TimeZoneId1, group.TimeZoneId2, group.TimeZoneId3));
                                if (flag)
                                {
                                    flag = this.AloneSDK.SetSysOption(this.dev.MachineNumber, "GVS" + group.GroupNo, group.VerifyMode.ToString());
                                }
                            }
                            if (!flag)
                            {
                                this.AloneSDK.GetLastError(ref operationSuccess);
                                break;
                            }
                        }
                    }
                }
                if (operationSuccess == this.OperationSuccess)
                {
                    operationSuccess = this.EndCommunicationWithoutLock(false, false);
                }
                else if (operationSuccess != this.ErrOperationTimeOut)
                {
                    this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                }
            }
            return operationSuccess;
        }

        public int SetHoliday(List<AccHolidays> lstHoliday)
        {
            Exception exception;
            int operationSuccess = this.OperationSuccess;
            if (this._LockFunOn < 2)
            {
                return 0;
            }
            if (!Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                operationSuccess = this.ErrOperationTimeOut;
                goto Label_0139;
            }
            try
            {
                switch (this.dev.ConnectType)
                {
                    case 0:
                        {
                            ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                            if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                            {
                                break;
                            }
                            try
                            {
                                operationSuccess = this.SetHolidayWithoutLock(lstHoliday);
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetHolidayCOM " + exception.Message);
                            }
                            Monitor.Exit(comToken);
                            goto Label_0120;
                        }
                    case 1:
                        operationSuccess = this.SetHolidayWithoutLock(lstHoliday);
                        goto Label_0120;

                    case 2:
                        operationSuccess = this.SetHolidayWithoutLock(lstHoliday);
                        goto Label_0120;

                    default:
                        goto Label_0120;
                }
                operationSuccess = this.ErrOperationTimeOut;
            }
            catch (Exception exception2)
            {
                exception = exception2;
                operationSuccess = this.ErrUnknown;
                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetHoliday " + exception.Message);
            }
            Label_0120:
            Monitor.Exit(this.ThreadLock);
            Label_0139:
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }

        protected int SetHolidayWithoutLock(List<AccHolidays> lstHoliday)
        {
            int operationSuccess = this.OperationSuccess;
            if (this._LockFunOn < 2)
            {
                return 0;
            }
            bool flag = true;
            int count = lstHoliday.Count;
            string holiday = "";
            DateTime time3 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            operationSuccess = this.InitialCommunicationWithoutLock((this.dev.ConnectType == 0) ? (count * 2) : (count + 2), false);
            if (operationSuccess == this.OperationSuccess)
            {
                if ((lstHoliday != null) && (lstHoliday.Count > 0))
                {
                    for (int i = 0; i < lstHoliday.Count; i++)
                    {
                        AccHolidays holidays = lstHoliday[i];
                        if (!holidays.start_date.HasValue || !holidays.end_date.HasValue)
                        {
                            if (this.AloneSDK.IsTFTMachine(this.dev.MachineNumber))
                            {
                                flag = this.AloneSDK.SSR_SetHoliday(this.dev.MachineNumber, holidays.HolidayNo, 0, 0, 0, 0, holidays.HolidayTZ);
                            }
                            else
                            {
                                flag = this.AloneSDK.SetHoliday(this.dev.MachineNumber, "00000000");
                            }
                        }
                        else
                        {
                            DateTime time = new DateTime(holidays.start_date.Value.Year, holidays.start_date.Value.Month, holidays.start_date.Value.Day);
                            DateTime time2 = new DateTime(holidays.end_date.Value.Year, holidays.end_date.Value.Month, holidays.end_date.Value.Day);
                            if (this.AloneSDK.IsTFTMachine(this.dev.MachineNumber))
                            {
                                flag = this.AloneSDK.SSR_SetHoliday(this.dev.MachineNumber, holidays.HolidayNo, time.Month, time.Day, time2.Month, time2.Day, holidays.HolidayTZ);
                            }
                            else
                            {
                                int num5;
                                if ((holiday == null) || ("" == holiday.Trim()))
                                {
                                    num5 = time.Month - 1;
                                    num5 = time2.Month - 1;
                                    holiday = num5.ToString().PadLeft(2, '0') + time.ToString("dd") + num5.ToString().PadLeft(2, '0') + time2.ToString("dd");
                                }
                                else if (time2 > time3)
                                {
                                    num5 = time.Month - 1;
                                    num5 = time2.Month - 1;
                                    holiday = num5.ToString().PadLeft(2, '0') + time.ToString("dd") + num5.ToString().PadLeft(2, '0') + time2.ToString("dd");
                                }
                            }
                        }
                        if (!flag)
                        {
                            this.AloneSDK.GetLastError(ref operationSuccess);
                            break;
                        }
                    }
                    if (!this.AloneSDK.IsTFTMachine(this.dev.MachineNumber) && !this.AloneSDK.SetHoliday(this.dev.MachineNumber, holiday))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                }
                if (operationSuccess == this.OperationSuccess)
                {
                    operationSuccess = this.EndCommunicationWithoutLock(false, false);
                }
                else if (operationSuccess != this.ErrOperationTimeOut)
                {
                    this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                }
            }
            return operationSuccess;
        }*/

        public int SetSysOption(string OptionName, string Value, bool EffectiveImmediately = true)
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
                                if (!((operationSuccess != this.OperationSuccess) || this.AloneSDK.SetSysOption(this.dev.MachineNumber, OptionName, Value)))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetSysOption_Com " + exception.Message);
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
                    else if (!((operationSuccess != this.OperationSuccess) || this.AloneSDK.SetSysOption(this.dev.MachineNumber, OptionName, Value)))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetSysOption " + exception.Message);
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

        public int SetTimeZone(Dictionary<int, string> DicTimeZone)
        {
            Exception exception;
            int operationSuccess = this.OperationSuccess;
            if (this._LockFunOn < 2)
            {
                return 0;
            }
            if (!Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                operationSuccess = this.ErrOperationTimeOut;
                goto Label_0139;
            }
            try
            {
                switch (this.dev.ConnectType)
                {
                    case 0:
                        {
                            ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                            if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                            {
                                break;
                            }
                            try
                            {
                                operationSuccess = this.SetTimeZoneWithoutLock(DicTimeZone);
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetTimeZone_COM " + exception.Message);
                            }
                            Monitor.Exit(comToken);
                            goto Label_0120;
                        }
                    case 1:
                        operationSuccess = this.SetTimeZoneWithoutLock(DicTimeZone);
                        goto Label_0120;

                    case 2:
                        operationSuccess = this.SetTimeZoneWithoutLock(DicTimeZone);
                        goto Label_0120;

                    default:
                        goto Label_0120;
                }
                operationSuccess = this.ErrOperationTimeOut;
            }
            catch (Exception exception2)
            {
                exception = exception2;
                operationSuccess = this.ErrUnknown;
                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetTimeZone " + exception.Message);
            }
            Label_0120:
            Monitor.Exit(this.ThreadLock);
            Label_0139:
            return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        }

        protected int SetTimeZoneWithoutLock(Dictionary<int, string> DicTimeZone)
        {
            int operationSuccess = this.OperationSuccess;
            if (this._LockFunOn < 2)
            {
                return 0;
            }
            if ((DicTimeZone == null) || (DicTimeZone.Count <= 0))
            {
                return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
            }
            int count = DicTimeZone.Count;
            operationSuccess = this.InitialCommunicationWithoutLock((this.dev.ConnectType == 0) ? (count * 2) : (count + 2), false);
            if (operationSuccess == this.OperationSuccess)
            {
                foreach (KeyValuePair<int, string> pair in DicTimeZone)
                {
                    if (!this.AloneSDK.SetTZInfo(this.dev.MachineNumber, pair.Key, pair.Value))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                        break;
                    }
                }
                if (operationSuccess == this.OperationSuccess)
                {
                    operationSuccess = this.EndCommunicationWithoutLock(false, false);
                }
                else if (operationSuccess != this.ErrOperationTimeOut)
                {
                    this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                }
            }
            return operationSuccess;
        }

        public int SetUnlockGroup(List<ObjMultimCard> lstMultiCard)
        {
            Exception exception;
            int operationSuccess = this.OperationSuccess;
            if (this._LockFunOn < 2)
            {
                return 0;
            }
            if (!Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                operationSuccess = this.ErrOperationTimeOut;
                goto Label_0139;
            }
            try
            {
                switch (this.dev.ConnectType)
                {
                    case 0:
                        {
                            ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                            if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                            {
                                break;
                            }
                            try
                            {
                                operationSuccess = this.SetUnlockGroupWithoutLock(lstMultiCard);
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetUnlockGroupCOM " + exception.Message);
                            }
                            Monitor.Exit(comToken);
                            goto Label_0120;
                        }
                    case 1:
                        operationSuccess = this.SetUnlockGroupWithoutLock(lstMultiCard);
                        goto Label_0120;

                    case 2:
                        operationSuccess = this.SetUnlockGroupWithoutLock(lstMultiCard);
                        goto Label_0120;

                    default:
                        goto Label_0120;
                }
                operationSuccess = this.ErrOperationTimeOut;
            }
            catch (Exception exception2)
            {
                exception = exception2;
                operationSuccess = this.ErrUnknown;
                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetUnlockGroup " + exception.Message);
            }
            Label_0120:
            Monitor.Exit(this.ThreadLock);
            Label_0139:
            return ((operationSuccess == this.OperationSuccess) ? lstMultiCard.Count : (operationSuccess - 0x186a0));
        }

        //protected int SetUnlockGroupWithoutLock(List<ObjMultimCard> lstMultiCard)
        //{
        //    int operationSuccess = this.OperationSuccess;
        //    if (this._LockFunOn < 2)
        //    {
        //        return 0;
        //    }
        //    bool flag = true;
        //    int count = lstMultiCard.Count;
        //    int num9 = 0;
        //    int num10 = 0;
        //    StringBuilder builder = new StringBuilder();
        //    operationSuccess = this.InitialCommunicationWithoutLock((this.dev.ConnectType == 0) ? (count * 2) : (count + 2), false);
        //    if (operationSuccess == this.OperationSuccess)
        //    {
        //        if ((lstMultiCard != null) && (lstMultiCard.Count > 0))
        //        {
        //            for (int i = 0; i < lstMultiCard.Count; i++)
        //            {
        //                int num3;
        //                int num4;
        //                int num5;
        //                int num6;
        //                int num7;
        //                int num8;
        //                ObjMultimCard card = lstMultiCard[i];
        //                int.TryParse(card.Index, out num3);
        //                int.TryParse(card.Group1, out num4);
        //                int.TryParse(card.Group2, out num5);
        //                int.TryParse(card.Group3, out num6);
        //                int.TryParse(card.Group4, out num7);
        //                int.TryParse(card.Group5, out num8);
        //                if (this.AloneSDK.IsTFTMachine(this.dev.MachineNumber))
        //                {
        //                    flag = this.AloneSDK.SSR_SetUnLockGroup(this.dev.MachineNumber, num3, num4, num5, num6, num7, num8);
        //                    if (flag)
        //                    {
        //                        num9++;
        //                        num10++;
        //                    }
        //                }
        //                else
        //                {
        //                    num9++;
        //                    num10++;
        //                    builder.AppendFormat("{0}", (num4 > 0) ? num4.ToString() : "");
        //                    builder.AppendFormat("{0}", (num5 > 0) ? num5.ToString() : "");
        //                    builder.AppendFormat("{0}", (num6 > 0) ? num6.ToString() : "");
        //                    builder.AppendFormat("{0}", (num7 > 0) ? num7.ToString() : "");
        //                    builder.AppendFormat("{0}", (num8 > 0) ? num8.ToString() : "");
        //                    builder.Append(":");
        //                }
        //                if (!flag)
        //                {
        //                    this.AloneSDK.GetLastError(ref operationSuccess);
        //                    break;
        //                }
        //            }
        //            if (!this.AloneSDK.IsTFTMachine(this.dev.MachineNumber))
        //            {
        //                for (int j = num9; j < 9; j++)
        //                {
        //                    builder.Append(":");
        //                }
        //                if (!this.AloneSDK.SetUnlockGroups(this.dev.MachineNumber, builder.ToString()))
        //                {
        //                    num9 = 0;
        //                    flag = false;
        //                    this.AloneSDK.GetLastError(ref operationSuccess);
        //                }
        //            }
        //        }
        //        if (operationSuccess == this.OperationSuccess)
        //        {
        //            operationSuccess = this.EndCommunicationWithoutLock(false, false);
        //        }
        //        else if (operationSuccess != this.ErrOperationTimeOut)
        //        {
        //            this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
        //        }
        //    }
        //    return operationSuccess;
        //}

        //public int SetUserBioTemplate(List<BioTemplate> lstBioTemplate, BioTemplateType bioTemplateType)
        //{
        //    int operationSuccess = this.OperationSuccess;
        //    int num = 0;
        //    if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
        //    {
        //        Exception exception;
        //        try
        //        {
        //            switch (this.dev.ConnectType)
        //            {
        //                case 0:
        //                    {
        //                        ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
        //                        if (!Monitor.TryEnter(comToken, this.LockTimeOut))
        //                        {
        //                            break;
        //                        }
        //                        try
        //                        {
        //                            num = this.SetUserBioTemplateWithoutLock(lstBioTemplate, bioTemplateType);
        //                        }
        //                        catch (Exception exception1)
        //                        {
        //                            exception = exception1;
        //                            operationSuccess = this.ErrUnknown;
        //                            //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetUserBioTemplateCOM  BioTemplateType=" + bioTemplateType);
        //                            //SysLogServer.WriteLog(" Exception Infomation : " + exception.Message);
        //                        }
        //                        Monitor.Exit(comToken);
        //                        goto Label_00F7;
        //                    }
        //                case 1:
        //                    num = this.SetUserBioTemplateWithoutLock(lstBioTemplate, bioTemplateType);
        //                    goto Label_00F7;

        //                case 2:
        //                    num = this.SetUserBioTemplateWithoutLock(lstBioTemplate, bioTemplateType);
        //                    goto Label_00F7;

        //                default:
        //                    goto Label_00F7;
        //            }
        //            operationSuccess = this.ErrOperationTimeOut - 0x186a0;
        //            Label_00F7:
        //            if (num < 0)
        //            {
        //                operationSuccess = num + 0x186a0;
        //            }
        //            else
        //            {
        //                operationSuccess = this.OperationSuccess;
        //            }
        //        }
        //        catch (Exception exception2)
        //        {
        //            exception = exception2;
        //            operationSuccess = this.ErrUnknown;
        //            //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetUserBioTemplate  BioTemplateType=" + bioTemplateType);
        //            //SysLogServer.WriteLog(" Exception Infomation : " + exception.Message);
        //        }
        //        Monitor.Exit(this.ThreadLock);
        //    }
        //    else
        //    {
        //        operationSuccess = this.ErrOperationTimeOut;
        //    }
        //    return ((operationSuccess == this.OperationSuccess) ? num : (operationSuccess - 0x186a0));
        //}

        protected int SetUserBioTemplateWithoutLock(List<BioTemplate> lstBioTemplate, BioTemplateType bioTemplateType)
        {
            int operationSuccess = this.OperationSuccess;
            bool flag = false;
            int count = lstBioTemplate.Count;
            if (((lstBioTemplate == null) || (lstBioTemplate.Count <= 0)) && (this.dev.BiometricType[(int)bioTemplateType] == '0'))
            {
                return 0;
            }
            operationSuccess = this.InitialCommunicationWithoutLock((this.dev.ConnectType == 0) ? (count * 2) : (count + 2), true);
            if (operationSuccess != this.OperationSuccess)
            {
                return (operationSuccess - 0x186a0);
            }
            flag = this.AloneSDK.SSR_SetDeviceData(this.dev.MachineNumber, Tables.pers_biotemplate.ToString(), this.ConvertLstBioTemplate2StringBuff(lstBioTemplate), "");
            if (!flag)
            {
                this.AloneSDK.GetLastError(ref operationSuccess);
            }
            this.EndCommunicationWithoutLock(true, true);
            this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
            return (flag ? count : (operationSuccess - 0x186a0));
        }

        protected int SetUserBioTemplateWithoutLock(string data, out string persList)
        {
            int operationSuccess = this.OperationSuccess;
            int length = 0;
            bool flag = false;
            persList = string.Empty;
            if (!string.IsNullOrWhiteSpace(data))
            {
                string[] strArray = data.Split("\r\n#".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if ((strArray != null) && (strArray.Length > 0))
                {
                    length = strArray.Length;
                    operationSuccess = this.InitialCommunicationWithoutLock((this.dev.ConnectType == 0) ? (length * 2) : (length + 2), true);
                    if (operationSuccess != this.OperationSuccess)
                    {
                        return (operationSuccess - 0x186a0);
                    }
                    flag = this.AloneSDK.SSR_SetDeviceData(this.dev.MachineNumber, Tables.pers_biotemplate.ToString(), data, "");
                    if (flag)
                    {
                        this.EndCommunicationWithoutLock(true, true);
                        this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                    }
                    else
                    {
                        int result = -1;
                        Dictionary<string, int> dictionary = new Dictionary<string, int>();
                        foreach (string str in strArray)
                        {
                            string[] strArray2 = str.Split("\t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            if (((strArray2 != null) && (strArray2.Length > 0)) && (strArray2.Length == 10))
                            {
                                if (!int.TryParse(strArray2[3].Split(new char[] { '=' })[1], out result))
                                {
                                    result = -1;
                                }
                                if ((result == -1) || (this.dev.BiometricType[result] == '0'))
                                {
                                    if (!dictionary.ContainsKey(strArray2[0]))
                                    {
                                        dictionary.Add(strArray2[0], 0);
                                    }
                                    persList = persList + strArray2[0] + ",";
                                }
                                else if (!(this.AloneSDK.SSR_SetDeviceData(this.dev.MachineNumber, Tables.pers_biotemplate.ToString(), str + "\r\n", "") || dictionary.ContainsKey(strArray2[0])))
                                {
                                    dictionary.Add(strArray2[0], 1);
                                    persList = persList + strArray2[0] + ",";
                                }
                            }
                        }
                        length = 0;
                        if (!string.IsNullOrWhiteSpace(persList))
                        {
                            persList = persList.EndsWith(",") ? persList.Remove(persList.Length - 1, 1) : persList;
                            length = persList.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length;
                        }
                        flag = true;
                        this.EndCommunicationWithoutLock(true, true);
                        this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
                        length = strArray.Length - length;
                    }
                    return (flag ? length : (operationSuccess - 0x186a0));
                }
            }
            return 0;
        }

        //public int SetUserFaceTemplate(List<ObjFaceTemp> lstTemplate)
        //{
        //    int operationSuccess = this.OperationSuccess;
        //    int num = 0;
        //    if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
        //    {
        //        Exception exception;
        //        try
        //        {
        //            switch (this.dev.ConnectType)
        //            {
        //                case 0:
        //                    {
        //                        ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
        //                        if (!Monitor.TryEnter(comToken, this.LockTimeOut))
        //                        {
        //                            break;
        //                        }
        //                        try
        //                        {
        //                            num = this.SetUserFaceTemplateWithoutLock(lstTemplate);
        //                        }
        //                        catch (Exception exception1)
        //                        {
        //                            exception = exception1;
        //                            operationSuccess = this.ErrUnknown;
        //                            //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetUserFaceTemplateCOM " + exception.Message);
        //                        }
        //                        Monitor.Exit(comToken);
        //                        goto Label_00DE;
        //                    }
        //                case 1:
        //                    num = this.SetUserFaceTemplateWithoutLock(lstTemplate);
        //                    goto Label_00DE;

        //                case 2:
        //                    num = this.SetUserFaceTemplateWithoutLock(lstTemplate);
        //                    goto Label_00DE;

        //                default:
        //                    goto Label_00DE;
        //            }
        //            operationSuccess = this.ErrOperationTimeOut - 0x186a0;
        //            Label_00DE:
        //            if (num < 0)
        //            {
        //                operationSuccess = num + 0x186a0;
        //            }
        //            else
        //            {
        //                operationSuccess = this.OperationSuccess;
        //            }
        //        }
        //        catch (Exception exception2)
        //        {
        //            exception = exception2;
        //            operationSuccess = this.ErrUnknown;
        //            //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetUserFaceTemplate " + exception.Message);
        //        }
        //        Monitor.Exit(this.ThreadLock);
        //    }
        //    else
        //    {
        //        operationSuccess = this.ErrOperationTimeOut;
        //    }
        //    return ((operationSuccess == this.OperationSuccess) ? num : (operationSuccess - 0x186a0));
        //}

        //protected int SetUserFaceTemplateWithoutLock(List<ObjFaceTemp> lstTemplate)
        //{
        //    int operationSuccess = this.OperationSuccess;
        //    int count = lstTemplate.Count;
        //    bool flag = true;
        //    int num3 = 0;
        //    flag = true;
        //    bool flag2 = this.AloneSDK.IsTFTMachine(this.dev.MachineNumber);
        //    if (this.dev.FaceFunOn != 1)
        //    {
        //        return 0;
        //    }
        //    operationSuccess = this.InitialCommunicationWithoutLock((this.dev.ConnectType == 0) ? (count * 2) : (count + 2), false);
        //    if (operationSuccess != this.OperationSuccess)
        //    {
        //        return (operationSuccess - 0x186a0);
        //    }
        //    if ((lstTemplate != null) && (lstTemplate.Count > 0))
        //    {
        //        for (int i = 0; i < lstTemplate.Count; i++)
        //        {
        //            ObjFaceTemp temp = lstTemplate[i];
        //            if (this.AloneSDK.SetUserFaceStr(this.dev.MachineNumber, temp.Pin, temp.FaceID, temp.Face, temp.Size))
        //            {
        //                num3++;
        //            }
        //            else
        //            {
        //                for (int j = 0; j < 3; j++)
        //                {
        //                    if (this.AloneSDK.SetUserFaceStr(this.dev.MachineNumber, temp.Pin, temp.FaceID, temp.Face, temp.Size))
        //                    {
        //                        num3++;
        //                    }
        //                }
        //                this.AloneSDK.GetLastError(ref operationSuccess);
        //                //SysLogServer.WriteLogs(temp.Pin + "(" + this.dev.IP + ")failure:" + operationSuccess.ToString());
        //            }
        //        }
        //    }
        //    if (operationSuccess == this.OperationSuccess)
        //    {
        //        operationSuccess = this.EndCommunicationWithoutLock(false, true);
        //    }
        //    else if (operationSuccess != this.ErrOperationTimeOut)
        //    {
        //        this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
        //    }
        //    return ((operationSuccess == this.OperationSuccess) ? num3 : (operationSuccess - 0x186a0));
        //}

        public int SetUserFPTemplate(List<Template> lstTemplate)
        {
            int operationSuccess = this.OperationSuccess;
            int num = 0;
            if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                Exception exception;
                try
                {
                    switch (this.dev.ConnectType)
                    {
                        case 0:
                            {
                                ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                                if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                                {
                                    break;
                                }
                                try
                                {
                                    num = this.SetUserFPTemplateWithoutLock(lstTemplate);
                                }
                                catch (Exception exception1)
                                {
                                    exception = exception1;
                                    operationSuccess = this.ErrUnknown;
                                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetUserFPTemplateCOM " + exception.Message);
                                }
                                Monitor.Exit(comToken);
                                goto Label_00DE;
                            }
                        case 1:
                            num = this.SetUserFPTemplateWithoutLock(lstTemplate);
                            goto Label_00DE;

                        case 2:
                            num = this.SetUserFPTemplateWithoutLock(lstTemplate);
                            goto Label_00DE;

                        default:
                            goto Label_00DE;
                    }
                    operationSuccess = this.ErrOperationTimeOut - 0x186a0;
                    Label_00DE:
                    if (num < 0)
                    {
                        operationSuccess = num + 0x186a0;
                    }
                    else
                    {
                        operationSuccess = this.OperationSuccess;
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetUserFPTemplate " + exception.Message);
                }
                Monitor.Exit(this.ThreadLock);
            }
            else
            {
                operationSuccess = this.ErrOperationTimeOut;
            }
            return ((operationSuccess == this.OperationSuccess) ? num : (operationSuccess - 0x186a0));
        }

        protected int SetUserFPTemplateWithoutLock(List<Template> lstTemplate)
        {
            int operationSuccess = this.OperationSuccess;
            int count = lstTemplate.Count;
            bool flag = true;
            int num3 = 0;
            flag = true;
            bool flag2 = this.AloneSDK.IsTFTMachine(this.dev.MachineNumber);
            if (this.dev.CompatOldFirmware == "0")
            {
                if ((this.dev.FingerFunOn == null) || (this.dev.FingerFunOn.Trim() != "1"))
                {
                    return 0;
                }
            }
            else if (this.dev.CardFun == 1)
            {
                return 0;
            }
            operationSuccess = this.InitialCommunicationWithoutLock((this.dev.ConnectType == 0) ? (count * 2) : (count + 2), true);
            if (operationSuccess != this.OperationSuccess)
            {
                return (operationSuccess - 0x186a0);
            }
            if ((lstTemplate != null) && (lstTemplate.Count > 0))
            {
                for (int i = 0; i < lstTemplate.Count; i++)
                {
                    if (!string.IsNullOrEmpty(lstTemplate[i].Pin))
                    {
                        Template template = lstTemplate[i];
                        int num5 = template.Flag;
                        int dwFingerIndex = template.FINGERID | ((num5 == 3) ? 0x10 : 0);
                        flag = false;
                        if (flag2)
                        {
                            if (this.dev.FpVersion < 10)
                            {
                                if ((template.TEMPLATE3 != null) && (template.TEMPLATE3.Length > 0))
                                {
                                    if (this.FirmwareVersion >= 6.6M)
                                    {
                                        flag = this.AloneSDK.SetUserTmpEx(this.dev.MachineNumber, template.Pin, template.FINGERID, num5, ref template.TEMPLATE3[0]);
                                    }
                                    else
                                    {
                                        flag = this.AloneSDK.SSR_SetUserTmp(this.dev.MachineNumber, template.Pin, dwFingerIndex, ref template.TEMPLATE3[0]);
                                    }
                                }
                            }
                            else if ((template.TEMPLATE4 != null) && (template.TEMPLATE4.Length > 0))
                            {
                                flag = this.AloneSDK.SetUserTmpEx(this.dev.MachineNumber, template.Pin, template.FINGERID, num5, ref template.TEMPLATE4[0]);
                            }
                        }
                        else if (this.dev.FpVersion < 10)
                        {
                            if ((template.TEMPLATE3 != null) && (template.TEMPLATE3.Length > 0))
                            {
                                if (this.FirmwareVersion >= 6.6M)
                                {
                                    flag = this.AloneSDK.SetUserTmpEx(this.dev.MachineNumber, template.Pin, template.FINGERID, num5, ref template.TEMPLATE3[0]);
                                }
                                else
                                {
                                    flag = this.AloneSDK.SetUserTmp(this.dev.MachineNumber, int.Parse(template.Pin), dwFingerIndex, ref template.TEMPLATE3[0]);
                                }
                            }
                        }
                        else if ((template.TEMPLATE4 != null) && (template.TEMPLATE4.Length > 0))
                        {
                            flag = this.AloneSDK.SetUserTmpEx(this.dev.MachineNumber, template.Pin, template.FINGERID, num5, ref template.TEMPLATE4[0]);
                        }
                        num3 += flag ? 1 : 0;
                        if (!flag)
                        {
                            this.AloneSDK.GetLastError(ref operationSuccess);
                            operationSuccess = this.ReturnMemoryNotEnough(operationSuccess);
                            if (this.OperationSuccess == operationSuccess)
                            {
                                flag = true;
                            }
                            break;
                        }
                    }
                }
            }
            if (operationSuccess == this.OperationSuccess)
            {
                operationSuccess = this.EndCommunicationWithoutLock(true, true);
            }
            else if (operationSuccess != this.ErrOperationTimeOut)
            {
                this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
            }
            return ((operationSuccess == this.OperationSuccess) ? num3 : (operationSuccess - 0x186a0));
        }

        //public int SetUserGroup(List<UserInfo> lstUser, Dictionary<int, int> dicMCGroupId_GroupId)
        //{
        //    Exception exception;
        //    int operationSuccess = this.OperationSuccess;
        //    if (!Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
        //    {
        //        operationSuccess = this.ErrOperationTimeOut;
        //        goto Label_0122;
        //    }
        //    try
        //    {
        //        switch (this.dev.ConnectType)
        //        {
        //            case 0:
        //                {
        //                    ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
        //                    if (!Monitor.TryEnter(comToken, this.LockTimeOut))
        //                    {
        //                        break;
        //                    }
        //                    try
        //                    {
        //                        operationSuccess = this.SetUserGroupWithoutLock(lstUser, dicMCGroupId_GroupId);
        //                    }
        //                    catch (Exception exception1)
        //                    {
        //                        exception = exception1;
        //                        operationSuccess = this.ErrUnknown;
        //                        //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetUserGroupCOM " + exception.Message);
        //                    }
        //                    Monitor.Exit(comToken);
        //                    goto Label_0109;
        //                }
        //            case 1:
        //                operationSuccess = this.SetUserGroupWithoutLock(lstUser, dicMCGroupId_GroupId);
        //                goto Label_0109;

        //            case 2:
        //                operationSuccess = this.SetUserGroupWithoutLock(lstUser, dicMCGroupId_GroupId);
        //                goto Label_0109;

        //            default:
        //                goto Label_0109;
        //        }
        //        operationSuccess = this.ErrOperationTimeOut;
        //    }
        //    catch (Exception exception2)
        //    {
        //        exception = exception2;
        //        operationSuccess = this.ErrUnknown;
        //        //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetUserGroup " + exception.Message);
        //    }
        //    Label_0109:
        //    Monitor.Exit(this.ThreadLock);
        //    Label_0122:
        //    return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
        //}

        //protected int SetUserGroupWithoutLock(List<UserInfo> lstUser, Dictionary<int, int> dicMCGroupId_GroupId)
        //{
        //    int operationSuccess = this.OperationSuccess;
        //    int count = lstUser.Count;
        //    int num4 = 0;
        //    if (null == dicMCGroupId_GroupId)
        //    {
        //        dicMCGroupId_GroupId = new Dictionary<int, int>();
        //    }
        //    operationSuccess = this.InitialCommunicationWithoutLock((this.dev.ConnectType == 0) ? (count * 2) : (count + 2), false);
        //    if (operationSuccess == this.OperationSuccess)
        //    {
        //        if ((lstUser != null) && (lstUser.Count > 0))
        //        {
        //            for (int i = 0; i < lstUser.Count; i++)
        //            {
        //                UserInfo info = lstUser[i];
        //                int morecardGroupId = info.MorecardGroupId;
        //                if (dicMCGroupId_GroupId.ContainsKey(morecardGroupId))
        //                {
        //                    morecardGroupId = dicMCGroupId_GroupId[morecardGroupId];
        //                }
        //                if ((morecardGroupId < 1) || (morecardGroupId > 0x63))
        //                {
        //                    morecardGroupId = 1;
        //                }
        //                if (!this.AloneSDK.SetUserGroup(this.dev.MachineNumber, int.Parse(info.BadgeNumber), morecardGroupId))
        //                {
        //                    this.AloneSDK.GetLastError(ref operationSuccess);
        //                    break;
        //                }
        //                num4++;
        //            }
        //        }
        //        if (operationSuccess == this.OperationSuccess)
        //        {
        //            operationSuccess = this.EndCommunicationWithoutLock(false, false);
        //        }
        //        else if (operationSuccess != this.ErrOperationTimeOut)
        //        {
        //            this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
        //        }
        //    }
        //    return operationSuccess;
        //}

        public int SetUserInfo(List<ObjUser> lstUser, Dictionary<int, int> dicPullGroupId_StdGroupId)
        {
            int operationSuccess = this.OperationSuccess;
            int num = 0;
            if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                Exception exception;
                try
                {
                    switch (this.dev.ConnectType)
                    {
                        case 0:
                            {
                                ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                                if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                                {
                                    break;
                                }
                                try
                                {
                                    num = this.SetUserInfoWithoutLock(lstUser, dicPullGroupId_StdGroupId);
                                }
                                catch (Exception exception1)
                                {
                                    exception = exception1;
                                    operationSuccess = this.ErrUnknown;
                                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetUserInfoCOM " + exception.Message);
                                }
                                Monitor.Exit(comToken);
                                goto Label_00E1;
                            }
                        case 1:
                            num = this.SetUserInfoWithoutLock(lstUser, dicPullGroupId_StdGroupId);
                            goto Label_00E1;

                        case 2:
                            num = this.SetUserInfoWithoutLock(lstUser, dicPullGroupId_StdGroupId);
                            goto Label_00E1;

                        default:
                            goto Label_00E1;
                    }
                    operationSuccess = this.ErrOperationTimeOut - 0x186a0;
                    Label_00E1:
                    if (num < 0)
                    {
                        operationSuccess = num + 0x186a0;
                    }
                    else
                    {
                        operationSuccess = this.OperationSuccess;
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetUserInfo " + exception.Message);
                }
                Monitor.Exit(this.ThreadLock);
            }
            else
            {
                operationSuccess = this.ErrOperationTimeOut;
            }
            return ((operationSuccess == this.OperationSuccess) ? num : (operationSuccess - 0x186a0));
        }

        public int SetUserInfoEx(List<UserVerifyType> lstUserVT)
        {
            int operationSuccess = this.OperationSuccess;
            int num = 0;
            if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                Exception exception;
                try
                {
                    switch (this.dev.ConnectType)
                    {
                        case 0:
                            {
                                ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                                if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                                {
                                    break;
                                }
                                try
                                {
                                    num = this.SetUserInfoExWithoutLock(lstUserVT);
                                }
                                catch (Exception exception1)
                                {
                                    exception = exception1;
                                    operationSuccess = this.ErrUnknown;
                                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetUserInfoExCOM " + exception.Message);
                                }
                                Monitor.Exit(comToken);
                                goto Label_00DE;
                            }
                        case 1:
                            num = this.SetUserInfoExWithoutLock(lstUserVT);
                            goto Label_00DE;

                        case 2:
                            num = this.SetUserInfoExWithoutLock(lstUserVT);
                            goto Label_00DE;

                        default:
                            goto Label_00DE;
                    }
                    operationSuccess = this.ErrOperationTimeOut - 0x186a0;
                    Label_00DE:
                    if (num < 0)
                    {
                        operationSuccess = num + 0x186a0;
                    }
                    else
                    {
                        operationSuccess = this.OperationSuccess;
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetUserInfoEx " + exception.Message);
                }
                Monitor.Exit(this.ThreadLock);
            }
            else
            {
                operationSuccess = this.ErrOperationTimeOut;
            }
            return ((operationSuccess == this.OperationSuccess) ? num : (operationSuccess - 0x186a0));
        }

        protected int SetUserInfoExWithoutLock(List<UserVerifyType> lstUserVT)
        {
            int operationSuccess = this.OperationSuccess;
            if (this.dev.UserExtFmt != 1)
            {
                return 0;
            }
            byte reserved = 0;
            int num2 = 0;
            int count = lstUserVT.Count;
            operationSuccess = this.InitialCommunicationWithoutLock((this.dev.ConnectType == 0) ? (count * 2) : (count + 2), false);
            if (operationSuccess != this.OperationSuccess)
            {
                return (operationSuccess - 0x186a0);
            }
            if ((lstUserVT != null) && (lstUserVT.Count > 0))
            {
                for (int i = 0; i < lstUserVT.Count; i++)
                {
                    bool flag;
                    UserVerifyType type = lstUserVT[i];
                    if (type.UseGroupVT)
                    {
                        flag = this.AloneSDK.SetUserInfoEx(this.dev.MachineNumber, type.Pin, 0, ref reserved);
                    }
                    else
                    {
                        int verifyStyle = 0x80 | type.VerifyType;
                        flag = this.AloneSDK.SetUserInfoEx(this.dev.MachineNumber, type.Pin, verifyStyle, ref reserved);
                    }
                    if (flag)
                    {
                        num2++;
                    }
                    else
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                        operationSuccess = this.ReturnMemoryNotEnough(operationSuccess);
                        if (this.OperationSuccess == operationSuccess)
                        {
                            flag = true;
                        }
                        break;
                    }
                }
            }
            if (operationSuccess == this.OperationSuccess)
            {
                operationSuccess = this.EndCommunicationWithoutLock(false, true);
            }
            else if (operationSuccess != this.ErrOperationTimeOut)
            {
                this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
            }
            return ((operationSuccess == this.OperationSuccess) ? num2 : (operationSuccess - 0x186a0));
        }

        protected int SetUserInfoWithoutLock(List<ObjUser> lstUser, Dictionary<int, int> dicPullGroupId_StdGroupId)
        {
            ObjUser user;
            int num7;
            int operationSuccess = this.OperationSuccess;
            int num3 = 0;
            int count = lstUser.Count;
            if (count <= 0)
            {
                return ((operationSuccess == this.OperationSuccess) ? 0 : (operationSuccess - 0x186a0));
            }
            operationSuccess = this.InitialCommunicationWithoutLock((this.dev.ConnectType == 0) ? (count * 2) : (count + 2), true);
            if (operationSuccess != this.OperationSuccess)
            {
                return (operationSuccess - 0x186a0);
            }
            bool flag2 = false;
            bool flag = this.AloneSDK.IsTFTMachine(this.dev.MachineNumber);
            for (num7 = 0; num7 < lstUser.Count; num7++)
            {
                int num2;
                int num6;
                user = lstUser[num7];
                int num = int.Parse(user.Id);
                this.AloneSDK.SetStrCardNumber(user.CardNo);
                int.TryParse(user.Group, out num2);
                if ((num2 > 0) && dicPullGroupId_StdGroupId.ContainsKey(num2))
                {
                    this.AloneSDK.AccGroup = dicPullGroupId_StdGroupId[num2];
                }
                else
                {
                    this.AloneSDK.AccGroup = 1;
                }
                switch (user.Privilege)
                {
                    case 0:
                        num6 = 0;
                        break;

                    case 3:
                    case 14:
                        num6 = 3;
                        break;

                    default:
                        num6 = 0;
                        break;
                }
                if ((user.Privilege == 4) && (this.dev.CompatOldFirmware == "0"))
                {
                    num6 = 8;
                }
                if (flag)
                {
                    flag2 = this.AloneSDK.SSR_SetUserInfo(this.dev.MachineNumber, user.Pin, user.Name, user.Password, num6, true);
                }
                else
                {
                    flag2 = this.AloneSDK.SetUserInfo(this.dev.MachineNumber, int.Parse(user.Pin), user.Name, user.Password, num6, true);
                }
                if (flag2)
                {
                    num3++;
                }
                else
                {
                    this.AloneSDK.GetLastError(ref operationSuccess);
                    break;
                }
            }
            if (operationSuccess == this.OperationSuccess)
            {
                operationSuccess = this.EndCommunicationWithoutLock(true, true);
            }
            else if (operationSuccess != this.ErrOperationTimeOut)
            {
                this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
            }
            operationSuccess = this.InitialCommunicationWithoutLock((this.dev.ConnectType == 0) ? (count * 2) : (count + 2), true);
            if (operationSuccess != this.OperationSuccess)
            {
                return (operationSuccess - 0x186a0);
            }
            bool flag3 = false;
            for (num7 = 0; num7 < lstUser.Count; num7++)
            {
                user = lstUser[num7];
                if (!string.IsNullOrWhiteSpace(user.StartTime))
                {
                    flag3 = this.AloneSDK.SetUserValidDate(this.dev.MachineNumber, user.Pin, 1, 1, user.StartTime, user.EndTime);
                }
            }
            if (operationSuccess == this.OperationSuccess)
            {
                operationSuccess = this.EndCommunicationWithoutLock(true, true);
            }
            else if (operationSuccess != this.ErrOperationTimeOut)
            {
                this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
            }
            return ((operationSuccess == this.OperationSuccess) ? num3 : (operationSuccess - 0x186a0));
        }

        //public int SetUserTimeZone(List<ObjUserAuthorize> lstUserAuth)
        //{
        //    Exception exception;
        //    int operationSuccess = this.OperationSuccess;
        //    if (!Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
        //    {
        //        return (this.ErrOperationTimeOut - 0x186a0);
        //    }
        //    try
        //    {
        //        switch (this.dev.ConnectType)
        //        {
        //            case 0:
        //                {
        //                    ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
        //                    if (!Monitor.TryEnter(comToken, this.LockTimeOut))
        //                    {
        //                        break;
        //                    }
        //                    try
        //                    {
        //                        operationSuccess = this.SetUserTimeZoneWithoutLock(lstUserAuth);
        //                    }
        //                    catch (Exception exception1)
        //                    {
        //                        exception = exception1;
        //                        operationSuccess = this.ErrUnknown - 0x186a0;
        //                        //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetUserTimeZoneCOM " + exception.Message);
        //                    }
        //                    Monitor.Exit(comToken);
        //                    goto Label_0118;
        //                }
        //            case 1:
        //                operationSuccess = this.SetUserTimeZoneWithoutLock(lstUserAuth);
        //                goto Label_0118;

        //            case 2:
        //                operationSuccess = this.SetUserTimeZoneWithoutLock(lstUserAuth);
        //                goto Label_0118;

        //            default:
        //                goto Label_0118;
        //        }
        //        operationSuccess = this.ErrOperationTimeOut - 0x186a0;
        //    }
        //    catch (Exception exception2)
        //    {
        //        exception = exception2;
        //        operationSuccess = this.ErrUnknown - 0x186a0;
        //        //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetUserTimeZone " + exception.Message);
        //    }
        //    Label_0118:
        //    Monitor.Exit(this.ThreadLock);
        //    return operationSuccess;
        //}

        //protected int SetUserTimeZoneWithoutLock(List<ObjUserAuthorize> lstUserAuth)
        //{
        //    int operationSuccess = this.OperationSuccess;
        //    string tZs = "";
        //    bool flag = true;
        //    int count = lstUserAuth.Count;
        //    int num3 = lstUserAuth.Count;
        //    operationSuccess = this.InitialCommunicationWithoutLock((this.dev.ConnectType == 0) ? (count * 2) : (count + 2), false);
        //    if (operationSuccess != this.OperationSuccess)
        //    {
        //        return (operationSuccess - 0x186a0);
        //    }
        //    if ((lstUserAuth != null) && (lstUserAuth.Count > 0))
        //    {
        //        for (int i = 0; i < lstUserAuth.Count; i++)
        //        {
        //            ObjUserAuthorize authorize = lstUserAuth[i];
        //            tZs = string.Format("{0}:{1}:{2}", authorize.TimeZone1, authorize.TimeZone2, authorize.TimeZone3);
        //            if (((((authorize.TimeZone1 > 50) || (authorize.TimeZone2 > 50)) || ((authorize.TimeZone3 > 50) || (authorize.TimeZone1 < 0))) || (authorize.TimeZone2 < 0)) || (authorize.TimeZone3 < 0))
        //            {
        //                LogServer.Log("STD TZ Error\tPin=" + authorize.Pin + "\tTZ=" + tZs);
        //            }
        //            if (this.AloneSDK.IsTFTMachine(this.dev.MachineNumber))
        //            {
        //                flag = this.AloneSDK.SetUserTZStr(this.dev.MachineNumber, int.Parse(authorize.Pin), tZs + ":1");
        //            }
        //            else
        //            {
        //                flag = this.AloneSDK.SetUserTZStr(this.dev.MachineNumber, int.Parse(authorize.Pin), tZs);
        //            }
        //            if (!flag)
        //            {
        //                this.AloneSDK.GetLastError(ref operationSuccess);
        //                if (operationSuccess == -2001)
        //                {
        //                    //SysLogServer.WriteLogs(ShowMsgInfos.GetInfo("SetTimeSegmentNoSupported", "不支持设置时间段") + ":" + operationSuccess.ToString());
        //                    operationSuccess = this.OperationSuccess;
        //                }
        //                else if ((operationSuccess == -4999) || (operationSuccess == -104999))
        //                {
        //                    operationSuccess = this.OperationSuccess;
        //                }
        //                break;
        //            }
        //        }
        //    }
        //    if (operationSuccess == this.OperationSuccess)
        //    {
        //        operationSuccess = this.EndCommunicationWithoutLock(false, false);
        //    }
        //    else if (operationSuccess != this.ErrOperationTimeOut)
        //    {
        //        this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
        //    }
        //    return ((operationSuccess != this.OperationSuccess) ? (operationSuccess - 0x186a0) : num3);
        //}

        public int StartEnroll(string Pin, int FingerId, int Flag = 1)
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
                    int num2;
                    int.TryParse(Pin, out num2);
                    if (this.dev.ConnectType == 0)
                    {
                        ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                        if (Monitor.TryEnter(comToken, this.LockTimeOut))
                        {
                            try
                            {
                                this.AloneSDK.CancelOperation();
                                if (!this.AloneSDK.StartEnrollEx(Pin, FingerId, Flag))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on StartEnroll_Com " + exception.Message);
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
                        this.AloneSDK.CancelOperation();
                        if (!this.AloneSDK.StartEnrollEx(Pin, FingerId, Flag))
                        {
                            this.AloneSDK.GetLastError(ref operationSuccess);
                        }
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on StartEnroll " + exception.Message);
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

        public int StartIdentify()
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
                                if (!this.AloneSDK.StartIdentify())
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on StartIdentify_Com " + exception.Message);
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
                    else if (!this.AloneSDK.StartIdentify())
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on StartIdentify " + exception.Message);
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
                                if (!this.AloneSDK.ACUnlock(this.dev.MachineNumber, DelayTime))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on Unlock_Com " + exception.Message);
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
                    else if (!this.AloneSDK.ACUnlock(this.dev.MachineNumber, DelayTime))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on Unlock " + exception.Message);
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

        public int UpdateFirmware(string FileName)
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
                                if (!this.AloneSDK.UpdateFirmware(FileName))
                                {
                                    this.AloneSDK.GetLastError(ref operationSuccess);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                operationSuccess = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on UpdateFirmware_Com " + exception.Message);
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
                    else if (!this.AloneSDK.UpdateFirmware(FileName))
                    {
                        this.AloneSDK.GetLastError(ref operationSuccess);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    operationSuccess = this.ErrUnknown;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on UpdateFirmware " + exception.Message);
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

        //public int UploadUserPhoto(List<UserInfo> lstUser)
        //{
        //    Exception exception;
        //    int operationSuccess = this.OperationSuccess;
        //    if (!Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
        //    {
        //        return (this.ErrOperationTimeOut - 0x186a0);
        //    }
        //    try
        //    {
        //        switch (this.dev.ConnectType)
        //        {
        //            case 0:
        //                {
        //                    ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
        //                    if (!Monitor.TryEnter(comToken, this.LockTimeOut))
        //                    {
        //                        break;
        //                    }
        //                    try
        //                    {
        //                        operationSuccess = this.UploadUserPhotoWithoutLock(lstUser);
        //                    }
        //                    catch (Exception exception1)
        //                    {
        //                        exception = exception1;
        //                        operationSuccess = this.ErrUnknown - 0x186a0;
        //                        //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on UploadUserPhotoCOM " + exception.Message);
        //                    }
        //                    Monitor.Exit(comToken);
        //                    goto Label_0118;
        //                }
        //            case 1:
        //                operationSuccess = this.UploadUserPhotoWithoutLock(lstUser);
        //                goto Label_0118;

        //            case 2:
        //                operationSuccess = this.UploadUserPhotoWithoutLock(lstUser);
        //                goto Label_0118;

        //            default:
        //                goto Label_0118;
        //        }
        //        operationSuccess = this.ErrOperationTimeOut - 0x186a0;
        //    }
        //    catch (Exception exception2)
        //    {
        //        exception = exception2;
        //        operationSuccess = this.ErrUnknown - 0x186a0;
        //        //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on UploadUserPhoto " + exception.Message);
        //    }
        //    Label_0118:
        //    Monitor.Exit(this.ThreadLock);
        //    return operationSuccess;
        //}

        //protected int UploadUserPhotoWithoutLock(List<UserInfo> lstUser)
        //{
        //    int operationSuccess = this.OperationSuccess;
        //    int num3 = 0;
        //    bool flag = true;
        //    int count = lstUser.Count;
        //    string path = Application.StartupPath + @"\TmpPhoto";
        //    operationSuccess = this.InitialCommunicationWithoutLock((this.dev.ConnectType == 0) ? (count * 2) : (count + 2), false);
        //    if (operationSuccess != this.OperationSuccess)
        //    {
        //        return (operationSuccess - 0x186a0);
        //    }
        //    if ((lstUser != null) && (lstUser.Count > 0))
        //    {
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }
        //        for (int i = 0; i < lstUser.Count; i++)
        //        {
        //            UserInfo info = lstUser[i];
        //            if (null != info.Photo)
        //            {
        //                flag = false;
        //                string str2 = path + @"\" + info.BadgeNumber + ".jpg";
        //                Bitmap bitmap = new Bitmap(new MemoryStream(info.Photo));
        //                if (File.Exists(str2))
        //                {
        //                    File.Delete(str2);
        //                }
        //                bitmap.Save(str2, ImageFormat.Jpeg);
        //                if (!this.AloneSDK.SendFile(this.dev.MachineNumber, str2))
        //                {
        //                    this.AloneSDK.GetLastError(ref operationSuccess);
        //                    break;
        //                }
        //                num3++;
        //            }
        //        }
        //    }
        //    if (operationSuccess == this.OperationSuccess)
        //    {
        //        operationSuccess = this.EndCommunicationWithoutLock(false, false);
        //    }
        //    else if (operationSuccess != this.ErrOperationTimeOut)
        //    {
        //        this.AloneSDK.EnableDevice(this.dev.MachineNumber, true);
        //    }
        //    return ((operationSuccess == this.OperationSuccess) ? num3 : (operationSuccess - 0x186a0));
        //}

        // Properties
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

        public bool EventRegistered
        {
            get
            {
                return this.bolEventRegistered;
            }
        }

        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }
        }

        public int LockFunOn
        {
            get
            {
                return this._LockFunOn;
            }
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
                        //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on PinWidth. " + exception.Message);
                    }
                    finally
                    {
                        Monitor.Exit(this.ThreadLock);
                    }
                }
                return pINWidth;
            }
        }

        // Nested Types
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

        public enum RegRTEventCode
        {
            AllEvent = 0xffff,
            OnAttTransaction_OnAttTransactionEx = 1,
            OnDeleteTemplate = 0x4000,
            OnDoor_OnAlarm = 0x400,
            OnEmptyCard = 0x2000,
            OnEnrollFinger = 8,
            OnFinger = 2,
            OnFingerFeature = 0x200,
            OnHIDNum = 0x800,
            OnKeyPress = 0x10,
            OnNewUser = 4,
            OnVerify = 0x100,
            OnWriteCard = 0x1000
        }

        public delegate void VerifyEventHandler(int UserID);

        public delegate void WriteCardEventHandler(int EnrollNumber, int ActionResult, int Length);
    }

}
}
