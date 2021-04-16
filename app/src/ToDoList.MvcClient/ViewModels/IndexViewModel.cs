using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using ToDoList.MvcClient.Models;

namespace ToDoList.MvcClient.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class IndexViewModel
    {
        public IEnumerable<TodoItemModel> TodoItems { get; set; }
        public IEnumerable<ChecklistModel> ChecklistModels { get; set; }

        public string SelectedCategory { get; set; }
        public string SelectedStatus { get; set; }
    }
}
