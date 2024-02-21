namespace AuctionService;

using System.Text.Json.Serialization;
using BuildingBlocks.Extensions.Authentication;
using BuildingBlocks.Extensions.Logging;
using Consumers;
using Data;
using Data.Repo;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Polly;
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
    
            x.AddConsumers(typeof(Program).Assembly);
            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));
    
            x.UsingRabbitMq((context, config) =>
            {
                config.UseMessageRetry(r => 
                {
                    r.Handle<RabbitMqConnectionException>();
                    r.Interval(5, TimeSpan.FromSeconds(10));
                });
                
                config.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
                {
                    host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
                    host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
                });
                config.ConfigureEndpoints(context);
            });
        });

        builder.ConfigureWheelDealBidsAuthentication();

        builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
        
        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        var retryPolicy = Policy.Handle<NpgsqlException>()
            .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(10));

        retryPolicy.ExecuteAndCapture(() => SeedInitialData.Initialize(app));
        return app;
    }
    
}