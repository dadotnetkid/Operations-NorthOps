using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Models
{
    public partial class NotificationsTemplates
    {
        public string Status => this.isActive == true ?"Active":"Not Active";
    }

    public enum NotificationType
    {
        Resume,
        PhoneInterview,
        PersonalInterview,
        Training,
        OnBoarding,
        Contract,
        IsPersonalInterviewPassed,
        IsPersonalInterviewFailed,
        IsExamPassed,
        IsExamFailed,
        IsPhoneInterviewPassed,
        IsPhoneInterviewFailed
    }
}
