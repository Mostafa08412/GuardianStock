using Hangfire;
using IMS.Application.Common.Interfaces;
using IMS.Domain.Abstractions;
using IMS.Domain.Categories;
using IMS.Domain.Core.Errors;
using IMS.Domain.Inventories;
using IMS.Domain.Products;
using IMS.Domain.StockHistories;
using IMS.Domain.Transactions;
using IMS.Infrastructure.Authentication;
using IMS.Infrastructure.Common;
using IMS.Infrastructure.CsvFileReader.Products;
using IMS.Infrastructure.EmailServices;
using IMS.Infrastructure.EmailServices.Options;
using IMS.Infrastructure.FileService;
using IMS.Infrastructure.HubServices;
using IMS.Infrastructure.Persistence;
using IMS.Infrastructure.Persistence.BackgroundJobs;
using IMS.Infrastructure.Persistence.Identity;
using IMS.Infrastructure.Persistence.Repositories;
using IMS.Infrastructure.Tokens;
using IMS.Infrastructure.Tokens.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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
        public static IServiceCollection AddHangFireBackgroundJobWorker(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddHangfire(X => X.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfireServer();

            services.AddScoped<BackgroundJobBridge>();

            services.AddScoped<IBackgroundJobWorker, BackgroundJobWorker>();

            return services;
        }


        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAuthorizationBuilder();

            services.AddOptions<TokenSettings>().Bind(configuration.GetSection(TokenSettings.SectionName));

            var section = configuration.GetSection(TokenSettings.SectionName);

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

                        OnChallenge = context =>
                        {
                            if (context.Response.HasStarted)
                                return Task.CompletedTask;

                            else if (context.AuthenticateFailure is SecurityTokenInvalidSignatureException)
                            {
                                context.Response.Headers.Add("Auth-Fail-Type", Errors.Identity.InvalidToken.Code);
                                context.HandleResponse();
                                return Task.CompletedTask;
                            }

                            else if (context.AuthenticateFailure is SecurityTokenExpiredException)
                            {
                                context.Response.Headers.Add("Auth-Fail-Type", Errors.Identity.ExpiredToken.Code);
                                context.HandleResponse();
                                return Task.CompletedTask;
                            }

                            else if (!context.Request.Headers.ContainsKey("Authorization"))
                            {
                                context.Response.Headers.Add("Auth-Fail-Type", Errors.Identity.MissingToken.Code);
                                context.HandleResponse();
                                return Task.CompletedTask;
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


        public static IServiceCollection RegisterRepositoriesAndUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            // Register Repositories here....
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IStockHistoryRepository, StockHistoriesRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            return services;
        }
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            //Register any additonal services here...
            services.AddTransient<ITokenService, TokenService>();
            services.AddSingleton<IProductCsvReader, ProductCsvReader>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddSingleton<ISkuGenerator, SkuGenerator>();
            services.AddTransient<IDateTime, SettableDateProvider>();
            services.AddScoped<IFileManager, FileManager>();
            services.AddScoped<ISignalService, SignalService>();
            services.AddScoped<ApplicationDbContextInitializer>();


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

            services.AddOptions<SmtpSettings>().Bind(configuration.GetSection(SmtpSettings.SectionName));

            var smtpSettings = configuration.GetSection(SmtpSettings.SectionName).Get<SmtpSettings>();

            services.AddFluentEmail(smtpSettings!.FromEmail)
                    .AddRazorRenderer()
                    .AddSmtpSender(() => new SmtpClient(smtpSettings.SmtpHost, smtpSettings.SmtpPort)
                    {
                        EnableSsl = smtpSettings.UseSSL,
                        UseDefaultCredentials = false,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Credentials = smtpSettings.UseSSL ? new NetworkCredential(smtpSettings.FromEmail, smtpSettings.Password) : null
                    });


            return services;




        }

        public static IServiceCollection RegisterHybridCache(this IServiceCollection services)
        {
            services.AddHybridCache((o) =>
            {
                o.MaximumPayloadBytes = 1024 * 1024;
                o.DefaultEntryOptions = new Microsoft.Extensions.Caching.Hybrid.HybridCacheEntryOptions
                {
                    LocalCacheExpiration = TimeSpan.FromSeconds(60),
                    Expiration = TimeSpan.FromMinutes(60)
                };
            });

            return services;
        }
        public static IServiceCollection RegisterSignalR(this IServiceCollection services)
        {
            services.AddSignalR();
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
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {


            services.RegisterDbContext(configuration)
                    .RegisterFluentEmail(configuration)
                    .RegisterAutoMapper()
                    .RegisterRepositoriesAndUnitOfWork()
                    .RegisterServices()
                    .RegisterIdentity()
                    .AddJwtAuthentication(configuration)
                    .AddHangFireBackgroundJobWorker(configuration)
                    .RegisterSignalR()
                    .RegisterHybridCache();

            return services;
        }
    }
}
