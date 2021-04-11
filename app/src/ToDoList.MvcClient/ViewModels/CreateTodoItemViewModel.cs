using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using ToDoList.MvcClient.Models;

namespace ToDoList.MvcClient.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class CreateTodoItemViewModel
    {
        public TodoItemModel TodoItemModel { get; set; }

        public IEnumerable<ChecklistModel> ChecklistModels { get; set; }
        public IEnumerable<CategoryModel> CategoryModels { get; set; }
        public IEnumerable<StatusModel> StatusModels { get; set; }

        public int SelectedChecklistId { get; set; }
        public int? SelectedCategoryId { get; set; }
        public int? SelectedStatusId { get; set; }
    }
}
