using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKTeco.SDK.Model;

namespace ZKTeco.SDK.MachineManager
{
    public interface IDeviceServer
    {
        event STD_FingerFeature FingerFeature;
        event STD_OnEnrollFinger OnEnrollFinger;
        event DeviceRTEventHandler RTEvent;
        event DeviceRTEventHandler SwippingCard;

        // Methods
        int CancelAlarm(int DoorId = 0);
        int ClearFvTemplate();
        int ClearUserAuthorize();
        int CloseAuxiliary(int auxiliaryID);
        int CloseDoor(DoorType doorType);
        int Connect(int timeout = 0xbb8);
        int ConnectExt(int TimeOut);
        int ControlDevice(int operationID, int param1, int param2, int param3, int param4, string options);
        int DeleteDeviceData(string tableName, string filter, string options);
        int Disconnect();
        int GetDeviceData(ref byte buffer, int bufferSize, string tableName, string fileName, string filter, string options);
        int GetDeviceDataCount(string tableName, string filter, string options);
        int GetDeviceFileData(ref byte buffer, ref int bufferSize, string fileName, string options);
        int GetDeviceParam(ref byte buffer, int bufferSize, string itemValues);
        int GetOptionValue(string OptionNames, out string OptionValues);
        int GetRTLog(ref byte buffer, int bufferSize);
        int GetRTLogExt(out string RTLogs);
        List<ObjRTLogInfo> GetRTLogs(ref int errorNo);
        string GetSDKVersion();
        bool InitDevice(Machines dev);
        bool IsMonitoringSwippingCard();
        int ModifyIPAddress(string commType, string address, string buffer);
        int NormalOpenDoor(DoorType doorType, bool state);
        int OpenDoor(DoorType doorType);
        int OpenDoor(DoorType doorType, int time);
        int RebootDevice();
        int SearchDevice(string commType, string address, ref byte buffer);
        List<ObjMachine> SearchDeviceEx(string commType, string address);
        int SetDeviceData(string data, out string persList);
        int SetDeviceData(string tableName, string data, string options);
        int SetDeviceFileData(string fileName, ref byte buffer, int bufferSize, string options);
        int SetDeviceParam(string itemValues);
     //   int SetFirstCard(List<ObjFirstCard> lstFirstCard);
      //  int SetFvTemplate(List<FingerVein> lstFvTemplate);
     //   int SetLinkage(List<AccLinkAgeIo> lstLinkage);
     //   int SetUserAutorize(List<ObjUserAuthorize> lstAuthorize);
        int STD_CancelOperation();
        int STD_ClearAdministrators();
        int STD_ClearGLog();
        int STD_ClearKeeperData();
        int STD_ClearUser();
        int STD_ClearUserFaceTemplate();
        int STD_ClearUserFPTemplate();
       // int STD_DeleteUserFaceTemplate(List<FaceTemp> lstFaceTemplate);
        //int STD_DeleteUserFPTemplate(List<Template> lstTemplate);
        //int STD_DeleteUserInfo(List<UserInfo> lstUser);
        int STD_GetAllTransaction(out List<Transactions> lstTransaction);
       // int STD_GetAllUserBioTemplate(out List<BioTemplate> lstBioTemplate, BioTemplateType BioTpye);
        //int STD_GetAllUserFaceBioTemplate(out List<BioTemplate> lstBioTemplate, BioTemplateType BioTpye);
        //int STD_GetAllUserFaceTemplate(out List<FaceTemp> lstFaceTemplate);
        //bool STD_GetAllUserFingerVein(out List<FingerVein> lstFingerVein);
//        int STD_GetAllUserFPBioTemplate(out List<BioTemplate> lstBioTemplate, BioTemplateType BioTpye);
        //int STD_GetAllUserInfo(out List<UserInfo> lstUser);
        //int STD_GetAllUserTemplate(out List<Template> lstTemplate);
        int STD_GetDeviceParam(Machines objDevParam);
        //int STD_GetDoorParam(AccDoor objDevParam);
        int STD_GetDoorState(out int StateCode);
        int STD_GetFirmwareVersion(out string FirmwareVer);
        int STD_GetRecordCount(MachineDataStatusCode code, out int count);
        int STD_GetSysOption(string optName, out string optValue);
        //int STD_GetUserBioPalmVeinTemplate(out List<BioTemplate> lstBioTemplate, BioTemplateType BioTpye);
        //int STD_GetUserFpTemplate(int Pin, int FingerId, out BioTemplate Template);
       // int STD_GetUserFpTemplate(int Pin, int FingerId, out Template Template);
        //int STD_GetWiegandFmt(out STD_WiegandFmt WiegandFmt);
        int STD_InitializeDeviceData();
        int STD_OpenDoor(int DoorId, int DelaySeconds);
        int STD_RaiseEvent();
        List<ObjMachine> STD_SearchDeviceEx();
        int STD_SendFile(string FileName, int EnabledTimeOut);
        int STD_SetCloseTimeZone(int TZId);
        int STD_SetDeviceCommPwd(int Pwd);
       // int STD_SetDeviceInfo(DeviceInfoCode code, int value);
        int STD_SetDeviceParam(Machines objDevParam);
        int STD_SetDeviceTime(DateTime? dateTime = new DateTime?());
      //  int STD_SetDoorParam(AccDoor objDoorParam, int DefaultTimeZoneId);
       // bool STD_SetFingerVein(List<FingerVein> lstFingerVein, out List<FingerVein> failedList);
        int STD_SetGateWay(string GateWay, bool EffectiveImmediately = true);
       // int STD_SetGroup(List<Group> lstGroup);
        //int STD_SetHoliday(List<AccHolidays> lstHoliday);
        int STD_SetIpAddress(string IP);
        int STD_SetOpenTimeZone(int TZId);
        int STD_SetSubnetMask(string SubnetMask, bool EffectiveImmediately = true);
        int STD_SetSysOption(string optName, string optValue);
     //   int STD_SetTimeZone(List<AccTimeseg> lstTimeseg, int DefaultTimesegId);
      //  int STD_SetUnlockGroup(List<ObjMultimCard> lstMultiGroup);
      //  int STD_SetUserFaceTemplate(List<BioTemplate> lstBioTemplate);
     //   int STD_SetUserFaceTemplate(List<FaceTemp> lstTemplate);
      //  int STD_SetUserFaceTemplate(List<FaceTemp> lstTemplate, out List<string> pinList);
      //  int STD_SetUserFPTemplate(List<BioTemplate> lstBioTemplate);
       // int STD_SetUserFPTemplate(List<Template> lstTemplate);
       // int STD_SetUserGroup(List<UserInfo> lstUser, Dictionary<int, int> dicMCGroupId_GroupId);
     //   int STD_SetUserInfo(List<UserInfo> lstUser, Dictionary<int, int> dicPullGroupId_StdGroupId);
    //    int STD_SetUserVerifyMode(List<UserVerifyType> lstUserVT);
      //  int STD_SetWiegandFmt(STD_WiegandFmt WiegandFmt);
        int STD_ShutdownDevice();
        int STD_StartEnroll(string Pin, int FingerId, int Flag = 1);
        int STD_StartIdentify();
        int STD_StartMonitor();
        int STD_StopMonitor();
        int STD_UpdateFirmware(string FileName);
       // int STD_UploadUserPhoto(List<UserInfo> lstUser);
        void UpdateDevInfo(Machines machine);

        // Properties
        Machines DevInfo { get; }
        bool IsConnected { get; }
        bool Std_IsRTEventNull { get; }
    }
}
