namespace AuctionService.Data.ModelConfiguration;

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.HasOne(item => item.Auction)
                .WithOne(auction => auction.Item)
                .HasForeignKey<Item>(item => item.AuctionId)
                .OnDelete(DeleteBehavior.Restrict);
    }
}