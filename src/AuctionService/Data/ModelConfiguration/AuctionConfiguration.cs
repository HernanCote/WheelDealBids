namespace AuctionService.Data.ModelConfiguration;

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AuctionConfiguration : IEntityTypeConfiguration<Auction>
{
    public void Configure(EntityTypeBuilder<Auction> builder)
    {
        builder.HasOne(auction => auction.Item)
                .WithOne(item => item.Auction)
                .OnDelete(DeleteBehavior.Cascade);
    }
}