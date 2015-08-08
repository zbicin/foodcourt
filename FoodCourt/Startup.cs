using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FoodCourt.Startup))]
namespace FoodCourt
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
