namespace IntegrationTests.Fixtures;

using AuctionService.Data;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Utils;

public class CustomWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private PostgreSqlContainer _postgresSqlContainer = new PostgreSqlBuilder().Build(); 
        
    public async Task InitializeAsync()
    {
        await _postgresSqlContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveDbContext<AuctionDbContext>();

            services.AddDbContext<AuctionDbContext>(options =>
            {
                options.UseNpgsql(_postgresSqlContainer.GetConnectionString());
            });
            
            services.AddMassTransitTestHarness();

            services.EnsureCreated<AuctionDbContext>();
        });
    }

    public Task DisposeAsync() => _postgresSqlContainer.DisposeAsync().AsTask();
}

internal class PostgresSqlContainer {}