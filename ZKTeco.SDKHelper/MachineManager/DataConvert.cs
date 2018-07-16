using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZKTeco.SDK.Model;

namespace ZKTeco.SDK.MachineManager
{
    public class DataConvert
    {
        public static byte[] GetDataBuffer(byte[] buffer, bool Reverse = false, string splitChar = "\r\n")
        {
            if ((buffer != null) && (buffer.Length > 0))
            {
                int num2;
                byte[] buffer2;
                char[] chArray;
                int num = 0;
                if (!Reverse)
                {
                    for (num2 = 0; num2 < buffer.Length; num2++)
                    {
                        if (buffer[num2] == 0)
                        {
                            num++;
                            if (num > 5)
                            {
                                int length = (num2 + 1) - num;
                                buffer2 = new byte[length];
                                Array.Copy(buffer, 0, buffer2, 0, length);
                                buffer = null;
                                return buffer2;
                            }
                        }
                        else
                        {
                            num = 0;
                        }
                    }
                    return buffer;
                }
                bool flag = false;
                if ((splitChar != null) && (splitChar.Length > 0))
                {
                    chArray = splitChar.ToCharArray();
                }
                else
                {
                    chArray = new char[1];
                }
                for (num2 = buffer.Length - 1; num2 >= 0; num2--)
                {
                    for (int i = 0; i < chArray.Length; i++)
                    {
                        if (buffer[num2] == chArray[i])
                        {
                            buffer2 = new byte[num2 + 1];
                            Array.Copy(buffer, buffer2, buffer2.Length);
                            buffer = buffer2;
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        return buffer;
                    }
                }
            }
            return buffer;
        }

        public static void InitModel(object model, string data)
        {
            if (model != null)
            {
                PropertyInfo[] properties = model.GetType().GetProperties();
                if ((properties != null) && (properties.Length > 0))
                {
                    string[] strArray = data.Split(",\t".ToCharArray());
                    if ((strArray != null) && (strArray.Length > 0))
                    {
                        for (int i = 0; i < strArray.Length; i++)
                        {
                            string[] strArray2 = strArray[i].Split(new char[] { '=' });
                            if (((strArray2 != null) && (strArray2.Length == 2)) && !string.IsNullOrEmpty(strArray2[1]))
                            {
                                strArray2[0] = strArray2[0].Replace("~", "");
                                foreach (PropertyInfo info in properties)
                                {
                                    if (info.Name.ToLower() == strArray2[0].ToLower())
                                    {
                                        try
                                        {
                                            string str = strArray2[1];
                                            if (string.IsNullOrWhiteSpace(str) || (str.Trim().ToLower() == "null".ToLower()))
                                            {
                                                str = string.Empty;
                                            }
                                            SetKValue(model, str, info);
                                        }
                                        catch
                                        {
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void InitModel(object model, string columns, string data)
        {
            if (model != null)
            {
                PropertyInfo[] properties = model.GetType().GetProperties();
                if ((properties != null) && (properties.Length > 0))
                {
                    string[] strArray = data.Split(new char[] { ',' });
                    string[] strArray2 = columns.Split(new char[] { ',' });
                    if ((((strArray != null) && (strArray.Length > 0)) && (strArray2 != null)) && (strArray2.Length == strArray.Length))
                    {
                        for (int i = 0; i < strArray2.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(strArray[i]))
                            {
                                foreach (PropertyInfo info in properties)
                                {
                                    if (info.Name.ToLower() == strArray2[i].ToLower())
                                    {
                                        try
                                        {
                                            string str = strArray[i];
                                            if (!string.IsNullOrEmpty(str))
                                            {
                                                SetKValue(model, str, info);
                                            }
                                        }
                                        catch
                                        {
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void SetKValue(object info, string pvalue, PropertyInfo pi)
        {
            if (pi.PropertyType == typeof(int))
            {
                try
                {
                    int num = int.Parse(pvalue);
                    pi.SetValue(info, num, null);
                }
                catch
                {
                }
            }
            else if (pi.PropertyType == typeof(string))
            {
                pi.SetValue(info, pvalue, null);
            }
            else if (pi.PropertyType == typeof(DateTime))
            {
                try
                {
                    DateTime time = DateTime.Parse(pvalue);
                    pi.SetValue(info, time, null);
                }
                catch
                {
                }
            }
            else if (pi.PropertyType == typeof(EventType))
            {
                try
                {
                    EventType type = (EventType)Enum.Parse(typeof(EventType), pvalue);
                    pi.SetValue(info, type, null);
                }
                catch
                {
                    pi.SetValue(info, EventType.Type0, null);
                }
            }
            else if (pi.PropertyType == typeof(FingerType))
            {
                try
                {
                    FingerType type2 = (FingerType)Enum.Parse(typeof(FingerType), pvalue);
                    pi.SetValue(info, type2, null);
                }
                catch
                {
                    pi.SetValue(info, FingerType.Type0, null);
                }
            }
            else if (pi.PropertyType == typeof(HolidayType))
            {
                try
                {
                    HolidayType type3 = (HolidayType)Enum.Parse(typeof(HolidayType), pvalue);
                    pi.SetValue(info, type3, null);
                }
                catch
                {
                    pi.SetValue(info, HolidayType.Type1, null);
                }
            }
            else if (pi.PropertyType == typeof(InAddrType))
            {
                try
                {
                    InAddrType type4 = (InAddrType)Enum.Parse(typeof(InAddrType), pvalue);
                    pi.SetValue(info, type4, null);
                }
                catch
                {
                    pi.SetValue(info, InAddrType.Type1, null);
                }
            }
            else if (pi.PropertyType == typeof(LoopType))
            {
                try
                {
                    LoopType type5 = (LoopType)Enum.Parse(typeof(LoopType), pvalue);
                    pi.SetValue(info, type5, null);
                }
                catch
                {
                    pi.SetValue(info, LoopType.Type1, null);
                }
            }
            else if (pi.PropertyType == typeof(OutAddrType))
            {
                try
                {
                    OutAddrType type6 = (OutAddrType)Enum.Parse(typeof(OutAddrType), pvalue);
                    pi.SetValue(info, type6, null);
                }
                catch
                {
                    pi.SetValue(info, OutAddrType.Type1, null);
                }
            }
            else if (pi.PropertyType == typeof(OutType))
            {
                try
                {
                    OutType type7 = (OutType)Enum.Parse(typeof(OutType), pvalue);
                    pi.SetValue(info, type7, null);
                }
                catch
                {
                    pi.SetValue(info, OutType.Type1, null);
                }
            }
            else if (pi.PropertyType == typeof(ValidType))
            {
                try
                {
                    ValidType type8 = (ValidType)Enum.Parse(typeof(ValidType), pvalue);
                    pi.SetValue(info, type8, null);
                }
                catch
                {
                    pi.SetValue(info, ValidType.Type1, null);
                }
            }
            else if (pi.PropertyType == typeof(VerifiedType))
            {
                try
                {
                    VerifiedType type9 = (VerifiedType)Enum.Parse(typeof(VerifiedType), pvalue);
                    pi.SetValue(info, type9, null);
                }
                catch
                {
                    pi.SetValue(info, VerifiedType.Type0, null);
                }
            }
        }
    }
}
