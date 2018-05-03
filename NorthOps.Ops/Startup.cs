using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NorthOps.Ops.Startup))]
namespace NorthOps.Ops {
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}