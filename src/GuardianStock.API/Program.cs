using GuardianStock.API.Extensions;
using Serilog;

namespace GuardianStock.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LogBootstrapper.CreateBootstrapLogger();

            try
            {
                Log.Information("Starting GuardianStock API...");

                var builder = WebApplication.CreateBuilder(args);

                builder.ConfigureSerilog();

                builder.Services.RegisterAllServices(builder.Configuration);

                var app = builder.Build();

                app.ConfigureMiddlewarePipeline(builder.Configuration);

                Log.Information("GuardianStock API started successfully");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
