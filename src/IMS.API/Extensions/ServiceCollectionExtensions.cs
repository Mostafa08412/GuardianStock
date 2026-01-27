using IMS.Application;
using IMS.Infrastructure;
namespace IMS.API.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static void RegisterApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddWeb(configuration);
            services.AddApplication();
            services.AddInfrastructure(configuration);

        }

    }
}
