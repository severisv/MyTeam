using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;


namespace MyTeam.Filters
{
    public static class FilterConfiguration
    {
        public static void ConfigureFilters(this MvcOptions options)
        {
            options.Filters.Add(new LoadTenantDataAttribute());
            options.Filters.Add(new HandleErrorAttribute());
        }
    }
}
