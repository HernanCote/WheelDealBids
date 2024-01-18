namespace SearchService.StartupExtensions;

using System.Text.Json;
using Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Entities;
using Services;
using Services.Interfaces;
using Settings;

public static class ConfigureMongoDb
{
    public static async Task UseMongoDb(this IApplicationBuilder app)
    {
        var mongoDbSettings = app.ApplicationServices.GetService<IOptions<MongoDbSettings>>()?.Value;

        await DB.InitAsync(mongoDbSettings?.DatabaseName ?? throw new Exception("Database name not found"),
            MongoClientSettings.FromConnectionString(
                mongoDbSettings.ConnectionString ?? throw new Exception("Connection string not found")
            ));

        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .CreateAsync();
    }

    public static async Task Initialize(this IApplicationBuilder app)
    {
        var count = await DB.CountAsync<Item>();

        Console.WriteLine("Fetching data from Auction Service");

        using var scope = app.ApplicationServices.CreateScope();
        var httpClient = scope.ServiceProvider.GetRequiredService<IAuctionServiceHttpClient>();
        var items = await httpClient.GetItemsForSearchDb();

        Console.WriteLine($"{items.Count()} items returned from auction service" );

        if (!items.Any())
            return;

        Console.WriteLine("Saving new items to database");

        await DB.SaveAsync(items);
    }
}