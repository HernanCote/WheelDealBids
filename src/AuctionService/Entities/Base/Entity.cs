namespace AuctionService.Entities.Base;

using Interfaces;

public abstract class Entity : IEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}