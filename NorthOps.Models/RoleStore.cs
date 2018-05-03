using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace NorthOps.Models
{
    public class RoleStore : IQueryableRoleStore<UserRoles, string>
    {
        private readonly northopsEntities db;

        public RoleStore(northopsEntities db)
        {
            this.db = db;
        }

        //// IQueryableRoleStore<UserRole, TKey>

        public IQueryable<UserRoles> Roles
        {
            get { return this.db.UserRoles; }
        }

        //// IRoleStore<UserRole, TKey>

        public virtual Task CreateAsync(UserRoles role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            this.db.UserRoles.Add(role);
            return this.db.SaveChangesAsync();
        }

        public Task DeleteAsync(UserRoles role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            this.db.UserRoles.Remove(role);
            return this.db.SaveChangesAsync();
        }

        public Task<UserRoles> FindByIdAsync(string roleId)
        {
            return this.db.UserRoles.FindAsync(new[] { roleId });
        }

        public Task<UserRoles> FindByNameAsync(string roleName)
        {
            return this.db.UserRoles.FirstOrDefaultAsync(r => r.Name == roleName);
        }

        public Task UpdateAsync(UserRoles role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            this.db.Entry(role).State = EntityState.Modified;
            return this.db.SaveChangesAsync();
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
