using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Entities;

public class User : IEntity
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public uint Version { get; set; }

    public List<License> Licenses { get; set; } = [];
    public List<Seat> Seats { get; set; } = [];
}

public class UserTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(b => new { b.Id });

        builder
            .Property(b => b.Id)
            .ValueGeneratedOnAdd();

        builder.Property(b => b.Version)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.HasMany(u => u.Licenses)
            .WithMany()
            .UsingEntity<Seat>(
                j => j.HasOne(s => s.License)
                      .WithMany(l => l.Seats)
                      .HasForeignKey(s => s.LicenseId)
                      .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne(s => s.AssignedTo)
                      .WithMany(u => u.Seats)
                      .HasForeignKey(s => s.AssignedToId)
                      .OnDelete(DeleteBehavior.SetNull)
            );
    }
}