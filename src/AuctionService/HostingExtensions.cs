namespace AuctionService;

using System.Text.Json.Serialization;
using BuildingBlocks.Extensions.Authentication;
using BuildingBlocks.Extensions.Logging;
using Consumers;
using Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Utils;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.ConfigureCarstiesLogs();
        
        builder.Services.AddControllers().AddJsonOptions(options => 
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
        
        builder.Services.AddDbContext<AuctionDbContext>(options =>
        {
            options
                .UseNpgsql(builder.Configuration.GetConnectionString("CarstiesConnection"))
                .UseSnakeCaseNamingConvention();
        });

        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<AuctionDbContext>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(10);
                o.UsePostgres();
                o.UseBusOutbox();
            });
    
            x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();
            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));
    
            x.UsingRabbitMq((context, config) =>
            {
                config.ConfigureEndpoints(context);
            });
        });

        builder.ConfigureCarstiesAuthentication();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        SeedInitialData.Initialize(app);

        return app;
    }
    
}