using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using NorthOps.Models.Config;
using NorthOps.Services.AttendanceService;
using NorthOps.Services.Helpers;
using Owin;
using ZKTeco.SDK.MachineManager;
using ZKTeco.SDK.Model;

[assembly: OwinStartupAttribute(typeof(NorthOps.Ops.Startup))]
namespace NorthOps.Ops
{
    public partial class Startup
    {
        public static STDDevComm devComm;

        public void Configuration(IAppBuilder app)
        {
            IdentityStartup.ConfigureAuth(app);
            UseWebApi(app);

            UseHangfire(app);
            InitializeSTDDevComm();
            UseHttpConfiguration();

        }
        public void UseHangfire(IAppBuilder app)
        {

            Hangfire.GlobalConfiguration.Configuration
                .UseSqlServerStorage("Data Source=web;Initial Catalog=northops;Persist Security Info=True;User ID=sa;Password=n0rth@dm1N;MultipleActiveResultSets=True");

            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new[] { new HangfireAuthorization() }
            });
            RecurringJob.AddOrUpdate(() => SaveAttendanceDaily(), Cron.Daily(0));
            RecurringJob.AddOrUpdate(() => GetDailyTimeRecordsDaily(), Cron.Daily(0));
            app.UseHangfireServer();
        }

        void InitializeSTDDevComm()
        {
            devComm = new STDDevComm(new Machines(ip: "10.10.20.50"));
            devComm.Connect();

        }

        void UseHttpConfiguration()
        {
            System.Web.Http.HttpConfiguration config = System.Web.Http.GlobalConfiguration.Configuration;

            config.Formatters.JsonFormatter
                        .SerializerSettings
                        .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }
        void UseWebApi(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions options = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(60),
                Provider = new ApplicationOAuthProvider("self"),
            };
            app.UseOAuthAuthorizationServer(options);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
        public async Task SaveAttendanceDaily()
        {
            await new AttendanceServices().SaveAttendanceLogDaily();
            STDDevComm devComm = new STDDevComm(new Machines(ip: "10.10.20.50"));
            devComm.ClearGLog();
        }

        public void GetDailyTimeRecordsDaily()
        {
            new AttendanceServices().GetDailyTimeRecordsDaily();
        }
    }
}