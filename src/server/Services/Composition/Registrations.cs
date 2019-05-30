using Microsoft.Extensions.DependencyInjection;
using MyTeam.Services.Application;
using MyTeam.Services.Domain;
using MyTeam.Settings;
using Services.Utils;

namespace MyTeam.Services.Composition
{
    public static class Registrations
    {
        public static void RegisterDependencies(this IServiceCollection services)
        {
            services.AddTransient<IPlayerService, PlayerService>();
            services.AddTransient<IEventService, EventService>();
            services.AddTransient<ICacheHelper, CacheHelper>();
            services.AddTransient<Cloudinary>();
            services.AddTransient<EmailSender>();
        }
    }
}
