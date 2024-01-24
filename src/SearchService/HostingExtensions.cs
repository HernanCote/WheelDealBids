namespace SearchService;

using System.Net;
using BuildingBlocks.Extensions.Logging;
using Clients;
using Clients.Interfaces;
using Consumers;
using MassTransit;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Settings;
using StartupExtensions;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.ConfigureCarstiesLogs();
        
        builder.Services.AddControllers();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services
            .AddHttpClient<IAuctionServiceHttpClient, AuctionServiceHttpClient>()
            .AddPolicyHandler(GetRetryPolicy());

        builder.Services.Configure<MongoDbSettings>(
            builder.Configuration.GetSection(nameof(MongoDbSettings)));
        builder.Services.Configure<AuctionServiceSettings>(
            builder.Configuration.GetSection(nameof(AuctionServiceSettings)));

        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
            x.AddConsumersFromNamespaceContaining<AuctionUpdatedConsumer>();
            x.AddConsumersFromNamespaceContaining<AuctionDeletedConsumer>();
            x.AddConsumersFromNamespaceContaining<AuctionFinishedConsumer>();
            x.AddConsumersFromNamespaceContaining<BidPlacedConsumer>();
    
            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
            x.UsingRabbitMq((context, config) =>
            {
                config.ReceiveEndpoint("search-auction-created", e =>
                {
                    e.UseMessageRetry(r => r.Interval(5, 5));
                    e.ConfigureConsumer<AuctionCreatedConsumer>(context);
                });
        
                config.ReceiveEndpoint("search-auction-updated", e =>
                {
                    e.UseMessageRetry(r => r.Interval(5, 5));
                    e.ConfigureConsumer<AuctionUpdatedConsumer>(context);
                });
        
                config.ReceiveEndpoint("search-auction-deleted", e =>
                {
                    e.UseMessageRetry(r => r.Interval(5, 5));
                    e.ConfigureConsumer<AuctionDeletedConsumer>(context);
                });
        
                config.ConfigureEndpoints(context);
            });
        });

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        
        app.UseAuthorization();
        app.MapControllers();

        app.Lifetime.ApplicationStarted.Register(async () =>
        {
            await app.UseMongoDb();
            await app.Initialize();
        });

        return app;
    }
    
    static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() => HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
}