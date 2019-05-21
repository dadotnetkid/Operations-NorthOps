using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Models
{
    public partial class Breaks
    {
        public string TotalTime
        {
            get
            {
                var total = this.EndTime - this.StartTime;
                var time = "";
                if (total?.Minutes >= 60)
                {
                    time = total?.Minutes / 60 + " hr(s)";
                }
                else
                {
                    time = total?.Minutes + " min(s)";
                }
                return time;
            }
        }


    }
}
