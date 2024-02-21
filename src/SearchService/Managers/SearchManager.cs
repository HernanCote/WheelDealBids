namespace SearchService.Managers;

using System.Linq.Expressions;
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
        
        ApplySorting(searchParams, query);
        ApplyFilters(searchParams, query);
        ApplyOtherFilters(searchParams, query);
        ApplyPaging(searchParams, query);
        
        var result = await query.ExecuteAsync();

        return BuildPagedResponse(searchParams, result)!;
    }

    private static PagedResponse<IEnumerable<Item>> BuildPagedResponse(SearchParams searchParams,
        (IReadOnlyList<Item> Results, long TotalCount, int PageCount) result)
    {
        if (result.Results.Count == 0)
            return PagedResponse<IEnumerable<Item>>.EmptyPagedResponse();
        
        return PagedResponse<IEnumerable<Item>>.From(result.Results, searchParams.PageNumber, result.PageCount, result.TotalCount);
    }

    private static void ApplyPaging(SearchParams searchParams, PagedSearch<Item, Item> query)
    {
        query.PageNumber(searchParams.PageNumber)
            .PageSize(searchParams.PageSize);
    }

    private static void ApplyOtherFilters(SearchParams searchParams, PagedSearch<Item, Item> query)
    {
        if (!string.IsNullOrEmpty(searchParams.Seller))
        {
            query.Match(x => x.Seller == searchParams.Seller);
        }

        if (!string.IsNullOrEmpty(searchParams.Winner))
        {
            query.Match(x => x.Winner == searchParams.Winner);
        }
    }
    
    private void ApplySorting(SearchParams searchParams, PagedSearch<Item, Item> query)
    {
        if (!string.IsNullOrEmpty(searchParams.OrderBy))
        {
            var sortExpression = GetSortExpression(searchParams.OrderBy);
            query.Sort(sb => sb.Ascending(sortExpression));
        }
        else
        {
            query.Sort(x => x.Ascending(a => a.AuctionEnd));
        }
    }

    private Expression<Func<Item, object>> GetSortExpression(string orderByField)
    {
        Enum.TryParse<SearchOrderBy>(orderByField, true, out var orderBy);
        return orderBy switch
        {
            SearchOrderBy.Make => x => x.Make!,
            SearchOrderBy.New => x => x.CreatedAt,
            SearchOrderBy.EndingSoon => x => x.AuctionEnd,
            SearchOrderBy.Mileage => x => x.Mileage,
            _ => x => x.AuctionEnd
        };
    }

    private void ApplyFilters(SearchParams searchParams, PagedSearch<Item, Item> query)
    {
        if (!string.IsNullOrEmpty(searchParams.FilterBy))
        {
            var filterExpression = GetFilterExpression(searchParams.FilterBy);
            query.Match(filterExpression);
        }
        else
        {
            query.Match(x => x.AuctionEnd > DateTime.UtcNow);
        }
    }

    private Expression<Func<Item, bool>> GetFilterExpression(string filterBy)
    {
        Enum.TryParse<SearchFilterBy>(filterBy, true, out var filter);
        return filter switch
        {
            SearchFilterBy.Finished => x => x.AuctionEnd < DateTime.UtcNow,
            SearchFilterBy.EndingSoon => x => x.AuctionEnd < DateTime.UtcNow.AddHours(6) 
                                              && x.AuctionEnd > DateTime.UtcNow,
            _ => x => x.AuctionEnd > DateTime.UtcNow
        };
    }
}