using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using NorthOps.Models;

namespace NorthOps.Internals.Helpers
{
    public class ActiveDirectoryHelpers
    {
        private string ActiveDirectoryPath;
        public ActiveDirectoryHelpers()
        {

        }

        public ActiveDirectoryHelpers(string ActiveDirectoryPath)
        {
            this.ActiveDirectoryPath = ActiveDirectoryPath;
        }
        public Task CreateUser(Users models)
        {

            using (var pc = new PrincipalContext(ContextType.Domain, "northops.local", "OU=Agents,DC=northops,DC=local", @"northops\Administrator", "n0rth@dm1N"))
            {
               
                using (var up = new UserPrincipal(pc))
                {
                    up.PasswordNeverExpires = false;
                    up.SamAccountName = models.UserName;
                    up.EmailAddress = models.Email;
                    up.SetPassword(models.Password);
                    up.Enabled = true;
                    up.ExpirePasswordNow();
                    up.Save();
                }
            }
            return null;
        }

        public Task Create(Users model)
        {
            DirectoryEntry ouEntry = new DirectoryEntry($"LDAP://{ActiveDirectoryPath}", @"northops\northadmin", "n0rth@dm1N", AuthenticationTypes.Secure);
            try
            {

                DirectoryEntry childEntry = ouEntry.Children.Add($"CN={model.UserName}", "Users");
                childEntry.CommitChanges();
                ouEntry.CommitChanges();
                childEntry.Invoke("SetPassword", new object[] { model.Password });
                childEntry.CommitChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }
    }
}