using System.Text.Json.Serialization;
using AuctionService.Data;
using AuctionService.Utils;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddDbContext<AuctionDbContext>(options =>
{
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("AuctionConnection"))
        .UseSnakeCaseNamingConvention();
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

SeedInitialData.Initialize(app);

app.Run();
