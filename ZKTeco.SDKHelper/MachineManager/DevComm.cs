using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZKTeco.SDK.Model;
using static ZKTeco.SDK.MachineManager.ComTokenManager;

namespace ZKTeco.SDK.MachineManager
{
    public class DevComm
    {
        // Fields
        private Machines dev;
        private readonly int ErrUnknown;
        private IntPtr h;
        private int LockTimeOut;
        private bool m_isLocked;
        private int m_OutTime;
        private int m_returncode;
        private object ThreadLock;

        // Methods
        public DevComm()
        {
            this.LockTimeOut = 0xbb8;
            this.ThreadLock = new object();
            this.ErrUnknown = -99;
            this.m_returncode = -1000;
            this.m_OutTime = -1001;
            this.m_isLocked = false;
            this.dev = new Machines();
            this.h = IntPtr.Zero;
        }

        public DevComm(Machines device)
        {
            this.LockTimeOut = 0xbb8;
            this.ThreadLock = new object();
            this.ErrUnknown = -99;
            this.m_returncode = -1000;
            this.m_OutTime = -1001;
            this.m_isLocked = false;
            //    this.dev = new Machines();
            this.h = IntPtr.Zero;
            this.dev = device;
        }

        public int Connect(int timeout = 0xbb8)
        {
            int errUnknown = -1000;
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
                                errUnknown = this.ConnectWithoutLock(timeout);
                                if ((-206 == errUnknown) && (this.dev.DevSDKType != SDKType.StandaloneSDK))
                                {
                                    this.KillPlrscagent();
                                    errUnknown = this.ConnectWithoutLock(timeout);
                                }
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                errUnknown = this.ErrUnknown;
                                // //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on Connect_Com " + exception.Message);
                            }
                            finally
                            {
                                Monitor.Exit(comToken);
                            }
                        }
                        else
                        {
                            errUnknown = this.m_OutTime;
                        }
                    }
                    else
                    {
                        errUnknown = this.ConnectWithoutLock(timeout);
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on Connect " + exception.Message);
                    errUnknown = this.ErrUnknown;
                }
                finally
                {
                    Monitor.Exit(this.ThreadLock);
                }
            }
            else
            {
                errUnknown = this.m_OutTime;
            }
            if (this.h != IntPtr.Zero)
            {
                this.m_returncode = this.h.ToInt32();
                int bufferSize = 0x4000;
                byte[] buffer = new byte[bufferSize];
                GetRTLog(this.h, ref buffer[0], bufferSize);
                this.m_isLocked = false;
                return this.m_returncode;
            }
            return errUnknown;
        }

        [DllImport("plcommpro.dll")]
        public static extern IntPtr Connect(string connStr);
        public int ConnectExt(int TimeOut)
        {
            int errUnknown;
            Exception exception;
            if (!Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                return this.m_OutTime;
            }
            try
            {
                if (this.dev.ConnectType == 0)
                {
                    ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                    if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                    {
                        return this.m_OutTime;
                    }
                    try
                    {
                        try
                        {
                            errUnknown = this.ConnectExtWithoutLock(TimeOut);
                        }
                        catch (Exception exception1)
                        {
                            exception = exception1;
                            errUnknown = this.ErrUnknown;
                            //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on ConnectExt_Com " + exception.Message);
                        }
                        return errUnknown;
                    }
                    finally
                    {
                        Monitor.Exit(comToken);
                    }
                    return errUnknown;
                }
                return this.ConnectExtWithoutLock(TimeOut);
            }
            catch (Exception exception2)
            {
                exception = exception2;
                errUnknown = this.ErrUnknown;
                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on ConnectExt " + exception.Message);
            }
            finally
            {
                Monitor.Exit(this.ThreadLock);
            }
            return errUnknown;
        }

        [DllImport("plcommpro.dll")]
        public static extern IntPtr ConnectExt(string Parameters, out int pErrCode);
        protected int ConnectExtWithoutLock(int TimeOut)
        {
            int num;
            string str;
            object[] objArray;
            switch (this.dev.ConnectType)
            {
                case ConnectType.Com:
                    {
                        string[] strArray = new string[] { "protocol=RS485,port=COM", this.dev.SerialPort.ToString(), ",baudrate=", this.dev.Baudrate.ToString(), "bps,deviceid=", this.dev.MachineNumber.ToString(), ",timeout=", TimeOut.ToString(), ",passwd=", (this.dev.CommPassword ?? "").Trim() };
                        str = string.Concat(strArray);
                        break;
                    }
                case ConnectType.Net:
                    objArray = new object[] { "protocol=TCP,ipaddress=", this.dev.IP, ",port=", this.dev.Port, ",timeout=", TimeOut.ToString(), ",passwd=", (this.dev.CommPassword ?? "").Trim() };
                    str = string.Concat(objArray);
                    break;

                case ConnectType.Usb:
                    str = "protocol=USB,timeout=" + TimeOut.ToString() + ",passwd=" + (this.dev.CommPassword ?? "").Trim();
                    break;

                default:
                    objArray = new object[] { "protocol=TCP,ipaddress=", this.dev.IP, ",port=", this.dev.Port, ",timeout=", TimeOut.ToString(), ",passwd=", (this.dev.CommPassword ?? "").Trim() };
                    str = string.Concat(objArray);
                    break;
            }
            this.h = ConnectExt(str, out num);
            return num;
        }

        protected int ConnectWithoutLock(int timeout = 0xbb8)
        {
            int num = -1000;
            if (IntPtr.Zero == this.h)
            {
                StringBuilder builder = new StringBuilder();
                string commPassword = "";
                if (!string.IsNullOrEmpty(this.dev.CommPassword))
                {
                    commPassword = this.dev.CommPassword;
                }
                switch (this.dev.ConnectType)
                {
                    case ConnectType.Com:
                        builder.AppendFormat(@"protocol=RS485,port=\\.\COM{0},baudrate={1}bps,deviceid={2},timeout={3},passwd={4}", new object[] { this.dev.SerialPort, this.dev.Baudrate, this.dev.MachineNumber, timeout, this.dev.CommPassword });
                        break;

                    case ConnectType.Net:
                        builder.AppendFormat("protocol=TCP,ipaddress={0},port={1},timeout={2},passwd={3}", new object[] { this.dev.IP, this.dev.Port, timeout, commPassword });
                        break;

                    case ConnectType.Usb:
                        builder.AppendFormat("protocol=USB,timeout={0},passwd={1}", timeout, commPassword);
                        break;
                }
                this.h = Connect(builder.ToString());
                if (this.h == IntPtr.Zero)
                {
                    num = PullLastError();
                    return ((num > 0) ? -num : num);
                }
            }
            return this.h.ToInt32();
        }

        public int ControlDevice(int operationID, int param1, int param2, int param3, int param4, string options)
        {
            if (this.Wait())
            {
                if (IntPtr.Zero == this.h)
                {
                    this.m_returncode = this.Connect(0xbb8);
                }
                this.m_isLocked = true;
                if (IntPtr.Zero == this.h)
                {
                    this.m_isLocked = false;
                    return this.m_returncode;
                }
                int errUnknown = this.ErrUnknown;
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
                                    errUnknown = ControlDevice(this.h, operationID, param1, param2, param3, param4, options);
                                }
                                catch (Exception exception1)
                                {
                                    exception = exception1;
                                    errUnknown = this.ErrUnknown;
                                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on ControlDevice_Com " + exception.Message);
                                }
                                finally
                                {
                                    Monitor.Exit(comToken);
                                }
                            }
                            else
                            {
                                errUnknown = this.m_OutTime;
                            }
                        }
                        else
                        {
                            errUnknown = ControlDevice(this.h, operationID, param1, param2, param3, param4, options);
                        }
                        this.m_isLocked = false;
                        return errUnknown;
                    }
                    catch (Exception exception2)
                    {
                        exception = exception2;
                        //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on ControlDevice " + exception.Message);
                    }
                    finally
                    {
                        Monitor.Exit(this.ThreadLock);
                    }
                    this.m_isLocked = false;
                    return errUnknown;
                }
                this.m_isLocked = false;
            }
            return this.m_OutTime;
        }

        [DllImport("plcommpro.dll")]
        public static extern int ControlDevice(IntPtr h, int operationID, int param1, int param2, int param3, int param4, string options);
        public int DeleteDeviceData(string tableName, string filter, string options)
        {
            if (this.Wait())
            {
                if (IntPtr.Zero == this.h)
                {
                    this.m_returncode = this.Connect(0xbb8);
                }
                this.m_isLocked = true;
                if (IntPtr.Zero == this.h)
                {
                    this.m_isLocked = false;
                    return this.m_returncode;
                }
                int errUnknown = this.ErrUnknown;
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
                                    errUnknown = DeleteDeviceData(this.h, tableName, filter, options);
                                }
                                catch (Exception exception1)
                                {
                                    exception = exception1;
                                    errUnknown = this.ErrUnknown;
                                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on DeleteDeviceData_Com " + exception.Message);
                                }
                                finally
                                {
                                    Monitor.Exit(comToken);
                                }
                            }
                            else
                            {
                                errUnknown = this.m_OutTime;
                            }
                        }
                        else
                        {
                            errUnknown = DeleteDeviceData(this.h, tableName, filter, options);
                        }
                        this.m_isLocked = false;
                        return errUnknown;
                    }
                    catch (Exception exception2)
                    {
                        exception = exception2;
                        //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on DeleteDeviceData " + exception.Message);
                    }
                    finally
                    {
                        Monitor.Exit(this.ThreadLock);
                    }
                    this.m_isLocked = false;
                    return errUnknown;
                }
                this.m_isLocked = false;
            }
            return this.m_OutTime;
        }

        [DllImport("plcommpro.dll")]
        public static extern int DeleteDeviceData(IntPtr h, string tableName, string data, string options);
        public void Disconnect()
        {
            if ((this.Wait() && !this.m_isLocked) && ((IntPtr.Zero != this.h) && Monitor.TryEnter(this.ThreadLock, this.LockTimeOut)))
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
                                Disconnect(this.h);
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
                        Disconnect(this.h);
                    }
                    this.h = IntPtr.Zero;
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on Disconnect " + exception.Message);
                    this.h = IntPtr.Zero;
                }
                finally
                {
                    Monitor.Exit(this.ThreadLock);
                }
            }
        }

        [DllImport("plcommpro.dll")]
        public static extern void Disconnect(IntPtr h);
        public int GetDeviceData(ref byte buffer, int bufferSize, string tableName, string fileName, string filter, string options)
        {
            if (this.Wait())
            {
                if (IntPtr.Zero == this.h)
                {
                    this.m_returncode = this.Connect(0xbb8);
                }
                this.m_isLocked = true;
                if (IntPtr.Zero == this.h)
                {
                    this.m_isLocked = false;
                    return this.m_returncode;
                }
                int errUnknown = this.ErrUnknown;
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
                                    errUnknown = GetDeviceData(this.h, ref buffer, bufferSize, tableName, fileName, filter, options);
                                }
                                catch (Exception exception1)
                                {
                                    exception = exception1;
                                    errUnknown = this.ErrUnknown;
                                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetDeviceData_Com " + exception.Message);
                                }
                                finally
                                {
                                    Monitor.Exit(comToken);
                                }
                            }
                            else
                            {
                                errUnknown = this.m_OutTime;
                            }
                        }
                        else
                        {
                            errUnknown = GetDeviceData(this.h, ref buffer, bufferSize, tableName, fileName, filter, options);
                        }
                        this.m_isLocked = false;
                        return errUnknown;
                    }
                    catch (Exception exception2)
                    {
                        exception = exception2;
                        //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetDeviceData " + exception.Message);
                    }
                    finally
                    {
                        Monitor.Exit(this.ThreadLock);
                    }
                    this.m_isLocked = false;
                    return errUnknown;
                }
                this.m_isLocked = false;
            }
            return this.m_OutTime;
        }

        [DllImport("plcommpro.dll")]
        public static extern int GetDeviceData(IntPtr h, ref byte buffer, int bufferSize, string tableName, string fileName, string filter, string options);
        public int GetDeviceDataCount(string tableName, string filter, string options)
        {
            if (this.Wait())
            {
                if (IntPtr.Zero == this.h)
                {
                    this.m_returncode = this.Connect(0xbb8);
                }
                this.m_isLocked = true;
                if (IntPtr.Zero == this.h)
                {
                    this.m_isLocked = false;
                    return this.m_returncode;
                }
                int errUnknown = this.ErrUnknown;
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
                                    errUnknown = GetDeviceDataCount(this.h, tableName, filter, options);
                                }
                                catch (Exception exception1)
                                {
                                    exception = exception1;
                                    errUnknown = this.ErrUnknown;
                                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetDeviceDataCount_Com " + exception.Message);
                                }
                                finally
                                {
                                    Monitor.Exit(comToken);
                                }
                            }
                            else
                            {
                                errUnknown = this.m_OutTime;
                            }
                        }
                        else
                        {
                            errUnknown = GetDeviceDataCount(this.h, tableName, filter, options);
                        }
                        this.m_isLocked = false;
                        return errUnknown;
                    }
                    catch (Exception exception2)
                    {
                        exception = exception2;
                        //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetDeviceDataCount " + exception.Message);
                    }
                    finally
                    {
                        Monitor.Exit(this.ThreadLock);
                    }
                    this.m_isLocked = false;
                    return errUnknown;
                }
                this.m_isLocked = false;
            }
            return this.m_OutTime;
        }

        [DllImport("plcommpro.dll")]
        public static extern int GetDeviceDataCount(IntPtr h, string tableName, string filter, string options);
        public int GetDeviceFileData(ref byte buffer, ref int bufferSize, string fileName, string options)
        {
            if (this.Wait())
            {
                if (IntPtr.Zero == this.h)
                {
                    this.m_returncode = this.Connect(0xbb8);
                }
                this.m_isLocked = true;
                if (IntPtr.Zero == this.h)
                {
                    this.m_isLocked = false;
                    return this.m_returncode;
                }
                int errUnknown = this.ErrUnknown;
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
                                    errUnknown = GetDeviceFileData(this.h, ref buffer, ref bufferSize, fileName, options);
                                }
                                catch (Exception exception1)
                                {
                                    exception = exception1;
                                    errUnknown = this.ErrUnknown;
                                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetDeviceFileData_Com " + exception.Message);
                                }
                                finally
                                {
                                    Monitor.Exit(comToken);
                                }
                            }
                            else
                            {
                                errUnknown = this.m_OutTime;
                            }
                        }
                        else
                        {
                            errUnknown = GetDeviceFileData(this.h, ref buffer, ref bufferSize, fileName, options);
                        }
                        this.m_isLocked = false;
                        return errUnknown;
                    }
                    catch (Exception exception2)
                    {
                        exception = exception2;
                        //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetDeviceFileData " + exception.Message);
                    }
                    finally
                    {
                        Monitor.Exit(this.ThreadLock);
                    }
                    this.m_isLocked = false;
                    return errUnknown;
                }
                this.m_isLocked = false;
            }
            return this.m_OutTime;
        }

        [DllImport("plcommpro.dll")]
        public static extern int GetDeviceFileData(IntPtr h, ref byte buffer, ref int bufferSize, string fileName, string options);
        public int GetDeviceParam(ref byte buffer, int bufferSize, string itemValues)
        {
            if (this.Wait())
            {
                if (IntPtr.Zero == this.h)
                {
                    this.m_returncode = this.Connect(0xbb8);
                }
                this.m_isLocked = true;
                if (IntPtr.Zero == this.h)
                {
                    this.m_isLocked = false;
                    return this.m_returncode;
                }
                int errUnknown = this.ErrUnknown;
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
                                    errUnknown = GetDeviceParam(this.h, ref buffer, bufferSize, itemValues);
                                }
                                catch (Exception exception1)
                                {
                                    exception = exception1;
                                    errUnknown = this.ErrUnknown;
                                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetDeviceParam_Com " + exception.Message);
                                }
                                finally
                                {
                                    Monitor.Exit(comToken);
                                }
                            }
                            else
                            {
                                errUnknown = this.m_OutTime;
                            }
                        }
                        else
                        {
                            errUnknown = GetDeviceParam(this.h, ref buffer, bufferSize, itemValues);
                        }
                        this.m_isLocked = false;
                        return errUnknown;
                    }
                    catch (Exception exception2)
                    {
                        exception = exception2;
                        //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetDeviceParam " + exception.Message);
                    }
                    finally
                    {
                        Monitor.Exit(this.ThreadLock);
                    }
                    this.m_isLocked = false;
                    return errUnknown;
                }
                this.m_isLocked = false;
            }
            return this.m_OutTime;
        }

        [DllImport("plcommpro.dll")]
        public static extern int GetDeviceParam(IntPtr h, ref byte buffer, int bufferSize, string itemValues);
        public int GetRTLog(ref byte buffer, int bufferSize)
        {
            if (this.Wait())
            {
                if (IntPtr.Zero == this.h)
                {
                    this.m_returncode = this.Connect(0xbb8);
                }
                this.m_isLocked = true;
                if (IntPtr.Zero == this.h)
                {
                    this.m_isLocked = false;
                    return this.m_returncode;
                }
                int errUnknown = this.ErrUnknown;
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
                                    errUnknown = GetRTLog(this.h, ref buffer, bufferSize);
                                }
                                catch (Exception exception1)
                                {
                                    exception = exception1;
                                    errUnknown = this.ErrUnknown;
                                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetRTLog_Com " + exception.Message);
                                }
                                finally
                                {
                                    Monitor.Exit(comToken);
                                }
                            }
                            else
                            {
                                errUnknown = this.m_OutTime;
                            }
                        }
                        else
                        {
                            errUnknown = GetRTLog(this.h, ref buffer, bufferSize);
                        }
                        this.m_isLocked = false;
                        return errUnknown;
                    }
                    catch (Exception exception2)
                    {
                        exception = exception2;
                        //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetRTLog " + exception.Message);
                    }
                    finally
                    {
                        Monitor.Exit(this.ThreadLock);
                    }
                    this.m_isLocked = false;
                    return errUnknown;
                }
                this.m_isLocked = false;
            }
            return this.m_OutTime;
        }

        [DllImport("plcommpro.dll")]
        public static extern int GetRTLog(IntPtr h, ref byte buffer, int bufferSize);
        public int GetRTLogExt(out string RTLogs)
        {
            int rTLogExtWithoutLock;
            Exception exception;
            RTLogs = "";
            if (!Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                return this.m_OutTime;
            }
            try
            {
                if (this.dev.ConnectType == 0)
                {
                    ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                    if (!Monitor.TryEnter(comToken, this.LockTimeOut))
                    {
                        return this.m_OutTime;
                    }
                    try
                    {
                        try
                        {
                            rTLogExtWithoutLock = this.GetRTLogExtWithoutLock(out RTLogs);
                        }
                        catch (Exception exception1)
                        {
                            exception = exception1;
                            rTLogExtWithoutLock = this.ErrUnknown;
                            //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception onGetRTLogExt_Com " + exception.Message);
                        }
                        return rTLogExtWithoutLock;
                    }
                    finally
                    {
                        Monitor.Exit(comToken);
                    }
                    return rTLogExtWithoutLock;
                }
                return this.GetRTLogExtWithoutLock(out RTLogs);
            }
            catch (Exception exception2)
            {
                exception = exception2;
                rTLogExtWithoutLock = this.ErrUnknown;
                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on GetRTLogExt " + exception.Message);
            }
            finally
            {
                Monitor.Exit(this.ThreadLock);
            }
            return rTLogExtWithoutLock;
        }

        [DllImport("plcommpro.dll")]
        public static extern int GetRTLogExt(IntPtr h, byte[] buffer, int bufferSize);
        protected int GetRTLogExtWithoutLock(out string RTlogs)
        {
            RTlogs = "";
            byte[] buffer = new byte[0x1400];
            int num = GetRTLogExt(this.h, buffer, buffer.Length);
            if (num >= 0)
            {
                buffer = DataConvert.GetDataBuffer(buffer, false, "\r\n");
                RTlogs = Encoding.Default.GetString(buffer);
            }
            buffer = null;
            return num;
        }

        public void KillPlrscagent()
        {
            Process[] processes = Process.GetProcesses();
            if (processes != null)
            {
                for (int i = 0; i < processes.Length; i++)
                {
                    Process process = processes[i];
                    if ((process.ProcessName.ToLower() == "rscagent.dll".ToLower()) || (process.ProcessName.ToLower() == "plrscagent.dll".ToLower()))
                    {
                        try
                        {
                            process.Kill();
                            Thread.Sleep(500);
                        }
                        catch
                        {
                        }
                        break;
                    }
                }
            }
        }

        public int ModifyIPAddress(string commType, string address, string buffer)
        {
            if (IntPtr.Zero == this.h)
            {
                this.m_returncode = this.Connect(0xbb8);
            }
            this.m_isLocked = true;
            if (IntPtr.Zero == this.h)
            {
                this.m_isLocked = false;
                return this.m_returncode;
            }
            int errUnknown = this.ErrUnknown;
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
                                errUnknown = ModifyIPAddressbySDK(commType, address, buffer);
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                errUnknown = this.ErrUnknown;
                                //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on ModifyIPAddress_Com " + exception.Message);
                            }
                            finally
                            {
                                Monitor.Exit(comToken);
                            }
                            return errUnknown;
                        }
                        return this.m_OutTime;
                    }
                    return ModifyIPAddressbySDK(commType, address, buffer);
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on ModifyIPAddress " + exception.Message);
                }
                finally
                {
                    Monitor.Exit(this.ThreadLock);
                }
                return errUnknown;
            }
            return this.m_OutTime;
        }

        [DllImport("plcommpro.dll", EntryPoint = "ModifyIPAddress")]
        public static extern int ModifyIPAddressbySDK(string commType, string address, string buffer);
        [DllImport("plcommpro.dll")]
        public static extern int PullLastError();
        public int SearchDevice(string commType, string address, ref byte buffer)
        {
            int errUnknown = this.ErrUnknown;
            if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
            {
                try
                {
                    Exception exception;
                    try
                    {
                        if (this.dev.ConnectType != 0)
                        {
                            return SearchDevicebySDK(commType, address, ref buffer);
                        }
                        ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                        if (Monitor.TryEnter(comToken, this.LockTimeOut))
                        {
                            try
                            {
                                try
                                {
                                    errUnknown = SearchDevicebySDK(commType, address, ref buffer);
                                }
                                catch (Exception exception1)
                                {
                                    exception = exception1;
                                    errUnknown = this.ErrUnknown;
                                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SearchDevice_Com " + exception.Message);
                                }
                                return errUnknown;
                            }
                            finally
                            {
                                Monitor.Exit(comToken);
                            }
                            return errUnknown;
                        }
                        return this.m_OutTime;
                    }
                    catch (Exception exception2)
                    {
                        exception = exception2;
                        //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SearchDevice " + exception.Message);
                    }
                    return errUnknown;
                }
                finally
                {
                    Monitor.Exit(this.ThreadLock);
                }
                return errUnknown;
            }
            return this.m_OutTime;
        }

        [DllImport("plcommpro.dll", EntryPoint = "SearchDevice")]
        public static extern int SearchDevicebySDK(string commType, string address, ref byte buffer);
        public int SetDeviceData(string data, out string persList)
        {
            persList = string.Empty;
            return -13;
        }

        public int SetDeviceData(string tableName, string data, string options)
        {
            Encoding encoding = ((("0" == this.dev.CompatOldFirmware) && ("86" == this.dev.language)) && ((tableName != null) && ("user" == tableName.ToLower()))) ? Encoding.UTF8 : Encoding.Default;
            if (this.Wait())
            {
                if (IntPtr.Zero == this.h)
                {
                    this.m_returncode = this.Connect(0xbb8);
                }
                this.m_isLocked = true;
                if (IntPtr.Zero == this.h)
                {
                    this.m_isLocked = false;
                    return this.m_returncode;
                }
                int errUnknown = this.ErrUnknown;
                if (Monitor.TryEnter(this.ThreadLock, this.LockTimeOut))
                {
                    Exception exception;
                    try
                    {
                        byte[] bytes;
                        if (this.dev.ConnectType == 0)
                        {
                            ComToken comToken = ComTokenManager.GetComToken(this.dev.SerialPort);
                            if (Monitor.TryEnter(comToken, this.LockTimeOut))
                            {
                                try
                                {
                                    if (Encoding.Default == encoding)
                                    {
                                        errUnknown = SetDeviceData(this.h, tableName, data, options);
                                    }
                                    else
                                    {
                                        bytes = encoding.GetBytes(data);
                                        errUnknown = SetDeviceData(this.h, tableName, ref bytes[0], options);
                                    }
                                }
                                catch (Exception exception1)
                                {
                                    exception = exception1;
                                    errUnknown = this.ErrUnknown;
                                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetDeviceData_Com " + exception.Message);
                                }
                                finally
                                {
                                    Monitor.Exit(comToken);
                                }
                            }
                            else
                            {
                                errUnknown = this.m_OutTime;
                            }
                        }
                        else if (Encoding.Default == encoding)
                        {
                            errUnknown = SetDeviceData(this.h, tableName, data, options);
                        }
                        else
                        {
                            bytes = encoding.GetBytes(data);
                            errUnknown = SetDeviceData(this.h, tableName, ref bytes[0], options);
                        }
                        this.m_isLocked = false;
                        return errUnknown;
                    }
                    catch (Exception exception2)
                    {
                        exception = exception2;
                        //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetDeviceData " + exception.Message);
                    }
                    finally
                    {
                        Monitor.Exit(this.ThreadLock);
                    }
                    this.m_isLocked = false;
                    return errUnknown;
                }
                this.m_isLocked = false;
            }
            return this.m_OutTime;
        }

        [DllImport("plcommpro.dll")]
        public static extern int SetDeviceData(IntPtr h, string tableName, string data, string options);
        [DllImport("plcommpro.dll")]
        public static extern int SetDeviceData(IntPtr h, string tableName, ref byte data, string options);
        public int SetDeviceFileData(string fileName, ref byte buffer, int bufferSize, string options)
        {
            if (this.Wait())
            {
                if (IntPtr.Zero == this.h)
                {
                    this.m_returncode = this.Connect(0xbb8);
                }
                this.m_isLocked = true;
                if (IntPtr.Zero == this.h)
                {
                    this.m_isLocked = false;
                    return this.m_returncode;
                }
                int errUnknown = this.ErrUnknown;
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
                                    errUnknown = SetDeviceFileData(this.h, fileName, ref buffer, bufferSize, options);
                                }
                                catch (Exception exception1)
                                {
                                    exception = exception1;
                                    errUnknown = this.ErrUnknown;
                                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetDeviceFileData_Com " + exception.Message);
                                }
                                finally
                                {
                                    Monitor.Exit(comToken);
                                }
                            }
                            else
                            {
                                errUnknown = this.m_OutTime;
                            }
                        }
                        else
                        {
                            errUnknown = SetDeviceFileData(this.h, fileName, ref buffer, bufferSize, options);
                        }
                        this.m_isLocked = false;
                        return errUnknown;
                    }
                    catch (Exception exception2)
                    {
                        exception = exception2;
                        //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetDeviceFileData " + exception.Message);
                    }
                    finally
                    {
                        Monitor.Exit(this.ThreadLock);
                    }
                    this.m_isLocked = false;
                    return errUnknown;
                }
                this.m_isLocked = false;
            }
            return this.m_OutTime;
        }

        [DllImport("plcommpro.dll")]
        public static extern int SetDeviceFileData(IntPtr h, string fileName, ref byte buffer, int bufferSize, string options);
        public int SetDeviceParam(string itemValues)
        {
            if (this.Wait())
            {
                if (IntPtr.Zero == this.h)
                {
                    this.m_returncode = this.Connect(0xbb8);
                }
                this.m_isLocked = true;
                if (IntPtr.Zero == this.h)
                {
                    this.m_isLocked = false;
                    return this.m_returncode;
                }
                int errUnknown = this.ErrUnknown;
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
                                    errUnknown = SetDeviceParam(this.h, itemValues);
                                }
                                catch (Exception exception1)
                                {
                                    exception = exception1;
                                    errUnknown = this.ErrUnknown;
                                    //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetDeviceParam_Com " + exception.Message);
                                }
                                finally
                                {
                                    Monitor.Exit(comToken);
                                }
                            }
                            else
                            {
                                errUnknown = this.m_OutTime;
                            }
                        }
                        else
                        {
                            errUnknown = SetDeviceParam(this.h, itemValues);
                        }
                    }
                    catch (Exception exception2)
                    {
                        exception = exception2;
                        //SysLogServer.WriteLog(this.dev.MachineAlias + " Exception on SetDeviceParam " + exception.Message);
                    }
                    finally
                    {
                        Monitor.Exit(this.ThreadLock);
                    }
                    this.m_isLocked = false;
                    return errUnknown;
                }
                this.m_isLocked = false;
            }
            return this.m_OutTime;
        }

        [DllImport("plcommpro.dll")]
        public static extern int SetDeviceParam(IntPtr h, string itemValues);
        private bool Wait()
        {
            int num = 0;
            while (this.m_isLocked)
            {
                Thread.Sleep(0x3e8);
                num++;
                if (num >= 2)
                {
                    return false;
                }
            }
            return true;
        }

        // Properties
        public Machines Dev
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

        public bool IsConnected
        {
            get
            {
                if (IntPtr.Zero == this.h)
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsLocked
        {
            get
            {
                return this.m_isLocked;
            }
        }
    }
}
