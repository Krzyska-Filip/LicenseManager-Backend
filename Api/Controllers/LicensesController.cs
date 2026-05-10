using System.Net;
using System.Text.Json;
using Api.Requests;
using Api.Services;
using Database;
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
    private readonly ApplicationDbContext _context;
    private readonly IIdempotencyKeyService _idempotency;

    public LicensesController(ApplicationDbContext context, IIdempotencyKeyService idempotency)
    {
        _context = context;
        _idempotency = idempotency;
    }


    [EnableQuery(PageSize=5)]
    public IActionResult Get()
    {
        return Ok(_context.Licenses);
    }
    
    [EnableQuery]
    public IActionResult Get([FromRoute] int key)
    {
        var entity = _context.Licenses.Where(x => x.Id == key);
        
        var etag = Request.Headers.IfNoneMatch.FirstOrDefault();
        if (etag is not null)
        {
            var version = entity.Select(x => x.Version).First();
            if (etag == version.ToString())
                return StatusCode(304);
        }

        return Ok(SingleResult.Create(entity));
    }
    
    public async Task<IActionResult> Post([FromBody] NewLicenseRequest request)
    {
        var idempotencyKeyHeader = Request.Headers["Idempotency-Key"].FirstOrDefault();
        if (String.IsNullOrEmpty(idempotencyKeyHeader))
        {
            return BadRequest("Idempotency-Key Required");
        }
        
        var idempotencyKey = _idempotency.Get(idempotencyKeyHeader);
        if (idempotencyKey is not null)
        {
            return idempotencyKey.Response;
        }
        
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
        
        await using var transaction = await _context.Database.BeginTransactionAsync();
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

            _context.Licenses.Add(entity);
            await _context.SaveChangesAsync();

            if (request.Seats > 0)
            {
                var seats = request.PreviousId is not null
                    ? await CloneSeats(request.PreviousId, entity.Id, request.Seats, request.ValidFrom)
                    : CreateSeats(entity.Id, request.Seats, request.ValidFrom);

                await _context.Seats.AddRangeAsync(seats);
                await _context.SaveChangesAsync();
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
        
        _idempotency.Set(
            idempotencyKeyHeader,
            new IdempotencyBody
            {
                Response = Created(entity),
                CreatedAt = DateTime.UtcNow
            });

        return Created(entity);
    }
    
    public async Task<IActionResult> Patch([FromRoute] int key, [FromBody] Delta<License> delta)
    {
        var ifMatch = Request.Headers.IfMatch.FirstOrDefault();
        if (string.IsNullOrEmpty(ifMatch))
            return StatusCode(StatusCodes.Status428PreconditionRequired);
        
        var entity = await _context.Licenses.FindAsync(key);
        
        if (entity == null)
            return NotFound();
        
        if (ifMatch != entity.Version.ToString())
            return StatusCode(StatusCodes.Status412PreconditionFailed);

        delta.Patch(entity);

        await _context.SaveChangesAsync();

        return Updated(entity);
    }
    
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        var entity = await _context.Licenses.FindAsync(key);

        if (entity == null)
            return NotFound();

        _context.Licenses.Remove(entity);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpGet("odata/Licenses({key})/History")]
    [HttpGet("odata/Licenses/{key}/History")]
    [EnableQuery]
    public IActionResult GetHistory([FromRoute] int key)
    {
        var sql = 
              """
                  WITH RECURSIVE "LicenseChain" AS (
                      SELECT *, xmin FROM "Licenses"
                      WHERE "Id" = {0}
                      
                      UNION ALL
                      
                      SELECT l.*, xmin FROM "Licenses" l
                      INNER JOIN "LicenseChain" lc ON l."Id" = lc."PreviousId"
                  )
                  SELECT * FROM "LicenseChain"
              """;
        
        var items = _context.Licenses
            .FromSqlRaw(sql, key)
            .AsNoTracking();

        return Ok(items);
    }

    private async Task<IActionResult> HandleRenewal(int? previousId)
    {
        var previousLicense = await _context.Licenses.FindAsync(previousId);

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
        List<Seat> seatsInExistingLicense = await _context.Seats
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