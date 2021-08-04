using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

using ToDoList.SharedClientLibrary.Models;

namespace ToDoList.BlazorClient.Components
{
    public partial class EditNameForm<TItem> where TItem : BaseModel
    {
        private string originalName;
        private bool inFocus;

        private string activeClass;

        [Parameter]
        public TItem Item { get; set; }

        [Parameter]
        public EventCallback OnValidSubmit { get; set; }

        [Parameter]
        public string SubmitButtonClass { get; set; }
        [Parameter]
        public string ResetButtonClass { get; set; }

        protected override void OnInitialized()
        {
            inFocus = false;
            originalName = Item.Name;

            activeClass = "";
        }

        private async Task Submit()
        {
            if (Item.Name != originalName)
            {
                await OnValidSubmit.InvokeAsync();
                originalName = Item.Name;
            }

            RemoveFocus();
        }

        private void Reset()
        {
            Item.Name = originalName;
            RemoveFocus();
        }

        private void SetFocus()
        {
            inFocus = true;
            activeClass = "active";
        }

        private void RemoveFocus()
        {
            inFocus = false;
            activeClass = "";
        }
    }
}
