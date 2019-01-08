using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Models.ViewModels
{
    public class AcknowledgeTokenViewModel
    {
        public DateTime AcknowledgeDate { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public object AcknowledgeBy { get; set; }
        public object ModifiedBy { get; set; }
        public DateTime DateModified { get; set; }
    }
}
