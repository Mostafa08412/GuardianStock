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
            var resetDatabase = configuration.GetSection("InitializeDatabase:ResetDatabase").Get<bool>();
            var InitialDatabase = configuration.GetSection("InitializeDatabase:InitializeDatabase").Get<bool>();
            var seedData = configuration.GetSection("InitializeDatabase:SeedData").Get<bool>();
            var importStatusChannel = configuration.GetSection("HubSettings:ImportProducts:Status").Get<string>();
            var corsPolicyName = configuration.GetSection("CorsSettings:PolicyName").Get<string>();





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

            if (resetDatabase)
                app.ResetDatabaseIfExists().Wait();

            if (InitialDatabase)
                app.InitializeDatabase().Wait();

            if (seedData)
                app.SeedData().Wait();


            app.MapHub<ImportHub>(importStatusChannel!);
            app.UseHangfireDashboard();
            app.UseCors(corsPolicyName!);
            app.UseRouting();
            app.UseMiddleware<HandleAuthenticationErrorMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

        }


    }

}


