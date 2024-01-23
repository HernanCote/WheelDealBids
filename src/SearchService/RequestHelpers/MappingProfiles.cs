namespace SearchService.RequestHelpers;

using AutoMapper;
using BuildingBlocks.Contracts;
using Entities;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<AuctionCreated, Item>();
        CreateMap<AuctionUpdated, Item>();
    }
}