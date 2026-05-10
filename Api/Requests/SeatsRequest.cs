namespace Api.Requests;

public class NewSeatRequest
{
    public int? AssignedToId { get; set; } = null;
    public bool ProratedPurchase { get; set; }
    public DateOnly ValidFrom { get; set; }
}

public class UpdateSeatRequest
{
    public int LicenseId { get; set; }
    public int? AssignedToId { get; set; } = null;
    public bool ProratedPurchase { get; set; }
    public DateOnly ValidFrom { get; set; }
}