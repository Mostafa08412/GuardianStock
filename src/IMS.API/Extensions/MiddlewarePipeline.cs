using Hangfire;
using IMS.API.Middleware;
using IMS.Infrastructure.HubServices;
using IMS.Infrastructure.Persistence;
using Serilog;
namespace IMS.API.Extensions
{
    public static class MiddlewarePipeline
    {

        public static void ConfigureMiddlewarePipeline(this WebApplication app, IConfiguration configuration)
        {
            var useSeedData = configuration.GetSection("Seeding:UseSeedData").Get<bool>();

            app.UseGloabalExceptionHandler();

            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {

                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Version 1");
                    options.SwaggerEndpoint("/swagger/v2/swagger.json", "Version 2");
                    options.DisplayRequestDuration();
                });
            }
            if (useSeedData)
                app.RegisterInitializer();


            app.MapHub<ImportHub>(configuration.GetSection("HubSettings:ImportProducts:Status").Get<string>());
            app.UseHangfireDashboard();
            app.UseCors(configuration.GetSection("CorsSettings:PolicyName").Get<string>());
            app.UseRouting();
            app.UseMiddleware<HandleAuthenticationErrorMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

        }


    }

}


