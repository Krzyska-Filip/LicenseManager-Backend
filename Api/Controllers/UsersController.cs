using Api.Requests;
using Database;
using Database.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Api.Controllers;

public partial class UsersController : ODataController
{
    private readonly ApplicationDbContext _context;
    
    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [EnableQuery(PageSize=5)]
    public IActionResult Get()
    {
        return Ok(_context.Users);
    }
    
    [EnableQuery]
    public IActionResult Get([FromRoute] int key)
    {
        var entity = _context.Users.Where(x => x.Id == key);

        return Ok(SingleResult.Create(entity));
    }
    
    public async Task<IActionResult> Post([FromBody] NewUserRequest request)
    {
        var entity = new User
        {
            Username = request.Username,
            Email = request.Email,
        };

        _context.Users.Add(entity);
        await _context.SaveChangesAsync();

        return Created(entity);
    }
    
    public async Task<IActionResult> Patch([FromRoute] int key, [FromBody] Delta<User> delta)
    {
        var entity = await _context.Users.FindAsync(key);

        if (entity == null)
            return NotFound();

        delta.Patch(entity);

        await _context.SaveChangesAsync();

        return Updated(entity);
    }
    
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        var entity = await _context.Users.FindAsync(key);

        if (entity == null)
            return NotFound();

        _context.Users.Remove(entity);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}