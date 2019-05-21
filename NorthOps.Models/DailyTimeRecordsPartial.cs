using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Models
{
    public partial class DailyTimeRecords
    {
        public string Schedule => this.Schedules?.ScheduleDateFrom.ToString("MM/dd/yy hh:mm tt") + " - " + Schedules?.ScheduleDateTo.ToString("MM/dd/yy hh:mm tt");
        public string Subject => this.Schedules.Subject;
        public string Description => "Your Schedule " + Schedule;
        public string Absent => this.isAbsent == true ? "Absent" : "";
        public DateTime? In => isAbsent == true ? null : DateFrom;
        public DateTime? Out => isAbsent == true ? null : DateTo;

        public string ModifyStatus =>
            isAdminApproved == true ? "Approved" :
            isAdminApproved == null && ModifiedBy != null ? "Pending " : isAdminApproved == null && Modified == null ? "Un Modified" : "Not Approved";

        //public int? Tardiness
        //{
        //    get
        //    {
        //        var from = (DateFrom - this.Schedules?.ScheduleDateFrom)?.TotalMinutes;
        //        from = from <= 0 ? 0 : from;
        //        var to = (this.Schedules?.ScheduleDateTo - DateTo)?.TotalMinutes;
        //        to = to <= 0 ? 0 : to;
        //        return Convert.ToInt32(from) + Convert.ToInt32(to);

        //    }
        //}
    }
    public partial class DailyTimeRecords
    {
        public int Label => 3;
        public int Type => 0;
        public string Modified => this.Users?.FullName;
        public string Created => this.CreatedByUser.FullName;

        public double TimeRendered => ((this.DateTo ?? this.DateFrom ?? DateTime.Now) - (this.DateFrom ?? this.DateTo ?? DateTime.Now)).TotalMinutes / 60;
    }
}
