using Asp.Versioning;
using GuardianStock.API.Contracts;
using GuardianStock.Application;
using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Infrastructure;
namespace GuardianStock.API.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static void RegisterAllServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddWebServices(configuration);
            services.AddApplicationServices();
            services.AddInfrastructureServices(configuration);

        }
        private static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddCorsPolicy(configuration);
            services.AddControllers();
            services.AddSwaggerDocumentation();
            services.AddVersioning();
            return services;
        }


        private static void AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            var CorsSettings = configuration.GetSection("CorsSettings");

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


        private static void AddVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(op =>
            {
                op.DefaultApiVersion = new Asp.Versioning.ApiVersion(1);
                op.AssumeDefaultVersionWhenUnspecified = true;
                op.ReportApiVersions = true;
                op.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
                .AddMvc()
                .AddApiExplorer(op =>
                {
                    op.DefaultApiVersion = new ApiVersion(1);
                    op.AssumeDefaultVersionWhenUnspecified = true;
                    op.GroupNameFormat = "'v'VVV";
                    op.SubstituteApiVersionInUrl = true;
                })
              ;
        }
    }
}
