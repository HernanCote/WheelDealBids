namespace AuctionService.Entities;

using Base;
using Interfaces;

public class Item : Entity
{
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string Color { get; set; }
    public int Mileage { get; set; }
    public string ImageUrl { get; set; }
    public Auction Auction { get; set; }
    public Guid AuctionId { get; set; }
}