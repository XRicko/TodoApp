using System;
using System.Threading.Tasks;

using ToDoList.SharedClientLibrary.Models;

namespace ToDoList.BlazorClient.Services
{
    public class Notifier
    {
        public event Func<int, Task> ChecklistChanged;
        public event Func<BaseModel, Task> ItemAdded;
        public event Func<string, string, Task> FilterChosen;

        public async Task OnChecklistChanged(int checklistId) => await ChecklistChanged?.Invoke(checklistId);
        public async Task OnItemAdded(BaseModel model) => await ItemAdded?.Invoke(model);
        public async Task OnFilterChosen(string statusName, string categoryName) => await FilterChosen?.Invoke(statusName, categoryName);
    }
}
