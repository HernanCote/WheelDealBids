using BuildingBlocks.Extensions.Authentication;
using GatewayService;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up Gateway 💻");

try
{
    var builder = WebApplication.CreateBuilder(args);

    var app = builder
        .ConfigureServices()
        .ConfigurePipeline();

    app.Run();
}
catch(Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("GatewayService shut down complete");
    Log.CloseAndFlush();
}
