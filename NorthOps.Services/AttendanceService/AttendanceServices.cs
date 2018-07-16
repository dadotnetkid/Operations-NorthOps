using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKTeco.SDK.Model;

namespace NorthOps.Services.DTRService
{
    public class DtrServices
    {
        private IDtrServices dtrServices;

        public DtrServices(IDtrServices dtrServices)
        {
            this.dtrServices = dtrServices;
        }

        public Task<List<Transactions>> AttendanceLog()
        {
            return dtrServices.AttendanceLog();
        }

        public async Task SaveAttendanceLog()
        {
            await dtrServices.SaveAttendanceLog();
        }
    }
}
