using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Licenses.Database;

public static class ApplicationDbExtension
{
    public static void RegisterDatabase(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
    }

    public static void MigrateDatabase(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
    
    public static void SeedDatabase(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (db.Users.Any()) return;

        db.Database.ExecuteSqlRaw(@"
            TRUNCATE TABLE ""Seats"", 
                           ""Licenses"", 
                           ""LicenseGroups"", 
                           ""Users""
            RESTART IDENTITY CASCADE;
        ");
        
        new UserSeeder().Seed(db);
        new LicenseGroupSeeder().Seed(db);
        new LicenseSeeder().Seed(db);
        new SeatSeeder().Seed(db);
        db.SaveChanges();
    }
}