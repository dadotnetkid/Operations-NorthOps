using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using NorthOps.Models;
using NorthOps.Models.Repository;

namespace NorthOps.Services.Helpers
{

    public class SchedulerSettingsHelper
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
                    appointmentStorage.Mappings.Start = "ScheduleDateFrom";
                    appointmentStorage.Mappings.End = "ScheduleDateTo";
                    appointmentStorage.Mappings.Subject = "Subject";
                    appointmentStorage.Mappings.Type = "AppointmentType";
                    appointmentStorage.Mappings.Label = "Label";
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
        /*
                static void InsertAppointments(object appointmentContext, object resourceContext)
                {
                    System.Collections.IEnumerable appointments = null;
                    System.Collections.IEnumerable resources = null;

                    var newAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToInsert<NorthOps.Models.DailyTimeRecords>("Scheduler", appointments, resources,
                        AppointmentStorage, ResourceStorage);
                    foreach (var appointment in newAppointments)
                    {
                        // Add appointment to your data context
                        throw new NotImplementedException();
                    }
                }
               

                static void DeleteAppointments(object appointmentContext, object resourceContext)
                {
                    System.Collections.IEnumerable appointments = null;
                    System.Collections.IEnumerable resources = null;

                    var delAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToRemove<NorthOps.Models.DailyTimeRecords>("Scheduler", appointments, resources,
                        AppointmentStorage, ResourceStorage);
                    foreach (var appointment in delAppointments)
                    {
                        // Remove the appointment from your data context
                        throw new NotImplementedException();
                    }
                }*/
        static void DeleteAppointments(object appointmentContext, object resourceContext)
        {
            System.Collections.IEnumerable appointments = null;// new UnitOfWork().SchedulesRepo.Get();
            System.Collections.IEnumerable resources = null;

            var delAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToRemove<NorthOps.Models.Schedules>("Scheduler", appointmentContext, resources,AppointmentStorage, ResourceStorage);
            foreach (var appointment in delAppointments)
            {
                // Remove the appointment from your data conte
                // xt
                var Id = appointment.Id;
                var unitOfWork = new UnitOfWork();
                var schedule = unitOfWork.SchedulesRepo.Find(m => m.Id == Id);
                unitOfWork.SchedulesRepo.Delete(schedule);
                unitOfWork.Save();

            }
        }
        static void InsertAppointments(object appointmentContext, object resourceContext)
        {
            System.Collections.IEnumerable appointments = null;
            System.Collections.IEnumerable resources = null;

            var newAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToInsert<NorthOps.Models.Schedules>("Scheduler", appointmentContext, resources,
                AppointmentStorage, ResourceStorage);
            foreach (var appointment in newAppointments)
            {
                UnitOfWork unitOfWork = new UnitOfWork();
                appointment.Id = Guid.NewGuid().ToString();
                appointment.UserId = HttpContext.Current.User.Identity.GetUserId();
                unitOfWork.SchedulesRepo.Insert(appointment);
                

                unitOfWork.Save();
            }
        }
        static void UpdateAppointments(object appointmentContext, object resourceContext)
        {
            System.Collections.IEnumerable appointments = null;
            System.Collections.IEnumerable resources = null;
            UnitOfWork unitOfWork = new UnitOfWork();
            var updAppointments = DevExpress.Web.Mvc.SchedulerExtension.GetAppointmentsToUpdate<NorthOps.Models.Schedules>("Scheduler", appointmentContext, resources,
                AppointmentStorage, ResourceStorage);
            foreach (var appointment in updAppointments)
            {
                appointment.UserId= HttpContext.Current.User.Identity.GetUserId();
                unitOfWork.SchedulesRepo.Update(appointment);
                unitOfWork.Save();
            }
        }
    }
}
