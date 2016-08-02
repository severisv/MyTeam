using Microsoft.Extensions.DependencyInjection;
using MyTeam.Services.Application;
using MyTeam.Services.Domain;
using MyTeam.Settings;
using ILogger = MyTeam.Services.Application.ILogger;

namespace MyTeam.Services.Composition
{
    public static class Registrations
    {
        public static void RegisterDependencies(this IServiceCollection services)
        {
            services.AddTransient<IPlayerService, PlayerService>();
            services.AddTransient<IArticleService, ArticleService>();
            services.AddTransient<IRemedyRateService, RemedyRateService>();
            services.AddTransient<IFineService, FineService>();
            services.AddTransient<IFixtureService, FixtureService>();
            services.AddTransient<IEventService, EventService>();
            services.AddTransient<IGameService, GameService>();
            services.AddTransient<IGameEventService, GameEventService>();
            services.AddTransient<ISeasonService, SeasonService>();
            services.AddTransient<IStatsService, StatsService>();
            services.AddTransient<ITableService, TableService>();
            services.AddTransient<ICacheHelper, CacheHelper>();
            services.AddTransient<SlackService, SlackService>();
            services.AddTransient<ILogger, Logger>();
            services.AddTransient<ICloudinary, Cloudinary>();

            services.AddTransient<IEmailSender, AuthMessageSender>();



        }
    }

    
}
