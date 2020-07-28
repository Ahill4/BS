using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BakerySquared.Startup))]
namespace BakerySquared
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
