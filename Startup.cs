using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AirsoftApp.Startup))]
namespace AirsoftApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
