namespace BuildingBlocks.Models;

public class PagedResponse<T>
{
    public T? Results { get; set; }
    public int CurrentPage { get; set; }
    public int PageCount { get; set; }
    public long TotalCount { get; set; }
    
    public static PagedResponse<T> From(T? results, int currentPage, int pageCount, long totalCount)
    {
        return new PagedResponse<T>()
        {
            Results = results,
            CurrentPage = currentPage,
            PageCount = pageCount,
            TotalCount = totalCount
        };
    }
    
    public static PagedResponse<T> EmptyPagedResponse()
    {
        return new PagedResponse<T>()
        {
            Results = default,
            CurrentPage = 1,
            PageCount = 1,
            TotalCount = 0
        };
    }
}