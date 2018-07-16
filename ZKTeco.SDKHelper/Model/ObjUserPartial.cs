using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKTeco.SDK.Model
{
    public partial class ObjUser
    {
        //public List<UserAttendance> userAttendances ()
        //{

        //    var res = this.Transactions.GroupBy(m => m.Time_second.ToShortDateString()).Select(g => new
        //    {
        //        KeyId = g.Key,
        //        Date = g.Select(x => new { x.Pin, x.Time_second, x.InOutState })
        //    });
        //    //Iterate the groupings
        //    foreach (var i in res)
        //    {
        //        //iterate the inner data

        //        foreach (var d in i.Date.OrderBy(m => m.Time_second))
        //        {
        //            if (d.Time_second.ToString("tt").ToLower() == "am")
        //            {

        //            }
        //        }
        //    }
        //    return null;
        //}


    }
    public enum TimeType
    {
        AM,
        PM
    }

}
