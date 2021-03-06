using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using ToDoList.MvcClient.Models;

namespace ToDoList.MvcClient.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class IndexViewModel
    {
        public IEnumerable<TodoItemModel> ActiveTodoItems { get; set; }
        public IEnumerable<TodoItemModel> DoneTodoItems { get; set; }

        public IEnumerable<ChecklistModel> ChecklistModels { get; set; }
    }
}
