using Database.Entities;

namespace Database.Seeders.V1;

public class GroupSeeder : ISeeder
{
    public void Seed(ApplicationDbContext db)
    {
        var users = db.Users.ToList();

        var groups = new List<Group>
        {
            new Group { Name = "Microsoft", MaintainerId = users[0].Id },
            new Group { Name = "Podpisy Elektroniczne", MaintainerId = users[1].Id },
            new Group { Name = "Oprogramowanie", MaintainerId = users[2].Id },
            new Group { Name = "Serwery", MaintainerId = users[3].Id },

            // without maintainer
            new Group { Name = "Marketing", MaintainerId = null },
            new Group { Name = "Human Resources", MaintainerId = null },

            // duplicates (same name, different maintainer / null)
            new Group { Name = "AI", MaintainerId = users[2].Id },
            new Group { Name = "AI", MaintainerId = null },

            // duplicates (same maintainer)
            new Group { Name = "Domeny", MaintainerId = users[4].Id },
            new Group { Name = "Certyfikaty", MaintainerId = users[4].Id }
        };

        db.Groups.AddRange(groups);
        db.SaveChanges();
    }
}