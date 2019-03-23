using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVC_LoginAdmin.Startup))]
namespace MVC_LoginAdmin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
