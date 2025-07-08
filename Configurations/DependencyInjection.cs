using DAMApi.Ingestion;
using DAMApi.Repository.Implementation;
using DAMApi.Repository.Interfaces;
using DAMApi.Services.Implementation;
using DAMApi.Services.Interfaces;
using DAMApi.Settings;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

public static class DependencyInjection
{
    public static IServiceCollection ApplicationServices(this IServiceCollection services,
                                                         IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));
        services.AddSingleton<ApplicationDbContext>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) 
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtSettings:JwtIssuer"],
                    ValidAudience = configuration["JwtSettings:JwtAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:JwtKey"]!))
                };
            });

        //google api keys
        services.Configure<GoogleApiSettings>(configuration.GetSection("GoogleApiSettings"));

        // Register FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register services
        services.AddScoped<GoogleApiService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFolderRepositroy ,FolderRepository>();
        services.AddScoped<IJWTService, JWTService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<CsvIngestionService>();


        //services.AddDataProtection();

        // Register AutoMapper
        //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        return services;
    }
}

