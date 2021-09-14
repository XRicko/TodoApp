using System.IO.Abstractions;
using System.Linq;

using AutoMapper;

using ToDoList.Core.Entities;
using ToDoList.Core.Mediator.Response;
using ToDoList.SharedKernel;

namespace ToDoList.Core.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<TodoItemResponse> AsTodoItemResponses(this IQueryable<TodoItem> todoItems, IMapper mapper, IFileSystem fileSystem)
        {
            return todoItems.Select(x => new TodoItemResponse(x.Id, x.Name, x.StartDate,
                                                              x.ChecklistId, x.Checklist.Name,
                                                              x.StatusId, x.Status.Name,
                                                              x.DueDate,
                                                              mapper.Map<GeoCoordinate>(x.GeoPoint),
                                                              x.CategoryId, x.Category.Name,
                                                              x.ImageId, x.Image.Name,
                                                              string.IsNullOrWhiteSpace(x.Image.Path)
                                                                ? null
                                                                : fileSystem.File.ReadAllBytes(x.Image.Path)));
        }
    }
}
