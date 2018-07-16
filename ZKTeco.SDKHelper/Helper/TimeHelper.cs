using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKTeco.SDK.Model;

namespace ZKTeco.SDK.Helper
{
    public class TimeHelper : ITimeHelper
    {
        private List<ObjUser> users;
        private List<Transactions> transactions;
        public List<UserAttendance> userAttendances { get; set; }
        public TimeHelper(List<ObjUser> users, List<Transactions> transactions)
        {
            this.users = users;
            this.transactions = transactions;
            this.userAttendances = new List<UserAttendance>();
        }

        public TimeHelper()
        {
        }

        public List<UserAttendance> Attendance()
        {
            var res = this.transactions.GroupBy(m => m.LogDateTime.ToShortDateString()).Select(g => new
            {
                KeyId = g.Key,
                Date = g.Select(x => new { x.Pin, Time_second = x.LogDateTime, x.InOutState, })
            });
            //Iterate the groupings
            foreach (var i in res)
            {
                //iterate the inner data


                var AmIn = i.Date.OrderBy(m => m.Time_second).Where(m => m.InOutState == InOutState.CheckIn && m.Time_second.ToString("tt").ToLower() == "am").Select(x => new { AmIn = x }).Take(1);
                var AmOut = i.Date.OrderByDescending(m => m.Time_second).Where(m => m.InOutState == InOutState.CheckOut && m.Time_second.ToString("tt").ToLower() == "am").Select(x => new { AmOut = x }).Take(1);
                foreach (var x in AmIn.Join(AmOut, amIn => amIn.AmIn.Pin, amOut => amOut.AmOut.Pin, (amIn, amOut) => new { In = amIn, Out = amOut }))
                {
                    userAttendances.Add(new UserAttendance()
                    {
                        IdNumber = x.In.AmIn.Pin,
                        AttendanceTime = Convert.ToDateTime(i.KeyId),
                        timeType = TimeType.AM,
                        TotalTimeRendered = (x.Out.AmOut.Time_second - x.In.AmIn.Time_second).TotalMinutes
                    });
                }
                var PmIn = i.Date.OrderBy(m => m.Time_second).Where(m => m.InOutState == InOutState.CheckIn && m.Time_second.ToString("tt").ToLower() == "pm").Select(x => new { PmIn = x }).Take(1);
                var PmOut = i.Date.OrderByDescending(m => m.Time_second).Where(m => m.InOutState == InOutState.CheckOut && m.Time_second.ToString("tt").ToLower() == "pm").Select(x => new { PmOut = x }).Take(1);
                foreach (var x in PmIn.Join(PmOut, pmIn => pmIn.PmIn.Pin, pmOut => pmOut.PmOut.Pin, (pmIn, pmOut) => new { In = pmIn, Out = pmOut }))
                {
                    userAttendances.Add(new UserAttendance()
                    {
                        IdNumber = x.In.PmIn.Pin,
                        AttendanceTime = Convert.ToDateTime(i.KeyId),
                        timeType = TimeType.PM,
                        TotalTimeRendered = (x.Out.PmOut.Time_second - x.In.PmIn.Time_second).TotalMinutes
                    });
                }
            }
            return userAttendances;
        }
    }
}

