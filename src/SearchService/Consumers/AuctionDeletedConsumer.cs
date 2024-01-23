namespace SearchService.Consumers;

using BuildingBlocks.Contracts;
using Entities;
using MassTransit;
using MongoDB.Entities;

public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
{
    private readonly ILogger<AuctionDeletedConsumer> _logger;

    public AuctionDeletedConsumer(ILogger<AuctionDeletedConsumer> logger)
    {
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<AuctionDeleted> context)
    {

        _logger.LogInformation("Received {Event} event with id: {AuctionId}", nameof(AuctionDeleted), context.Message.Id);

        var item = await DB.Find<Item>().OneAsync(context.Message.Id);

        if (item is null)
        {
            _logger.LogInformation("Auction with id: {AuctionId} was not found in the database", context.Message.Id);
            return;
        }

        var result = await item.DeleteAsync();

        if (!result.IsAcknowledged)
            throw new MessageException(typeof(AuctionDeleted), "Auction was not deleted. Please contact admin.");
        
        _logger.LogInformation("Auction (Id: {AuctionId}) was deleted from the database", context.Message.Id);
    }
}