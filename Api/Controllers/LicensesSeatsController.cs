using Api.Requests;
using Database.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

public partial class LicensesController : ODataController
{
    [EnableQuery(PageSize = 5)]
    [HttpGet("odata/Licenses({key})/Seats")]
    [HttpGet("odata/Licenses/{key}/Seats")]
    public IActionResult GetSeatsFromLicense([FromRoute] int key)
    {
        var seats = _context.Seats.Where(s => s.LicenseId == key);
        return Ok(seats);
    }

    [EnableQuery]
    [HttpGet("odata/Licenses({key})/Seats({relatedKey})")]
    [HttpGet("odata/Licenses/{key}/Seats/{relatedKey}")]
    public IActionResult GetSeatFromLicense(
        [FromRoute] int key,
        [FromRoute] int relatedKey)
    {
        var seats = _context.Seats
            .Where(s => s.LicenseId == key && s.Id == relatedKey);

        return Ok(SingleResult.Create(seats));
    }
    
    [HttpPost("odata/Licenses({key})/Seats")]
    [HttpPost("odata/Licenses/{key}/Seats")]
    public async Task<IActionResult> PostSeatToLicense(
        [FromRoute] int key,
        [FromBody] NewSeatRequest request)
    {
        var entity = new Seat
        {
            LicenseId = key,
            AssignedToId = request.AssignedToId,
            ProratedPurchase = request.ProratedPurchase,
            ValidFrom = request.ValidFrom,
        };

        _context.Seats.Add(entity);
        await _context.SaveChangesAsync();

        return Created(entity);
    }

    [HttpPatch("odata/Licenses({key})/Seats({relatedKey})")]
    [HttpPatch("odata/Licenses/{key}/Seats/{relatedKey}")]
    public async Task<IActionResult> PatchSeatInLicense(
        [FromRoute] int key,
        [FromRoute] int relatedKey,
        [FromBody] Delta<Seat> delta)
    {
        var entity = await _context.Seats
            .FirstOrDefaultAsync(s => s.LicenseId == key && s.Id == relatedKey);

        if (entity == null)
            return NotFound();

        delta.Patch(entity);
        await _context.SaveChangesAsync();

        return Updated(entity);
    }

    [HttpDelete("odata/Licenses({key})/Seats({relatedKey})")]
    [HttpDelete("odata/Licenses/{key}/Seats/{relatedKey}")]
    public async Task<IActionResult> DeleteSeatFromLicense(
        [FromRoute] int key,
        [FromRoute] int relatedKey)
    {
        var entity = await _context.Seats
            .FirstOrDefaultAsync(s => s.LicenseId == key && s.Id == relatedKey);

        if (entity == null)
            return NotFound();

        _context.Seats.Remove(entity);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}