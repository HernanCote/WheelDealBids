namespace SearchService.Controllers;

using Entities;
using Enums;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using RequestHelpers;

[ApiController]
[Route("search")]
public class SearchController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Item>>> SearchItems([FromQuery] SearchParams searchParams)
    {
        var query = DB.PagedSearch<Item, Item>().Sort(x => x.Ascending(a => a.Make));

        if (!string.IsNullOrEmpty(searchParams.SearchTerm))
        {
            query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();
        }
        
        Enum.TryParse<SearchOrderBy>(searchParams.OrderBy?.ToLower(), true, out var orderBy);
        query = orderBy switch
        {
            SearchOrderBy.Make => query.Sort(x => x.Ascending(a => a.Make)),
            SearchOrderBy.New => query.Sort(x => x.Descending(a => a.CreatedAt)),
            _ => query.Sort(x => x.Ascending(a => a.AuctionEnd))
        };

        Enum.TryParse<SearchFilterBy>(searchParams.FilterBy?.ToLower(), true, out var filterBy);
        query = filterBy switch
        {
            SearchFilterBy.Finished => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
            SearchFilterBy.EndingSoon => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6)
                                                          && x.AuctionEnd > DateTime.UtcNow),
            _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
        };

        if (!string.IsNullOrEmpty(searchParams.Seller))
        {
            query.Match(x => x.Seller == searchParams.Seller);
        }
        
        if (!string.IsNullOrEmpty(searchParams.Winner))
        {
            query.Match(x => x.Winner == searchParams.Winner);
        }

        query
            .PageNumber(searchParams.PageNumber)
            .PageSize(searchParams.PageSize);
        
        var result = await query.ExecuteAsync();

        return Ok(new
        {
            results = result.Results,
            currentPage = searchParams.PageNumber,
            pageCount = result.PageCount,
            totalCount = result.TotalCount
        });
    }
}