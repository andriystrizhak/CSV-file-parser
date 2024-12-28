using AutoMapper;
using CSVParser.Data;
using CSVParser.Models;

namespace CSVParser.Mappers;

public class TripRecordToTripMap : Profile
{
    public TripRecordToTripMap()
    {
        CreateMap<TripRecord, Trip>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PickupDatetime, opt => opt.MapFrom(src => src.TpepPickupDatetime))
            .ForMember(dest => dest.DropoffDatetime, opt => opt.MapFrom(src => src.TpepDropoffDatetime));
    }
}