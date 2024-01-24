namespace BuildingBlocks.Contracts;

public record AuctionFinished(
    bool ItemSold, 
    string AuctionId, 
    string Seller, 
    string? Winner = null, 
    int? Amount = null);