namespace SearchService.Clients;

using Microsoft.Extensions.Options;
using MongoDB.Entities;
using SearchService.Clients.Interfaces;
using SearchService.Entities;
using SearchService.Settings;

public class AuctionServiceHttpClient : IAuctionServiceHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly AuctionServiceSettings _auctionSettings;

    public AuctionServiceHttpClient(HttpClient httpClient, IOptionsMonitor<AuctionServiceSettings> auctionOptions)
    {
        _httpClient = httpClient;
        _auctionSettings = auctionOptions.CurrentValue;
    }

    public async Task<IEnumerable<Item>> GetItemsForSearchDb()
    {
        var lastUpdated = await DB.Find<Item, string>()
            .Sort(x => x.Descending(y => y.UpdatedAt))
            .Project(x => x.UpdatedAt.ToString() ?? x.CreatedAt.ToString())
            .ExecuteFirstAsync();

        return await _httpClient.GetFromJsonAsync<IEnumerable<Item>>($"{_auctionSettings.BaseUrl}/auctions?date={lastUpdated}") ?? Array.Empty<Item>();
    }
}

