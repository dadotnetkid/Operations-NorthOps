using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Models
{
    public partial class Schedules
    {
        private string _timeZoneId;
        private static Random random => new Random();

        public int Label => 3;

        public int AppointmentType => 0;
        public string FullName => this.Users.FullName;
        public string Subject => this.ScheduleDateFrom.ToShortTimeString() + " - " + this.ScheduleDateTo.ToShortTimeString();

        public string TimeZoneId
        {
            get
            {
                if (string.IsNullOrEmpty(_timeZoneId))
                {
                    _timeZoneId = "Central Standard Time";
                }
                return _timeZoneId;
            }

            set => _timeZoneId = value;
        }

        public List<TimeZones> TimeZones
        {
            get
            {
                var lst = TimeZoneInfo.GetSystemTimeZones().Select(x => new TimeZones() { Id = x.Id, StandardName = x.DisplayName }).ToList();

                return lst;
            }
        }
    }

    public class TimeZones
    {
        public string Id { get; set; }
        public string StandardName { get; set; }
    }

}
