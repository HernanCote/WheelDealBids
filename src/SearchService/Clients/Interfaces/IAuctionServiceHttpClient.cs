namespace SearchService.Clients.Interfaces;

using Entities;

public interface IAuctionServiceHttpClient
{
    public Task<IEnumerable<Item>> GetItemsForSearchDb();
}