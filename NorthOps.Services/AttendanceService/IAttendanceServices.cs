
using System.Collections.Generic;
using System.Threading.Tasks;
using ZKTeco.SDK.Model;

namespace NorthOps.Services.DTRService
{
    public interface IDtrServices
    {
        Task<List<Transactions>> AttendanceLog();
        Task SaveAttendanceLog();
    }
}
