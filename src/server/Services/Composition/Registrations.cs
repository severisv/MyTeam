using Microsoft.Extensions.DependencyInjection;
using MyTeam.Services.Application;
using MyTeam.Services.Domain;
using MyTeam.Settings;

namespace MyTeam.Services.Composition
{
    public static class Registrations
    {
        public static void RegisterDependencies(this IServiceCollection services)
        {
            services.AddTransient<IPlayerService, PlayerService>();
            services.AddTransient<IRemedyRateService, RemedyRateService>();
            services.AddTransient<IFineService, FineService>();
            services.AddTransient<IFixtureService, FixtureService>();
            services.AddTransient<IEventService, EventService>();
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<ISeasonService, SeasonService>();
            services.AddTransient<ITableService, TableService>();
            services.AddTransient<ICacheHelper, CacheHelper>();
            services.AddTransient<Cloudinary>();
            services.AddTransient<IEmailSender, AuthMessageSender>();
        }
    }
}
