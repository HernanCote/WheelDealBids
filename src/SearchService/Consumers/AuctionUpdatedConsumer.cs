namespace SearchService.Consumers;

using AutoMapper;
using BuildingBlocks.Contracts;
using Entities;
using MassTransit;
using MongoDB.Entities;

public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
{
    private readonly IMapper _mapper;
    private readonly ILogger<AuctionUpdatedConsumer> _logger;

    public AuctionUpdatedConsumer(IMapper mapper, ILogger<AuctionUpdatedConsumer> logger)
    {
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        _logger.LogInformation("Received {Event} event with id: {AuctionId}", nameof(AuctionUpdated), context.Message.Id);
        
        // update item in the database
        var item = await DB.Find<Item>().OneAsync(context.Message.Id);
        
        if (item is null)
        {
            _logger.LogInformation("Auction with id: {AuctionId} was not found in the database", context.Message.Id);
            return;
        }

        var updatedItem = _mapper.Map<Item>(context.Message);
        
        var result = await DB.Update<Item>()
            .MatchID(item.ID)
            .ModifyOnly(m => new {m.Make, m.Model, m.Year, m.Color, m.Mileage}, updatedItem)
            .ExecuteAsync();

        if (!result.IsAcknowledged)
            throw new MessageException(typeof(AuctionUpdated), "Item was not updated in the database");

        _logger.LogInformation("Auction (Id: {AuctionId}) was updated sucessfully", context.Message.Id);
    }
}