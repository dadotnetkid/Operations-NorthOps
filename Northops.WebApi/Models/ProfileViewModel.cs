using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Northops.WebApi.Models
{
    public class ProfileViewModel
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Skills { get; set; }
        public string Cellular { get; set; }
        public string Email { get; set; }
    }
}