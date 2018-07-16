using NorthOps.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorthOps.Models;

namespace NorthOps.Services.SchedulerService
{
    public class AgentSchedulerServices : ISchedulerServices
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        public AgentSchedulerServices(SchedulerServiceModel schedulerServiceModel)
        {
            this.SchedulerServiceModel = schedulerServiceModel;
        }

        public SchedulerServiceModel SchedulerServiceModel { get; set; }

        public void SetAgentSchedules()
        {
            var unitOfWork = new UnitOfWork();
            foreach (var i in unitOfWork.UserRepository.Fetch(includeProperties: "RestDays"))
            {
                for (var d = 1; d <= 7; d++)
                {

                    if (i.RestDays.Count() < 2)
                    {
                        if (unitOfWork.RestDaysRepo.Get().Count(n => n.RestDate == Convert.ToDateTime($"01-{d}-2018")) <= 3)
                        {
                            unitOfWork.RestDaysRepo.Insert(new Models.RestDays()
                            {

                                UserId = i.Id,
                                RestDate = Convert.ToDateTime($"01-{d}-2018")

                            });
                            unitOfWork.Save();
                        }
                        else
                        if (i.RestDays.All(m => m.RestDate != Convert.ToDateTime($"01-{d}-2018")))
                        {
                            unitOfWork.SchedulesRepo.Insert(new Models.Schedules()
                            {
                                ScheduleDate = Convert.ToDateTime($"01-{d}-2018"),
                                UserId = i.Id
                            });
                            unitOfWork.Save();
                        }
                    }
                    else
                    {
                        if (i.RestDays.All(m => m.RestDate != Convert.ToDateTime($"01-{d}-2018")))
                        {
                            unitOfWork.SchedulesRepo.Insert(new Models.Schedules()
                            {
                                ScheduleDate = Convert.ToDateTime($"01-{d}-2018"),
                                UserId = i.Id
                            });
                            unitOfWork.Save();
                        }

                    }



                }
            }
        }

        protected void SetCountOfDaysHasDayOff()
        {
            
        }

        protected int EmployeesInCampaign
        {
            get
            {
                var retval = unitOfWork.UsersInCampaignShiftRepo.Fetch(m =>
                        m.ShiftId == this.SchedulerServiceModel.ShiftId &&
                                    m.CampaignId == this.SchedulerServiceModel.CampaignId).Count();

                return retval;
            }
        }

        
    }

    public class DateDayOff
    {
        public DateTime DayOffDate { get; set; }
        public int Count { get; set; }
    }

    public class SchedulerServiceModel
    {
        public DateTime To { get; set; }
        public DateTime From { get; set; }
        public string ShiftId { get; set; }
        public string CampaignId { get; set; }
        
    }
}
