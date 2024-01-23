namespace SearchService.Consumers;

using AutoMapper;
using BuildingBlocks.Contracts;
using Entities;
using MassTransit;
using MongoDB.Entities;

public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
    private readonly IMapper _mapper;
    private readonly ILogger<AuctionCreatedConsumer> _logger;

    public AuctionCreatedConsumer(IMapper mapper, ILogger<AuctionCreatedConsumer> logger)
    {
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        _logger.LogInformation("Received {Event} event with id: {AuctionId}", nameof(AuctionCreated), context.Message.Id);

        var item = _mapper.Map<Item>(context.Message);
        await item.SaveAsync();
        
        _logger.LogInformation("Successfully saved auction with id: {AuctionId}", item.ID);
    }
}