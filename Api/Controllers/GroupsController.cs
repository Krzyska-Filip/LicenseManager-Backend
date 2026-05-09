using Api.Requests;
using Licenses.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Api.Controllers;

public class GroupsController(ApplicationDbContext context) : ODataController
{
    [EnableQuery(PageSize=5)]
    public IActionResult Get()
    {
        return Ok(context.Groups);
    }
    
    [EnableQuery]
    public IActionResult Get([FromRoute] int key)
    {
        var entity = context.Groups.FirstOrDefault(x => x.Id == key);

        if (entity == null)
            return NotFound();

        return Ok(entity);
    }
    
    public async Task<IActionResult> Post([FromBody] NewGroupRequests request)
    {
        var entity = new Group
        {
            Name = request.Name,
            MaintainerId = request.MaintainerId,
        };

        context.Groups.Add(entity);
        await context.SaveChangesAsync();

        return Created(entity);
    }
    
    public async Task<IActionResult> Patch([FromRoute] int key, [FromBody] Delta<Group> delta)
    {
        var entity = await context.Groups.FindAsync(key);

        if (entity == null)
            return NotFound();

        delta.Patch(entity);

        await context.SaveChangesAsync();

        return Updated(entity);
    }
    
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        var entity = await context.Groups.FindAsync(key);

        if (entity == null)
            return NotFound();

        context.Groups.Remove(entity);
        await context.SaveChangesAsync();

        return NoContent();
    }
}