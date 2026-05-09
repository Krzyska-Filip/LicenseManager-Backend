using Licenses.Database;
using Licenses.Database.Seeders;

public class LicenseGroupSeeder : ISeeder
{
    public void Seed(ApplicationDbContext db)
    {
        var users = db.Users.ToList();

        var groups = new List<LicenseGroup>
        {
            new LicenseGroup { Name = "Microsoft", MaintainerId = users[0].Id },
            new LicenseGroup { Name = "Podpisy Elektroniczne", MaintainerId = users[1].Id },
            new LicenseGroup { Name = "Oprogramowanie", MaintainerId = users[2].Id },
            new LicenseGroup { Name = "Serwery", MaintainerId = users[3].Id },

            // without maintainer
            new LicenseGroup { Name = "Marketing", MaintainerId = null },
            new LicenseGroup { Name = "Human Resources", MaintainerId = null },

            // duplicates (same name, different maintainer / null)
            new LicenseGroup { Name = "AI", MaintainerId = users[2].Id },
            new LicenseGroup { Name = "AI", MaintainerId = null },

            // duplicates (same maintainer)
            new LicenseGroup { Name = "Domeny", MaintainerId = users[4].Id },
            new LicenseGroup { Name = "Certyfikaty", MaintainerId = users[4].Id }
        };

        db.LicenseGroups.AddRange(groups);
        db.SaveChanges();
    }
}