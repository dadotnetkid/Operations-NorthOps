using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKTeco.SDK.MachineManager;

namespace ZKTeco.SDK.Model
{
    public enum SDKType
    {
        Undefined,
        PullSDK,
        StandaloneSDK
    }
    public enum EventType
    {
        AntibackAlarm = 0x186c2,
        DeviceBroken = 0x186d7,
        Doorbell = 0xe0,
        DoorClosed = 0x186a5,
        DoorNotClosedOrOpened = 0x186a4,
        DoorOpenedByButton = 0x186d5,
        DoorUnexpectedOpened = 0x186a1,
        DuressAlarm = 0x186c0,
        LinkageAlarm = 0x186db,
        NormalOpenEanbled = 11,
        NormalOpened = 0xcd,
        Type0 = 0,
        Type1 = 1,
        Type10 = 10,
        Type100 = 100,
        Type1000 = 0x3e8,
        Type1001 = 0x3e9,
        Type1002 = 0x3ea,
        Type1003 = 0x3eb,
        Type101 = 0x65,
        Type102 = 0x66,
        Type1020 = 0x3fc,
        Type1021 = 0x3fd,
        Type1026 = 0x402,
        Type1027 = 0x403,
        Type1029 = 0x405,
        Type103 = 0x67,
        Type1048 = 0x418,
        Type106 = 0x6a,
        Type107 = 0x6b,
        Type1101 = 0x44d,
        Type2 = 2,
        Type20 = 20,
        Type200 = 200,
        Type201 = 0xc9,
        Type202 = 0xca,
        Type206 = 0xce,
        Type207 = 0xcf,
        Type21 = 0x15,
        Type22 = 0x16,
        Type220 = 220,
        Type221 = 0xdd,
        Type23 = 0x17,
        Type24 = 0x18,
        Type25 = 0x19,
        Type252 = 0xfc,
        Type253 = 0xfd,
        Type254 = 0xfe,
        Type255 = 0xff,
        Type26 = 0x1a,
        Type27 = 0x1b,
        Type28 = 0x1c,
        Type29 = 0x1d,
        Type3 = 3,
        Type30 = 30,
        Type300 = 300,
        Type301 = 0x12d,
        Type302 = 0x12e,
        Type31 = 0x1f,
        Type4 = 4,
        Type5 = 5,
        Type51 = 0x33,
        Type6 = 6,
        Type7 = 7,
        Type8 = 8,
        Type9 = 9,
        ValidNoRight = 0x18a89,
        VerifyFailed = 0x186a0,
        VerifyTimesOut = 0x186da
    }

    public enum FingerType
    {
        Type0 = 0,
        Type1 = 1,
        Type16 = 0x10,
        Type17 = 0x11,
        Type18 = 0x12,
        Type19 = 0x13,
        Type2 = 2,
        Type20 = 20,
        Type21 = 0x15,
        Type22 = 0x16,
        Type23 = 0x17,
        Type24 = 0x18,
        Type25 = 0x19,
        Type3 = 3,
        Type4 = 4,
        Type5 = 5,
        Type6 = 6,
        Type7 = 7,
        Type8 = 8,
        Type9 = 9
    }
    public enum HolidayType
    {
        Type1 = 1,
        Type2 = 2,
        Type3 = 3
    }
    public enum OutAddrType
    {
        Type1 = 1,
        Type10 = 10,
        Type2 = 2,
        Type3 = 3,
        Type4 = 4,
        Type5 = 5,
        Type6 = 6,
        Type7 = 7,
        Type8 = 8,
        Type9 = 9
    }
    public enum LoopType
    {
        Type1 = 1,
        Type2 = 2
    }

