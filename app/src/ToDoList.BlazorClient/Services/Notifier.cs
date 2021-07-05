using System;
using System.Threading.Tasks;

namespace ToDoList.BlazorClient.Services
{
    public class Notifier
    {
        public event Func<int, Task> ChecklistChanged;
        public event Func<Task> BadgeClicked;

        public async Task UpdateChecklist(int checklistId)
        {
            await ChecklistChanged?.Invoke(checklistId);
        }

        public async Task FilterTodoItems()
        {
            await BadgeClicked?.Invoke();
        }
    }
}
