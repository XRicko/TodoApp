using System;
using System.Threading.Tasks;

using ToDoList.SharedClientLibrary.Models;

namespace ToDoList.BlazorClient.Services
{
    public class Notifier
    {
        public event Func<int, Task> ChecklistChanged;
        public event Func<BaseModel, Task> ItemAdded;
        public event Func<Task> FilterChosen;
        public event Func<Task> ProjectsChanged;

        public async Task OnChecklistChanged(int checklistId) => await ChecklistChanged?.Invoke(checklistId);
        public async Task OnItemAdded(BaseModel model) => await ItemAdded?.Invoke(model);
        public async Task OnFilterChosen() => await FilterChosen?.Invoke();
        public async Task OnProjectsChanged() => await ProjectsChanged?.Invoke();
    }
}
