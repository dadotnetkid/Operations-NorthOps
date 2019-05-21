using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using ZKTeco.SDK.MachineManager;
using ZKTeco.SDK.Model;
using Microsoft.AspNet.OData.Extensions;
[assembly: OwinStartup(typeof(Northops.WebApi.Startup))]

namespace Northops.WebApi
{
    public partial class Startup
    {

        public static Random random = new Random(10000000);
        public static STDDevComm devComm;
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public static GoogleOAuth2AuthenticationOptions googleAuthOptions { get; private set; }
        public static FacebookAuthenticationOptions facebookAuthOptions { get; private set; }
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            InitializeStdDevComm();

            
            /* app.Use(async (context, next) =>
             {
                 if (context.Request.QueryString.HasValue)
                 {
                     if (string.IsNullOrWhiteSpace(context.Request.Headers.Get("Authorization")))
                     {
                         var queryString = HttpUtility.ParseQueryString(context.Request.QueryString.Value);
                         string token = queryString.Get("access_token");

                         if (!string.IsNullOrWhiteSpace(token))
                         {
                             HttpContext.Current.Request.Headers.Add("Authorization", "Bearer " + token);
                         }
                     }
                 }
                 await next.Invoke();
             });*/
        }
        void InitializeStdDevComm()
        {
            devComm = new STDDevComm(new Machines(ip: "10.10.20.50"));
            devComm.Connect();

        }
    }
}
