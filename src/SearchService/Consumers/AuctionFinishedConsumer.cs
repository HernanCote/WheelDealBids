namespace SearchService.Consumers;

using BuildingBlocks.Contracts;
using Entities;
using MassTransit;
using MongoDB.Entities;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
    private readonly ILogger<AuctionFinishedConsumer> _logger;

    public AuctionFinishedConsumer(ILogger<AuctionFinishedConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        _logger.LogInformation("Consuming AuctionFinished event for Auction (Id: {AuctionId})", context.Message.AuctionId);

        var auction = await DB.Find<Item>().OneAsync(context.Message.AuctionId);

        if (auction is null)
        {
            _logger.LogError("Auction (Id: {AuctionId}) not found", context.Message.AuctionId);
            return;
        }

        if (context.Message.ItemSold)
        {
            auction.Winner = context.Message.Winner;
            auction.SoldAmount = (decimal) context.Message.Amount!;
        }

        auction.Status = "Finished";
        
        await auction.SaveAsync();
    }
}