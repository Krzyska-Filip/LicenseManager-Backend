using Licenses.Database.Dto;
using Licenses.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Licenses.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<License> Licenses { get; set; }
    public DbSet<Seat> Seats { get; set; }
    
    public DbSet<LicenseCostDto> LicenseCosts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}   