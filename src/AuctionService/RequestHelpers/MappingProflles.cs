namespace AuctionService.RequestHelpers;

using AutoMapper;
using BuildingBlocks.Contracts;
using Dtos;
using Entities;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Auction, AuctionDto>()
            .IncludeMembers(x => x.Item);
        CreateMap<Item, AuctionDto>();
        CreateMap<CreateAuctionDto, Auction>()
            .ForMember(d => d.Item, o => o.MapFrom(s => s));
        CreateMap<CreateAuctionDto, Item>();
        
        CreateMap<AuctionDto, AuctionCreated>();
        CreateMap<Auction, AuctionUpdated>()
            .IncludeMembers();
        CreateMap<Item, AuctionUpdated>();
        CreateMap<Auction, AuctionDeleted>();
        
    }
}