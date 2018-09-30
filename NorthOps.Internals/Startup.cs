using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NorthOps.Internals.Startup))]
namespace NorthOps.Internals
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AspIdentity.IdentityStartup.ConfigureADAuth(app);
        }
    }
}
