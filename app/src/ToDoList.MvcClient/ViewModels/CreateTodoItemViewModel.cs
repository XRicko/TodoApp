using System.Collections.Generic;

using ToDoList.MvcClient.Models;
using ToDoList.SharedClientLibrary.Models;

namespace ToDoList.MvcClient.ViewModels
{
    public class CreateTodoItemViewModel
    {
        public TodoItemModelWithFile TodoItemModel { get; set; }

        public IEnumerable<ChecklistModel> ChecklistModels { get; set; }
        public IEnumerable<CategoryModel> CategoryModels { get; set; }
        public IEnumerable<StatusModel> StatusModels { get; set; }

        public int SelectedChecklistId { get; set; }
        public int? SelectedCategoryId { get; set; }
        public int? SelectedStatusId { get; set; }
    }
}
