using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NorthOps.Services.SmsService.Device
{
    public class InformationData : ResponseData
    {
        public InformationData(DateTime acquisitionTime, XElement xe) : base(acquisitionTime, xe) { }

        public string Imei => (string)GetValue(nameof(Imei));
    }
}
