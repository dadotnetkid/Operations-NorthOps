using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NorthOps.Portal.Models
{
    public partial class User
    {
        [MinLength(8,ErrorMessage ="Required Password Length must 8 digits")]
        public string Password { get; set; }
    }
    public enum  Gender
    {
        Male,
        Female
    }
    public enum CivilStatus
    {
        Single,
        Married,
        Widowed,
        Separated

    }
}