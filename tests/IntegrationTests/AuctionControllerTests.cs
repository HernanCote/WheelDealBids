namespace IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.Dtos;
using Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Utils;

[Collection("Shared Collection")]
public class AuctionControllerTests : IAsyncLifetime
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
        
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        var auctionsInDb = await db.Auctions.ToListAsync();
        
        Assert.Equal(auctionsInDb.Count, response.Count);
    }
    
    [Fact]
    public async Task GetAuctionById_WithValidId_ShouldReturnAuction()
    {
        var response = await _httpClient.GetFromJsonAsync<AuctionDto>($"auctions/{DbHelpers.GT_ID}");

        Assert.NotNull(response);
        Assert.Equal(DbHelpers.GT_ID, response.Id.ToString());
        Assert.Equal("Ford", response.Make);
        Assert.Equal("GT", response.Model);
    }
    
    [Fact]
    public async Task GetAuctionById_WithInvalidId_ShouldReturnNotFound()
    {
        var response = await _httpClient.GetAsync($"auctions/{Guid.NewGuid()}");

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetAuctionById_WithInvalidGuid_ShouldReturnBadRequest()
    {
        var response = await _httpClient.GetAsync("auctions/noguid");

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateAuction_WithWithNoAuth_ShouldReturnNotAuthorized()
    {
        var auction = new CreateAuctionDto
        {
            Make = "Ford"
        };
        
        var response = await _httpClient.PostAsJsonAsync("auctions", auction);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateAuction_WithWithAuth_ShouldReturnCreated()
    {
        var auction = GetAuctionForCreate();
        var username = "Hernan";
        _httpClient.SetFakeJwtBearerToken(AuthHelpers.GetBearerForUser(username));
        
        var response = await _httpClient.PostAsJsonAsync("auctions", auction);

        Assert.NotNull(response);
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var createdAuction = await response.Content.ReadFromJsonAsync<AuctionDto>();
        
        Assert.NotNull(createdAuction);
        Assert.Equal(username, createdAuction.Seller);
        Assert.Equal(auction.Model, createdAuction.Model);
        Assert.Equal(auction.Make, createdAuction.Make);
    }
    
    [Fact]
    public async Task CreateAuction_WithWithAuthAndInvalidModel_ShouldReturnBadRequest()
    {
        var auction = GetAuctionForCreate();
        auction.Model = null;
        _httpClient.SetFakeJwtBearerToken(AuthHelpers.GetBearerForUser("Hernan"));
        
        var response = await _httpClient.PostAsJsonAsync("auctions", auction);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateAuction_WithWithAuthAndInvalidMake_ShouldReturnBadRequest()
    {
        var auction = GetAuctionForCreate();
        auction.Make = null;
        _httpClient.SetFakeJwtBearerToken(AuthHelpers.GetBearerForUser("Hernan"));
        
        var response = await _httpClient.PostAsJsonAsync("auctions", auction);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateAuction_WithWithAuthAndInvalidYear_ShouldReturnBadRequest()
    {
        var auction = GetAuctionForCreate();
        auction.Year = null;
        _httpClient.SetFakeJwtBearerToken(AuthHelpers.GetBearerForUser("Hernan"));
        
        var response = await _httpClient.PostAsJsonAsync("auctions", auction);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateAuction_WithWithAuthAndInvalidColor_ShouldReturnBadRequest()
    {
        var auction = GetAuctionForCreate();
        auction.Color = null;
        _httpClient.SetFakeJwtBearerToken(AuthHelpers.GetBearerForUser("Hernan"));
        
        var response = await _httpClient.PostAsJsonAsync("auctions", auction);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateAuction_WithWithAuthAndValidMileage_ShouldReturnBadRequest()
    {
        var auction = GetAuctionForCreate();
        auction.Mileage = 0;
        _httpClient.SetFakeJwtBearerToken(AuthHelpers.GetBearerForUser("Hernan"));
        
        var response = await _httpClient.PostAsJsonAsync("auctions", auction);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateAuction_WithWithAuthAndInvalidImageUrl_ShouldReturnBadRequest()
    {
        var auction = GetAuctionForCreate();
        auction.ImageUrl = null;
        _httpClient.SetFakeJwtBearerToken(AuthHelpers.GetBearerForUser("Hernan"));
        
        var response = await _httpClient.PostAsJsonAsync("auctions", auction);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithNoAuth_ShouldReturnNotAuthorized()
    {
        var auction = new UpdateAuctionDto
        {
            Make = "Ford"
        };
        
        var response = await _httpClient.PutAsJsonAsync($"auctions/{DbHelpers.GT_ID}", auction);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateAuction_WithAuthAndInvalidId_ShouldReturnNotFound()
    {
        var auction = new UpdateAuctionDto
        {
            Make = "Ford"
        };
        _httpClient.SetFakeJwtBearerToken(AuthHelpers.GetBearerForUser("Hernan"));
        
        var response = await _httpClient.PutAsJsonAsync($"auctions/{Guid.NewGuid()}", auction);

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithAuthAndValidData_ShouldReturnNoContent()
    {
        var auction = new UpdateAuctionDto
        {
            Make = "FordUpdated",
            Model = "GTUpdated",
            Year = 2021,
            Color = "RedUpd",
            Mileage = 10
        };
        _httpClient.SetFakeJwtBearerToken(AuthHelpers.GetBearerForUser("bob"));
        
        var response = await _httpClient.PutAsJsonAsync($"auctions/{DbHelpers.GT_ID}", auction);

        Assert.NotNull(response);
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteAuction_WithNoAuth_ShouldReturnNotAuthorized()
    {
        var response = await _httpClient.DeleteAsync($"auctions/{DbHelpers.GT_ID}");

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateAuction_WithNoAuth_ShouldReturn401()
    {
        // arrange? 
        var auction = new CreateAuctionDto { Make = "test" };

        // act
        var response = await _httpClient.PostAsJsonAsync($"auctions", auction);

        // assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateAuction_WithAuth_ShouldReturn201()
    {
        // arrange? 
        var auction = GetAuctionForCreate();
        _httpClient.SetFakeJwtBearerToken(AuthHelpers.GetBearerForUser("bob"));

        // act
        var response = await _httpClient.PostAsJsonAsync($"auctions", auction);

        // assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdAuction = await response.Content.ReadFromJsonAsync<AuctionDto>();
        Assert.Equal("bob", createdAuction.Seller);
    }

    [Fact]
    public async Task CreateAuction_WithInvalidCreateAuctionDto_ShouldReturn400()
    {
        // arrange? 
        var auction = GetAuctionForCreate();
        auction.Make = null;
        _httpClient.SetFakeJwtBearerToken(AuthHelpers.GetBearerForUser("bob"));

        // act
        var response = await _httpClient.PostAsJsonAsync($"auctions", auction);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndUser_ShouldReturn200()
    {
        // arrange? 
        var updateAuction = new UpdateAuctionDto { Make = "Updated" };
        _httpClient.SetFakeJwtBearerToken(AuthHelpers.GetBearerForUser("bob"));

        // act
        var response = await _httpClient.PutAsJsonAsync($"auctions/{DbHelpers.GT_ID}", updateAuction);

        // assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndInvalidUser_ShouldReturn403()
    {
        // arrange? 
        var updateAuction = new UpdateAuctionDto { Make = "Updated" };
        _httpClient.SetFakeJwtBearerToken(AuthHelpers.GetBearerForUser("notbob"));

        // act
        var response = await _httpClient.PutAsJsonAsync($"auctions/{DbHelpers.GT_ID}", updateAuction);

        // assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
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

    public Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        DbHelpers.ReinitializeDbForTests(db);
        return Task.CompletedTask;
    }
}