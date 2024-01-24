namespace AuctionService.Consumers;

using BuildingBlocks.Contracts;
using Data;
using Entities;
using MassTransit;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
    private readonly AuctionDbContext _auctionDbContext;
    private readonly ILogger<AuctionFinishedConsumer> _logger;

    public AuctionFinishedConsumer(AuctionDbContext auctionDbContext, ILogger<AuctionFinishedConsumer> logger)
    {
        _auctionDbContext = auctionDbContext;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        _logger.LogInformation("Received AuctionFinished event for auction {AuctionId}", context.Message.AuctionId);
        var auction = await _auctionDbContext.Auctions.FindAsync(context.Message.AuctionId);

        if (auction is null)
        {
            _logger.LogError("Auction with id {AuctionId} was not found", context.Message.AuctionId);
            return;
        }

        if (context.Message.ItemSold)
        {
            auction.Winner = context.Message.Winner;
            auction.SoldAmount = context.Message.Amount;
        }
        
        auction.Status = auction.SoldAmount > auction.ReservePrice ? Status.Finished : Status.ReserveNotMet;

        await _auctionDbContext.SaveChangesAsync();
    }
}