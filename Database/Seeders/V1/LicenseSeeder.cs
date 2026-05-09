using Licenses.Database;
using Licenses.Database.Seeders;

public class LicenseSeeder : ISeeder
{
    public void Seed(ApplicationDbContext db)
    {
        if (db.Licenses.Any())
            return;

        var groups = db.Groups.ToList();

        var licenses = new List<License>
        {
            new License { Name = "Microsoft Business Basic", Type = "License", PricePerSeat = 9.99m, IsProrated = true, GroupId = groups[0].Id, ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6)), ValidTo = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(+6)) },
            new License { Name = "Microsoft Business Standard", Type = "License", PricePerSeat = 14.99m, IsProrated = true, GroupId = groups[0].Id, ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-3)), ValidTo = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(+9)) },
            new License { Name = "Microsoft Business Premium", Type = "License", PricePerSeat = 19.99m, IsProrated = true, GroupId = groups[0].Id, ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-12)), ValidTo = DateOnly.FromDateTime(DateTime.UtcNow) },
            
            // Same pricing model, different billing periods
            new License { Name = "Podpis elektroniczny - Adam", Type = "Podpis", PricePerSeat = 19.99m, IsProrated = false, GroupId = groups[1].Id, ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)), ValidTo = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2)) },
            new License { Name = "Podpis elektroniczny - Tomek", Type = "Podpis", PricePerSeat = 19.99m, IsProrated = false, GroupId = groups[1].Id, ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-2)), ValidTo = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)) },

            // with previous chain
            new License { Name = "Jetbrains", Type = "IDE", PricePerSeat = 19.99m, IsProrated = false, GroupId = groups[2].Id, PreviousId = null, ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-2)), ValidTo = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)) },
            new License { Name = "Jetbrains", Type = "IDE", PricePerSeat = 19.99m, IsProrated = false, GroupId = groups[2].Id, PreviousId = null, ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)), ValidTo = null },

            //
            new License { Name = "Docusign", Type = "Subscription", PricePerSeat = 19.99m, IsProrated = false, GroupId = groups[5].Id, ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)), ValidTo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)) },
            new License { Name = "Traffit", Type = "Subscription", PricePerSeat = 19.99m, IsProrated = false, GroupId = groups[5].Id, ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-2)), ValidTo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)) },

            new License { Name = "Internal Tools v2", Type = "Internal", PricePerSeat = 19.99m, IsProrated = false, GroupId = groups[3].Id, ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow), ValidTo = null }
        };

        db.Licenses.AddRange(licenses);
        db.SaveChanges();
    }
}