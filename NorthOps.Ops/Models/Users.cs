using NorthOps.Ops.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NorthOps.Ops.Models
{
    public partial class User
    {
        
        public string Password { get; set; }
        public string MemberRoles { get { return string.Join(Environment.NewLine, this.UserRoles.Select(x => x.Name)); } }
        public IEnumerable<UserRole> Rolelist { get { return new UnitOfWork().RoleRepository.Get(); } }
        public string userRole { get; set; }
     
    }

    public enum Gender
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
    public class UserMetaData
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string MiddleName { get; set; }
        [Required]
        public string CivilStatus { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public Nullable<System.DateTime> BirthDate { get; set; }
        public string AddressLine2 { get; set; }
        [Required]
        public string AddressLine1 { get; set; }
        [Required]
        public Nullable<int> TownCity { get; set; }
        [Required]
        public string Cellular { get; set; }
        [Required]
        public string Religion { get; set; }
        [Required]
        public string Citizenship { get; set; }
        [Required]
        public string Languages { get; set; }
        [Required]
        public string Skills { get; set; }
    }
}
