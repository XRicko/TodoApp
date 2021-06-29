
using Microsoft.AspNetCore.Components;

using ToDoList.SharedClientLibrary.Models;

namespace ToDoList.BlazorClient.Models
{
    public record ChangeTodoItemStatusArgs(TodoItemModel TodoItemModel, ChangeEventArgs Args);
}
