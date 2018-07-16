using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKTeco.SDK.Model
{
    public partial class ObjUser
    {
        public ObjUser()
        {
            Transactions = new List<Transactions>();
        }
        // Properties
        public string CardNo { get; set; }

        public string EndTime { get; set; }

        public string Group { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string Pin { get; set; }

        public int Privilege { get; set; }

        public string StartTime { get; set; }

        public int TemplateCount { get; set; }
        public List<Transactions> Transactions { get; set; }
    }


}
