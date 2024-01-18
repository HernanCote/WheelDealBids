namespace SearchService.RequestHelpers;

using Enums;

public class SearchParams
{
    public string? SearchTerm { get; set; } = null;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 4;
    public string? Seller { get; set; } = null;
    public string? Winner { get; set; } = null;
    public string? OrderBy { get; set; } = null;
    public string? FilterBy { get; set; } = null;
}