using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Models
{
    public enum ScheduleType
    {
        Grid,
        Calendar
    }
    public enum DocumentType
    {
        Video = 0,
        PDF = 1,
        Excel = 2,
        Word=3
    }

    public enum Position
    {
        Trainee,
        Probationary,
        Regular
    }

    public enum PayPeriod
    {
        Semi,
        Monthly,
        Weeekly
    }

    public enum EducationAttainment
    {
        Elementary,
        HighSchool,
        College,
        Master,
        TechVoc

    }

    public enum HolidayType
    {
        Regular,
        Special,

    }
    public enum InOutState
    {
        CheckIn = 10,
        CheckOut = 11
    }
}
