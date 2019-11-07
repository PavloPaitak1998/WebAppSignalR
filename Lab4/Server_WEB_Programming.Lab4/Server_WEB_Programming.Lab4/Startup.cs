using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Server_WEB_Programming.Lab4.Startup))]

namespace Server_WEB_Programming.Lab4
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();

            ConfigureAuth(app);
        }
    }
}
