namespace GatewayService;

using BuildingBlocks.Extensions.Authentication;
using BuildingBlocks.Extensions.Logging;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.ConfigureCarstiesLogs();
        builder.ConfigureCarstiesAuthentication();
        
        builder.Services.AddReverseProxy()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
        
        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.MapReverseProxy();

        app.UseAuthentication();
        app.UseAuthorization();
        
        return app;
    }
}