    public enum VerifiedType
    {
        FACE = 15,
        FACE_FP_PW = 20,
        FACE_FP_RF = 0x13,
        FACE_PW = 0x11,
        FACE_RF = 0x12,
        FP = 1,
        FP_FACE = 0x10,
        FP_PW = 9,
        FP_PW_RF = 12,
        FP_RF = 10,
        FP_RForPIN = 14,
        FPorPW = 5,
        FPorRF = 6,
        Fv = 0x15,
        Fv_PW = 0x16,
        Fv_PW_RF = 0x18,
        Fv_RF = 0x17,
        Others = 200,
        PIN = 2,
        PIN_FP = 8,
        PIN_FP_PW = 13,
        PV = 0x19,
        PV_FACE = 0x1b,
        PV_FACE_FP = 0x1d,
        PV_FP = 0x1c,
        PV_RF = 0x1a,
        PW = 3,
        PWorRF = 7,
        RF = 4,
        RF_PW = 11,
        SuperPW = 0xc9,
        Type0 = 0
    }
    public enum InAddrType
    {
        Type0,
        Type1,
        Type2,
        Type3,
        Type4,
        Type5,
        Type6,
        Type7,
        Type8,
        Type9,
        Type10,
        Type11,
        Type12
    }
    public enum OutType
    {
        Type0,
        Type1
    }
    public enum ValidType
    {
        Type1 = 1,
        Type3 = 3
    }
    public enum DoorType
    {
        Door1 = 1,
        Door10 = 10,
        Door11 = 11,
        Door12 = 12,
        Door13 = 13,
        Door14 = 14,
        Door15 = 15,
        Door16 = 0x10,
        Door17 = 0x11,
        Door18 = 0x12,
        Door19 = 0x13,
        Door2 = 2,
        Door20 = 20,
        Door3 = 3,
        Door4 = 4,
        Door5 = 5,
        Door6 = 6,
        Door7 = 7,
        Door8 = 8,
        Door9 = 9
    }
    public enum MachineDataStatusCode
    {
        AttRecordCapacity = 9,
        AttRecordCount = 6,
        FaceTemplateCapacity = 0x16,
        FaceTemplateCount = 0x15,
        FingerVeinCount = 0x17,
        ManagerCount = 1,
        OperationRecordCount = 5,
        PalmVeinCount = 0x19,
        PasswordCount = 4,
        RegistedUserCount = 2,
        RemainAttRecordCount = 12,
        RemainTemplateCount = 10,
        RemainUserCount = 11,
        TemplateCapacity = 7,
        TemplateCount = 3,
        UserCapacity = 8
    }

    public delegate void STD_FingerFeature(int Score);

    public delegate void STD_OnEnrollFinger(string EnrollNumber, int FingerIndex, int ActionResult, int TemplateLength);

    public delegate void STD_RTEventHandler(object sender, ObjRTLogInfo RTLog);
    public delegate void DeviceRTEventHandler(Machines sender, ObjRTLogInfo RTLog);
    public enum DeviceConnType
    {
        Normel = 0,
        Wifi = 11,
        Wired = 10
    }
    public enum DeviceInfoCode
    {
        AttRecordAlarmCount = 6,
        BaudRate = 9,
        CheckInterval = 8,
        DateFormat = 0x22,
        HeightSpeedCompare = 0x11,
        IdleTime = 4,
        IdleType = 0x12,
        Languge = 3,
        LockDrivingTime = 5,
        MachineNumber = 2,
        MaxManagerCount = 1,
        MifareCardHaveToRegister = 30,
        MifareTemplateCount = 0x40,
        MifareTemplateSectorCount = 0x3f,
        MifareTemplateStartSector = 0x3e,
        NetworkEnabled = 13,
        Only1_1Mode = 0x23,
        OnlyCheckCard = 0x1c,
        OpRecordAlarmCount = 7,
        RS232Enabled = 14,
        RS485Enabled = 15,
        Threshold121 = 0x19,
        Threshold12N = 0x17,
        ThresholdRegister = 0x18,
        VoiceEnabled = 0x10,
        WiegandAreaCode = 0x3b,
        WiegandDuressId = 0x3a,
        WiegandFailureId = 0x39,
        WiegandPulseInterval = 0x3d,
        WiegandPulseWidth = 60
    }

    public enum ConnectType
    {
        Com=0,
        Net=1,
        Usb=2,

    }

    public enum Privilege
    {
        Admin=0,
        Employee=14
    }





}
