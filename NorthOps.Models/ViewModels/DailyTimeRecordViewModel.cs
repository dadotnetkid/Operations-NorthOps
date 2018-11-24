using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Models.ViewModels
{
    public class DailyTimeRecordViewModel
    {
        private List<Overtimes> _overtimes;
        private List<Schedules> _schedules;
        private List<DailyTimeRecords> _dailyTimeRecords;
        

        public List<Overtimes> Overtimes
        {
            get
            {
                if (_overtimes == null)
                {
                    _overtimes = new List<Overtimes>();
                }
                return _overtimes;
            }

            set => _overtimes = value;
        }

        public List<Schedules> Schedules
        {
            get
            {
                if (_schedules == null)
                {
                    _schedules = new List<Schedules>();
                }
                return _schedules;
            }

            set => _schedules = value;
        }

        public Users Users { get; set; }

        public List<DailyTimeRecords> DailyTimeRecords
        {
            get => _dailyTimeRecords;
            set => _dailyTimeRecords = value;
        }
    }
}
