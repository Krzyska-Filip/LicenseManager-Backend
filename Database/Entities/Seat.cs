using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Entities;

public class Seat : IEntity
{
    public int Id { get; set; }
    public Guid AggregatedId { get; set; }
    public int LicenseId { get; set; }
    public License License { get; set; }
    public int? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }
    public bool ProratedPurchase { get; set; }
    public DateOnly? ValidFrom { get; set; }
    public uint Version { get; set; }
}

public class SeatTypeConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.HasKey(b => new { b.Id });

        builder
            .Property(b => b.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(b => b.AggregatedId)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(b => b.Version)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
    }
}