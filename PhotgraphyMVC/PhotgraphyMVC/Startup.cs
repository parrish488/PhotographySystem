using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PhotgraphyMVC.Startup))]
namespace PhotgraphyMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
