using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKTeco.SDK.Model;

namespace ZKTeco.SDK.Helper
{
    public interface ITimeHelper
    {
        List<UserAttendance> Attendance();
    }
}
