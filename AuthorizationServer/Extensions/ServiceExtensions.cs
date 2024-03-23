
using AuthorizationServer.Persistence;
using AuthorizationServer.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Text;

namespace AuthorizationServer.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
    IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings))
           .Get<DatabaseSettings>();
        services.AddSingleton(databaseSettings);

        return services;
    }
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.ConfigureAuthServerDbContext(configuration);
        // services.AddInfrastructureServices();
        services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

        return services;
    }

    private static IServiceCollection ConfigureAuthServerDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
        var builder = new MySqlConnectionStringBuilder(databaseSettings.ConnectionString);

        services.AddDbContext<AuthServerContext>(options =>
                {
                    // Configure the context to use sqlite.
                    options.UseMySql(builder.ConnectionString,
                    ServerVersion.AutoDetect(builder.ConnectionString), e =>
                    {
                        e.MigrationsAssembly("AuthorizationServer");
                        e.SchemaBehavior(MySqlSchemaBehavior.Ignore);
                    });

                    // Register the entity sets needed by OpenIddict.
                    // Note: use the generic overload if you need
                    // to replace the default OpenIddict entities.
                    options.UseOpenIddict();
                });
        return services;
    }

    // private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    // {
    //     return services.AddScoped(typeof(IRepositoryBaseAsync<,,>), typeof(RepositoryQueryBase<,,>))
    //             .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
    //             .AddScoped<IAuthServerRepository, ProductRepository>()
    //         ;
    // }
}
