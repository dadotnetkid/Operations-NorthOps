using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Web.Mvc;
namespace NorthOps.Services.Helpers
{
    public class CampaignsControllerSchedulerSettings
    {

        static DevExpress.Web.Mvc.MVCxAppointmentStorage appointmentStorage;
        public static DevExpress.Web.Mvc.MVCxAppointmentStorage AppointmentStorage
        {
            get
            {
                if (appointmentStorage == null)
                {
                    appointmentStorage = new DevExpress.Web.Mvc.MVCxAppointmentStorage();
                    appointmentStorage.Mappings.AppointmentId = "AppointmentId";
                    appointmentStorage.Mappings.Start = "StartFrom";
                    appointmentStorage.Mappings.End = "EndTo";
                    appointmentStorage.Mappings.Subject = "FullName";
                    appointmentStorage.Mappings.Description = "Description";
                    appointmentStorage.Mappings.AllDay = "ScheduleDate";
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
                    resourceStorage.Mappings.ResourceId = "Id";
                    resourceStorage.Mappings.Caption = "FullName";
                }
                return resourceStorage;
            }
        }

        public static void UpdateEditableDataObject(NorthOps.Models.northopsEntities appointmentContext, NorthOps.Models.northopsEntities resourceContext)
        {
           
        }

       
    }

}
