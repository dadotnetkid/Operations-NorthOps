using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Models.ViewModels
{
    public class DailyTimeRecordViewModel
    {
        private IList<Overtimes> _overtimes;
        private IList<Schedules> _schedules;
        private IList<DailyTimeRecords> _dailyTimeRecords;

        public IList<Overtimes> Overtimes
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

        public IList<Schedules> Schedules
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

        public IList<DailyTimeRecords> DailyTimeRecords
        {
            get => _dailyTimeRecords;
            set => _dailyTimeRecords = value;
        }

        public IList<RestDays> RestDays { get; set; }


        public int Id { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public DateTime? DateLogFrom { get; set; }
        public DateTime? DateLogTo { get; set; }
        public string UserId { get; set; }
        public DateTime? LogDate { get; set; }
        public string DateLog { get; set; }
        public bool isGenerated { get; set; }
    }
}
