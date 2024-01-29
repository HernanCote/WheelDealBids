namespace UnitTests;

using AuctionService.Controllers;
using AuctionService.Data.Repo;
using AuctionService.Dtos;
using AuctionService.Entities;
using AuctionService.RequestHelpers;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utils;

public class AuctionControllerTests
{
    private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
    private readonly IMapper _mapperMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly Mock<ILogger<AuctionsController>> _loggerMock;
    private readonly Fixture _fixture;

    private readonly AuctionsController _sut;
    
    public AuctionControllerTests()
    {
        _fixture = new Fixture();
        _auctionRepositoryMock = new Mock<IAuctionRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _loggerMock = new Mock<ILogger<AuctionsController>>();

        var mockMapper = new MapperConfiguration(x =>
        {
            x.AddMaps(typeof(MappingProfiles).Assembly);
        }).CreateMapper().ConfigurationProvider;

        _mapperMock = new Mapper(mockMapper); 

        _sut = new AuctionsController(
            _auctionRepositoryMock.Object, 
            _mapperMock, 
            _publishEndpointMock.Object,
            _loggerMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = Helpers.GetClaimsPrincipal()
                }
            }
        };
    }
    
    [Theory]
    [InlineData(10)]
    public async Task GetAuctions_WithNoParams_ReturnsOk(int numberOfDataToCreate)
    {
        var auctions = _fixture.CreateMany<AuctionDto>(numberOfDataToCreate).ToList();
        
        _auctionRepositoryMock.Setup(x => x.GetAuctions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(auctions);
        
        var result = await _sut.GetAuctions(null, CancellationToken.None);
        
        Assert.IsType<ActionResult<IEnumerable<AuctionDto>>>(result);
        Assert.Equal(10, result.Value!.Count());
    }
    
    [Fact]
    public async Task GetAuctions_WithDateParam_ReturnsOk()
    {
        var auctions = _fixture.CreateMany<AuctionDto>(10).ToList();
        
        _auctionRepositoryMock.Setup(x => x.GetAuctions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(auctions);
        
        var result = await _sut.GetAuctions("2021-01-01", CancellationToken.None);
        
        Assert.IsType<ActionResult<IEnumerable<AuctionDto>>>(result);
        Assert.Equal(10, result.Value!.Count());
    }

    [Fact]
    public async Task GetAuctionById_WithValidId_ReturnsAuction()
    {
        var auction = _fixture.Create<AuctionDto>();
        _auctionRepositoryMock.Setup(repo => repo.GetAuctionById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(auction);

        var result = await _sut.GetAuctionById(auction.Id);
        
        Assert.Equal(auction.Make, result.Value!.Make);
        Assert.IsType<ActionResult<AuctionDto>>(result);
    }
    
    [Fact]
    public async Task GetAuctionById_WithInvalidId_ReturnsNotFound()
    {
        _auctionRepositoryMock.Setup(repo => repo.GetAuctionById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AuctionDto) null!);

        var result = await _sut.GetAuctionById(Guid.NewGuid());
        
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateAuction_WithValidCreateAuctionDto_ReturnsCreatedAtAction()
    {
        var auction = _fixture.Create<CreateAuctionDto>();
        _auctionRepositoryMock.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
        _auctionRepositoryMock.Setup(repo => repo.SaveChanges(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var result = await _sut.CreateAuction(auction);
        var createdResult = result.Result as CreatedAtActionResult;
        
        Assert.NotNull(createdResult);
        Assert.Equal("GetAuctionById", createdResult.ActionName);
        Assert.IsType<AuctionDto>(createdResult.Value);
    }
    
    [Fact]
    public async Task CreateAuction_WithInvalidCreateAuctionDto_ReturnsBadRequest()
    {
        var auction = _fixture.Create<CreateAuctionDto>();
        _sut.ModelState.AddModelError("Make", "Required");
        
        var result = await _sut.CreateAuction(auction);
        
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
    
    [Fact]
    public async Task UpdateAuction_WithValidUpdateAuctionDto_ReturnsOk()
    {
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        auction.Seller = "test";
        
        var updateAuctionDto = _fixture.Create<UpdateAuctionDto>();
        _auctionRepositoryMock.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(auction);
        _auctionRepositoryMock.Setup(repo => repo.SaveChanges(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var result = await _sut.UpdateAuction(auction.Id, updateAuctionDto);
        
        Assert.IsType<NoContentResult>(result);
    }
    
    [Fact]
    public async Task UpdateAuction_WithInvalidUpdateAuctionDto_ReturnsBadRequest()
    {
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        auction.Seller = "test";
        
        var updateAuctionDto = _fixture.Create<UpdateAuctionDto>();
        _auctionRepositoryMock.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(auction);
        _sut.ModelState.AddModelError("Make", "Required");
        
        var result = await _sut.UpdateAuction(auction.Id, updateAuctionDto);
        
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async Task UpdateAuction_WithInvalidId_ReturnsNotFound()
    {
        var updateAuctionDto = _fixture.Create<UpdateAuctionDto>();
        _auctionRepositoryMock.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Auction) null!);
        
        var result = await _sut.UpdateAuction(Guid.NewGuid(), updateAuctionDto);
        
        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public async Task UpdateAuction_WithInvalidSeller_ReturnsForbid()
    {
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        
        var updateAuctionDto = _fixture.Create<UpdateAuctionDto>();
        _auctionRepositoryMock.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(auction);
        
        var result = await _sut.UpdateAuction(auction.Id, updateAuctionDto);
        
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task DeleteAuction_WithValidId_ReturnsNoContent()
    {
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        
        _auctionRepositoryMock.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(auction);
        _auctionRepositoryMock.Setup(repo => repo.SaveChanges(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        auction.Seller = "test";
        
        var result = await _sut.DeleteAuction(auction.Id);
        
        Assert.IsType<OkResult>(result);
    }
    
    [Fact]
    public async Task DeleteAuction_WithInvalidId_ReturnsNotFound()
    {
        _auctionRepositoryMock.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Auction) null!);
        
        var result = await _sut.DeleteAuction(Guid.NewGuid());
        
        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public async Task DeleteAuction_WithInvalidSeller_ReturnsForbid()
    {
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        
        _auctionRepositoryMock.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(auction);
        
        var result = await _sut.DeleteAuction(auction.Id);
        
        Assert.IsType<ForbidResult>(result);
    }
    
    [Fact]
    public async Task DeleteAuction_WithValidId_DatabaseFailedToSave_ReturnsBadRequest()
    {
        var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
        auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        auction.Seller = "test";
        
        _auctionRepositoryMock.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(auction);
        _auctionRepositoryMock.Setup(repo => repo.SaveChanges(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        var result = await _sut.DeleteAuction(auction.Id);
        
        Assert.IsType<BadRequestObjectResult>(result);
    }
}