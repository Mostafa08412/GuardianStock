using GuardianStock.API.Middleware;
using GuardianStock.Infrastructure.HubServices;
using GuardianStock.Infrastructure.HubServices.Settings;
using GuardianStock.Infrastructure.Persistence;
using Hangfire;
using Serilog;
namespace GuardianStock.API.Extensions
{
    public static class MiddlewarePipeline
    {

        public static void ConfigureMiddlewarePipeline(this WebApplication app, IConfiguration configuration)
        {

            var hubSettings = configuration
                .GetSection(HubSettings.SectionName)
                .Get<HubSettings>() ?? new HubSettings();

            var databaseInitializationSettings = configuration
                .GetSection(DatabaseInitializationSettings.SectionName)
                .Get<DatabaseInitializationSettings>() ?? new DatabaseInitializationSettings();

            var corsSettings = configuration
                .GetSection(CorsSettings.SectionName)
                .Get<CorsSettings>() ?? new CorsSettings();


            app.UseGloabalExceptionHandler();

            app.UseSerilogRequestLogging();


            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Version 1");
                options.SwaggerEndpoint("/swagger/v2/swagger.json", "Version 2");
                options.DisplayRequestDuration();
            });


            if (databaseInitializationSettings.ResetDatabase)
                app.ResetDatabaseIfExists().Wait();

            if (databaseInitializationSettings.InitializeDatabase)
                app.InitializeDatabase().Wait();

            if (databaseInitializationSettings.SeedData)
                app.SeedData().Wait();


            app.MapHub<ImportHub>(hubSettings.ImportProducts.Status);
            app.UseHangfireDashboard();
            app.UseCors(corsSettings.PolicyName);
            app.UseRouting();
            app.UseMiddleware<HandleAuthenticationErrorMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

        }


    }

}


