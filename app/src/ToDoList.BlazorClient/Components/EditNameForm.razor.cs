using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

using ToDoList.SharedClientLibrary.Models;

namespace ToDoList.BlazorClient.Components
{
    public partial class EditNameForm<TItem> : ComponentBase where TItem : BaseModel
    {
        private string originalName;
        private bool isInFocus;
        private string activeClass;

        [Parameter]
        public TItem Item { get; set; }

        [Parameter]
        public EventCallback OnValidSubmit { get; set; }

        protected override void OnInitialized()
        {
            isInFocus = false;
            originalName = Item.Name;
            activeClass = "";
        }

        private async Task Submit()
        {
            await OnValidSubmit.InvokeAsync();
            originalName = Item.Name;

            RemoveFocus();
        }

        private void Reset()
        {
            Item.Name = originalName;
            RemoveFocus();
        }

        private void SetFocus()
        {
            isInFocus = true;
            activeClass = "active";
        }

        private void RemoveFocus()
        {
            isInFocus = false;
            activeClass = "";
        }
    }
}
