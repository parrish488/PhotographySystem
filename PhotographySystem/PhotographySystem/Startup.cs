using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MvcTODO.Startup))]
namespace MvcTODO
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

        }
    }
}
