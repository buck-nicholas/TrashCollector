using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TrashCollectorWebApp.Startup))]
namespace TrashCollectorWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
