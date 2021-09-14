using System.IO;

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
                .ReverseMap()
                .ForCtorParam("x",
                              opt => opt.MapFrom(src => src.Longitude))
                .ForCtorParam("y",
                              opt => opt.MapFrom(src => src.Latitude))
                .ForMember(dest => dest.SRID,
                           opt => opt.MapFrom(src => GeoCoordinate.SRID))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<User, UserResponse>();
            CreateMap<UserRequest, User>(MemberList.Source);

            CreateMap<RefreshToken, RefreshTokenResponse>();
            CreateMap<RefreshTokenCreateRequest, RefreshToken>(MemberList.Source);

            CreateMap<Image, ImageResponse>()
                .ForCtorParam(nameof(ImageResponse.Content),
                              opt => opt.MapFrom(src => File.ReadAllBytes(src.Path)))
                .ForAllOtherMembers(opt => opt.Ignore());
            CreateMap<ImageCreateRequest, Image>(MemberList.Source);

            CreateMap<Category, CategoryResponse>();
            CreateMap<CategoryCreateRequest, Category>(MemberList.Source);

            CreateMap<Status, StatusResponse>();

            CreateMap<Checklist, ChecklistResponse>();
            CreateMap<ChecklistCreateRequest, Checklist>(MemberList.Source);
            CreateMap<ChecklistUpdateRequest, Checklist>(MemberList.Source);

            CreateMap<TodoItem, TodoItemResponse>()
                .ForMember(dest => dest.StatusName,
                           opt => opt.MapFrom(src => src.Status.Name))
                .ForMember(dest => dest.CategoryName,
                           opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ChecklistName,
                           opt => opt.MapFrom(src => src.Checklist.Name))
                .ForMember(dest => dest.ImageName,
                           opt => opt.MapFrom(src => src.Image.Name))
                .ForMember(dest => dest.ImageContent,
                           opt => opt.MapFrom(src => src.Image == null ? null : File.ReadAllBytes(src.Image.Path)))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<TodoItemCreateRequest, TodoItem>(MemberList.Source);
            CreateMap<TodoItemUpdateRequest, TodoItem>(MemberList.Source);
        }
    }
}
