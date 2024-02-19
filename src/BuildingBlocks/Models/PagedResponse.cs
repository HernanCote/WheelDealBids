namespace BuildingBlocks.Models;

public class PagedResponse<T>
{
    public T? Results { get; set; }
    public int CurrentPage { get; set; }
    public int PageCount { get; set; }
    public long TotalCount { get; set; }
}