using AutoMapper;

using NetTopologySuite.Geometries;

using ToDoList.Core.DTOs;
using ToDoList.Core.Entities;

namespace ToDoList.Core.MappingProfiles
{
    class EntityToDTOMappingProfile : Profile
    {
        public EntityToDTOMappingProfile()
        {
            CreateMap<Point, GeoCoordinate>()
               .ForMember(dest => dest.Longitude,
                          opt => opt.MapFrom(src => src.X))
               .ForMember(dest => dest.Latitude,
                          opt => opt.MapFrom(src => src.Y))
               .ForCtorParam("Longitude",
                             opt => opt.MapFrom(src => src.X))
               .ForCtorParam("Latitude",
                             opt => opt.MapFrom(src => src.Y))
               .ReverseMap();

            CreateMap<GeoCoordinate, Point>()
                .ForCtorParam("x",
                              opt => opt.MapFrom(src => src.Longitude))
                .ForCtorParam("y",
                              opt => opt.MapFrom(src => src.Latitude));

            CreateMap<ChecklistItem, ChecklistItemDTO>().ReverseMap();

            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Checklist, ChecklistDTO>().ReverseMap();
            CreateMap<Image, ImageDTO>().ReverseMap();
        }
    }
}
