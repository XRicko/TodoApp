
using Microsoft.AspNetCore.Components;

using ToDoList.SharedClientLibrary.Models;

namespace ToDoList.BlazorClient.Args
{
    public record ChangeTodoItemStatusArgs(TodoItemModel TodoItemModel, ChangeEventArgs Args);
}
