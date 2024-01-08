using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AIDB.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddAIDBContext(this IServiceCollection services, string conn)
        {
            services.AddDbContext<AIDBMainContext>(options =>
            {
                options.UseSqlite($"Data Source={conn}",
                    x => x.MigrationsAssembly(typeof(AIDBMainContext).Assembly.GetName().Name));
            });

            return services;
        }
    }
}