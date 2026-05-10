using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;

namespace Api.Controllers;

public partial class LicensesController
{
    [HttpGet("odata/Licenses/Cost")]
    [EnableQuery(PageSize=5)]
    public IActionResult GetCost()
    {
        return Ok(_context.LicenseCosts);
    }
    
    [HttpGet("odata/Licenses({key})/Cost")]
    [HttpGet("odata/Licenses/{key}/Cost")]
    [EnableQuery]
    public IActionResult GetCost([FromRoute] int key)
    {
        return Ok(SingleResult.Create(_context.LicenseCosts.Where(x => x.Id == key)));
    }
}