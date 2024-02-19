namespace SearchService.Managers;

using BuildingBlocks.Models;
using BuildingBlocks.Utils;
using Entities;
using RequestHelpers;

public interface ISearchManager
{
    Task<Result<PagedResponse<IEnumerable<Item>>>> SearchItems(SearchParams searchParams);
}