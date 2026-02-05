using IMS.API.Contracts;
using IMS.Application;
using IMS.Application.Common.Interfaces;
using IMS.Infrastructure;
namespace IMS.API.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static void RegisterAllServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddWebServices(configuration);
            services.AddApplicationServices();
            services.AddInfrastructureServices(configuration);

        }
        private static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddHttpContextAccessor();
            services.AddHealthChecks();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddCorsPolicy(configuration);
            services.AddControllers();
            services.AddSwaggerDocumentation();
            return services;
        }


        private static void AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {

            var origins = configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();
            var policyName = configuration.GetSection("CorsSettings:PolicyName").Get<string>();
            services.AddCors(options =>
            {
                options.AddPolicy(policyName, policy =>
                {
                    policy.WithOrigins(origins)
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();

                });
            });
        }



    }
}
