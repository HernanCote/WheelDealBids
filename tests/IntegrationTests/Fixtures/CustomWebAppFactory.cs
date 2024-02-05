namespace IntegrationTests.Fixtures;

using AuctionService.Data;
using AuctionService.Dtos;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Utils;
using WebMotions.Fake.Authentication.JwtBearer;

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
                options.UseNpgsql(_postgresSqlContainer.GetConnectionString())
                    .UseSnakeCaseNamingConvention();
            });
            
            services.AddMassTransitTestHarness();

            services.EnsureCreated<AuctionDbContext>();

            services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme)
                .AddFakeJwtBearer(options =>
                {
                    options.BearerValueType = FakeJwtBearerBearerValueType.Jwt;
                });
        });
    }

#pragma warning disable CS0108, CS0114
    public Task DisposeAsync() => _postgresSqlContainer.DisposeAsync().AsTask();
#pragma warning restore CS0108, CS0114
}

internal class PostgresSqlContainer {}