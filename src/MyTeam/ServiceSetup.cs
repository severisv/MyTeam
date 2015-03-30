using Microsoft.Framework.DependencyInjection;
using MyTeam.Models.Domain;
using MyTeam.Services.Repositories;

namespace MyTeam
{
    public class ServiceSetup
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<IRepository<Player>, GenericRepository<Player>>();
            services.AddSingleton<TestRepository>();
        }
    }
}