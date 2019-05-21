using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKTeco.SDK.Model;

namespace ZKTeco.SDK.MachineManager
{
   public class DevServer:IDeviceServer
    {
        // Fields
        public DevServer(DevComm devComm)
        {
            this._devInstance = devComm;
        }
        private DevComm _devInstance = null;
        private int errorCode = -999;
        private bool m_isInitDevice = false;

        // Events
        public event STD_FingerFeature FingerFeature;

        public event STD_OnEnrollFinger OnEnrollFinger;

        public event DeviceRTEventHandler RTEvent;

        public event DeviceRTEventHandler SwippingCard;

        // Methods
        public int CancelAlarm(int DoorId = 0)
        {
            if (this._devInstance != null)
            {
                return this._devInstance.ControlDevice(2, DoorId, 0, 0, 0, "");
            }
            return this.errorCode;
        }

        public int ClearFvTemplate()
        {
            if (this._devInstance != null)
            {
                return this.DeleteDeviceData("fvtemplate", "", "");
            }
            return this.errorCode;
        }

        public int ClearUserAuthorize()
        {
            if (this._devInstance != null)
            {
                return this.DeleteDeviceData("UserAuthorize", "", "");
            }
            return this.errorCode;
        }

        public int CloseAuxiliary(int auxiliaryID)
        {
            if (this._devInstance != null)
            {
                return this._devInstance.ControlDevice(1, auxiliaryID, 2, 0, 0, "");
            }
            return this.errorCode;
        }

        public int CloseDoor(DoorType doorType)
        {
            if (this._devInstance != null)
            {
                return this._devInstance.ControlDevice(1, (int)doorType, 1, 0, 0, "");
            }
            return this.errorCode;
        }

        public int Connect(int timeout = 0xbb8)
        {
            if (this._devInstance != null)
            {
                return this._devInstance.Connect(timeout);
            }
            return this.errorCode;
        }

        public int ConnectExt(int TimeOut)
        {
            if (this._devInstance != null)
            {
                return this._devInstance.ConnectExt(TimeOut);
            }
            return this.errorCode;
        }

        public int ControlDevice(int operationID, int param1, int param2, int param3, int param4, string options)
        {
            if (this._devInstance != null)
            {
                return this._devInstance.ControlDevice(operationID, param1, param2, param3, param4, options);
            }
            return this.errorCode;
        }

        public int DeleteDeviceData(string tableName, string filter, string options)
        {
            if (this._devInstance != null)
            {
                return this._devInstance.DeleteDeviceData(tableName, filter, options);
            }
            return this.errorCode;
        }

        public int Disconnect()
        {
            if (this._devInstance != null)
            {
                this._devInstance.Disconnect();
                return 1;
            }
            return this.errorCode;
        }

        public int GetDeviceData(ref byte buffer, int bufferSize, string tableName, string fileName, string filter, string options)
        {
            if (this._devInstance != null)
            {
                return this._devInstance.GetDeviceData(ref buffer, bufferSize, tableName, fileName, filter, options);
            }
            return this.errorCode;
        }

        public int GetDeviceDataCount(string tableName, string filter, string options)
        {
            if (this._devInstance != null)
            {
                return this._devInstance.GetDeviceDataCount(tableName, filter, options);
            }
            return this.errorCode;
        }

        public int GetDeviceFileData(ref byte buffer, ref int bufferSize, string fileName, string options)
        {
            if (this._devInstance != null)
            {
                return this._devInstance.GetDeviceFileData(ref buffer, ref bufferSize, fileName, options);
            }
            return this.errorCode;
        }

        public int GetDeviceParam(ref byte buffer, int bufferSize, string itemValues)
        {
            if (this._devInstance != null)
            {
                return this._devInstance.GetDeviceParam(ref buffer, bufferSize, itemValues);
            }
            return this.errorCode;
        }

        public int GetOptionValue(string OptionNames, out string OptionValues)
        {
            int errorCode = this.errorCode;
            OptionValues = "";
            if (this._devInstance != null)
            {
                int bufferSize = 0x2800;
                byte[] buffer = new byte[bufferSize];
                errorCode = this._devInstance.GetDeviceParam(ref buffer[0], bufferSize, OptionNames);
                if (errorCode > 0)
                {
                    buffer = DataConvert.GetDataBuffer(buffer, false, "\r\n");
                    OptionValues = Encoding.Default.GetString(buffer);
                }
            }
            return errorCode;
        }

