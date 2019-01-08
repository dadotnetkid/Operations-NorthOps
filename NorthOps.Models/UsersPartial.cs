using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using NorthOps.Models.Repository;
using NorthOps.Models.ViewModels;

namespace NorthOps.Models
{
    public partial class Users : IUser<string>
    {
        private List<DailyTimeRecordViewModel> _dtrReportViewModels;

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<Users, string> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public string FullName
        {
            get
            {
                string dspFirstName = string.IsNullOrWhiteSpace(this.FirstName) ? "" : this.FirstName;
                string dspLastName = string.IsNullOrWhiteSpace(this.LastName) ? "" : this.LastName;

                return string.Format("{0} {1}", dspFirstName, dspLastName);
            }
        }

        public string Password { get; set; }
        public string MemberRoles { get { return string.Join(Environment.NewLine, this.UserRoles.Select(x => x.Name)); } }
        public IEnumerable<UserRoles> Rolelist { get { return new UnitOfWork().RoleRepository.Get(); } }
        public string userRole { get; set; }

        //public int? BiometricId => new UnitOfWork().BiometricsRepo.Fetch(m => m.UserId == this.Id).FirstOrDefault()?.BiometricId;
        //public int _BiometricId { get; set; }

        public List<DailyTimeRecordViewModel> DtrReportViewModels
        {
            get
            {
                if (_dtrReportViewModels == null)
                    _dtrReportViewModels = new List<DailyTimeRecordViewModel>();
                return _dtrReportViewModels;
            }
            set => _dtrReportViewModels = value;
        }
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
}
