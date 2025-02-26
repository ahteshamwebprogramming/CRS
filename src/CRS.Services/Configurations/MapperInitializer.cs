
using AutoMapper;
//using CRS.Core.EHRMSEntities;
using CRS.Core.Entities;
using CRS.Infrastructure.Models.Masters;


namespace CRS.Services.Configurations;

public class MapperInitializer : Profile
{
    public MapperInitializer()
    {        
        CreateMap<RoomType, RoomTypeDTO>().ReverseMap();        
    }
}
