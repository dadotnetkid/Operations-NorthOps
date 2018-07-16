﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Services.Helpers
{
   public static class DateTimeHelper
    {
        public static int MondaysInMonth(this DateTime time)
        {
            //extract the month
            int daysInMonth = DateTime.DaysInMonth(time.Year, time.Month);
            var firstOfMonth = new DateTime(time.Year, time.Month, 1);
            //days of week starts by default as Sunday = 0
            var firstDayOfMonth = (int)firstOfMonth.DayOfWeek;
            var weeksInMonth = (int)Math.Ceiling((firstDayOfMonth + daysInMonth) / 7.0);

            return weeksInMonth;
        }
    }
}
