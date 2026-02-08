using IMS.API.Contracts.Examples;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace IMS.API.Extensions
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerExamplesFromAssemblyOf<ApiResponseExample>();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Inventory Management System",
                    Version = "The First Version",
                    Description = "This is documentation for Inventory Management System Api Version 1.0"
                });

                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "Inventory Management System",
                    Version = "The Second Version",
                    Description = "This is documentation for Inventory Management System Api Version 2.0"
                });

                c.EnableAnnotations();
                c.ExampleFilters();
                c.AddJwtSecurityDefinition();
            });
        }

        private static void AddJwtSecurityDefinition(this SwaggerGenOptions c)
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "Json Web Token",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
        }
    }


}


