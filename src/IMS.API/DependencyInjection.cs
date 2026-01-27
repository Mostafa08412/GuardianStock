using IMS.API.Contracts;
using IMS.API.Extensions;
using IMS.Application.Common.Interfaces;

namespace IMS.API
{
    public static class DependencyInjection
    {
        private const string CorsPolicyName = "Frontend-Application-Origin";

        public static IServiceCollection AddWeb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddHealthChecks();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddCorsPolicy();
            services.AddControllers();
            services.AddSwaggerDocumentation();
            return services;
        }


        private static void AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName, policy =>
                {
                    policy.WithOrigins("http://localhost:8080")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
        }




    }
}
