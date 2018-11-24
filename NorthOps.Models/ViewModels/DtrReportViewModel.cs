using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Models.ViewModels
{
    public class DtrReportViewModel
    {
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
