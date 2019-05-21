using Hangfire;
using Microsoft.Owin;
using NorthOps.Models.Config;
using NorthOps.Services.Helpers;
using Owin;

[assembly: OwinStartupAttribute(typeof(NorthOps.Portal.Startup))]
namespace NorthOps.Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
          
            IdentityStartup.ConfigureAuth(app);

            GlobalConfiguration.Configuration
                .UseSqlServerStorage("Data Source=10.10.20.42;Initial Catalog=northops;Persist Security Info=True;User ID=sa;Password=n0rth@dm1N;MultipleActiveResultSets=True");

            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new[] { new HangfireAuthorization() }
            });
            app.UseHangfireServer();
        }
    }
}