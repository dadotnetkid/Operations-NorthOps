using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NorthOps.Portal.Startup))]
namespace NorthOps.Portal {
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}