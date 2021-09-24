using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

using ToDoList.BlazorClient.Services;
using ToDoList.SharedClientLibrary.Models;

namespace ToDoList.BlazorClient.Components
{
    public partial class EditNameForm<TItem> where TItem : BaseModel
    {
        private string originalName;

        private EditContext editContext;

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private Notifier Notifier { get; set; }

        [Parameter]
        public TItem Item { get; set; }

        [Parameter]
        public EventCallback OnValidSubmit { get; set; }
        
        [Parameter]
        public EventCallback OnInvalidSubmit { get; set; }

        [Parameter]
        public string ResetButtonClass { get; set; }

        protected override void OnInitialized()
        {
            originalName = Item.Name;

            editContext = new EditContext(Item);

            Notifier.ItemAdded += FocusInput;
        }

        private async Task HandleSubmit()
        {
            if (editContext.Validate())
                await Submit();
            else
                await OnInvalidSubmit.InvokeAsync();
        }

        private async Task Submit()
        {
            if (Item.Name != originalName)
            {
                await OnValidSubmit.InvokeAsync();
                originalName = Item.Name;
            }
        }

        private void Reset() => Item.Name = originalName;

        private async Task FocusInput(BaseModel item) =>
            await JSRuntime.InvokeVoidAsync("focusInput", GetInputId(item)); // TODO: Change in .Net 6

        private static string GetInputId(BaseModel item) => $"{item.GetType().Name}-name-{item.Id}";
    }
}
