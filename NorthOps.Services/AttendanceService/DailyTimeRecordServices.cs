using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKTeco.SDK.Model;

namespace NorthOps.Services.DTRService
{
    public class DailyTimeRecordServices
    {
        private IDailyTimeRecordServices dailyTimeRecordServices;

        public DailyTimeRecordServices(IDailyTimeRecordServices dailyTimeRecordServices)
        {
            this.dailyTimeRecordServices = dailyTimeRecordServices;
        }

        public Task<List<Transactions>> AttendanceLog()
        {
            return dailyTimeRecordServices.AttendanceLog();
        }

        public async Task SaveAttendanceLog()
        {
            await dailyTimeRecordServices.SaveAttendanceLog();
        }
        public async Task SaveAttendanceLog(DateTime dateFrom, DateTime dateTo)
        {
            await dailyTimeRecordServices.SaveAttendanceLog(dateFrom, dateTo);
        }
    }
}
