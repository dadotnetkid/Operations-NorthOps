using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using NorthOps.Models.Repository;

namespace NorthOps.Services.Helpers
{
    public class DailyTimeRecordSchedulerSettings
    {
        
        private static string UserId => HttpContext.Current.User.Identity.GetUserId();
        static System.Collections.IEnumerable appointments => new UnitOfWork().DailyTimeRecordsRepo.Get(m => m.Schedules.UserId == UserId);
        static System.Collections.IEnumerable resources = null;
        static DevExpress.Web.Mvc.MVCxAppointmentStorage appointmentStorage;

        public static DevExpress.Web.Mvc.MVCxAppointmentStorage AppointmentStorage
        {
            get
            {
                if (appointmentStorage == null)
                {
                    appointmentStorage = new DevExpress.Web.Mvc.MVCxAppointmentStorage();
                    appointmentStorage.Mappings.AppointmentId = "Id";
                    appointmentStorage.Mappings.Start = "DateFrom";
                    appointmentStorage.Mappings.End = "DateTo";
                    appointmentStorage.Mappings.Subject = "Subject";
                    appointmentStorage.Mappings.Description = "Description";
                    appointmentStorage.Mappings.Location = "";
                    appointmentStorage.Mappings.AllDay = "";
                    appointmentStorage.Mappings.Type = "Type";
                    appointmentStorage.Mappings.RecurrenceInfo = "";
                    appointmentStorage.Mappings.ReminderInfo = "";
                    appointmentStorage.Mappings.Label = "Label";
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


            var newAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToInsert<NorthOps.Models.DailyTimeRecords>("DailyTimeRecordScheduler", appointmentContext, resources,
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

            var updAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToUpdate<NorthOps.Models.DailyTimeRecords>("DailyTimeRecordScheduler", appointmentContext, resources,
                AppointmentStorage, ResourceStorage);
            UnitOfWork unitOfWork = new UnitOfWork();
            foreach (var appointment in updAppointments)
            {
                // Update the appointment in your data context
                var dailyTimeRecords = unitOfWork.DailyTimeRecordsRepo.Find(m => m.Id == appointment.Id);
                dailyTimeRecords.ModifiedBy=UserId;
                dailyTimeRecords.DateFrom = appointment.DateFrom;
                dailyTimeRecords.DateTo = appointment.DateTo;
                
                unitOfWork.Save();
            }
        }

        static void DeleteAppointments(object appointmentContext, object resourceContext)
        {
            System.Collections.IEnumerable appointments = null;
            System.Collections.IEnumerable resources = null;

            var delAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToRemove<NorthOps.Models.DailyTimeRecords>("DailyTimeRecordScheduler", appointmentContext, resources,
                AppointmentStorage, ResourceStorage);
            foreach (var appointment in delAppointments)
            {
                // Remove the appointment from your data context
                throw new NotImplementedException();
            }
        }
    }
}
