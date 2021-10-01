using System;
using System.Linq;

using ToDoList.Core.Entities;
using ToDoList.SharedKernel.Interfaces;

namespace ToDoList.Core.Extensions
{
    public static class IRepositoryExtensions
    {
        public static void RemoveChecklist(this IRepository repository, int checklistId)
        {
            _ = repository ?? throw new ArgumentNullException(nameof(repository));

            var checklist = repository.GetAll<Checklist>()
                                      .Select(x => new Checklist
                                      {
                                          Id = x.Id,
                                          Name = x.Name,
                                          TodoItems = x.TodoItems.Select(t => new TodoItem
                                          {
                                              Id = t.Id,
                                              ChecklistId = t.ChecklistId
                                          }).ToList()
                                      })
                                      .SingleOrDefault(x => x.Id == checklistId);

            if (checklist is not null)
            {
                foreach (var item in checklist.TodoItems)
                {
                    repository.Remove(item);
                }
                repository.Remove(checklist);
            }
        }
    }
}
