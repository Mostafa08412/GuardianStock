using IMS.API.Middleware;
using Serilog;
namespace IMS.API.Extensions
{
    public static class MiddlewarePipeline
    {
        private const string CorsPolicyName = "Frontend-Application-Origin";

        public static void ConfigureMiddlewarePipeline(this WebApplication app)
        {


            app.UseGloabalExceptionHandler();

            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {

                // app.RegisterInitializer();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.DisplayRequestDuration();
                });
            }

            app.UseCors(CorsPolicyName);
            app.UseRouting();
            app.UseMiddleware<HandleAuthenticationErrorMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

        }


    }

}


