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
            services.AddTransient<IRemedyRateService, RemedyRateService>();
            services.AddTransient<IFineService, FineService>();
            services.AddTransient<IEventService, EventService>();
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<ICacheHelper, CacheHelper>();
            services.AddTransient<Cloudinary>();
            services.AddTransient<EmailSender>();
        }
    }
}
