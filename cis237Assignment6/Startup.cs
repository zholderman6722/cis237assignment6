using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(cis237Assignment6.Startup))]
namespace cis237Assignment6
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
