using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Models
{
    public partial class Leaves
    {
        public string Status => this.isAdminApproved == null ? "" : this.isAdminApproved == true ? "Approved" : "Not Approved";
        public int? NumberOfDay => Convert.ToInt32((this.DateTo - this.DateFrom)?.TotalDays);
    }
}
