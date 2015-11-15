using Microsoft.Framework.DependencyInjection;
using MyTeam.Models.Domain;
using MyTeam.Services.Application;
using MyTeam.Services.Domain;
using MyTeam.Services.Repositories;

namespace MyTeam.Services
{
    public class Registrations
    {
        public static void Setup(IServiceCollection services)
        {
            services.AddTransient<IRepository<Club>, GenericRepository<Club>>();
            services.AddTransient<IRepository<Player>, GenericRepository<Player>>();
            services.AddTransient<IRepository<Event>, GenericRepository<Event>>();
            services.AddTransient<IRepository<EventAttendance>, GenericRepository<EventAttendance>>();
            services.AddTransient<IRepository<Season>, GenericRepository<Season>>();
            services.AddTransient<IRepository<Table>, GenericRepository<Table>>();
            services.AddTransient<IPlayerService, PlayerService>();

            services.AddTransient<IEventService, EventService>();
            services.AddTransient<ISeasonService, SeasonService>();
            services.AddTransient<IStatsService, StatsService>();
            services.AddTransient<ITableService, TableService>();
            services.AddTransient<ICacheHelper, CacheHelper>();
            services.AddTransient<ILogger, Logger>();

            services.AddSingleton<TestRepository>();
            
         
        }
    }

    
}
