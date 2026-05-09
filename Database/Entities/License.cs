using Licenses.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class License : IEntity
{
    public int Id { get; set; }

    public int LicenseGroupId { get; set; }
    public LicenseGroup LicenseGroup { get; set; }

    public int? PreviousId { get; set; }
    public License? Previous { get; set; }
    
    public List<Seat> Seats { get; set; }

    public string Name { get; set; }
    public string Type { get; set; }
    public bool IsProrated { get; set; }
    public bool IsRenewed { get; set; }
    public decimal PricePerSeat { get; set; }
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
    public uint Version { get; set; }
}

public class LicenseTypeConfiguration : IEntityTypeConfiguration<License>
{
    public void Configure(EntityTypeBuilder<License> builder)
    {
        builder.HasKey(b => new { b.Id });

        builder
            .Property(b => b.Id)
            .ValueGeneratedOnAdd();

        builder
            .HasOne(b => b.LicenseGroup)
            .WithMany()
            .HasForeignKey(b => new { b.LicenseGroupId })
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(b => b.Previous)
            .WithMany()
            .HasForeignKey(b => new { b.PreviousId })
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(b => b.Version)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
    }
}