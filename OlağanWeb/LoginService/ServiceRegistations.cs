using Microsoft.IdentityModel.Tokens;
using OlağanWeb.Methods;

namespace OlağanWeb.LoginService
{
    public static class ServiceRegistations
    {
        public static void AddLoginServices(this IServiceCollection services)
        {
            services.AddScoped<ILoginControl, LoginControl>();
        }
    }
}
