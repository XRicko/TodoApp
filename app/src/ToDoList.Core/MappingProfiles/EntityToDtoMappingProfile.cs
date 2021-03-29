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
        private const int SRID = 4326;

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
              .ReverseMap()
              .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<GeoCoordinate, Point>()
                .ForCtorParam("x",
                              opt => opt.MapFrom(src => src.Longitude))
                .ForCtorParam("y",
                              opt => opt.MapFrom(src => src.Latitude))
<<<<<<< HEAD
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<User, UserResponse>();
            CreateMap<UserRequest, User>(MemberList.Source);
=======
                .ForMember(dest => dest.SRID,
                           opt => opt.MapFrom(src => SRID));

            CreateMap<User, UserResponse>();
            CreateMap<UserRequest, User>();
>>>>>>> master

            CreateMap<Image, ImageResponse>();
            CreateMap<ImageCreateRequest, Image>(MemberList.Source);

            CreateMap<Category, CategoryResponse>();
            CreateMap<CategoryCreateRequest, Category>(MemberList.Source);

            CreateMap<Status, StatusResponse>();

            CreateMap<Checklist, ChecklistResponse>();
<<<<<<< HEAD
            CreateMap<ChecklistCreateRequest, Checklist>(MemberList.Source);
            CreateMap<ChecklistUpdateRequest, Checklist>(MemberList.Source);
=======
            CreateMap<ChecklistCreateRequest, Checklist>();
            CreateMap<ChecklistUpdateRequest, Checklist>();
>>>>>>> master

            CreateMap<TodoItem, TodoItemResponse>()
                .ForMember(dest => dest.StatusName,
                           opt => opt.MapFrom(src => src.Status.Name))
                .ForMember(dest => dest.CategoryName,
                           opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ChecklistName,
                           opt => opt.MapFrom(src => src.Checklist.Name))
                .ForMember(dest => dest.ImagePath,
                           opt => opt.MapFrom(src => src.Image.Name))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<TodoItemCreateRequest, TodoItem>(MemberList.Source);
            CreateMap<TodoItemUpdateRequest, TodoItem>(MemberList.Source);
        }
    }
}
