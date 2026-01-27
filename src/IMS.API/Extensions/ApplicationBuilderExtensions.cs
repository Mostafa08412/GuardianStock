using Serilog;
namespace IMS.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        private const string CorsPolicyName = "Frontend-Application-Origin";

        public static void ConfigureMiddlewarePipeline(this WebApplication app)
        {


            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                //app.RegisterInitializer();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                });
            }

            app.UseCors(CorsPolicyName);

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHealthChecks("/health");
            app.MapHealthChecks("/health/ready");
        }
    }

}


