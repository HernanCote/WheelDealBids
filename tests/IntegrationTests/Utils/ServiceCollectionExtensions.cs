namespace IntegrationTests.Utils;

using AuctionService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void RemoveDbContext<T>(this IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AuctionDbContext>));
            
        if (descriptor is not null)
            services.Remove(descriptor);
    }

    public static void EnsureCreated<T>(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        using IServiceScope scope = serviceProvider.CreateScope();
        IServiceProvider scopedServices = scope.ServiceProvider;

        AuctionDbContext db = scopedServices.GetRequiredService<AuctionDbContext>();
        
        db.Database.Migrate();
        DbHelpers.InitDbForTests(db);
    }
}