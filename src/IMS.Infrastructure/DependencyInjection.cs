using IMS.Application.Common.Interfaces;
using IMS.Domain.Abstractions;
using IMS.Domain.Core.Errors;
using IMS.Domain.Core.Primitives;
using IMS.Infrastructure.Authentication;
using IMS.Infrastructure.Email_Services;
using IMS.Infrastructure.Email_Services.Options;
using IMS.Infrastructure.Persistence;
using IMS.Infrastructure.Persistence.Identity;
using IMS.Infrastructure.Persistence.Repositories;
using IMS.Infrastructure.Tokens;
using IMS.Infrastructure.Tokens.Options;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
namespace IMS.Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAuthorizationBuilder();


            services.AddOptions<TokenSettings>()
               .Bind(configuration.GetSection(TokenSettings.SectionName))
               .ValidateDataAnnotations()
               .ValidateOnStart();


            var section = configuration.GetSection("TokenSettings");
            services.Configure<TokenSettings>(section);

            var tokenSettings = section.Get<TokenSettings>() ?? throw new ArgumentNullException(nameof(section), "TokenSettings section is missing.");

            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.SecretKey));





            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = tokenSettings.Issuer,
                        ValidAudience = tokenSettings.Audience,
                        IssuerSigningKey = symmetricKey,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    options.IncludeErrorDetails = true; // Enable detailed errors


                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            return Task.CompletedTask;
                        },

                        OnTokenValidated = context =>
                        {
                            var claims = context.Principal?.Claims.Select(c => $"{c.Type}: {c.Value}");

                            // Log the Role claim specifically
                            var roles = context.Principal?.FindAll(System.Security.Claims.ClaimTypes.Role);


                            return Task.CompletedTask;
                        },

                        OnChallenge = context =>
                        {
                            if (context.Response.HasStarted)
                                return Task.CompletedTask;

                            else if (context.AuthenticateFailure is SecurityTokenInvalidSignatureException)
                            {
                                context.HandleResponse();
                                return WriteProblemDetailsAsync(context.HttpContext, StatusCodes.Status401Unauthorized, Errors.Identity.InvalidToken);
                            }

                            else if (context.AuthenticateFailure is SecurityTokenExpiredException)
                            {
                                context.HandleResponse();
                                return WriteProblemDetailsAsync(context.HttpContext, StatusCodes.Status401Unauthorized, Errors.Identity.ExpiredToken);
                            }

                            else if (!context.Request.Headers.ContainsKey("Authorization"))
                            {
                                context.HandleResponse();
                                return WriteProblemDetailsAsync(context.HttpContext, StatusCodes.Status401Unauthorized, Errors.Identity.MissingToken);

                            }
                            else
                            {
                                return Task.CompletedTask;
                            }

                        }
                    };
                });

            services.AddAuthorization();

            return services;
        }

        private static Task WriteProblemDetailsAsync(HttpContext context, int statusCode, Error error)
        {

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = error.Code,
                Detail = error.Description,
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            };
            context.Response.OnStarting(() =>
            {
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";


                context.Response.WriteAsJsonAsync(problemDetails);
                return Task.CompletedTask;
            });
            return Task.CompletedTask;

        }

        public static IServiceCollection RegisterRepositoriesAndUnitOfWork(this IServiceCollection services)
        {
            // Register Repositories here....


            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            return services;
        }
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            //Register any additonal services here...

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddSingleton<IDateTime, DateProvider>();
            services.AddScoped<ApplicationDbContextInitializer>();
            services.AddTransient<ITokenService, TokenService>();


            return services;
        }

        public static IServiceCollection RegisterAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(o =>
            {
                o.AddMaps(Assembly.GetExecutingAssembly());
            });

            return services;
        }
        public static IServiceCollection RegisterDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection")!;


            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
                options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
            });

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            return services;
        }

        public static IServiceCollection RegisterFluentEmail(this IServiceCollection services, IConfiguration configuration)
        {

            var smtpSettings = configuration.GetSection("SmtpSettings").Get<SmtpSettings>();

            services
             .AddFluentEmail(smtpSettings!.FromEmail)
             .AddSmtpSender(new SmtpClient(smtpSettings.SmtpHost, smtpSettings.SmtpPort)
             {
                 Credentials = new NetworkCredential(
                     smtpSettings.FromEmail,
                     smtpSettings.Password // App Password
                 ),
                 EnableSsl = true
             });

            return services;




        }

        public static IServiceCollection RegisterIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {

                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;


                options.User.RequireUniqueEmail = true;


                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultTokenProviders();

            return services;
        }
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {


            services.RegisterDbContext(configuration);
            services.RegisterFluentEmail(configuration);
            services.RegisterAutoMapper();
            services.RegisterRepositoriesAndUnitOfWork();
            services.RegisterServices();
            services.RegisterIdentity();
            services.AddJwtAuthentication(configuration);

            return services;
        }
    }
}
