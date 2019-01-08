using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NorthOps.Models.Repository;
using NorthOps.Models.ViewModels;
using NorthOps.Services.AttendanceService;
using NorthOps.Services.DTRService;

namespace NorthOps.Ops.Controllers
{
    [Authorize]
    public class DailyTimeRecordReportController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        // GET: AttendanceReport
        [Route("report/attendance")]
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> DtrReportGeneratePartial(DailyTimeRecordViewModel model = null)
        {
            try
            {
                DailyTimeRecordReports report = new DailyTimeRecordReports();
                var attendanceSercvices = new AttendanceServices();
                if (model.isGenerated)
                {
                    attendanceSercvices.GetDtrReport(model);
                    report = new DailyTimeRecordReports()
                    {
                        DataSource = attendanceSercvices.dailyTimeRecords(model)
                    };
                }
                else
                {
                    report = new DailyTimeRecordReports()
                    {
                        DataSource = attendanceSercvices.dailyTimeRecords(model)
                    };
                }

                return PartialView("_DtrReportPartial", report);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return PartialView("_DtrReportPartial");
            }

        }

     
        public ActionResult dtrReportSearchPartial(DailyTimeRecordViewModel model = null)
        {
            try
            {
                var users = unitOfWork.UserRepository.Fetch(m => m.UserRoles.Any(x => x.Name == "Employee"), includeProperties: "Schedules,Schedules.DailyTimeRecords,Overtimes,Overtimes.CreatedByUser,Overtimes.ModifiedByUser,Overtimes.Users");
                if (!string.IsNullOrEmpty(model.UserId))
                {
                    users = users.Where(m => m.Id == model.UserId);
                }

                DTRReport report = new DTRReport()
                {
                    DataSource = users.ToList()
                };


                return PartialView("_DtrReportPartial", report);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return PartialView("_DtrReportPartial");
            }

        }
        public ActionResult CboUsersPartial()
        {
            var model = unitOfWork.UserRepository.Get(m => m.UserRoles.Any(x => x.Name == "Employee")).ToList();
            model.Add(new Models.Users() { Id = "", FirstName = "All" });
            return PartialView("_cboUsersPartial", model);
        }
    }
}