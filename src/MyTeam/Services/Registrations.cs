using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;
using MyTeam.Models.Domain;
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
            services.AddTransient<IRepository<Training>, GenericRepository<Training>>();
            services.AddTransient<IRepository<Game>, GenericRepository<Game>>();
            services.AddTransient<IRepository<CustomEvent>, GenericRepository<CustomEvent>>();

            services.AddTransient<IEventService<Event>, EventService<Event>>();
            services.AddTransient<IEventService<Training>, EventService<Training>>();
            services.AddTransient<IEventService<Game>, EventService<Game>>();
            services.AddTransient<IEventService<CustomEvent>, EventService<CustomEvent>>();

            services.AddSingleton<TestRepository>();


        }
    }

    
}
