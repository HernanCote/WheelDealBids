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
        builder.ConfigureWheelDealBidsLogs();
        
        builder.Services.AddControllers().AddJsonOptions(options => 
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
        
        builder.Services.AddDbContext<AuctionDbContext>(options =>
        {
            options
                .UseNpgsql(builder.Configuration.GetConnectionString("WheelDealBidsConnection"))
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
                config.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
                {
                    host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
                    host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
                });
                config.ConfigureEndpoints(context);
            });
        });

        builder.ConfigureWheelDealBidsAuthentication();

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