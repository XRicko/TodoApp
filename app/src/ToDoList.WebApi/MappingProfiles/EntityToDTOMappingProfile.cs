using AutoMapper;

using NetTopologySuite.Geometries;

using ToDoList.Core.Entities;
using ToDoList.Core.Response;
using ToDoList.SharedKernel;
using ToDoList.WebApi.Requests.Create;
using ToDoList.WebApi.Requests.Update;

namespace ToDoList.WebApi.MappingProfiles
{
    internal class EntityToDtoMappingProfile : Profile
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
            CreateMap<UserCreateRequest, User>();

            CreateMap<Image, ImageResponse>();
            CreateMap<ImageCreateRequest, Image>();
            CreateMap<ImageUpdateRequest, Image>();

            CreateMap<Category, CategoryResponse>();
            CreateMap<CategoryCreateRequest, Category>();

            CreateMap<Status, StatusResponse>();

            CreateMap<Checklist, ChecklistResponse>()
                .ForMember(dest => dest.UserName,
                           opt => opt.MapFrom(src => src.User.Name));
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
