using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKTeco.SDK.Model;

namespace ZKTeco.SDK.Helper
{
    public class TimeHelperService : ITimeHelper
    {
        private ITimeHelper timeHelper;

        public TimeHelperService(ITimeHelper timeHelper)
        {
            this.timeHelper = timeHelper;
        }

        public List<UserAttendance> Attendance()
        {
            return timeHelper.Attendance();
        }
    }
}
