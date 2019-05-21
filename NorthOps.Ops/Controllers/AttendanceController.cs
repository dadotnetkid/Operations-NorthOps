using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NorthOps.Models;
using NorthOps.Models.Repository;
using NorthOps.Services.AttendanceService;
using NorthOps.Services.DTRService;

namespace NorthOps.Ops.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AttendanceController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        object AttendanceModel()
        {
            var model = unitOfWork.AttendancesRepo.Fetch().OrderByDescending(m => m.LogDateTime).Select(x => new
            {
                x.LogDateTime,
                x.InOutState,
                FullName = x.Biometrics.Users.FirstName + " " + x.Biometrics.Users.LastName,
                x.Id
            });
            return model;
        }

        // GET: Attendance
        public ActionResult AttendanceLog()
        {

            return View();
        }

        [ValidateInput(false)]
        public ActionResult AttendanceLogGridViewPartial()
        {

            return PartialView("_AttendanceLogGridViewPartial", AttendanceModel());
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> AttendanceLogGridViewPartialAddNew([ModelBinder(typeof(DevExpressEditorsBinder))]Attendances item, [ModelBinder(typeof(DevExpressEditorsBinder))] string userId)
        {
            try
            {

                unitOfWork.AttendancesRepo.Insert(item);
                unitOfWork.Save();
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }


            return PartialView("_AttendanceLogGridViewPartial", AttendanceModel());
        }
        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> GetAttendanceLog(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                DailyTimeRecordServices dtrServices = new DailyTimeRecordServices(new AttendanceServices());
                await dtrServices.SaveAttendanceLog(dateFrom, dateTo);
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }


            return PartialView("_AttendanceLogGridViewPartial", AttendanceModel());
        }


        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> AttendanceLogGridViewPartialUpdate([ModelBinder(typeof(DevExpressEditorsBinder))]Attendances item)
        {
            try
            {
                unitOfWork.AttendancesRepo.Update(item);
                await unitOfWork.SaveAsync();
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }


            return PartialView("_AttendanceLogGridViewPartial", AttendanceModel());
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> AttendanceLogGridViewPartialDelete([ModelBinder(typeof(DevExpressEditorsBinder))]int Id)
        {
            try
            {
                unitOfWork.AttendancesRepo.Delete(m => m.Id == Id);
                await unitOfWork.SaveAsync();
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }


            return PartialView("_AttendanceLogGridViewPartial", AttendanceModel());
        }

        public ActionResult AddEditAttendancePartial([ModelBinder(typeof(DevExpressEditorsBinder))] Attendances item)
        {
            var model = unitOfWork.AttendancesRepo.Find(m => m.Id == item.Id);
            return PartialView("_AddEditAttendancePartial", model);
        }
        public ActionResult DTR()
        {
            return View();
        }

        object appointmentContext = null;
        object resourceContext = null;

        public ActionResult SchedulerPartial()
        {
            System.Collections.IEnumerable appointments = null; // Get appointments from the context
            System.Collections.IEnumerable resources = null; // Get resources from the context

            ViewData["Appointments"] = appointments;
            ViewData["Resources"] = resources;

            return PartialView("_SchedulerPartial");
        }
        public ActionResult SchedulerPartialEditAppointment()
        {
            System.Collections.IEnumerable appointments = null; // Get appointments from the context
            System.Collections.IEnumerable resources = null; // Get resources from the context

            try
            {
                AttendanceControllerSchedulerSettings.UpdateEditableDataObject(appointmentContext, resourceContext);
            }
            catch (Exception e)
            {
                ViewData["SchedulerErrorText"] = e.Message;
            }

            ViewData["Appointments"] = appointments;
            ViewData["Resources"] = resources;

            return PartialView("_SchedulerPartial");
        }

    }
    public class AttendanceControllerSchedulerSettings
    {
        static DevExpress.Web.Mvc.MVCxAppointmentStorage appointmentStorage;
        public static DevExpress.Web.Mvc.MVCxAppointmentStorage AppointmentStorage
        {
            get
            {
                if (appointmentStorage == null)
                {
                    appointmentStorage = new DevExpress.Web.Mvc.MVCxAppointmentStorage();
                    appointmentStorage.Mappings.AppointmentId = "Id";
                    appointmentStorage.Mappings.Start = "LogDateTime";
                    appointmentStorage.Mappings.End = "LogDateTime";
                    appointmentStorage.Mappings.Subject = "BiometricId";
                    appointmentStorage.Mappings.Description = "";
                    appointmentStorage.Mappings.Location = "";
                    appointmentStorage.Mappings.AllDay = "";
                    appointmentStorage.Mappings.Type = "InOutState";
                    appointmentStorage.Mappings.RecurrenceInfo = "";
                    appointmentStorage.Mappings.ReminderInfo = "";
                    appointmentStorage.Mappings.Label = "";
                    appointmentStorage.Mappings.Status = "";
                    appointmentStorage.Mappings.ResourceId = "";
                }
                return appointmentStorage;
            }
        }

        static DevExpress.Web.Mvc.MVCxResourceStorage resourceStorage;
        public static DevExpress.Web.Mvc.MVCxResourceStorage ResourceStorage
        {
            get
            {
                if (resourceStorage == null)
                {
                    resourceStorage = new DevExpress.Web.Mvc.MVCxResourceStorage();
                    resourceStorage.Mappings.ResourceId = "";
                    resourceStorage.Mappings.Caption = "";
                }
                return resourceStorage;
            }
        }

        public static void UpdateEditableDataObject(object appointmentContext, object resourceContext)
        {
            InsertAppointments(appointmentContext, resourceContext);
            UpdateAppointments(appointmentContext, resourceContext);
            DeleteAppointments(appointmentContext, resourceContext);
        }

        static void InsertAppointments(object appointmentContext, object resourceContext)
        {
            System.Collections.IEnumerable appointments = null;
            System.Collections.IEnumerable resources = null;

            var newAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToInsert<NorthOps.Models.Attendances>("Scheduler", appointments, resources,
                AppointmentStorage, ResourceStorage);
            foreach (var appointment in newAppointments)
            {
                // Add appointment to your data context
                throw new NotImplementedException();
            }
        }
        static void UpdateAppointments(object appointmentContext, object resourceContext)
        {
            System.Collections.IEnumerable appointments = null;
            System.Collections.IEnumerable resources = null;

            var updAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToUpdate<NorthOps.Models.Attendances>("Scheduler", appointments, resources,
                AppointmentStorage, ResourceStorage);
            foreach (var appointment in updAppointments)
            {
                // Update the appointment in your data context
                throw new NotImplementedException();
            }
        }

        static void DeleteAppointments(object appointmentContext, object resourceContext)
        {
            System.Collections.IEnumerable appointments = null;
            System.Collections.IEnumerable resources = null;

            var delAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToRemove<NorthOps.Models.Attendances>("Scheduler", appointments, resources,
                AppointmentStorage, ResourceStorage);
            foreach (var appointment in delAppointments)
            {
                // Remove the appointment from your data context
                throw new NotImplementedException();
            }
        }
    }

}