namespace SearchService.Consumers;

using BuildingBlocks.Contracts;
using Entities;
using MassTransit;
using MongoDB.Entities;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    private readonly ILogger<BidPlacedConsumer> _logger;

    public BidPlacedConsumer(ILogger<BidPlacedConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        _logger.LogInformation("Consuming BidPlaced event for Auction (Id: {BidPlaced})", context.Message.AuctionId);

        var auction = await DB.Find<Item>().OneAsync(context.Message.AuctionId);

        if (auction is null)
        {
            _logger.LogError("Auction (Id: {AuctionId}) not found", context.Message.AuctionId);
            return;
        }
        
        if (context.Message.BidStatus.Contains("Accepted") && context.Message.Amount > auction.CurrentHighBid)
        {
            auction.CurrentHighBid = context.Message.Amount;
            await auction.SaveAsync();
        }
    }
}