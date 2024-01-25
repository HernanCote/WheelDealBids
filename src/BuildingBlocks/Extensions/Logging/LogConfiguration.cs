namespace BuildingBlocks.Extensions.Logging;

using Microsoft.AspNetCore.Builder;
using Serilog;

public static class LogConfiguration
{
    public static void ConfigureWheelDealBidsLogs(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, lc) => lc
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(ctx.Configuration));
    }
}