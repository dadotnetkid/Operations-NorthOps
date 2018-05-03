using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using NorthOps.Models.Properties;

namespace NorthOps.Models
{
    public partial class UserStore :
     IQueryableUserStore<Users>, IUserPasswordStore<Users>, IUserLoginStore<Users, string>,
     IUserClaimStore<Users, string>, IUserRoleStore<Users, string>, IUserSecurityStampStore<Users, string>,
     IUserEmailStore<Users, string>, IUserPhoneNumberStore<Users, string>, IUserTwoFactorStore<Users, string>,
     IUserLockoutStore<Users, string>
    {
        private readonly northopsEntities db;

        public UserStore(northopsEntities db)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            this.db = db;
        }

        //// IQueryableUserStore<User, int>

        public IQueryable<Users> Users
        {
            get { return this.db.Users; }
        }

        //// IUserStore<User, Key>

        public Task CreateAsync(Users user)
        {
            this.db.Users.Add(user);
            return this.db.SaveChangesAsync();
        }

        public Task DeleteAsync(Users user)
        {
            this.db.Users.Remove(user);
            return this.db.SaveChangesAsync();
        }

        public Task<Users> FindByIdAsync(string userId)
        {
            return this.db.Users
                .Include(u => u.UserLogins).Include(u => u.UserRoles).Include(u => u.UserClaims)
                .FirstOrDefaultAsync(u => u.Id.Equals(userId));
        }

        public Task<Users> FindByNameAsync(string userName)
        {
            return this.db.Users
                .Include(u => u.UserLogins).Include(u => u.UserRoles).Include(u => u.UserClaims)
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public Task UpdateAsync(Users user)
        {
            this.db.Entry<Users>(user).State = EntityState.Modified;
            return this.db.SaveChangesAsync();
        }

        //// IUserPasswordStore<User, Key>

        public Task<string> GetPasswordHashAsync(Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(Users user)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetPasswordHashAsync(Users user, string passwordHash)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        //// IUserLoginStore<User, Key>

        public Task AddLoginAsync(Users user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var userLogin = Activator.CreateInstance<UserLogins>();
            userLogin.UserId = user.Id;
            userLogin.LoginProvider = login.LoginProvider;
            userLogin.ProviderKey = login.ProviderKey;
            user.UserLogins.Add(userLogin);
            return Task.FromResult(0);
        }

        public async Task<Users> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var provider = login.LoginProvider;
            var key = login.ProviderKey;

            var userLogin = await this.db.UserLogins.FirstOrDefaultAsync(l => l.LoginProvider == provider && l.ProviderKey == key);

            if (userLogin == null)
            {
                return default(Users);
            }

            return await this.db.Users
                .Include(u => u.UserLogins).Include(u => u.UserRoles).Include(u => u.UserClaims)
                .FirstOrDefaultAsync(u => u.Id.Equals(userLogin.UserId));
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<IList<UserLoginInfo>>(user.UserLogins.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey)).ToList());
        }

        public Task RemoveLoginAsync(Users user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var provider = login.LoginProvider;
            var key = login.ProviderKey;

            var item = user.UserLogins.SingleOrDefault(l => l.LoginProvider == provider && l.ProviderKey == key);

            if (item != null)
            {
                user.UserLogins.Remove(item);
            }

            return Task.FromResult(0);
        }

        //// IUserClaimStore<User, int>

        public Task AddClaimAsync(Users user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            var item = Activator.CreateInstance<UserClaims>();
            item.UserId = user.Id;
            item.ClaimType = claim.Type;
            item.ClaimValue = claim.Value;
            user.UserClaims.Add(item);
            return Task.FromResult(0);
        }

        public Task<IList<Claim>> GetClaimsAsync(Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<IList<Claim>>(user.UserClaims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList());
        }

        public Task RemoveClaimAsync(Users user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            foreach (var item in user.UserClaims.Where(uc => uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type).ToList())
            {
                user.UserClaims.Remove(item);
            }

            foreach (var item in this.db.UserClaims.Where(uc => uc.UserId.Equals(user.Id) && uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type).ToList())
            {
                this.db.UserClaims.Remove(item);
            }

            return Task.FromResult(0);
        }

        //// IUserRoleStore<User, int>

        public Task AddToRoleAsync(Users user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException(Resources.ValueCannotBeNullOrEmpty, "roleName");
            }

            var userRole = this.db.UserRoles.SingleOrDefault(r => r.Name == roleName);

            if (userRole == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.RoleNotFound, new object[] { roleName }));
            }

            user.UserRoles.Add(userRole);
            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRolesAsync(Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<IList<string>>(user.UserRoles.Join(this.db.UserRoles, ur => ur.Id, r => r.Id, (ur, r) => r.Name).ToList());
        }

        public Task<bool> IsInRoleAsync(Users user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException(Resources.ValueCannotBeNullOrEmpty, "roleName");
            }

            return
                Task.FromResult<bool>(
                    this.db.UserRoles.Any(r => r.Name == roleName && r.Users.Any(u => u.Id.Equals(user.Id))));
        }

        public Task RemoveFromRoleAsync(Users user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException(Resources.ValueCannotBeNullOrEmpty, "roleName");
            }

            var userRole = user.UserRoles.SingleOrDefault(r => r.Name == roleName);

            if (userRole != null)
            {
                user.UserRoles.Remove(userRole);
            }

            return Task.FromResult(0);
        }

        //// IUserSecurityStampStore<User, int>

        public Task<string> GetSecurityStampAsync(Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetSecurityStampAsync(Users user, string stamp)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        //// IUserEmailStore<User, int>

        public Task<Users> FindByEmailAsync(string email)
        {
            return this.db.Users
                .Include(u => u.UserLogins).Include(u => u.UserRoles).Include(u => u.UserClaims)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<string> GetEmailAsync(Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailAsync(Users user, string email)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.Email = email;
            return Task.FromResult(0);
        }

        public Task SetEmailConfirmedAsync(Users user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        //// IUserPhoneNumberStore<User, int>

        public Task<string> GetPhoneNumberAsync(Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberAsync(Users user, string phoneNumber)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task SetPhoneNumberConfirmedAsync(Users user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        //// IUserTwoFactorStore<User, int>

        public Task<bool> GetTwoFactorEnabledAsync(Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task SetTwoFactorEnabledAsync(Users user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        //// IUserLockoutStore<User, int>

        public Task<int> GetAccessFailedCountAsync(Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(
                user.LockoutEndDateUtc.HasValue ?
                    new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc)) :
                    new DateTimeOffset());
        }

        public Task<int> IncrementAccessFailedCountAsync(Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public Task SetLockoutEnabledAsync(Users user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task SetLockoutEndDateAsync(Users user, DateTimeOffset lockoutEnd)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.LockoutEndDateUtc = lockoutEnd == DateTimeOffset.MinValue ? null : new DateTime?(lockoutEnd.UtcDateTime);
            return Task.FromResult(0);
        }

        //// IDisposable

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.db != null)
            {
                this.db.Dispose();
            }
        }


    }
}
