﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Squil;

public static class Startup
{
    public static void AddSquilDb(this IServiceCollection services, AppSettings settings, IConfiguration configuration)
    {
        var provider = configuration?["provider"];
        var ignoreAssembly = configuration?["ignore-assembly"] != null;

        provider ??= settings.SquilDbProviderName;

        if (!String.IsNullOrEmpty(provider) && !provider.Equals("none", StringComparison.InvariantCultureIgnoreCase))
        {
            services.AddDbContextFactory<Db>(
                options => _ = provider switch
                {
                    "Sqlite" => options.UseSqlite(
                        settings.SquilDbSqliteConnectionString,
                        x => x.Apply(!ignoreAssembly, x2 => x2.MigrationsAssembly("Squil.Storage.Migrations.Sqlite"))),
                    "SqlServer" => options.UseSqlServer(
                        SqlServerConnectionProvider.CompatibilityPrefix + settings.SquilDbSqlServerConnectionString,
                        x => x.Apply(!ignoreAssembly, x2 => x2.MigrationsAssembly("Squil.Storage.Migrations.SqlServer"))),

                    _ => throw new Exception($"Unsupported provider: {provider}")
                });

            services.AddSingleton<LiveConfiguration>();
            services.AddSingleton<LightLiveConfiguration>(sp => sp.GetService<LiveConfiguration>());
        }
        else
        {
            services.AddSingleton<LightLiveConfiguration>();
        }

        services.AddSingleton<ILiveSourceProvider>(sp => sp.GetService<LightLiveConfiguration>());
    }

    public static void AddCommonSquilServices(this IServiceCollection services, AppSettings settings)
    {
        services.AddSingleton<SqlServerConnectionProvider>();
        services.AddScoped<UiQueryRunner>();
        services.AddScoped<ConnectionHolder>();
        services.AddScoped<CircuitState>();
    }

    public static void InitializeDbAndInstallStaticServices(this IServiceProvider services)
    {
        var dbFactory = services.GetService<IDbFactory>();

        if (dbFactory != null)
        {
            using var db = dbFactory.CreateDbContext();

            db.Database.Migrate();

            db.Database.ExecuteSqlRaw("delete from SqlServerHostConfigurations where Name in (select Name from SqlServerHostConfigurations group by Name having count(*) > 1)");
        }

        StaticServiceStack.Install<CancellationToken>(default);
    }
}
