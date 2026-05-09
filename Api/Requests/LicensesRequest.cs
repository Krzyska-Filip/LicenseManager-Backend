using Microsoft.OData.Edm;

namespace Api.Requests;

public class NewLicenseRequest
{
    public string Name { get; set; }
    public string Type { get; set; }
    public decimal PricePerSeat { get; set; }
    public bool IsProrated { get; set; }
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
    public int Seats { get; set; }
    
    public int GroupId { get; set; }
    public int? PreviousId { get; set; } = null;
}

public class UpdateLicenseRequest
{
    public string Name { get; set; }
    public string Type { get; set; }
    public decimal PricePerSeat { get; set; }
    public bool IsProrated { get; set; }
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
    public int Seats { get; set; }
    
    public int GroupId { get; set; }
    public int? PreviousId { get; set; } = null;
}