namespace BuildingBlocks.Contracts;

public record BidPlaced(
    string Id, 
    string AuctionId, 
    string Bidder, 
    DateTime BidOn, 
    int Amount, 
    string BidStatus);