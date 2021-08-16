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
        private bool inFocus;

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
        public string ResetButtonClass { get; set; }

        protected override void OnInitialized()
        {
            inFocus = false;
            originalName = Item.Name;

            editContext = new EditContext(Item);

            Notifier.ItemAdded += FocusInput;
        }

        private async Task HandleSubmit()
        {
            if (editContext.Validate())
            {
                await Submit();
            }
            else
            {
                Reset();
                Unfocus();
            }
        }

        private async Task Submit()
        {
            if (Item.Name != originalName)
            {
                await OnValidSubmit.InvokeAsync();
                originalName = Item.Name;
            }

            Unfocus();
        }

        private void Reset() => Item.Name = originalName;

        private void Focus() => inFocus = true;
        private void Unfocus() => inFocus = false;

        private async Task FocusInput(BaseModel item) => 
            await JSRuntime.InvokeVoidAsync("focusInput", GetInputId(item)); // Change in .Net 6

        private static string GetInputId(BaseModel item) => $"{item.GetType().Name}-name-{item.Id}";
    }
}
