using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class SchedulesController : Controller
    {
        public string UserId => User.Identity.GetUserId();

        private UnitOfWork unitOfWork = new UnitOfWork();

        private IQueryable<Schedules> scheduleModel => unitOfWork.SchedulesRepo.Fetch(m => m.UserId == this.UserId)
            .OrderBy(m => m.ScheduleDateFrom);

        #region Grid
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult SchedulesGridViewPartial()
        {
            return PartialView("_SchedulesGridViewPartial", scheduleModel.OrderByDescending(m=>m.CreatedDate).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SchedulesGridViewPartialAddNew([ModelBinder(typeof(DevExpressEditorsBinder))]NorthOps.Models.Schedules item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    item.Id = Guid.NewGuid().ToString();
                    item.UserId = UserId;
                    item.CreatedDate = DateTime.Now;
                    unitOfWork.SchedulesRepo.Insert(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("_SchedulesGridViewPartial", scheduleModel.OrderByDescending(m => m.CreatedDate).ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult SchedulesGridViewPartialUpdate([ModelBinder(typeof(DevExpressEditorsBinder))]Models.Schedules item)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    item.UserId = UserId;
                    
                    unitOfWork.SchedulesRepo.Update(item);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("_SchedulesGridViewPartial", scheduleModel.OrderByDescending(m => m.CreatedDate).ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult SchedulesGridViewPartialDelete([ModelBinder(typeof(DevExpressEditorsBinder))]string Id)
        {

            if (Id != null)
            {
                try
                {
                    unitOfWork.SchedulesRepo.Delete(m => m.Id == Id);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }

            return PartialView("_SchedulesGridViewPartial", scheduleModel.OrderByDescending(m => m.CreatedDate).ToList());
        }
        public ActionResult AddEditSchedulePartial([ModelBinder(typeof(DevExpressEditorsBinder))]System.String scheduleId)
        {
            var model = unitOfWork.SchedulesRepo.Find(m => m.Id == scheduleId);
            return PartialView("_AddEditSchedulePartial", model);
        }

        #endregion




        #region Scheduler Calendar
        object appointmentContext => unitOfWork.SchedulesRepo.Get(m => m.UserId == UserId).ToList();
        object resourceContext = null;

        public ActionResult SchedulerPartial(string TimeZone="")
        {
            var appointments = unitOfWork.SchedulesRepo.Get(includeProperties: "Users", filter: m => m.UserId.Contains(UserId)).ToList();
            ViewData["Appointments"] = appointments;
            //  ViewData["Resources"] = resources;
            ViewBag.UserId = UserId;
            if (!string.IsNullOrEmpty(TimeZone))
            {
                //Debug.WriteLine(Request.Params["TimeZone"]);
                ViewBag.TimeZone = TimeZone;//Request.Params["TimeZone"] ?? "Easter Island Standard Time";
                appointments
                    .ForEach(m =>
                    {
                        m.ScheduleDateFrom =
                            TimeZoneInfo.ConvertTime(m.ScheduleDateFrom, TimeZoneInfo.FindSystemTimeZoneById(TimeZone));
                        m.ScheduleDateTo =
                            TimeZoneInfo.ConvertTime(m.ScheduleDateTo, TimeZoneInfo.FindSystemTimeZoneById(TimeZone));
                
                    });
            }
            return PartialView("_SchedulerPartial", appointments);
        }
        public ActionResult SchedulerPartialEditAppointment(string TimeZone = "")
        {
            try
            {
                SchedulerSettingsHelper.UpdateEditableDataObject(appointmentContext, resourceContext);
            }
            catch (Exception e)
            {
                ViewData["SchedulerErrorText"] = e.Message;
            }

            var appointments = unitOfWork.SchedulesRepo.Get(includeProperties: "Users", filter: m => m.UserId.Contains(UserId)).ToList() ;
            ViewData["Appointments"] = appointments;
            if (!string.IsNullOrEmpty(TimeZone))
            {
                //Debug.WriteLine(Request.Params["TimeZone"]);
                ViewBag.TimeZone = TimeZone;//Request.Params["TimeZone"] ?? "Easter Island Standard Time";
                appointments
                    .ForEach(m =>
                    {
                        m.ScheduleDateFrom =
                            TimeZoneInfo.ConvertTime(m.ScheduleDateFrom, TimeZoneInfo.FindSystemTimeZoneById(TimeZone));
                        m.ScheduleDateTo =
                            TimeZoneInfo.ConvertTime(m.ScheduleDateTo, TimeZoneInfo.FindSystemTimeZoneById(TimeZone));
                    });
            }
            return PartialView("_SchedulerPartial", appointments);
        }


        public ActionResult EditSchedulerPartial(Appointment appointment)
        {
            var appointmentId = appointment?.Id?.ToString();
            var appointments = unitOfWork.SchedulesRepo.Find(includeProperties: "Users", filter: m => m.Id == appointmentId);
            return PartialView("_EditSchedulerPartial", appointments);
        }

        public ActionResult SchedulerPartialAddEditUpdate([ModelBinder(typeof(DevExpressEditorsBinder))]Schedules item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(item.TimeZoneId);
                    //if (item.TimeZoneId == "Central Standard Time")
                    //{
                    //    item.ScheduleDateFrom = item.ScheduleDateFrom.AddHours(13);
                    //    item.ScheduleDateTo = item.ScheduleDateTo.AddHours(13);
                    //}

                    if (item.Id == null)
                    {
                        item.Id = Guid.NewGuid().ToString();
                        item.UserId = UserId;
                        unitOfWork.SchedulesRepo.Insert(item);
                        unitOfWork.Save();
                    }
                    else
                    {
                        item.UserId = UserId;
                        unitOfWork.SchedulesRepo.Update(item);
                        unitOfWork.Save();
                    }

                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            var appointments = unitOfWork.SchedulesRepo.Find(includeProperties: "Users", filter: m => m.Id == item.Id);
            return PartialView("_AddSchedulerPartial", appointments);
        }

        public ActionResult PcAddSchedulerPartial()
        {
            return PartialView("_PcAddSchedulerPartial", new Schedules());
        }
        #endregion

        public ActionResult SchedulerCallbackPanelPartial(ScheduleType scheduleType=ScheduleType.Calendar)
        {
            ViewBag.ScheduleType = scheduleType;
            return PartialView("_SchedulerCallbackPanelPartial");
        }
    }


}