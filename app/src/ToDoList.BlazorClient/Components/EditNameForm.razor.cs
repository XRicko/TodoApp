using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

using ToDoList.BlazorClient.Services;
using ToDoList.SharedClientLibrary.Models;

namespace ToDoList.BlazorClient.Components
{
    public partial class EditNameForm<TItem> : IDisposable
        where TItem : BaseModel
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

        protected override void OnInitialized() => Notifier.ItemAdded += FocusInput;

        protected override void OnParametersSet()
        {
            editContext = new EditContext(Item);
            originalName = Item.Name;
        }

        private async Task HandleSubmit()
        {
            if (Item.Name == originalName)
                return;

            if (editContext.Validate())
                await Submit();
            else if (string.IsNullOrWhiteSpace(originalName))
                await OnInvalidSubmit.InvokeAsync();
            else
                Item.Name = originalName;
        }

        private async Task Submit()
        {
            await OnValidSubmit.InvokeAsync();
            originalName = Item.Name;
        }

        private void Reset() => Item.Name = originalName;

        private async Task FocusInput(BaseModel item) =>
            await JSRuntime.InvokeVoidAsync("focusInput", GetInputId(item)); // TODO: Change in .Net 6

        private static string GetInputId(BaseModel item) => $"{item.GetType().Name}-name-{item.Id}";
        public void Dispose() => Notifier.ItemAdded -= FocusInput;
    }
}
