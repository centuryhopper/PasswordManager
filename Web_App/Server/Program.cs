global using ConfigurationProvider = Server.Utils.ConfigurationProvider;
global using Shared;

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using NpgsqlTypes;
using Server.Contexts;
using Server.Entities;
using Server.Repositories;
using Server.Utils;
using Swashbuckle.AspNetCore.Filters;

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
    builder.Services.AddSwaggerGen(options=>{
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
        });

        options.OperationFilter<SecurityRequirementsOperationFilter>();
    });

    builder.Services.AddSingleton<EncryptionContext>();
    builder.Services.AddSingleton<ConfigurationProvider>();
    builder.Services.AddScoped<IAccountRepository, AccountRepository>();
    builder.Services.AddScoped<IPasswordManagerDbRepository, PasswordManagerDbRepository>();

    // Configure the Identity database context
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(
            builder.Environment.IsDevelopment()
                    ?
                        builder.Configuration.GetConnectionString("UserManagementDB")
                    :
                        Environment.GetEnvironmentVariable("UserManagementDB"))
            );

    builder.Services.AddDbContext<PasswordManagerDbContext>(options =>
        options.UseNpgsql(
            builder.Environment.IsDevelopment()
            ?
                builder.Configuration.GetConnectionString("DB_CONN")
            :
                Environment.GetEnvironmentVariable("DB_CONN")
        )
    );

    builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddRoles<ApplicationRole>();

    builder.Services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequiredLength = 10;
        options.Password.RequiredUniqueChars = 3;
        options.Password.RequireNonAlphanumeric = false;
        options.SignIn.RequireConfirmedAccount = true;
    });

    builder.Services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Environment.IsDevelopment() ? builder.Configuration["Jwt:Issuer"] : Environment.GetEnvironmentVariable("Jwt_Issuer"),
            ValidAudience = builder.Environment.IsDevelopment() ? builder.Configuration["Jwt:Audience"] : Environment.GetEnvironmentVariable("Jwt_Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Environment.IsDevelopment() ? builder.Configuration["Jwt:Key"] : Environment.GetEnvironmentVariable("Jwt_Key")))
        };
    });

    if (!builder.Environment.IsDevelopment())
    {
        var port = Environment.GetEnvironmentVariable("PORT") ?? "8081";
        builder.WebHost.UseUrls($"http://*:{port}");
    }

    // Add CORS policy
    const string FLUTTER_CLIENT_CORS = "AllowFlutterClient";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(FLUTTER_CLIENT_CORS, policy =>
        {
            policy.WithOrigins("http://localhost:3000", "https://your-flutter-web-app.com") // Replace with Flutter app URL
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();  // If your Flutter client uses credentials like cookies or auth headers
        });
    });

    var app = builder.Build();

    app.UseCors(FLUTTER_CLIENT_CORS);

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
//  catch (Exception ex)
// {
//     logger.Error(ex, "Stopped program because of exception: " + ex);
//     throw ex;
// }
// finally {
//     LogManager.Shutdown();
// }