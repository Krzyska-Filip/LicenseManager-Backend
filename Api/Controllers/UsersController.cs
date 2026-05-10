using Api.Requests;
using Database;
using Database.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Api.Controllers;

public class UsersController(ApplicationDbContext context) : ODataController
{
    [EnableQuery(PageSize=5)]
    public IActionResult Get()
    {
        return Ok(context.Users);
    }
    
    [EnableQuery]
    public IActionResult Get([FromRoute] int key)
    {
        var entity = context.Users.Where(x => x.Id == key);

        return Ok(SingleResult.Create(entity));
    }
    
    public async Task<IActionResult> Post([FromBody] NewUserRequest request)
    {
        var entity = new User
        {
            Username = request.Username,
            Email = request.Email,
        };

        context.Users.Add(entity);
        await context.SaveChangesAsync();

        return Created(entity);
    }
    
    public async Task<IActionResult> Patch([FromRoute] int key, [FromBody] Delta<User> delta)
    {
        var entity = await context.Users.FindAsync(key);

        if (entity == null)
            return NotFound();

        delta.Patch(entity);

        await context.SaveChangesAsync();

        return Updated(entity);
    }
    
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        var entity = await context.Users.FindAsync(key);

        if (entity == null)
            return NotFound();

        context.Users.Remove(entity);
        await context.SaveChangesAsync();

        return NoContent();
    }
}