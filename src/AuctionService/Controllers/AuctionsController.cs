namespace AuctionService.Controllers;

using AutoMapper;
using BuildingBlocks.Contracts;
using Data.Repo;
using Dtos;
using Entities;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("auctions")]
public class AuctionsController : ControllerBase
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<AuctionsController> _logger;


    public AuctionsController(IAuctionRepository auctionRepository, IMapper mapper, IPublishEndpoint publishEndpoint,
        ILogger<AuctionsController> logger)
    {
        _auctionRepository = auctionRepository;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuctionDto>>> GetAuctions(string date = null, CancellationToken cancellationToken = default)
    {
        return (await _auctionRepository.GetAuctions(date, cancellationToken)).ToList();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var auction = await _auctionRepository.GetAuctionById(id, cancellationToken);
        
        if (auction is null)
            return NotFound();
        
        return auction;
    }
    
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<AuctionDto>> CreateAuction([FromBody] CreateAuctionDto request, CancellationToken cancellationToken = default)
    {
        var auction = _mapper.Map<Auction>(request);
        auction.Seller = User.Identity.Name;

        _auctionRepository.AddAuction(auction);
        
        var createdAuctionDto = _mapper.Map<AuctionDto>(auction);

        _logger.LogInformation("--> Publishing {Event} event with auction id: {AuctionId}", nameof(AuctionCreated), auction.Id);

        var auctionCreated = _mapper.Map<AuctionCreated>(createdAuctionDto);
        await _publishEndpoint.Publish(auctionCreated, cancellationToken);

        var result = await _auctionRepository.SaveChanges(cancellationToken);

        if (!result)
        {
            _logger.LogError("Auction was not created. Error while trying to save Auction to database");
            return BadRequest("Auction was not created. Please contact admin.");
        }

        return CreatedAtAction(nameof(GetAuctionById), new {auction.Id}, createdAuctionDto);
    }
    
    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateAuction([FromRoute] Guid id, [FromBody] UpdateAuctionDto request, CancellationToken cancellationToken = default)
    {
        var auction = await _auctionRepository.GetAuctionEntityById(id, cancellationToken);

        if (auction is null)
            return NotFound();

        if (auction.Seller != User.Identity.Name)
            return Forbid();

        auction.Item.Make = request.Make ?? auction.Item.Make;
        auction.Item.Model = request.Model ?? auction.Item.Model;
        auction.Item.Color = request.Color ?? auction.Item.Color;
        auction.Item.Year = request.Year ?? auction.Item.Year;
        auction.Item.Mileage = request.Mileage ?? auction.Item.Mileage;
        auction.Item.UpdatedAt = DateTime.UtcNow;
        auction.UpdatedAt = DateTime.UtcNow;

        var auctionUpdated = _mapper.Map<AuctionUpdated>(auction);
        _logger.LogInformation("--> Publishing {Event} event with auction id: {AuctionId}", nameof(AuctionUpdated), auction.Id);
        
        await _publishEndpoint.Publish(auctionUpdated, cancellationToken);

        var result = await _auctionRepository.SaveChanges(cancellationToken);
        
        if (!result)
        {
            _logger.LogError("Auction was not updated. Error while trying to update Auction to database");
            return BadRequest("Auction was not updated. Please contact admin.");
        }
        
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAuction([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var auction = await _auctionRepository.GetAuctionEntityById(id, cancellationToken);
        
        if (auction is null)
            return NotFound();

        if (auction.Seller != User.Identity.Name)
            return Forbid();
        
        _auctionRepository.RemoveAuction(auction);
        var auctionDeleted = _mapper.Map<AuctionDeleted>(auction);
        
        _logger.LogInformation("--> Publishing {Event} event with auction id: {AuctionId}", nameof(AuctionDeleted), auction.Id);
        await _publishEndpoint.Publish(auctionDeleted, cancellationToken);

        var result = await _auctionRepository.SaveChanges(cancellationToken);
        
        if (!result)
        {
            _logger.LogError("Auction was not deleted. Error while trying to delete Auction in database");
            return BadRequest("Auction was not deleted. Please contact admin.");
        }
        
        return Ok();
    }
}