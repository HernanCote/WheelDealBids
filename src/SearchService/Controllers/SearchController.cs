namespace SearchService.Controllers;

using Entities;
using Enums;
using Managers;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using RequestHelpers;

[ApiController]
[Route("search")]
public class SearchController : ControllerBase
{
    private readonly ISearchManager _searchManager;

    public SearchController(ISearchManager searchManager)
    {
        _searchManager = searchManager;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Item>>> SearchItems([FromQuery] SearchParams searchParams)
    {
        var result = await _searchManager.SearchItems(searchParams);

        if (result.NotSucceeded)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }
}