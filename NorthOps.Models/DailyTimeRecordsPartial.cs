using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Models
{
    public partial class DailyTimeRecords
    {
        public string Schedule => this.Schedules.ScheduleDateFrom.ToString("MM/dd/yy hh:mm tt") + " - " + Schedules.ScheduleDateTo.ToString("MM/dd/yy hh:mm tt");
        public string Subject => this.Schedules.Subject;
        public string Description => Schedule;
    }
    public partial class DailyTimeRecords
    {
        public int Label => 3;
        public int Type => 0;
        public string Modified => this.Users.FullName;
        public string Created => this.CreatedByUser.FullName;

        public double TimeRendered => ((this.DateTo ?? this.DateFrom ?? DateTime.Now) - (this.DateFrom ?? this.DateTo ?? DateTime.Now)).TotalMinutes / 60;
    }
}
