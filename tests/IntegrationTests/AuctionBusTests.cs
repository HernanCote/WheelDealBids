namespace IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.Dtos;
using BuildingBlocks.Contracts;
using Fixtures;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Utils;

[Collection("Shared Collection")]
public class AuctionBusTests : IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _httpClient;
    private readonly ITestHarness _testHarness;

    public AuctionBusTests(CustomWebAppFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
        _testHarness = factory.Services.GetTestHarness();
    }

    [Fact]
    public async Task CreateAuction_WithValidObject_ShouldPublishAuctionCreated()
    {
        var auction = GetAuctionForCreate();

        _httpClient.SetFakeJwtBearerToken(AuthHelpers.GetBearerForUser("bob"));

        var response = await _httpClient.PostAsJsonAsync("auctions", auction);

        response.EnsureSuccessStatusCode();
        Assert.True(await _testHarness.Published.Any<AuctionCreated>());
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        DbHelpers.ReinitializeDbForTests(db);
        return Task.CompletedTask;
    }
    
    private CreateAuctionDto GetAuctionForCreate()
    {
        return new CreateAuctionDto
        {
            Make = "test",
            Model = "testModel",
            ImageUrl = "test",
            Color = "test",
            Mileage = 10,
            Year = 2021
        };
    }
}