namespace SearchService;

using System.Net;
using BuildingBlocks.Extensions.Logging;
using Clients;
using Clients.Interfaces;
using Consumers;
using Managers;
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
        builder.ConfigureWheelDealBidsLogs();
        
        builder.Services.AddControllers();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services
            .AddHttpClient<IAuctionServiceHttpClient, AuctionServiceHttpClient>()
            .AddPolicyHandler(GetRetryPolicy());

        builder.Services.Configure<MongoDbSettings>(
            builder.Configuration.GetSection(nameof(MongoDbSettings)));
        builder.Services.Configure<AuctionServiceSettings>(
            builder.Configuration.GetSection(nameof(AuctionServiceSettings)));

        builder.Services.AddScoped<ISearchManager, SearchManager>();

        builder.Services.AddMassTransit(x =>
        {
            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
            
            x.AddConsumers(typeof(Program).Assembly);
            
            x.UsingRabbitMq((context, config) =>
            {
                config.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
                {
                    host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
                    host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
                });
                
                config.ReceiveEndpoint("search-auction-created", e =>
                {
                    e.UseMessageRetry(r => r.Interval(5, 5));
                    e.ConfigureConsumer<AuctionCreatedConsumer>(context);
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