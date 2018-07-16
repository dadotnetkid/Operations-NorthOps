using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Models
{
    public partial class Schedules
    {
        private static Random random;

        public Schedules()
        {
            if (random == null)
                random = new Random(9);
        }

        public DateTime StartFrom
        {
            get
            {
                var res = this.ScheduleDate?.ToString("MM-dd-yyyy ") + this.Users?.UsersInCampaignShift.FirstOrDefault()
                              ?.Shifts.TimeIn;
                return Convert.ToDateTime(res);
            }
        }
        public DateTime EndTo
        {
            get
            {
                var res = this.ScheduleDate?.ToString("MM-dd-yyyy ") + this.Users?.UsersInCampaignShift.FirstOrDefault()
                              ?.Shifts.TimeIn;
                return Convert.ToDateTime(res);
            }
        }
        public string Description
        {
            get
            {
                return "Test Description";
            }
        }
        public int Label
        {
            get
            {
                var lbl = random.Next(9);
                return lbl;
            }
        }
        public Guid AppointmentId { get { return Guid.Parse(this.Id); } }
        public int AppointmentType { get { return 0; } }
        public string FullName { get { return this.Users.FullName; } }
    }
}
