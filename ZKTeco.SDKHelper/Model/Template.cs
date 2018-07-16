using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKTeco.SDK.Model
{
    [Serializable]
    public class Template
    {
        // Fields
        private int _bio_type = 0;
        private byte[] _bitmappicture;
        private byte[] _bitmappicture2;
        private byte[] _bitmappicture3;
        private byte[] _bitmappicture4;
        private string _change_operator;
        private DateTime? _change_time;
        private string _create_operator;
        private DateTime? _create_time;
        private string _delete_operator;
        private DateTime? _delete_time;
        private short _divisionfp;
        private string _emachinenum;
        private int _fingerid;
        private short _flag;
        private string _fpversion;
        private int _sn = 0;
        private int _status = 0;
        private byte[] _template;
        private byte[] _template1;
        private byte[] _template2;
        private byte[] _template3;
        private byte[] _template4;
        private int _templateid;
        private int _userid;
        private short _usetype;
        private DateTime? _utime;
        private int _valid = 1;

        // Methods
        public Template Copy()
        {
            return (Template)base.MemberwiseClone();
        }

        public static bool PullCmd2Model(string CmdString, out Template model)
        {
            model = new Template();
            model.FINGERID = 13;
            bool flag = false;
            if (!string.IsNullOrEmpty(CmdString))
            {
                string[] strArray = CmdString.Split("\t\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (strArray.Length <= 0)
                {
                    return flag;
                }
                for (int i = 0; i < strArray.Length; i++)
                {
                    string str = strArray[i];
                    string[] strArray2 = str.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (strArray2.Length >= 2)
                    {
                        int num;
                        switch (strArray2[0].ToLower())
                        {
                            case "endtag":
                                flag = true;
                                break;

                            case "fingerid":
                                flag = int.TryParse(strArray2[1], out num);
                                if (flag)
                                {
                                    model.FINGERID = num;
                                }
                                break;

                            case "pin":
                                model.Pin = strArray2[1];
                                flag = true;
                                break;

                            case "reserved":
                                flag = true;
                                break;

                            case "size":
                                flag = true;
                                break;

                            case "valid":
                                flag = int.TryParse(strArray2[1], out num);
                                if (flag)
                                {
                                    model.Flag = (short)num;
                                }
                                break;

                            case "uid":
                                flag = true;
                                break;

                            case "template":
                                num = str.IndexOf('=') + 1;
                                model.TEMPLATE4 = Convert.FromBase64String(str.Substring(num, str.Length - num));
                                model.TEMPLATE3 = model.TEMPLATE4;
                                flag = true;
                                break;
                        }
                        if (!flag)
                        {
                            return flag;
                        }
                    }
                }
            }
            return flag;
        }

        public string ToPullCmdString(Machines machine)
        {
            return this.ToPullCmdString(machine.ToDeviceModel());
        }

        public string ToPullCmdString(ObjDevice dev)
        {
            StringBuilder builder = new StringBuilder();
            byte[] inArray = (dev.FPVersion == 10) ? this.TEMPLATE4 : this.TEMPLATE3;
            if (null != inArray)
            {
                builder.Append(string.Format("{0}={1}\t", "EndTag", ""));
                builder.Append(string.Format("{0}={1}\t", "FingerID", this.FINGERID));
                builder.Append(string.Format("{0}={1}\t", "Pin", this.Pin));
                builder.Append(string.Format("{0}={1}\t", "Resverd", ""));
                builder.Append(string.Format("{0}={1}\t", "Size", 0));
                builder.Append(string.Format("{0}={1}\t", "Template", Convert.ToBase64String(inArray)));
                builder.Append(string.Format("{0}={1}\t", "Valid", this.Flag));
                builder.Append(string.Format("{0}={1}\t", "UID", ""));
            }
            if (((builder != null) && (builder.Length > 0)) && (builder[builder.Length - 1] == '\t'))
            {
                builder.Remove(builder.Length - 1, 1);
            }
            return builder.ToString();
        }

        // Properties
        public int bio_type
        {
            get
            {
                return this._bio_type;
            }
            set
            {
                this._bio_type = value;
            }
        }

        public byte[] BITMAPPICTURE
        {
            get
            {
                return this._bitmappicture;
            }
            set
            {
                this._bitmappicture = value;
            }
        }

        public byte[] BITMAPPICTURE2
        {
            get
            {
                return this._bitmappicture2;
            }
            set
            {
                this._bitmappicture2 = value;
            }
        }

        public byte[] BITMAPPICTURE3
        {
            get
            {
                return this._bitmappicture3;
            }
            set
            {
                this._bitmappicture3 = value;
            }
        }

        public byte[] BITMAPPICTURE4
        {
            get
            {
                return this._bitmappicture4;
            }
            set
            {
                this._bitmappicture4 = value;
            }
        }

        public string change_operator
        {
            get
            {
                return this._change_operator;
            }
            set
            {
                this._change_operator = value;
            }
        }

        public DateTime? change_time
        {
            get
            {
                return this._change_time;
            }
            set
            {
                this._change_time = value;
            }
        }

        public string create_operator
        {
            get
            {
                return this._create_operator;
            }
            set
            {
                this._create_operator = value;
            }
        }

        public DateTime? create_time
        {
            get
            {
                return this._create_time;
            }
            set
            {
                this._create_time = value;
            }
        }

        public string delete_operator
        {
            get
            {
                return this._delete_operator;
            }
            set
            {
                this._delete_operator = value;
            }
        }

        public DateTime? delete_time
        {
            get
            {
                return this._delete_time;
            }
            set
            {
                this._delete_time = value;
            }
        }

        public short DivisionFP
        {
            get
            {
                return this._divisionfp;
            }
            set
            {
                this._divisionfp = value;
            }
        }

        public string EMACHINENUM
        {
            get
            {
                return this._emachinenum;
            }
            set
            {
                this._emachinenum = value;
            }
        }

        public int FINGERID
        {
            get
            {
                return this._fingerid;
            }
            set
            {
                this._fingerid = value;
            }
        }

        public short Flag
        {
            get
            {
                return this._flag;
            }
            set
            {
                this._flag = value;
            }
        }

        public string Fpversion
        {
            get
            {
                return this._fpversion;
            }
            set
            {
                this._fpversion = value;
            }
        }

        public string Pin { get; set; }

        public int SN
        {
            get
            {
                return this._sn;
            }
            set
            {
                this._sn = value;
            }
        }

        public int status
        {
            get
            {
                return this._status;
            }
            set
            {
                this._status = value;
            }
        }

        public byte[] TEMPLATE1
        {
            get
            {
                return this._template1;
            }
            set
            {
                this._template1 = value;
            }
        }

        public byte[] TEMPLATE2
        {
            get
            {
                return this._template2;
            }
            set
            {
                this._template2 = value;
            }
        }

        public byte[] TEMPLATE3
        {
            get
            {
                return this._template3;
            }
            set
            {
                this._template3 = value;
            }
        }

        public byte[] TEMPLATE4
        {
            get
            {
                return this._template4;
            }
            set
            {
                this._template4 = value;
            }
        }

        public int TEMPLATEID
        {
            get
            {
                return this._templateid;
            }
            set
            {
                this._templateid = value;
            }
        }

        public byte[] TempPlate
        {
            get
            {
                return this._template;
            }
            set
            {
                this._template = value;
            }
        }

        public int USERID
        {
            get
            {
                return this._userid;
            }
            set
            {
                this._userid = value;
            }
        }

        public short USETYPE
        {
            get
            {
                return this._usetype;
            }
            set
            {
                this._usetype = value;
            }
        }

        public DateTime? UTime
        {
            get
            {
                return this._utime;
            }
            set
            {
                this._utime = value;
            }
        }

        public int Valid
        {
            get
            {
                return this._valid;
            }
            set
            {
                this._valid = value;
            }
        }
    }


}
