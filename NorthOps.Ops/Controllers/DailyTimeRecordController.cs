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
    [Authorize(Roles = "Administrator")]
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
            var model = unitOfWork.DailyTimeRecordsRepo.Fetch(includeProperties: "Schedules,CreatedByUser,Users").OrderByDescending(m => m.DateFrom).ToList();

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
            var model = unitOfWork.DailyTimeRecordsRepo.Get(includeProperties: "Schedules,CreatedByUser,Users");
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
                    dailyTimeRecords.OriginalDateFrom = dailyTimeRecords.DateFrom;
                    dailyTimeRecords.OriginalDateTo = dailyTimeRecords.DateTo;
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
            var model = unitOfWork.DailyTimeRecordsRepo.Get(includeProperties: "Schedules,CreatedByUser,Users");
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
            var model = unitOfWork.DailyTimeRecordsRepo.Get(includeProperties: "Schedules,CreatedByUser,Users");
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

        #region Daily Time Record Approved By Admin
        [Route("un-approved-daily-time-record")]
        public ActionResult UnApprovedDailyTimeRecord()
        {
            return View();
        }
        [ValidateInput(false)]
        public ActionResult UnApprovedDailyTimeRecordGridViewPartial(int? Id, bool? isAdminApproved)
        {
            var model = unitOfWork.DailyTimeRecordsRepo.Fetch(includeProperties: "Schedules,CreatedByUser,Users").OrderByDescending(m => m.DateFrom)
                .Where(m => m.ModifiedBy != null)
                .Where(m => m.isAdminApproved == null).ToList();




            return PartialView("_UnApprovedDailyTimeRecordGridViewPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UnApprovedDailyTimeRecordGridViewPartialAddNew(NorthOps.Models.DailyTimeRecords item)
        {
            var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    // Insert here a code to insert the new item in your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_UnApprovedDailyTimeRecordGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UnApprovedDailyTimeRecordGridViewPartialUpdate([ModelBinder(typeof(DevExpressEditorsBinder))]NorthOps.Models.DailyTimeRecords item)
        {

            if (ModelState.IsValid)
            {
                try
                {

                    var isApproved = item.isAdminApproved != null;
                    var _item = unitOfWork.DailyTimeRecordsRepo.Find(m => m.Id == item.Id);
                    _item.isAdminApproved = item.isAdminApproved ?? false;
                    if (item.isAdminApproved == null)
                    {
                        if (_item.OriginalDateFrom != null)
                            _item.DateFrom = _item.OriginalDateFrom;
                        if (_item.OriginalDateTo != null)
                            _item.DateTo = _item.OriginalDateTo;
                    }
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var model = unitOfWork.DailyTimeRecordsRepo.Fetch(includeProperties: "Schedules,CreatedByUser,Users")
                .Where(m => m.ModifiedBy != null)
                .Where(m => m.isAdminApproved == null).ToList();
            return PartialView("_UnApprovedDailyTimeRecordGridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UnApprovedDailyTimeRecordGridViewPartialDelete(System.Int32 Id)
        {
            var model = new object[0];
            if (Id >= 0)
            {
                try
                {
                    // Insert here a code to delete the item from your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_UnApprovedDailyTimeRecordGridViewPartial", model);
        }

        public ActionResult EditUnApprovedDailyTimeRecordPartial([ModelBinder(typeof(DevExpressEditorsBinder))] int? Id)
        {
            var model = unitOfWork.DailyTimeRecordsRepo.Find(m => m.Id == Id);

            return PartialView("_EditUnApprovedDailyTimeRecordPartial", model);
        }

        #endregion


    }
}