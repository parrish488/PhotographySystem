using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PhotographySystem.Startup))]
namespace PhotographySystem
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
