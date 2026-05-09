namespace Api.Requests;

public class NewGroupRequests
{
    public string Name { get; set; }
    public int? MaintainerId { get; set; } = null;
}

public class UpdateGroupRequest
{
    public string Name { get; set; }
    public int? MaintainerId { get; set; } = null;
}