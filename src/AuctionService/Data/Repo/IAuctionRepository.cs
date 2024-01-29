namespace AuctionService.Data.Repo;

using Dtos;
using Entities;

public interface IAuctionRepository
{
    Task<IEnumerable<AuctionDto>> GetAuctions(string date = null, CancellationToken cancellationToken = default);
    Task<AuctionDto> GetAuctionById(Guid id, CancellationToken cancellationToken = default);
    Task<Auction> GetAuctionEntityById(Guid id, CancellationToken cancellationToken = default);
    void AddAuction(Auction auction);
    void RemoveAuction(Auction auction);
    Task<bool> SaveChanges(CancellationToken cancellationToken = default);
}