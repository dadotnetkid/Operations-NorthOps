using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKTeco.SDK.Model
{
    public class UserVerifyType
    {
        // Methods
        public UserVerifyType Copy()
        {
            return (UserVerifyType)base.MemberwiseClone();
        }

        public static bool PullCmd2Model(string CmdString, out UserVerifyType model)
        {
            model = new UserVerifyType();
            bool flag2 = false;
            if (!string.IsNullOrEmpty(CmdString))
            {
                string[] strArray = CmdString.Split("\t\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (strArray.Length <= 0)
                {
                    return flag2;
                }
                for (int i = 0; i < strArray.Length; i++)
                {
                    int num;
                    bool flag;
                    string[] strArray2 = strArray[i].Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (strArray2.Length < 2)
                    {
                        continue;
                    }
                    string str2 = strArray2[0].ToLower();
                    if (str2 != null)
                    {
                        if (!(str2 == "pin"))
                        {
                            if (str2 == "groupno")
                            {
                                goto Label_00F5;
                            }
                            if (str2 == "verifytype")
                            {
                                goto Label_0118;
                            }
                            if (str2 == "usegroupvt")
                            {
                                goto Label_013B;
                            }
                        }
                        else
                        {
                            flag2 = int.TryParse(strArray2[1], out num);
                            if (flag2)
                            {
                                model.Pin = num;
                            }
                        }
                    }
                    goto Label_015E;
                    Label_00F5:
                    flag2 = int.TryParse(strArray2[1], out num);
                    if (flag2)
                    {
                        model.GroupNo = num;
                    }
                    goto Label_015E;
                    Label_0118:
                    flag2 = int.TryParse(strArray2[1], out num);
                    if (flag2)
                    {
                        model.VerifyType = num;
                    }
                    goto Label_015E;
                    Label_013B:
                    flag2 = bool.TryParse(strArray2[1], out flag);
                    if (flag2)
                    {
                        model.UseGroupVT = flag;
                    }
                    Label_015E:
                    if (!flag2)
                    {
                        return flag2;
                    }
                }
            }
            return flag2;
        }

        public string ToPullCmdString(Machines machine)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("{0}={1}\t", "Pin", this.Pin));
            builder.Append(string.Format("{0}={1}\t", "GroupNo", this.GroupNo));
            builder.Append(string.Format("{0}={1}\t", "VerifyType", this.VerifyType));
            builder.Append(string.Format("{0}={1}\t", "UseGroupVT", this.UseGroupVT));
            if (((builder != null) && (builder.Length > 0)) && (builder[builder.Length - 1] == '\t'))
            {
                builder.Remove(builder.Length - 1, 1);
            }
            return builder.ToString();
        }

        // Properties
        public int GroupNo { get; set; }

        public int Pin { get; set; }

        public bool UseGroupVT { get; set; }

        public int VerifyType { get; set; }
    }


}
