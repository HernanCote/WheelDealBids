using AuctionService;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

Log.Information("Starting up AuctionService");

try
{
    var builder = WebApplication.CreateBuilder(args);

    var app = builder
        .ConfigureServices()
        .ConfigurePipeline();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("AuctionService shut down complete");
    Log.CloseAndFlush();
}

public partial class Program {}

