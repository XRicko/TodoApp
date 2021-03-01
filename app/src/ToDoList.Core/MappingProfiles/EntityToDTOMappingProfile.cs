using AutoMapper;

using ToDoList.Core.DTOs;
using ToDoList.Core.Entities;

namespace ToDoList.Core.MappingProfiles
{
    class EntityToDTOMappingProfile : Profile
    {
        public EntityToDTOMappingProfile()
        {
            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryDTO, Category>();

            CreateMap<ChecklistItem, ChecklistItemDTO>();
            CreateMap<ChecklistItemDTO, ChecklistItem>();

            CreateMap<Checklist, ChecklistDTO>();
            CreateMap<ChecklistDTO, Checklist>();

            CreateMap<Image, ImageDTO>();
            CreateMap<ImageDTO, Image>();
        }
    }
}
