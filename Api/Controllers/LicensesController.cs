using Api.Requests;
using Licenses.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

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
        
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            if (request.PreviousId is not null)
            {
                var renewalStatus = await HandleRenewal(request.PreviousId);
                if (renewalStatus is not OkResult)
                {
                    await transaction.RollbackAsync();
                    return renewalStatus;
                }
            }
            
            context.Licenses.Add(entity);
            await context.SaveChangesAsync();

            if (request.Seats > 0)
            {
                var seats = request.PreviousId is not null ?
                    await CloneSeats(request.PreviousId, entity.Id, request.Seats, request.ValidFrom) :
                    CreateSeats(entity.Id, request.Seats, request.ValidFrom);
                
                await context.Seats.AddRangeAsync(seats);
                await context.SaveChangesAsync();
            }
            
            await transaction.CommitAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync();
            return Conflict("License was modified by another request, please try again.");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

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

    private async Task<IActionResult> HandleRenewal(int? previousId)
    {
        var previousLicense = await context.Licenses.FindAsync(previousId);

        if (previousLicense is null)
            return BadRequest($"License {previousId} does not exist.");

        if (previousLicense.IsRenewed)
            return Conflict($"License {previousId} has already been renewed.");

        previousLicense.IsRenewed = true;

        return Ok();
    }

    private List<Seat> CreateSeats(int newId, int seats, DateOnly? validFrom)
    {
        List<Seat> newSeats = new List<Seat>();
        for (int i = 0; i < seats; i++)
        {
            newSeats.Add(new Seat
            {
                LicenseId = newId,
                ProratedPurchase = false,
                ValidFrom = validFrom,
            });
        }

        return newSeats;
    }
    
    private async Task<List<Seat>> CloneSeats(int? previousId, int newId, int seats, DateOnly? validFrom)
    {
        List<Seat> newSeats = new List<Seat>();
        List<Seat> seatsInExistingLicense = await context.Seats
            .Where(x => x.LicenseId == previousId)
            .ToListAsync();
            
        int i = 0;
        for (; i < seats && i < seatsInExistingLicense.Count; i++)
        {
            var item = new Seat();
            item.LicenseId = newId;
            item.AssignedToId = seatsInExistingLicense[i].AssignedToId;
            item.AggregatedId = seatsInExistingLicense[i].AggregatedId;
            item.ValidFrom = validFrom;
            newSeats.Add(item);
        }

        for (; i < seats; i++)
        {
            var item = new Seat();
            item.LicenseId = newId;
            item.ValidFrom = validFrom;
            newSeats.Add(item);
        }

        return newSeats;
    }
}