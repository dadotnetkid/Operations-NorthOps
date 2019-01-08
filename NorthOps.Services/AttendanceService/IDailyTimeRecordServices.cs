
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZKTeco.SDK.Model;

namespace NorthOps.Services.DTRService
{
    public interface IDailyTimeRecordServices
    {
        Task<List<Transactions>> AttendanceLog();
        Task SaveAttendanceLog();
        Task SaveAttendanceLog(DateTime dateFrom,DateTime dateTo);
    }
}
