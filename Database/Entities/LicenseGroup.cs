using Licenses.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class LicenseGroup : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int? MaintainerId { get; set; }
    public User? Maintainer { get; set; }
    public uint Version { get; set; }
}

public class LicenseGroupTypeConfiguration : IEntityTypeConfiguration<LicenseGroup>
{
    public void Configure(EntityTypeBuilder<LicenseGroup> builder)
    {
        builder.HasKey(b => new { b.Id });

        builder
            .Property(b => b.Id)
            .ValueGeneratedOnAdd();

        builder
            .HasOne(b => b.Maintainer)
            .WithMany()
            .HasForeignKey(b => new { b.MaintainerId })
            .OnDelete(DeleteBehavior.ClientSetNull);
        
        builder.Property(b => b.Version)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
    }
}