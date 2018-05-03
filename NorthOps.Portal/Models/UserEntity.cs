using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace NorthOps.Portal.Models {
    public partial class User : IUser<string> {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User, string> manager) {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public string FullName {
            get {
                string dspFirstName = string.IsNullOrWhiteSpace(this.FirstName) ? "" : this.FirstName;
                string dspLastName = string.IsNullOrWhiteSpace(this.LastName) ? "" : this.LastName;

                return string.Format("{0} {1}", dspFirstName, dspLastName);
            }
        }
    }

}