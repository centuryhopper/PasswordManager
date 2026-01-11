global using ConfigurationProvider = Server.Utils.ConfigurationProvider;
global using static Shared.Models.ServiceResponses;
global using static LanguageExt.Prelude;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Server.Contexts;
using Server.Repositories;
using Swashbuckle.AspNetCore.Filters;
// using NLog;
// using NLog.Web;

// MUST HAVE IT LIKE THIS FOR NLOG TO RECOGNIZE DOTNET USER-SECRETS INSTEAD OF HARDCODED DELIMIT PLACEHOLDER VALUE FROM APPSETTINGS.JSON

// #if DEBUG
//     var logger = LogManager.Setup().LoadConfigurationFromFile("nlog_dev.config").GetCurrentClassLogger();
// #else
//     var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
// #endif


// try
// {

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    // builder.Logging.ClearProviders();
    // builder.Host.UseNLog();


    builder.Services.AddControllers();
    builder.Services.AddRazorPages();
    builder.Services.AddHttpClient();

    // builder.Services.AddHttpContextAccessor();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition(
            "oauth2",
            new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
            }
        );

        options.OperationFilter<SecurityRequirementsOperationFilter>();
    });

    ConfigurationProvider configurationProvider = new(builder.Environment, builder.Configuration);
    builder.Services.AddSingleton(configurationProvider);
    builder.Services.AddSingleton<EncryptionContext>();
    builder.Services.AddScoped<IAccountRepository, AccountRepository>();
    builder.Services.AddScoped<IPasswordManagerDbRepository, PasswordManagerDbRepository>();

    builder.Services.AddDbContext<PasswordManagerDbContext>(options =>
        options.UseNpgsql(
            configurationProvider.GetPasswordDBConnectionString
        )
    );

    builder
        .Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = configurationProvider.JwtIssuer,
                ValidAudience = configurationProvider.JwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        configurationProvider.JwtKey
                    )
                ),
            };
        });

    if (!builder.Environment.IsDevelopment())
    {
        var port = Environment.GetEnvironmentVariable("PORT") ?? "8081";
        builder.WebHost.UseUrls($"http://*:{port}");
    }

    // Add CORS policy
    const string MOBILE_CLIENT_CORS = "AllowMobileApps";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(
            MOBILE_CLIENT_CORS,
            policy =>
            {
                policy
                    .WithOrigins("http://localhost:3000", "https://your-mobile-app.com") // Replace with MOBILE app URL
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials(); // If your mobile client uses credentials like cookies or auth headers
            }
        );
    });

    var app = builder.Build();

    app.UseCors(MOBILE_CLIENT_CORS);

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapRazorPages();
    app.MapControllers();
    app.MapFallbackToFile("index.html");

    app.Run();
// }
// catch (Exception ex)
// {
//     logger.Error(ex, "Stopped program because of exception: " + ex);
//     throw ex;
// }
// finally
// {
//     LogManager.Shutdown();
// }

