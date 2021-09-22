using ToDoList.SharedClientLibrary.Models;

namespace ToDoList.BlazorClient.State
{
    public class StateContainer
    {
        public string StatusName { get; set; }
        public string CategoryName { get; set; }

        public string SearchTerm { get; set; }

        public TodoItemModel DraggedTodoItem { get; set; }
    }
}
