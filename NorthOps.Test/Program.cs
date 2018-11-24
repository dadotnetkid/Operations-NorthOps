using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorthOps.Models.ViewModels;
using NorthOps.Services.AttendanceService;
using NorthOps.Services.DTRService;

namespace NorthOps.Test
{
    class Program
    {
        static  void Main(string[] args)
        {
            AttendanceServices attendanceServices = new AttendanceServices();
            var dateFrom = DateTime.Now.AddDays(-10);
         //   var res = attendanceServices.GetDtrReport(new DtrReportViewModel() { UserId = "b925d019-a3f6-4c67-ab0c-fb6e80e258f6", DateLogFrom = dateFrom, DateLogTo = DateTime.Now });
         //   Console.Write(res);
            Console.ReadKey();
        }
    }
}
