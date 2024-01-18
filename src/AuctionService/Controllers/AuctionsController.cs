namespace AuctionService.Controllers;

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data;
using Dtos;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("auctions")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;
    

    public AuctionsController(AuctionDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuctionDto>>> GetAuctions(string date = null)
    {
        var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();
        
        if (!string.IsNullOrEmpty(date))
            query = query.Where(x => x.CreatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0 
                                     || (x.UpdatedAt.HasValue 
                                         && x.UpdatedAt.Value.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0));

        return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById([FromRoute] Guid id)
    {
        var auction = await _context.Auctions
            .Include(a => a.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction is null)
            return NotFound();

        return _mapper.Map<AuctionDto>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction([FromBody] CreateAuctionDto request)
    {
        var auction = _mapper.Map<Auction>(request);
        //TODO: add current user as seller;
        auction.Seller = "system";

        _context.Auctions.Add(auction);

        var result = await _context.SaveChangesAsync() > 0;
        
        if (!result)
            return BadRequest("Auction was not created. Please contact admin.");
        
        var auctionDto = _mapper.Map<AuctionDto>(auction);
        return CreatedAtAction(nameof(GetAuctionById), new {auction.Id}, auctionDto);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateAuction([FromRoute] Guid id, [FromBody] UpdateAuctionDto request)
    {
        var auction = await _context.Auctions
            .Include(a => a.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction is null)
            return NotFound();
        
        //TODO: check seller to be current user

        auction.Item.Make = request.Make ?? auction.Item.Make;
        auction.Item.Model = request.Model ?? auction.Item.Model;
        auction.Item.Color = request.Color ?? auction.Item.Color;
        auction.Item.Year = request.Year ?? auction.Item.Year;
        auction.Item.Mileage = request.Mileage ?? auction.Item.Mileage;
        auction.Item.UpdatedAt = DateTime.UtcNow;
        auction.UpdatedAt = DateTime.UtcNow;

        var result = await _context.SaveChangesAsync() > 0;
        
        if (!result)
            return BadRequest("Auction was not updated. Please contact admin.");
        
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAuction([FromRoute] Guid id)
    {
        var auction = await _context.Auctions.Include(a => a.Item).FirstOrDefaultAsync(x => x.Id == id);
        
        if (auction is null)
            return NotFound();

        // TODO: check seller == current user name
        
        _context.Items.Remove(auction.Item);
        _context.Auctions.Remove(auction);
        
        var result = await _context.SaveChangesAsync() > 0;
        
        if (!result)
            return BadRequest("Auction could not be deleted. Please contact admin.");
        
        return Ok();
    }
}