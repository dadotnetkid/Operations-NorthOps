using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.XtraScheduler;
using Microsoft.AspNet.Identity;
using NorthOps.Models;
using NorthOps.Models.Repository;
using NorthOps.Services.Helpers;

namespace NorthOps.Portal.Controllers
{
    [Authorize(Roles = "Administrator,Employee")]

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
            var model = unitOfWork.DailyTimeRecordsRepo.Fetch(m => m.Schedules.UserId == UserId, includeProperties: "Schedules,CreatedByUser,Users").OrderByDescending(m=>m.DateFrom).ToList();

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
                    item.DateCreated = DateTime.Now;
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
            var model = unitOfWork.DailyTimeRecordsRepo.Fetch(m => m.Schedules.UserId == UserId, includeProperties: "Schedules,CreatedByUser,Users").OrderByDescending(m => m.DateFrom).ToList();
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
                    if (dailyTimeRecords.OriginalDateFrom == null)
                    {
                        dailyTimeRecords.OriginalDateFrom = dailyTimeRecords.DateFrom;
                        dailyTimeRecords.OriginalDateTo = dailyTimeRecords.DateTo;
                    }

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
            var model = unitOfWork.DailyTimeRecordsRepo.Fetch(m => m.Schedules.UserId == UserId, includeProperties: "Schedules,CreatedByUser,Users").OrderByDescending(m => m.DateFrom).ToList();
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
            var model = unitOfWork.DailyTimeRecordsRepo.Get(m => m.Schedules.UserId == UserId, includeProperties: "Schedules,CreatedByUser,Users");
            return PartialView("_DailyTimeRecordGridViewPartial", model);
        }

        public ActionResult AddEditDailyTimeRecordPartial([ModelBinder(typeof(DevExpressEditorsBinder))]
            int? dailyTimeRecordId)
        {
            return PartialView("_AddEditDailyTimeRecordPartial", unitOfWork.DailyTimeRecordsRepo.Find(m => m.Id == dailyTimeRecordId));
        }



        #endregion

        #region Scheduler

        object appointmentContext => unitOfWork.DailyTimeRecordsRepo.Get(m => m.Schedules.UserId == UserId);
        object resourceContext = null;

        private IQueryable<DailyTimeRecords> appointments => unitOfWork.DailyTimeRecordsRepo.Fetch(m => m.Schedules.UserId == UserId);
        System.Collections.IEnumerable resources => null;

        public ActionResult DailyTimeRecordSchedulerPartial()
        {
            return PartialView("_DailyTimeRecordSchedulerPartial", appointments.ToList());
        }
        public ActionResult DailyTimeRecordSchedulerPartialEditAppointment()
        {
            // Get resources from the context

            try
            {
                DailyTimeRecordSchedulerSettings.UpdateEditableDataObject(appointmentContext, resourceContext);
            }
            catch (Exception e)
            {
                ViewData["SchedulerErrorText"] = e.Message;
            }
            return PartialView("_DailyTimeRecordSchedulerPartial", appointments.ToList());
        }

        public ActionResult EditDailyTimerRecordSchedulerPartial([ModelBinder(typeof(DevExpressEditorsBinder))]Appointment appointment)
        {
            var appointmentId = Convert.ToInt32(appointment?.Id ?? 0);
            var appointments = unitOfWork.DailyTimeRecordsRepo.Find(includeProperties: "Users", filter: m => m.Id == appointmentId);
            return PartialView("_EditDailyTimerRecordSchedulerPartial", appointments);
        }
        #endregion


        public ActionResult DailyTimeRecordCallbackPanelPartial(ScheduleType scheduleType=ScheduleType.Calendar)
        {
            ViewBag.ScheduleType = scheduleType;
            return PartialView("_DailyTimeRecordCallbackPanelPartial");
        }
    }


}