namespace AuctionService.Consumers;

using BuildingBlocks.Contracts;
using MassTransit;

public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
{
    public Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"--> Received {nameof(AuctionCreated)} fault with message: {context.Message.Message}");
        
        return Task.CompletedTask;
    }
}