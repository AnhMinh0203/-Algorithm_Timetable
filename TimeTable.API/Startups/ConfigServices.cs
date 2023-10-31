using TimeTable.Respository.Configs;

namespace TimeTable.API.Startups
{
    public static class ConfigServices
    {
        public static void AddCustomService(this IServiceCollection services, IConfiguration configuration)
        {
            services.DependencyInjectionRepository(configuration);
        }
    }
}
