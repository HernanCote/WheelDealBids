namespace SearchService.Services.Interfaces;

using Entities;

public interface IAuctionServiceHttpClient
{
    public Task<IEnumerable<Item>> GetItemsForSearchDb();
}