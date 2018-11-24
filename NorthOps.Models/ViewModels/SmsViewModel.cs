using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Models.ViewModels
{
    public class SmsViewModel
    {
        public string XmlPassword { get; set; }
        public string SessionId { get; set; }
        public string VerificationToken { get; set; }
        public string Rsa { get; set; }
    }
}
