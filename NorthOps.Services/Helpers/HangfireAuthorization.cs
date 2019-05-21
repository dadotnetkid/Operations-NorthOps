using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Hangfire.Dashboard;

namespace NorthOps.Services.Helpers
{
    public class HangfireAuthorization : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }
    }
}
