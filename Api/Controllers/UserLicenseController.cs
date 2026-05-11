using Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

public partial class UsersController : ODataController
{
    [EnableQuery]
    [HttpGet("odata/Users({key})/Licenses")]
    [HttpGet("odata/Users/{key}/Licenses")]
    public IActionResult GetLicensesFromUser([FromRoute] int key)
    {
        return Ok(_context.Users.Where(u => u.Id == key).SelectMany(u => u.Licenses));
    }

    [EnableQuery]
    [HttpGet("odata/Users({key})/Licenses({relatedKey})")]
    [HttpGet("odata/Users/{key}/Licenses/{relatedKey}")]
    public IActionResult GetLicenseFromUser([FromRoute] int key, [FromRoute] int relatedKey)
    {
        var license = _context.Users
            .Where(u => u.Id == key)
            .SelectMany(u => u.Licenses)
            .Where(l => l.Id == relatedKey);

        return Ok(SingleResult.Create(license));
    }

    [HttpPost("odata/Users({key})/Licenses({relatedKey})")]
    [HttpPost("odata/Users/{key}/Licenses/{relatedKey}")]
    public async Task<IActionResult> PostLicenseToUser(
        [FromRoute] int key,
        [FromRoute] int relatedKey
        )
    {
        var user = await _context.Users.FindAsync(key);
        if (user is null)
            return NotFound("User not found");

        var license = await _context.Licenses
            .Include(x => x.Seats)
            .FirstOrDefaultAsync(x => x.Id == relatedKey);

        if (license is null)
            return NotFound("License not found");

        var firstFree = license.Seats.FirstOrDefault(x => x.AssignedToId is null);

        if (firstFree is null)
            return NotFound("No free seat found");

        firstFree.AssignedToId = user.Id;

        await _context.SaveChangesAsync();

        return Created(firstFree);
    }
    
    [HttpDelete("odata/Users({key})/Licenses({relatedKey})")]
    [HttpDelete("odata/Users/{key}/Licenses/{relatedKey}")]
    public async Task<IActionResult> DeleteLicenseFromUser(
        [FromRoute] int key,
        [FromRoute] int relatedKey
    )
    {
        var user = await _context.Users
            .Include(x => x.Seats)
            .FirstOrDefaultAsync(x => x.Id == key);
        
        if (user is null)
            return NotFound("User not found");

        var seat = user.Seats
            .FirstOrDefault(x => x.LicenseId == relatedKey);

        if (seat is null)
            return NotFound("User has not been assigned a license");
        
        seat.AssignedToId = null;

        await _context.SaveChangesAsync();

        return Ok(seat);
    }
}
