namespace AuctionService.Data.Repo;

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dtos;
using Entities;
using Microsoft.EntityFrameworkCore;

public class AuctionRepository : IAuctionRepository
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;

    public AuctionRepository(AuctionDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<AuctionDto>> GetAuctions(string date = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();
        
        if (!string.IsNullOrEmpty(date))
            query = query.Where(x => x.CreatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0 
                                     || (x.UpdatedAt.HasValue 
                                         && x.UpdatedAt.Value.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0));

        return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
    }

    public Task<AuctionDto> GetAuctionById(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Auctions
            .ProjectTo<AuctionDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<Auction> GetAuctionEntityById(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Auctions
            .Include(a => a.Item)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public void AddAuction(Auction auction)
    {
        if (auction is null)
            return;
        
        _context.Auctions.Add(auction);
    }

    public void RemoveAuction(Auction auction)
    {
        if (auction is null)
            return;

        _context.Auctions.Remove(auction);
    }

    public async Task<bool> SaveChanges(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}