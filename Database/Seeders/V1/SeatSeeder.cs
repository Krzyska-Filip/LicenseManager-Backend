using Database.Entities;

namespace Database.Seeders.V1;

public class SeatSeeder : ISeeder
{
    public void Seed(ApplicationDbContext db)
    {
        if (db.Seats.Any())
            return;

        var licenses = db.Licenses.ToList();
        var users = db.Users.ToList();

        var seats = new List<Seat>
        {
            new Seat { LicenseId = licenses[0].Id, AssignedToId = users[0].Id, ProratedPurchase = true,  ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6)) },
            new Seat { LicenseId = licenses[0].Id, AssignedToId = users[1].Id, ProratedPurchase = true,  ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-5)) },
            new Seat { LicenseId = licenses[1].Id, AssignedToId = users[2].Id, ProratedPurchase = false, ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-3)) },

            // unassigned seats
            new Seat { LicenseId = licenses[1].Id, AssignedToId = null, ProratedPurchase = false, ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-2)) },
            new Seat { LicenseId = licenses[2].Id, AssignedToId = null, ProratedPurchase = true,  ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-12)) },

            // mixed assignment
            new Seat { LicenseId = licenses[3].Id, AssignedToId = users[3].Id, ProratedPurchase = true,  ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)) },
            new Seat { LicenseId = licenses[3].Id, AssignedToId = users[4].Id, ProratedPurchase = false, ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-20)) },

            // more variation
            new Seat { LicenseId = licenses[4].Id, AssignedToId = users[0].Id, ProratedPurchase = false, ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)) },
            new Seat { LicenseId = licenses[4].Id, AssignedToId = null, ProratedPurchase = false, ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-8)) },

            new Seat { LicenseId = licenses[5].Id, AssignedToId = users[1].Id, ProratedPurchase = true, ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow) },
            new Seat { LicenseId = licenses[5].Id, AssignedToId = users[2].Id, ProratedPurchase = true, ValidFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)) }
        };

        db.Seats.AddRange(seats);
        db.SaveChanges();
    }
}