using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;
using MyTeam.Filters;
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
            services.AddTransient<IRepository<Player>, GenericRepository<Player>>();
            services.AddTransient<IRepository<Event>, GenericRepository<Event>>();
            services.AddTransient<IRepository<EventAttendance>, GenericRepository<EventAttendance>>();

            services.AddTransient<IEventService, EventService>();
            services.AddTransient<IMemoryStore, MemoryStore>();
            services.AddTransient<ILogger, Logger>();

            services.AddSingleton<TestRepository>();
            
         
        }
    }

    
}
