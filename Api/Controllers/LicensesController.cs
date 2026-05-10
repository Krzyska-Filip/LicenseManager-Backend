using Api.Requests;
using Licenses.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Api.Controllers;

public partial class LicensesController(ApplicationDbContext context) : ODataController
{
    [EnableQuery(PageSize=5)]
    public IActionResult Get()
    {
        return Ok(context.Licenses);
    }
    
    [EnableQuery]
    public IActionResult Get([FromRoute] int key)
    {
        var entity = context.Licenses.FirstOrDefault(x => x.Id == key);

        if (entity == null)
            return NotFound();

        return Ok(entity);
    }
    
    public async Task<IActionResult> Post([FromBody] NewLicenseRequest request)
    {
        var entity = new License
        {
            GroupId = request.GroupId,
            PreviousId = request.PreviousId,
            Name = request.Name,
            Type = request.Type,
            IsProrated = request.IsProrated,
            PricePerSeat = request.PricePerSeat,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
        };
        
        // TODO: renewal
        // TODO: create n seats

        context.Licenses.Add(entity);
        await context.SaveChangesAsync();

        return Created(entity);
    }
    
    public async Task<IActionResult> Patch([FromRoute] int key, [FromBody] Delta<License> delta)
    {
        var entity = await context.Licenses.FindAsync(key);

        if (entity == null)
            return NotFound();

        delta.Patch(entity);

        await context.SaveChangesAsync();

        return Updated(entity);
    }
    
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        var entity = await context.Licenses.FindAsync(key);

        if (entity == null)
            return NotFound();

        context.Licenses.Remove(entity);
        await context.SaveChangesAsync();

        return NoContent();
    }
}