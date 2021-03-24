using AutoMapper;

using NetTopologySuite.Geometries;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Requests;
using ToDoList.Core.Mediator.Requests.Create;
using ToDoList.Core.Mediator.Requests.Update;
using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel;

namespace ToDoList.Core.MappingProfiles
{
    public class EntityToDtoMappingProfile : Profile
    {
        public EntityToDtoMappingProfile()
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

            CreateMap<User, UserResponse>();
            CreateMap<UserRequest, User>();

            CreateMap<Image, ImageResponse>();
            CreateMap<ImageCreateRequest, Image>();

            CreateMap<Category, CategoryResponse>();
            CreateMap<CategoryCreateRequest, Category>();

            CreateMap<Status, StatusResponse>();

            CreateMap<Checklist, ChecklistResponse>();
            CreateMap<ChecklistCreateRequest, Checklist>();
            CreateMap<ChecklistUpdateRequest, Checklist>();

            CreateMap<TodoItem, TodoItemResponse>()
                .ForMember(dest => dest.StatusName,
                           opt => opt.MapFrom(src => src.Status.Name))
                .ForMember(dest => dest.CategoryName,
                           opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ChecklistName,
                           opt => opt.MapFrom(src => src.Checklist.Name))
                .ForMember(dest => dest.ImagePath,
                           opt => opt.MapFrom(src => src.Image.Name));

            CreateMap<TodoItemCreateRequest, TodoItem>();
            CreateMap<TodoItemUpdateRequest, TodoItem>();
        }
    }
}
