namespace Api.Requests;

public class NewUserRequest
{
    public string Username { get; set; }
    public string Email { get; set; }
}

public class UpdateUserRequest
{
    public string Username { get; set; }
    public string Email { get; set; }
}

public class AssignMultipleLicensesRequest
{
    public List<int> Ids { get; set; } = new();
}