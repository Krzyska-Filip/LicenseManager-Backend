using Licenses.Database;
using Licenses.Database.Entities;
using Licenses.Database.Seeders;

public class UserSeeder : ISeeder
{
    public void Seed(ApplicationDbContext db)
    {
        var users = new List<User>
        {
            new User { Id = 1, Username = "Adam Nowak", Email = "adam.nowak@example.com" },
            new User { Id = 2, Username = "Jan Kowalski", Email = "jan.kowalski@example.com" },
            new User { Id = 3, Username = "Anna Wiśniewska", Email = "anna.wisniewska@example.com" },
            new User { Id = 4, Username = "Piotr Zieliński", Email = "piotr.zielinski@example.com" },
            new User { Id = 5, Username = "Kasia Lewandowska", Email = "kasia.lewandowska@example.com" },
            new User { Id = 6, Username = "Marek Wójcik", Email = "marek.wojcik@example.com" },
            new User { Id = 7, Username = "Ewa Kamińska", Email = "ewa.kaminska@example.com" },
            new User { Id = 8, Username = "Tomasz Dąbrowski", Email = "tomasz.dabrowski@example.com" },
            new User { Id = 9, Username = "Magdalena Kaczmarek", Email = "magdalena.kaczmarek@example.com" },
            new User { Id = 10, Username = "Paweł Szymański", Email = "pawel.szymanski@example.com" }
        };

        db.Users.AddRange(users);
        db.SaveChanges();
    }
}