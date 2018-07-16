using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKTeco.SDK.Model
{
   public  class UserAttendance
    {
        public UserAttendance()
        {
        }
        public string IdNumber { get; set; }
        public TimeType timeType { get; set; }
        public Double? TotalTimeRendered { get; set; }
        public DateTime AttendanceTime { get; set; }
    }
}
