namespace SearchService.Managers;

using BuildingBlocks.Models;
using BuildingBlocks.Utils;
using Entities;
using Enums;
using MongoDB.Entities;
using RequestHelpers;

public class SearchManager : ISearchManager
{
    public async Task<Result<PagedResponse<IEnumerable<Item>>>> SearchItems(SearchParams searchParams)
    {
        var query = DB.PagedSearch<Item, Item>();

        if (!string.IsNullOrEmpty(searchParams.SearchTerm))
        {
            query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();
        }

        if (Enum.TryParse<SearchOrderBy>(searchParams.OrderBy, true, out var orderBy))
        {
            query = orderBy switch
            {
                SearchOrderBy.Make => query.Sort(x => x.Ascending(a => a.Make))
                    .Sort(x => x.Ascending(a => a.Model)),
                SearchOrderBy.New => query.Sort(x => x.Descending(a => a.CreatedAt)),
                SearchOrderBy.EndingSoon => query.Sort(x => x.Ascending(a => a.AuctionEnd)),
                SearchOrderBy.Mileage => query.Sort(x => x.Ascending(a => a.Mileage)),
                _ => query.Sort(x => x.Ascending(a => a.AuctionEnd))
            };
        }
        else
        {
            query.Sort(x => x.Ascending(a => a.AuctionEnd));
        }

        if (Enum.TryParse<SearchFilterBy>(searchParams.FilterBy, true, out var filterBy))
        {
            query = filterBy switch
            {
                SearchFilterBy.Finished => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
                SearchFilterBy.EndingSoon => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6)
                                                              && x.AuctionEnd > DateTime.UtcNow),
                _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
            };
        }
        else
        {
            query = query.Match(x => x.AuctionEnd > DateTime.UtcNow);
        }

        if (!string.IsNullOrEmpty(searchParams.Seller))
        {
            query.Match(x => x.Seller == searchParams.Seller);
        }
        
        if (!string.IsNullOrEmpty(searchParams.Winner))
        {
            query.Match(x => x.Winner == searchParams.Winner);
        }

        query.PageNumber(searchParams.PageNumber)
            .PageSize(searchParams.PageSize);
        
        var result = await query.ExecuteAsync();

        var response = new PagedResponse<IEnumerable<Item>>
        {
            Results = result.Results,
            CurrentPage = searchParams.PageNumber,
            PageCount = result.PageCount,
            TotalCount = result.TotalCount
        };
        
        return response!;
    }
}