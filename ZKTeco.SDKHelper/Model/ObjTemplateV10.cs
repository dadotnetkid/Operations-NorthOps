using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKTeco.SDK.Model
{
    public class ObjTemplateV10
    {
        public int Size { get; set; }

        public string UID { get; set; }

        public string Pin { get; set; }

        public FingerType FingerID { get; set; }

        public ValidType Valid { get; set; }

        public string Template { get; set; }

        public string Resverd { get; set; }

        public string EndTag { get; set; }
    }
}
