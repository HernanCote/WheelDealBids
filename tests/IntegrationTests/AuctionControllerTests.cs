namespace IntegrationTests;

using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.Dtos;
using Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Utils;

public class AuctionControllerTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _httpClient;

    public AuctionControllerTests(CustomWebAppFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }
    
    public Task InitializeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAuctions_NoParams_ShouldReturnAllAuctions()
    {
        var response = await _httpClient.GetFromJsonAsync<IList<AuctionDto>>("auctions");

        Assert.NotNull(response);
        Assert.Equal(3, response.Count);
    }

    public Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        DbHelpers.ReinitializeDbForTests(db);
        return Task.CompletedTask;
    }
}