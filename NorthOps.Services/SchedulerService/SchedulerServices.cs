using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Services.SchedulerService
{
    public class SchedulerServices
    {
        private ISchedulerServices schedulerServices;

        public SchedulerServices(ISchedulerServices schedulerServices)
        {
            this.schedulerServices = schedulerServices;
        }

        public void SetAgentSchedules()
        {
            this.schedulerServices.SetAgentSchedules();
        }
    }
}
