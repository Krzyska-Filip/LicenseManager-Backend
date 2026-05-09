using Api.Requests;
using Licenses.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Api.Controllers;

public class SeatController(ApplicationDbContext context) : ODataController
{
    [EnableQuery(PageSize=5)]
    public IActionResult Get()
    {
        return Ok(context.Seats);
    }
    
    [EnableQuery]
    public IActionResult Get([FromRoute] int key)
    {
        var entity = context.Seats.FirstOrDefault(x => x.Id == key);

        if (entity == null)
            return NotFound();

        return Ok(entity);
    }
    
    public async Task<IActionResult> Post([FromBody] NewSeatRequest request)
    {
        var entity = new Seat
        {
            LicenseId = request.LicenseId,
            AssignedToId = request.AssignedToId,
            ProratedPurchase = request.ProratedPurchase,
            ValidFrom = request.ValidFrom,
        };

        context.Seats.Add(entity);
        await context.SaveChangesAsync();

        return Created(entity);
    }
    
    public async Task<IActionResult> Patch([FromRoute] int key, [FromBody] Delta<Seat> delta)
    {
        var entity = await context.Seats.FindAsync(key);

        if (entity == null)
            return NotFound();

        delta.Patch(entity);

        await context.SaveChangesAsync();

        return Updated(entity);
    }
    
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        var entity = await context.Seats.FindAsync(key);

        if (entity == null)
            return NotFound();

        context.Seats.Remove(entity);
        await context.SaveChangesAsync();

        return NoContent();
    }
}