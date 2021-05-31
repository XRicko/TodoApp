using System.Collections.Generic;

using ToDoList.SharedClientLibrary.Models;

namespace ToDoList.MvcClient.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<TodoItemModel> TodoItems { get; set; }
        public IEnumerable<ChecklistModel> ChecklistModels { get; set; }

        public string SelectedCategory { get; set; }
        public string SelectedStatus { get; set; }
    }
}
