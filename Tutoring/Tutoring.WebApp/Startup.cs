using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Tutoring.WebApp.Startup))]
namespace Tutoring.WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
