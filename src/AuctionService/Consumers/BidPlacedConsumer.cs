namespace AuctionService.Consumers;

using BuildingBlocks.Contracts;
using Data;
using MassTransit;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    private readonly AuctionDbContext _auctionDbContext;
    private readonly ILogger<BidPlacedConsumer> _logger;

    public BidPlacedConsumer(AuctionDbContext auctionDbContext, ILogger<BidPlacedConsumer> logger)
    {
        _auctionDbContext = auctionDbContext;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        _logger.LogInformation("Received {Event} event for auction {AuctionId}", nameof(BidPlaced), context.Message.AuctionId);
        
        var auction = await _auctionDbContext.Auctions.FindAsync(context.Message.AuctionId);

        if (auction is null)
        {
            _logger.LogError("Auction with id {AuctionId} was not found", context.Message.AuctionId);
            return;
        }

        if (auction.CurrentHighBid is null ||
            (context.Message.BidStatus.Contains("Accepted") && context.Message.Amount > auction.CurrentHighBid))
        {
            auction.CurrentHighBid = context.Message.Amount;
            await _auctionDbContext.SaveChangesAsync();  
            _logger.LogInformation("Updated current high bid for auction {AuctionId}", context.Message.AuctionId);
        }
    }
}