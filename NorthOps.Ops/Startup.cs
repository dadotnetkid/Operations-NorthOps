using Microsoft.Owin;
using NorthOps.AspIdentity;
using Owin;

[assembly: OwinStartupAttribute(typeof(NorthOps.Ops.Startup))]
namespace NorthOps.Ops
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            IdentityStartup.ConfigureAuth(app);
        }
    }
}