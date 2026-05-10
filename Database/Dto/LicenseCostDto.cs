using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Dto;

public class LicenseCostDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
    public int Seats { get; set; }
    public decimal CurrentCost { get; set; }
    public decimal RenewalCost { get; set; }
}

public class LicenseCostDtoTypeConfiguration : IEntityTypeConfiguration<LicenseCostDto>
{
    public void Configure(EntityTypeBuilder<LicenseCostDto> builder)
    {
        builder.ToView("LicenseCosts");
        builder.HasKey(x => x.Id);
    }
}