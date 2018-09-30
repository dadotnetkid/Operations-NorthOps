using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace NorthOps.Internals.Helpers
{
    public class ActiveDirectoryHelpers
    {
        public DirectoryEntry directoryEntry;
        public GroupPrincipal groupPrincipal { get; set; }
        public string Container;
        public Domain DomainName { get; set; }

        public ActiveDirectoryHelpers()
        {

        }

        public ActiveDirectoryHelpers(string container)
        {
            this.Container = container;
        }
        public Task CreateUser(Users models, object principal)
        {
            try
            {
                using (var pc = new PrincipalContext(ContextType.Domain, DomainName.Name, Container, @"northops\northadmin", "n0rth@dm1N"))
                {
                    using (var up = new UserPrincipal(pc))
                    {
                        up.PasswordNeverExpires = false;

                        up.EmailAddress = models.Email;
                        up.SetPassword("Old" + models.Password);
                        up.GivenName = models.FirstName;
                        up.MiddleName = models.MiddleName;
                        up.Surname = models.LastName;
                        up.Description = "Created at C#";
                        up.Name = models.UserName;
                        up.SamAccountName = models.UserName;
                        up.UserPrincipalName = models.UserName;
                        up.UserCannotChangePassword = false;
                        up.PasswordNeverExpires = false;
                        up.ExpirePasswordNow();
                        up.Enabled = true;

                        up.Save();
                        up.ChangePassword("Old" + models.Password, models.Password);
                        up.Save();

                        var p = principal as GroupPrincipal;
                        p.Members.Add(up);
                        p.Save();

                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show(this.DomainName.Name + Environment.NewLine + Container + Environment.NewLine + e);
            }

            return null;
        }

        public Task Create(Users model)
        {
            DirectoryEntry ouEntry = new DirectoryEntry($"LDAP://{Container}", @"northops\administrator", "n0rth@dm1N", AuthenticationTypes.Secure);
            try
            {

                DirectoryEntry childEntry = ouEntry.Children.Add($"CN={model.UserName}", "User");
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
        public SearchResultCollection Containers()
        {
            DirectoryEntry ouEntry = new DirectoryEntry($"LDAP://{DomainName.GetDirectoryEntry().Properties["distinguishedName"].Value}", @"northops\northadmin", "n0rth@dm1N", AuthenticationTypes.Secure);
            return new DirectorySearcher(ouEntry).FindAll();
        }

        public DomainCollection DomainNames()
        {
            using (var forest = Forest.GetCurrentForest())
            {
                return forest.Domains;
            }
        }

        public PrincipalSearchResult<Principal> Groups()
        {
            var pc = new PrincipalContext(ContextType.Domain, DomainName.Name, @"northops\northadmin",
                "n0rth@dm1N");
            return new PrincipalSearcher(new GroupPrincipal(pc)).FindAll();
        }

    }

    public class Users
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
    }

    public enum ActiveDirectoryListof
    {
        ListofDomainNames,
        ListofContainers


    }
}