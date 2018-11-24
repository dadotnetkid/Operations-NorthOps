using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorthOps.Models.Repository;

namespace NorthOps.Models
{
    public partial class Overtimes
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        public string OvertimeShedule => this.DateFrom?.ToString("MM/dd/yy hh:mm tt") + "-" +
                                         this.DateTo?.ToString("MM/dd/yy hh:mm tt");

        private List<Attendances> attendances
        {
            get
            {
                var _attendances = unitOfWork.AttendancesRepo.Get(m => m.BiometricId == Users.BiometricId).Where(m =>
                      Convert.ToDateTime(m.LogDateTime.ToShortDateString()) >=
                      Convert.ToDateTime(this.DateFrom?.ToShortDateString() )
                      &&
                      Convert.ToDateTime(m.LogDateTime.ToShortDateString()) <=
                      Convert.ToDateTime(this.DateTo?.ToShortDateString() ) );
                return _attendances.ToList();
            }
        }
        public DateTime? TimeIn
        {
            get { return attendances.OrderBy(m => m.LogDateTime).FirstOrDefault(m=>(InOutState)m.InOutState == InOutState.CheckIn)?.LogDateTime; }
        }
        public DateTime? TimeOut
        {
            get { return attendances.OrderByDescending(m => m.LogDateTime).FirstOrDefault(m => (InOutState)m.InOutState == InOutState.CheckOut)?.LogDateTime; }
        }
    }
}
