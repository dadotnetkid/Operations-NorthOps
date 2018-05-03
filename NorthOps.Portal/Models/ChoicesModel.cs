using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NorthOps.Portal.Models
{
    public partial class Exam
    {
        public Dictionary<string, string> Answer { get { return _Answer; } set { value = _Answer; } }
        private Dictionary<string, string> _Answer = new Dictionary<string, string>();
        
    }
}