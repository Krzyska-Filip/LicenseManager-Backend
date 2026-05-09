namespace Licenses.Database.Entities;

public interface IEntity
{
    public int Id { get; set;}
    public uint Version { get; set; }
}