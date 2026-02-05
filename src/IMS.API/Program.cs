using IMS.API.Extensions;
using Serilog;

namespace IMS.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LogBootstrapper.CreateBootstrapLogger();

            try
            {
                Log.Information("Starting IMS API...");

                var builder = WebApplication.CreateBuilder(args);

                builder.ConfigureSerilog();

                builder.Services.RegisterAllServices(builder.Configuration);

                var app = builder.Build();

                app.ConfigureMiddlewarePipeline(builder.Configuration);

                Log.Information("IMS API started successfully");

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
