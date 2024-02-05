namespace IntegrationTests.Utils;

using AuctionService.Data;
using AuctionService.Entities;

public static class DbHelpers
{
    public const string GT_ID = "afbee524-3e3e-4e3e-8e3e-3e3e3e3e3e3e";
    
    public static void InitDbForTests(AuctionDbContext db)
    {
        db.Auctions.AddRange(GetTestAuctions());
        db.SaveChanges();
    }
    
    public static void ReinitializeDbForTests(AuctionDbContext db)
    {
        db.Auctions.RemoveRange(db.Auctions);
        db.SaveChanges();
        InitDbForTests(db);
    }

    private static IEnumerable<Auction> GetTestAuctions()
    {
        return new List<Auction>()
        {
            // 1 Ford GT
            new()
            {
                Id = Guid.Parse(GT_ID),
                Status = Status.Live,
                ReservePrice = 20000,
                Seller = "bob",
                AuctionEnd = DateTime.UtcNow.AddDays(10),
                Item = new Item
                {
                    Make = "Ford",
                    Model = "GT",
                    Color = "White",
                    Mileage = 50000,
                    Year = 2020,
                    ImageUrl = "https://cdn.pixabay.com/photo/2016/05/06/16/32/car-1376190_960_720.jpg"
                }
            },
            // 2 Bugatti Veyron
            new()
            {
                Id = Guid.NewGuid(),
                Status = Status.Live,
                ReservePrice = 90000,
                Seller = "alice",
                AuctionEnd = DateTime.UtcNow.AddDays(60),
                Item = new Item
                {
                    Make = "Bugatti",
                    Model = "Veyron",
                    Color = "Black",
                    Mileage = 15035,
                    Year = 2018,
                    ImageUrl = "https://cdn.pixabay.com/photo/2012/05/29/00/43/car-49278_960_720.jpg"
                }
            },
            // 3 Ford mustang
            new()
            {
                Id = Guid.NewGuid(),
                Status = Status.Live,
                Seller = "bob",
                AuctionEnd = DateTime.UtcNow.AddDays(4),
                Item = new Item
                {
                    Make = "Ford",
                    Model = "Mustang",
                    Color = "Black",
                    Mileage = 65125,
                    Year = 2023,
                    ImageUrl = "https://cdn.pixabay.com/photo/2012/11/02/13/02/car-63930_960_720.jpg"
                }
            }
        };
    }
}