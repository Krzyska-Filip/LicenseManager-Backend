using Database.Seeders.V1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Database;

public static class ApplicationDbExtension
{
    public static void RegisterDatabase(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrWhiteSpace(connectionString))
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("LicenseManager"));
        else
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
    }

    public static void MigrateDatabase(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (db.Database.IsRelational())
            db.Database.Migrate();
        else
            db.Database.EnsureCreated();
    }

    public static void SeedDatabase(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        new UserSeeder().Seed(db);
        new GroupSeeder().Seed(db);
        new LicenseSeeder().Seed(db);
        new SeatSeeder().Seed(db);
        db.SaveChanges();

        if (db.Database.IsRelational())
            db.Database.ExecuteSqlRaw(@"
                SELECT setval(pg_get_serial_sequence('""Users""', 'Id'), (SELECT MAX(""Id"") FROM ""Users""));
                SELECT setval(pg_get_serial_sequence('""Groups""', 'Id'), (SELECT MAX(""Id"") FROM ""Groups""));
                SELECT setval(pg_get_serial_sequence('""Licenses""', 'Id'), (SELECT MAX(""Id"") FROM ""Licenses""));
                SELECT setval(pg_get_serial_sequence('""Seats""', 'Id'), (SELECT MAX(""Id"") FROM ""Seats""));
            ");
    }
    
    public static void ClearDatabase(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (db.Database.IsRelational())
            db.Database.ExecuteSqlRaw(@"
                TRUNCATE TABLE ""Seats"",
                               ""Licenses"",
                               ""Groups"",
                               ""Users""
                RESTART IDENTITY CASCADE;
            ");
    }
}