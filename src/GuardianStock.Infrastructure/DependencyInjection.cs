using FluentEmail.MailKitSmtp;
using GuardianStock.Application.Common.Errors;
using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Domain.Abstractions;
using GuardianStock.Domain.Categories;
using GuardianStock.Domain.Inventories;
using GuardianStock.Domain.Products;
using GuardianStock.Domain.StockHistories;
using GuardianStock.Domain.Transactions;
using GuardianStock.Domain.Users;
using GuardianStock.Infrastructure.Authentication;
using GuardianStock.Infrastructure.Common;
using GuardianStock.Infrastructure.Common.Exceptions;
using GuardianStock.Infrastructure.CsvFileReader.Products;
using GuardianStock.Infrastructure.EmailServices;
using GuardianStock.Infrastructure.EmailServices.Settings;
using GuardianStock.Infrastructure.FileManager;
using GuardianStock.Infrastructure.HubServices;
using GuardianStock.Infrastructure.Persistence;
using GuardianStock.Infrastructure.Persistence.BackgroundJobs;
using GuardianStock.Infrastructure.Persistence.Identity;
using GuardianStock.Infrastructure.Persistence.Repositories;
using GuardianStock.Infrastructure.Tokens;
using GuardianStock.Infrastructure.Tokens.Options;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
namespace GuardianStock.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddHangFireBackgroundJobWorker(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new MissingConfigurationSettingsException("DefaultConnection");

            services.AddHangfire(X => X.UseSqlServerStorage(connectionString, new SqlServerStorageOptions
            {
                PrepareSchemaIfNecessary = true

            }));


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

            var tokenSettings = section.Get<TokenSettings>();


            if (tokenSettings is null)
                throw new MissingConfigurationSettingsException(TokenSettings.SectionName);

            if (string.IsNullOrWhiteSpace(tokenSettings.SecretKey))
                throw new MissingConfigurationSettingsException(nameof(tokenSettings.SecretKey));

            if (string.IsNullOrWhiteSpace(tokenSettings.Issuer))
                throw new MissingConfigurationSettingsException(nameof(tokenSettings.Issuer));

            if (string.IsNullOrWhiteSpace(tokenSettings.Audience))
                throw new MissingConfigurationSettingsException(nameof(tokenSettings.Audience));


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
                            var accessToken = context.HttpContext.Request.Query["access_Token"];
                            if (context.HttpContext.Request.Path.Value?.Contains("hubs") == true)
                            {
                                if (!string.IsNullOrWhiteSpace(accessToken))
                                {
                                    context.HttpContext.Request.Headers.TryAdd("Authorization", $"Bearer {accessToken}");

                                }
                            }

                            return Task.CompletedTask;
                        }
                        ,

                        OnChallenge = context =>
                        {
                            if (context.Response.HasStarted)
                                return Task.CompletedTask;

                            else if (context.AuthenticateFailure is SecurityTokenInvalidSignatureException)
                            {
                                context.Response.Headers.TryAdd("Auth-Fail-Type", ApplicationErrors.IdentityErrors.InvalidToken.Code);
                                context.HandleResponse();
                                return Task.CompletedTask;
                            }


                            else if (context.AuthenticateFailure is SecurityTokenExpiredException)
                            {
                                context.Response.Headers.TryAdd("Auth-Fail-Type", ApplicationErrors.IdentityErrors.ExpiredToken.Code);
                                context.HandleResponse();
                                return Task.CompletedTask;
                            }

                            else if (!context.Request.Headers.ContainsKey("Authorization"))
                            {
                                context.Response.Headers.TryAdd("Auth-Fail-Type", ApplicationErrors.IdentityErrors.MissingToken.Code);
                                context.HandleResponse();
                                return Task.CompletedTask;
                            }
                            else
                            {
                                return Task.CompletedTask;
                            }

                        }
                        ,
                        OnForbidden = context =>
                        {

                            context.Response.Headers.TryAdd("Auth-Fail-Type", ApplicationErrors.IdentityErrors.ForbiddenAccess.Code);



                            return Task.CompletedTask;

                        }
                    };
                });

            services.AddAuthorization();

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


            if (string.IsNullOrWhiteSpace(connectionString))
                throw new MissingConfigurationSettingsException("DefaultConnection");


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

            if (smtpSettings is null)
                throw new MissingConfigurationSettingsException(SmtpSettings.SectionName);

            if (string.IsNullOrWhiteSpace(smtpSettings.SmtpHost))
                throw new MissingConfigurationSettingsException(nameof(smtpSettings.SmtpHost));

            if (string.IsNullOrWhiteSpace(smtpSettings.FromEmail))
                throw new MissingConfigurationSettingsException(nameof(smtpSettings.FromEmail));


            services.AddFluentEmail(smtpSettings!.FromEmail)
                    .AddRazorRenderer()
                    .AddMailKitSender(new SmtpClientOptions
                    {
                        Server = smtpSettings.SmtpHost,
                        Port = smtpSettings.SmtpPort,
                        UseSsl = smtpSettings.UseSSL,
                        RequiresAuthentication = smtpSettings.UseCredentials,
                        User = smtpSettings.FromEmail,
                        Password = smtpSettings.Password
                    });



            return services;




        }
        public static IServiceCollection RegisterHybridCache(this IServiceCollection services)
        {
            services.AddHybridCache((o) =>
            {
                o.MaximumPayloadBytes = 1024 * 1024;
                o.DefaultEntryOptions = new HybridCacheEntryOptions
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
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Tokens.EmailConfirmationTokenProvider = "ResetPasswordOTPProvider";

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
             .AddDefaultTokenProviders()
             .AddTokenProvider<ResetPasswordOTPTokenProvider<ApplicationUser>>("ResetPasswordOTPProvider");


            return services;
        }

        public class ResetPasswordOTPTokenProvider<T> : TotpSecurityStampBasedTokenProvider<T> where T : class
        {
            public override async Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<T> manager, T user)
            {
                return await manager.IsEmailConfirmedAsync(user) && !await manager.IsLockedOutAsync(user);
            }

            public override Task<string> GenerateAsync(string purpose, UserManager<T> manager, T user)
            {
                return base.GenerateAsync("ResetPasswordOTP:" + purpose, manager, user);
            }

            public override Task<string> GetUserModifierAsync(string purpose, UserManager<T> manager, T user)
            {
                return base.GetUserModifierAsync("ResetPasswordOTP:" + purpose, manager, user);
            }

            public override Task<bool> ValidateAsync(string purpose, string token, UserManager<T> manager, T user)
            {
                return base.ValidateAsync("ResetPasswordOTP:" + purpose, token, manager, user);
            }
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
            services.AddScoped<IUserRepository, UserRepository>();
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
            services.AddScoped<IFileManagerService, FileManagerService>();
            services.AddScoped<IImportService, ImportService>();
            services.AddScoped<ApplicationDbContextInitializer>();


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
