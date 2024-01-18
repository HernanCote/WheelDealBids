using System.Net;
using Polly;
using Polly.Extensions.Http;
using SearchService.Services;
using SearchService.Services.Interfaces;
using SearchService.Settings;
using SearchService.StartupExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services
    .AddHttpClient<IAuctionServiceHttpClient, AuctionServiceHttpClient>()
    .AddPolicyHandler(GetRetryPolicy());

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection(nameof(MongoDbSettings)));
builder.Services.Configure<AuctionServiceSettings>(
    builder.Configuration.GetSection(nameof(AuctionServiceSettings)));

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    await app.UseMongoDb();
    await app.Initialize();
});

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() => HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
    .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