        public int GetRTLog(ref byte buffer, int bufferSize)
        {
            if (this._devInstance != null)
            {
                return this._devInstance.GetRTLog(ref buffer, bufferSize);
            }
            return this.errorCode;
        }

        public int GetRTLogExt(out string RTLogs)
        {
            RTLogs = "";
            if (this._devInstance != null)
            {
                return this._devInstance.GetRTLogExt(out RTLogs);
            }
            return this.errorCode;
        }

        public List<ObjRTLogInfo> GetRTLogs(ref int errorNo)
        {
            List<ObjRTLogInfo> list = new List<ObjRTLogInfo>();
            int rTLog = 0;
            int bufferSize = 0x4000;
            byte[] buffer = new byte[bufferSize];
            rTLog = this.GetRTLog(ref buffer[0], bufferSize);
            if (rTLog >= 0)
            {
                buffer = DataConvert.GetDataBuffer(buffer, false, "\r\n");
                string str = Encoding.UTF8.GetString(buffer);
                if (!string.IsNullOrEmpty(str))
                {
                    string[] strArray = str.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (strArray != null)
                    {
                        for (int i = 0; i < strArray.Length; i++)
                        {
                            string[] strArray2 = strArray[i].Split(new char[] { ',' });
                            if ((strArray2 != null) && (strArray2.Length >= 7))
                            {
                                ObjRTLogInfo item = new ObjRTLogInfo();
                                try
                                {
                                    item.Date = DateTime.Parse(strArray2[0]);
                                }
                                catch
                                {
                                    continue;
                                }
                                item.IP = this._devInstance.Dev.IP;
                                item.DevID = this._devInstance.Dev.ID;
                                if (strArray2[4] == "255")
                                {
                                    item.EType = EventType.Type255;
                                    item.VerifyType = strArray2[6];
                                    item.DoorStatus = strArray2[1];
                                    item.WarningStatus = strArray2[2];
                                    item.DoorID = strArray2[3];
                                    if (((item.DoorStatus != "0") && !string.IsNullOrEmpty(item.DoorStatus)) || ((item.WarningStatus != "0") && !string.IsNullOrEmpty(item.WarningStatus)))
                                    {
                                        long num4 = -1L;
                                        long num5 = -1L;
                                        try
                                        {
                                            num4 = long.Parse(item.WarningStatus);
                                        }
                                        catch
                                        {
                                        }
                                        try
                                        {
                                            num5 = long.Parse(item.DoorStatus);
                                        }
                                        catch
                                        {
                                        }
                                        try
                                        {
                                            long num6 = num5 & 0xffL;
                                            long num7 = num4 & 0xffL;
                                            ObjRTLogInfo info2 = new ObjRTLogInfo
                                            {
                                                EType = item.EType,
                                                VerifyType = item.VerifyType,
                                                WarningStatus = num7.ToString(),
                                                DoorStatus = num6.ToString(),
                                                DoorID = "1",
                                                Date = item.Date
                                            };
                                            list.Add(info2);
                                            num6 = (num5 >> 8) & 0xffL;
                                            num7 = (num4 >> 8) & 0xffL;
                                            info2 = new ObjRTLogInfo
                                            {
                                                EType = item.EType,
                                                VerifyType = item.VerifyType,
                                                WarningStatus = num7.ToString(),
                                                DoorStatus = num6.ToString(),
                                                DoorID = "2",
                                                Date = item.Date
                                            };
                                            list.Add(info2);
                                            num6 = (num5 >> 0x10) & 0xffL;
                                            num7 = (num4 >> 0x10) & 0xffL;
                                            info2 = new ObjRTLogInfo
                                            {
                                                EType = item.EType,
                                                VerifyType = item.VerifyType,
                                                WarningStatus = num7.ToString(),
                                                DoorStatus = num6.ToString(),
                                                DoorID = "3",
                                                Date = item.Date
                                            };
                                            list.Add(info2);
                                            num6 = (num5 >> 0x18) & 0xffL;
                                            num7 = (num4 >> 0x18) & 0xffL;
                                            info2 = new ObjRTLogInfo
                                            {
                                                EType = item.EType,
                                                VerifyType = item.VerifyType,
                                                WarningStatus = num7.ToString(),
                                                DoorStatus = num6.ToString(),
                                                DoorID = "4",
                                                Date = item.Date
                                            };
                                            list.Add(info2);
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    else
                                    {
                                        list.Add(item);
                                    }
                                }
                                else
                                {
                                    item.Pin = strArray2[1];
                                    item.CardNo = strArray2[2];
                                    item.DoorID = strArray2[3];
                                    item.EType = (EventType)Enum.Parse(typeof(EventType), strArray2[4]);
                                    item.InOutStatus = strArray2[5];
                                    item.VerifyType = strArray2[6];
                                    list.Add(item);
                                }
                            }
                        }
                    }
                }
            }
            buffer = null;
            errorNo = rTLog;
            return list;
        }

        public string GetSDKVersion()
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\plcommpro.dll");
            return ((versionInfo == null) ? "" : versionInfo.FileVersion.Replace(",", ".").Replace(" ", ""));
        }

        public bool InitDevice(Machines dev)
        {
            this.m_isInitDevice = false;
            if (dev != null)
            {
                if (this._devInstance != null)
                {
                    this._devInstance.Disconnect();
                }
                this._devInstance = new DevComm(dev);
                this.m_isInitDevice = true;
                return true;
            }
            return false;
        }

        public bool IsMonitoringSwippingCard()
        {
            return (null != this.SwippingCard);
        }

        public int ModifyIPAddress(string commType, string address, string buffer)
        {
            if (this._devInstance == null)
            {
                this._devInstance = new DevComm(new Machines());
                int num = this._devInstance.ModifyIPAddress(commType, address, buffer);
                this._devInstance = null;
                return num;
            }
            return this._devInstance.ModifyIPAddress(commType, address, buffer);
        }

        public int NormalOpenDoor(DoorType doorType, bool state)
        {
            if (this._devInstance != null)
            {
                if (state)
                {
                    return this._devInstance.ControlDevice(4, (int)doorType, 1, 0, 0, "");
                }
                return this._devInstance.ControlDevice(4, (int)doorType, 0, 0, 0, "");
            }
            return this.errorCode;
        }

        public int OpenDoor(DoorType doorType)
        {
            if (this._devInstance != null)
            {
                return this._devInstance.ControlDevice(1, (int)doorType, 1, 0xff, 0, "");
            }
            return this.errorCode;
        }

        public int OpenDoor(DoorType doorType, int time)
        {
            if (this._devInstance != null)
            {
                return this._devInstance.ControlDevice(1, (int)doorType, 1, time, 0, "");
            }
            return this.errorCode;
        }

        public int RebootDevice()
        {
            if (this._devInstance != null)
            {
                return this._devInstance.ControlDevice(3, 0, 0, 0, 0, "");
            }
            return this.errorCode;
        }

        public int SearchDevice(string commType, string address, ref byte buffer)
        {
            int num = new DevCommEx(new Machines()).SearchDevice(commType, address, ref buffer);
            return num;
        }

        public List<ObjMachine> SearchDeviceEx(string commType, string address)
        {
            List<ObjMachine> list = new List<ObjMachine>();
            byte[] buffer = new byte[0x10000];
            if (this.SearchDevice(commType, address, ref buffer[0]) > 0)
            {
                buffer = DataConvert.GetDataBuffer(buffer, false, "\r\n");
                string str = Encoding.UTF8.GetString(buffer);
                if (!string.IsNullOrEmpty(str))
                {
                    string[] strArray = str.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if ((strArray != null) && (strArray.Length > 0))
                    {
                        for (int i = 0; i < strArray.Length; i++)
                        {
                            ObjMachine model = new ObjMachine();
                            strArray[i] = strArray[i].Replace(",", "\t");
                            DataConvert.InitModel(model, strArray[i]);
                            model.DevType = 20;
                            if (!string.IsNullOrWhiteSpace(model.Protype))
                            {
                                string[] strArray2 = model.Protype.Split(new char[] { '_' });
                                if ((strArray2 != null) && (strArray2.Length == 2))
                                {
                                    model.Protype = strArray2[0];
                                    model.ConnType = Convert.ToInt32(DeviceConnType.Normel);
                                    if (!(string.IsNullOrWhiteSpace(strArray2[1]) || !(strArray2[1].ToLower() == "wifi".ToLower())))
                                    {
                                        model.ConnType = Convert.ToInt32(DeviceConnType.Wifi);
                                    }
                                }
                            }
                            list.Add(model);
                        }
                    }
                }
            }
            buffer = null;
            return list;
        }

        public int SetDeviceData(string data, out string persList)
        {
            persList = string.Empty;
            if (this._devInstance != null)
            {
                return this._devInstance.SetDeviceData(data, out persList);
            }
            return this.errorCode;
        }

        public int SetDeviceData(string tableName, string data, string options)
        {
            if (this._devInstance != null)
            {
                return this._devInstance.SetDeviceData(tableName, data, options);
            }
            return this.errorCode;
        }

        public int SetDeviceFileData(string fileName, ref byte buffer, int bufferSize, string options)
        {
            if (this._devInstance != null)
            {
                return this._devInstance.SetDeviceFileData(fileName, ref buffer, bufferSize, options);
            }
            return this.errorCode;
        }

        public int SetDeviceParam(string itemValues)
        {
            if (this._devInstance != null)
            {
                return this._devInstance.SetDeviceParam(itemValues);
            }
            return this.errorCode;
        }
/*
        public int SetFirstCard(List<ObjFirstCard> lstFirstCard)
        {
            if ((lstFirstCard == null) || (0 >= lstFirstCard.Count))
            {
                return 0;
            }
            if (this._devInstance == null)
            {
                return this.errorCode;
            }
            int num = 0;
            int num2 = 500;
            string tableName = "FirstCard";
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < lstFirstCard.Count; i++)
            {
                ObjFirstCard card = lstFirstCard[i];
                builder.Append(card.ToPullCmdString(this.DevInfo) + "\r\n");
                if (((i + 1) % num2) == 0)
                {
                    num = this.SetDeviceData(tableName, builder.ToString(), "");
                    builder = new StringBuilder();
                    if (num < 0)
                    {
                        return num;
                    }
                }
            }
            if (builder.Length > 0)
            {
                num = this.SetDeviceData(tableName, builder.ToString(), "");
            }
            return num;
        }

        public int SetFvTemplate(List<FingerVein> lstFvTemplate)
        {
            if ((lstFvTemplate == null) || (0 >= lstFvTemplate.Count))
            {
                return 0;
            }
            if (this._devInstance == null)
            {
                return this.errorCode;
            }
            int num = 0;
            int num2 = 200;
            string tableName = "fvtemplate";
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < lstFvTemplate.Count; i++)
            {
                FingerVein vein = lstFvTemplate[i];
                builder.Append(vein.ToPullCmdString(this.DevInfo) + "\r\n");
                if (((i + 1) % num2) == 0)
                {
                    num = this.SetDeviceData(tableName, builder.ToString(), "");
                    builder = new StringBuilder();
                    if (num < 0)
                    {
                        return num;
                    }
                }
            }
            if (builder.Length > 0)
            {
                num = this.SetDeviceData(tableName, builder.ToString(), "");
            }
            return num;
        }

        public int SetLinkage(List<AccLinkAgeIo> lstLinkage)
        {
            if ((lstLinkage == null) || (0 >= lstLinkage.Count))
            {
                return 0;
            }
            if (this._devInstance == null)
            {
                return this.errorCode;
            }
            int num = 0;
            int num2 = 500;
            string tableName = "InOutFun";
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < lstLinkage.Count; i++)
            {
                AccLinkAgeIo io = lstLinkage[i];
                builder.Append(io.ToPullCmdString(this.DevInfo) + "\r\n");
                if (((i + 1) % num2) == 0)
                {
                    num = this.SetDeviceData(tableName, builder.ToString(), "");
                    builder = new StringBuilder();
                    if (num < 0)
                    {
                        return num;
                    }
                }
            }
            if (builder.Length > 0)
            {
                num = this.SetDeviceData(tableName, builder.ToString(), "");
            }
            return num;
        }

        public int SetUserAutorize(List<ObjUserAuthorize> lstUserAuth)
        {
            if ((lstUserAuth == null) || (0 >= lstUserAuth.Count))
            {
                return 0;
            }
            if (this._devInstance == null)
            {
                return this.errorCode;
            }
            int num = 0;
            int num2 = 500;
            string tableName = "UserAuthorize";
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < lstUserAuth.Count; i++)
            {
                ObjUserAuthorize authorize = lstUserAuth[i];
                builder.Append(authorize.ToPullCmdString(this.DevInfo) + "\r\n");
                if (((i + 1) % num2) == 0)
                {
                    num = this.SetDeviceData(tableName, builder.ToString(), "");
                    builder = new StringBuilder();
                    if (num < 0)
                    {
                        return num;
                    }
                }
            }
            if (builder.Length > 0)
            {
                num = this.SetDeviceData(tableName, builder.ToString(), "");
            }
            return num;
        }

        public int STD_CancelOperation()
        {
            throw new NotImplementedException();
        }

        public int STD_ClearAdministrators()
        {
            throw new NotImplementedException();
        }

        public int STD_ClearGLog()
        {
            return this.DeleteDeviceData("TransAction", "", "");
        }

        public int STD_ClearKeeperData()
        {
            throw new NotImplementedException();
        }

        public int STD_ClearUser()
        {
            if (this._devInstance != null)
            {
                return this.DeleteDeviceData("User", "", "");
            }
            return this.errorCode;
        }

        public int STD_ClearUserFaceTemplate()
        {
            if (this._devInstance != null)
            {
                return this.DeleteDeviceData("ssrface7", "", "");
            }
            return this.errorCode;
        }

        public int STD_ClearUserFPTemplate()
        {
            if (this._devInstance != null)
            {
                return this.DeleteDeviceData("templatev10", "", "");
            }
            return this.errorCode;
        }

        //public int STD_DeleteUserFaceTemplate(List<FaceTemp> lstFaceTemplate)
        //{
        //    throw new NotImplementedException();
        //}

        //public int STD_DeleteUserFPTemplate(List<Template> lstTemplate)
        //{
        //    throw new NotImplementedException();
        //}

        //public int STD_DeleteUserInfo(List<UserInfo> lstUser)
        //{
        //    throw new NotImplementedException();
        //}

        //public int STD_GetAllTransaction(out List<ObjTransAction> lstTransaction)
        //{
        //    throw new NotImplementedException();
        //}

        //public int STD_GetAllUserBioTemplate(out List<BioTemplate> lstBioTemplate, BioTemplateType BioTpye)
        //{
        //    throw new NotImplementedException();
        //}

        //public int STD_GetAllUserFaceBioTemplate(out List<BioTemplate> lstBioTemplate, BioTemplateType BioTpye)
        //{
        //    throw new NotImplementedException();
        //}

        //public int STD_GetAllUserFaceTemplate(out List<FaceTemp> lstFaceTemplate)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool STD_GetAllUserFingerVein(out List<FingerVein> lstFingerVein)
        //{
        //    throw new NotImplementedException();
        //}

        //public int STD_GetAllUserFPBioTemplate(out List<BioTemplate> lstBioTemplate, BioTemplateType BioTpye)
        //{
        //    throw new NotImplementedException();
        //}

        //public int STD_GetAllUserInfo(out List<UserInfo> lstUser)
        //{
        //    throw new NotImplementedException();
        //}

        //public int STD_GetAllUserTemplate(out List<Template> lstTemplate)
        //{
        //    throw new NotImplementedException();
        //}

        public int STD_GetDeviceParam(Machines objDevParam)
        {
            throw new NotImplementedException();
        }

        //public int STD_GetDoorParam(AccDoor objDevParam)
        //{
        //    throw new NotImplementedException();
        //}

        public int STD_GetDoorState(out int StateCode)
        {
            throw new NotImplementedException();
        }

        public int STD_GetFirmwareVersion(out string FirmwareVer)
        {
            throw new NotImplementedException();
        }

        public int STD_GetRecordCount(MachineDataStatusCode code, out int count)
        {
            throw new NotImplementedException();
        }

        public int STD_GetSysOption(string optName, out string optValue)
        {
            throw new NotImplementedException();
        }

        public int STD_GetUserBioPalmVeinTemplate(out List<BioTemplate> lstBioTemplate, BioTemplateType BioTpye)
        {
            throw new NotImplementedException();
        }

        public int STD_GetUserFpTemplate(int Pin, int FingerId, out BioTemplate Template)
        {
            throw new NotImplementedException();
        }

        public int STD_GetUserFpTemplate(int Pin, int FingerId, out Template Template)
        {
            throw new NotImplementedException();
        }

        public int STD_GetWiegandFmt(out STD_WiegandFmt WiegandFmt)
        {
            throw new NotImplementedException();
        }

        public int STD_InitializeDeviceData()
        {
            throw new NotImplementedException();
        }

        public int STD_OpenDoor(int DoorId, int DelaySeconds)
        {
            throw new NotImplementedException();
        }

        public int STD_RaiseEvent()
        {
            throw new NotImplementedException();
        }

        public List<ObjMachine> STD_SearchDeviceEx()
        {
            return null;
        }

        public int STD_SendFile(string FileName, int EnabledTimeOut = 600)
        {
            throw new NotImplementedException();
        }

        public int STD_SetCloseTimeZone(int TZId)
        {
            throw new NotImplementedException();
        }

        public int STD_SetDeviceCommPwd(int Pwd)
        {
            throw new NotImplementedException();
        }

        public int STD_SetDeviceInfo(DeviceInfoCode code, int value)
        {
            throw new NotImplementedException();
        }

        public int STD_SetDeviceParam(Machines objDevParam)
        {
            throw new NotImplementedException();
        }

        public int STD_SetDeviceTime(DateTime? dateTime = new DateTime?())
        {
            DateTime time = !dateTime.HasValue ? DateTime.Now : dateTime.Value;
            int num = ((((((((time.Year - 0x7d0) * 12) * 0x1f) + ((time.Month - 1) * 0x1f)) + (time.Day - 1)) * 0x15180) + ((time.Hour * 60) * 60)) + (time.Minute * 60)) + time.Second;
            return this.SetDeviceParam("DateTime=" + num);
        }

        public int STD_SetDoorParam(AccDoor objDoorParam)
        {
            throw new NotImplementedException();
        }

        public int STD_SetDoorParam(AccDoor objDoorParam, int DefaultTimeZoneId)
        {
            return this.SetDeviceParam(objDoorParam.ToPullCmdString(this.DevInfo, DefaultTimeZoneId, null));
        }

        public bool STD_SetFingerVein(List<FingerVein> lstFingerVein, out List<FingerVein> failedList)
        {
            throw new NotImplementedException();
        }

        public int STD_SetGateWay(string GateWay, bool EffectiveImmediately = true)
        {
            throw new NotImplementedException();
        }

        public int STD_SetGroup(List<Group> lstGroup)
        {
            throw new NotImplementedException();
        }

        public int STD_SetHoliday(List<AccHolidays> lstHoliday)
        {
            if ((lstHoliday == null) || (0 >= lstHoliday.Count))
            {
                return 0;
            }
            if (this._devInstance == null)
            {
                return this.errorCode;
            }
            int num = 0;
            int num2 = 500;
            string tableName = "Holiday";
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < lstHoliday.Count; i++)
            {
                AccHolidays holidays = lstHoliday[i];
                builder.Append(holidays.ToPullCmdString(this.DevInfo) + "\r\n");
                if (((i + 1) % num2) == 0)
                {
                    num = this.SetDeviceData(tableName, builder.ToString(), "");
                    builder = new StringBuilder();
                    if (num < 0)
                    {
                        return num;
                    }
                }
            }
            if (builder.Length > 0)
            {
                num = this.SetDeviceData(tableName, builder.ToString(), "");
            }
            return num;
        }

        public int STD_SetIpAddress(string IP)
        {
            throw new NotImplementedException();
        }

        public int STD_SetOpenTimeZone(int TZId)
        {
            throw new NotImplementedException();
        }

        public int STD_SetSubnetMask(string SubnetMask, bool EffectiveImmediately = true)
        {
            throw new NotImplementedException();
        }

        public int STD_SetSysOption(string optName, string optValue)
        {
            throw new NotImplementedException();
        }

        public int STD_SetTimeZone(List<AccTimeseg> lstTimeseg, int DefaultTimesegId)
        {
            if ((lstTimeseg == null) || (0 >= lstTimeseg.Count))
            {
                return 0;
            }
            if (this._devInstance == null)
            {
                return this.errorCode;
            }
            int num = 0;
            int num2 = 500;
            string tableName = "TimeZone";
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < lstTimeseg.Count; i++)
            {
                AccTimeseg timeseg = lstTimeseg[i];
                builder.Append(timeseg.ToPullCmdString(this.DevInfo, DefaultTimesegId) + "\r\n");
                if (((i + 1) % num2) == 0)
                {
                    num = this.SetDeviceData(tableName, builder.ToString(), "");
                    builder = new StringBuilder();
                    if (num < 0)
                    {
                        return num;
                    }
                }
            }
            if (builder.Length > 0)
            {
                num = this.SetDeviceData(tableName, builder.ToString(), "");
            }
            return num;
        }

        public int STD_SetUnlockGroup(List<ObjMultimCard> lstMulCard)
        {
            if ((lstMulCard == null) || (0 >= lstMulCard.Count))
            {
                return 0;
            }
            if (this._devInstance == null)
            {
                return this.errorCode;
            }
            int num = 0;
            int num2 = 500;
            string tableName = "multimcard";
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < lstMulCard.Count; i++)
            {
                ObjMultimCard card = lstMulCard[i];
                builder.Append(card.ToPullCmdString(this.DevInfo) + "\r\n");
                if (((i + 1) % num2) == 0)
                {
                    num = this.SetDeviceData(tableName, builder.ToString(), "");
                    builder = new StringBuilder();
                    if (num < 0)
                    {
                        return num;
                    }
                }
            }
            if (builder.Length > 0)
            {
                num = this.SetDeviceData(tableName, builder.ToString(), "");
            }
            return num;
        }

        public int STD_SetUserFaceTemplate(List<BioTemplate> lstBioTemplate)
        {
            throw new NotImplementedException();
        }

        public int STD_SetUserFaceTemplate(List<FaceTemp> lstTemplate)
        {
            if ((lstTemplate == null) || (0 >= lstTemplate.Count))
            {
                return 0;
            }
            if (this._devInstance == null)
            {
                return this.errorCode;
            }
            int num = 0;
            string tableName = "ssrface7";
            StringBuilder builder = new StringBuilder();
            List<ObjFaceTemp> list = new List<ObjFaceTemp>();
            for (int i = 0; i < lstTemplate.Count; i++)
            {
                list.AddRange(FaceDataConverter.DBFace2PullFace(lstTemplate[i]));
            }
            foreach (ObjFaceTemp temp in list)
            {
                builder.Append(temp.ToPullCmdString(this.DevInfo) + "\r\n");
            }
            if (builder.Length > 0)
            {
                num = this.SetDeviceData(tableName, builder.ToString(), "");
            }
            return num;
        }

        public int STD_SetUserFaceTemplate(List<FaceTemp> lstTemplate, out List<string> pinList)
        {
            pinList = new List<string>();
            return -1;
        }

        public int STD_SetUserFPTemplate(List<BioTemplate> lstBioTemplate)
        {
            throw new NotImplementedException();
        }

        public int STD_SetUserFPTemplate(List<Template> lstTemplate)
        {
            if ((lstTemplate == null) || (0 >= lstTemplate.Count))
            {
                return 0;
            }
            if (this._devInstance == null)
            {
                return this.errorCode;
            }
            int num = 0;
            int num2 = 500;
            string tableName = "templatev10";
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < lstTemplate.Count; i++)
            {
                string str2 = lstTemplate[i].ToPullCmdString(this.DevInfo);
                if (!string.IsNullOrWhiteSpace(str2))
                {
                    builder.Append(str2 + "\r\n");
                    if (((i + 1) % num2) == 0)
                    {
                        num = this.SetDeviceData(tableName, builder.ToString(), "");
                        builder = new StringBuilder();
                        if (num < 0)
                        {
                            return num;
                        }
                    }
                }
            }
            if (builder.Length > 0)
            {
                num = this.SetDeviceData(tableName, builder.ToString(), "");
            }
            return num;
        }

        public int STD_SetUserGroup(List<UserInfo> lstUser, Dictionary<int, int> dicMCGroupId_GroupId)
        {
            throw new NotImplementedException();
        }

        public int STD_SetUserInfo(List<UserInfo> lstUser, Dictionary<int, int> dicPullGroupId_StdGroupId)
        {
            if ((lstUser == null) || (0 >= lstUser.Count))
            {
                return 0;
            }
            if (this._devInstance == null)
            {
                return this.errorCode;
            }
            int num = 0;
            int num2 = 500;
            string tableName = "User";
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < lstUser.Count; i++)
            {
                UserInfo info = lstUser[i];
                builder.Append(info.ToPullCmdString(this.DevInfo) + "\r\n");
                if (((i + 1) % num2) == 0)
                {
                    num = this.SetDeviceData(tableName, builder.ToString(), "");
                    builder = new StringBuilder();
                    if (num < 0)
                    {
                        return num;
                    }
                }
            }
            if (builder.Length > 0)
            {
                num = this.SetDeviceData(tableName, builder.ToString(), "");
            }
            return num;
        }

        public int STD_SetUserVerifyMode(List<UserVerifyType> lstUserVT)
        {
            throw new NotImplementedException();
        }

        public int STD_SetWiegandFmt(STD_WiegandFmt WiegandFmt)
        {
            throw new NotImplementedException();
        }*/

        public int STD_ShutdownDevice()
        {
            throw new NotImplementedException();
        }

        public int STD_StartEnroll(string Pin, int FingerId, int Flag = 1)
        {
            throw new NotImplementedException();
        }

        public int STD_StartIdentify()
        {
            throw new NotImplementedException();
        }

        public int STD_StartMonitor()
        {
            throw new NotImplementedException();
        }

        public int STD_StopMonitor()
        {
            throw new NotImplementedException();
        }

        public int STD_UpdateFirmware(string FileName)
        {
            throw new NotImplementedException();
        }

        //public int STD_UploadUserPhoto(List<UserInfo> lstUser)
        //{
        //    throw new NotImplementedException();
        //}

        public void UpdateDevInfo(Machines machine)
        {
            if (null != machine)
            {
                this._devInstance.Dev = machine;
            }
        }

        public int STD_CancelOperation()
        {
            throw new NotImplementedException();
        }

        public int STD_ClearAdministrators()
        {
            throw new NotImplementedException();
        }

        public int STD_ClearGLog()
        {
            return this.DeleteDeviceData("TransAction", "", "");
        }

        public int STD_ClearKeeperData()
        {
            throw new NotImplementedException();
        }

        public int STD_ClearUser()
        {
            throw new NotImplementedException();
        }

        public int STD_ClearUserFaceTemplate()
        {
            throw new NotImplementedException();
        }

        public int STD_ClearUserFPTemplate()
        {
            throw new NotImplementedException();
        }

        public int STD_GetAllTransaction(out List<Transactions> lstTransaction)
        {
            throw new NotImplementedException();
        }

        public int STD_GetDeviceParam(Machines objDevParam)
        {
            throw new NotImplementedException();
        }

        public int STD_GetDoorState(out int StateCode)
        {
            throw new NotImplementedException();
        }

        public int STD_GetFirmwareVersion(out string FirmwareVer)
        {
            throw new NotImplementedException();
        }

        public int STD_GetRecordCount(MachineDataStatusCode code, out int count)
        {
            throw new NotImplementedException();
        }

        public int STD_GetSysOption(string optName, out string optValue)
        {
            throw new NotImplementedException();
        }

        //public int STD_GetUserFpTemplate(int Pin, int FingerId, out Template Template)
        //{
        //    throw new NotImplementedException();
        //}

        public int STD_InitializeDeviceData()
        {
            throw new NotImplementedException();
        }

        public int STD_OpenDoor(int DoorId, int DelaySeconds)
        {
            throw new NotImplementedException();
        }

        public int STD_RaiseEvent()
        {
            throw new NotImplementedException();
        }

        public List<ObjMachine> STD_SearchDeviceEx()
        {
            throw new NotImplementedException();
        }

        public int STD_SendFile(string FileName, int EnabledTimeOut)
        {
            throw new NotImplementedException();
        }

        public int STD_SetCloseTimeZone(int TZId)
        {
            throw new NotImplementedException();
        }

        public int STD_SetDeviceCommPwd(int Pwd)
        {
            throw new NotImplementedException();
        }

        public int STD_SetDeviceParam(Machines objDevParam)
        {
            throw new NotImplementedException();
        }

        public int STD_SetDeviceTime(DateTime? dateTime = null)
        {
            throw new NotImplementedException();
        }

        public int STD_SetGateWay(string GateWay, bool EffectiveImmediately = true)
        {
            throw new NotImplementedException();
        }

        public int STD_SetIpAddress(string IP)
        {
            throw new NotImplementedException();
        }

        public int STD_SetOpenTimeZone(int TZId)
        {
            throw new NotImplementedException();
        }

        public int STD_SetSubnetMask(string SubnetMask, bool EffectiveImmediately = true)
        {
            throw new NotImplementedException();
        }

        public int STD_SetSysOption(string optName, string optValue)
        {
            throw new NotImplementedException();
        }

        // Properties
        public Machines DevInfo
        {
            get
            {
                return this._devInstance.Dev;
            }
        }

        public bool IsConnected
        {
            get
            {
                return ((this._devInstance != null) && this._devInstance.IsConnected);
            }
        }

        public bool IsInitDevice
        {
            get
            {
                return this.m_isInitDevice;
            }
        }

        public bool IsMonitoring
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool Std_IsRTEventNull
        {
            get
            {
                return (null == this.RTEvent);
            }
        }
    }
}
