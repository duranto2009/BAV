using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BAV.Startup))]
namespace BAV
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
