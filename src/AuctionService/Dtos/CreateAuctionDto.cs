namespace AuctionService.Dtos;

using System.ComponentModel.DataAnnotations;

public record CreateAuctionDto
{
    [Required]
    public string Make { get; set; }
    [Required]
    public string Model { get; set; }
    [Required]
    public int? Year { get; set; }
    [Required]
    public string Color { get; set; }
    [Required]
    public int? Mileage { get; set; }
    [Required]
    public string ImageUrl { get; set; }
    [Required]
    public decimal ReservePrice { get; set; }
    [Required]
    public DateTime AuctionEnd { get; set; }
}