using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Microsoft.AspNet.Identity;
using NorthOps.Models.Repository;

namespace NorthOps.Ops.Controllers
{
    public class DailyTimeRecordController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private string UserId => User.Identity.GetUserId();
        [Route("daily-time-record")]
        public ActionResult Index()
        {
            return View();
        }

        #region Grid
        [ValidateInput(false)]
        public ActionResult DailyTimeRecordGridViewPartial()
        {
            var model = unitOfWork.DailyTimeRecordsRepo.Get(includeProperties: "Schedules,CreatedByUser,Users");

            return PartialView("_DailyTimeRecordGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DailyTimeRecordGridViewPartialAddNew(NorthOps.Models.DailyTimeRecords item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    item.CreatedBy = UserId;
                    unitOfWork.DailyTimeRecordsRepo.Insert(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.DailyTimeRecordsRepo.Get( includeProperties: "Schedules,CreatedByUser,Users");
            return PartialView("_DailyTimeRecordGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult DailyTimeRecordGridViewPartialUpdate(NorthOps.Models.DailyTimeRecords item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var dailyTimeRecords = unitOfWork.DailyTimeRecordsRepo.Find(m => m.Id == item.Id);
                    dailyTimeRecords.ModifiedBy = UserId;
                    dailyTimeRecords.DateFrom = item.DateFrom;
                    dailyTimeRecords.DateTo = item.DateTo;
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.DailyTimeRecordsRepo.Get( includeProperties: "Schedules,CreatedByUser,Users");
            return PartialView("_DailyTimeRecordGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult DailyTimeRecordGridViewPartialDelete([ModelBinder(typeof(DevExpressEditorsBinder))]int? Id)
        {

            if (Id >= 0)
            {
                try
                {
                    unitOfWork.DailyTimeRecordsRepo.Delete(m => m.Id == Id);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            var model = unitOfWork.DailyTimeRecordsRepo.Get( includeProperties: "Schedules,CreatedByUser,Users");
            return PartialView("_DailyTimeRecordGridViewPartial", model);
        }

        public ActionResult AddEditDailyTimeRecordPartial([ModelBinder(typeof(DevExpressEditorsBinder))]
            int? dailyTimeRecordId)
        {
            return PartialView("_AddEditDailyTimeRecordPartial", unitOfWork.DailyTimeRecordsRepo.Find(m => m.Id == dailyTimeRecordId));
        }

        public ActionResult cboSchedules(string ScheduleId, string UserId)
        {
            var dailyTimeRecord = unitOfWork.DailyTimeRecordsRepo.Find(m => m.ScheduleId == ScheduleId, includeProperties: "Schedules");

            ViewBag.Schedules = dailyTimeRecord == null ?
                new UnitOfWork().SchedulesRepo.Fetch(m => m.UserId == UserId).ToList().Select(x => new { Schedule = x.ScheduleDateFrom.ToString("MM/dd/yy hh:mm tt") + " - " + x.ScheduleDateTo.ToString("MM/dd/yy hh:mm tt"), Id = x.Id }).ToList()
                : new UnitOfWork().SchedulesRepo.Fetch(m => m.UserId == dailyTimeRecord.Schedules.UserId).ToList().Select(x => new { Schedule = x.ScheduleDateFrom.ToString("MM/dd/yy hh:mm tt") + " - " + x.ScheduleDateTo.ToString("MM/dd/yy hh:mm tt"), Id = x.Id }).ToList();

            return PartialView("_cboSchedules", dailyTimeRecord);
        }

        #endregion
    }
}