namespace AuctionService.Data;

using Entities;
using Microsoft.EntityFrameworkCore;
using ModelConfiguration;

public class AuctionDbContext : DbContext
{
    public AuctionDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Auction> Auctions { get; set; }
    public DbSet<Item> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AuctionConfiguration());
        modelBuilder.ApplyConfiguration(new ItemConfiguration());
    }
}